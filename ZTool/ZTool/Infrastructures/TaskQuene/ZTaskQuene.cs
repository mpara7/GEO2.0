namespace ZTool.Infrastructures.TaskQuene;
/// <summary>
/// 一个简单的管理任务的队列
/// 独立线程工作,基于模板方法思路写的
/// </summary>
/// <typeparam name="T"></typeparam>
public class ZTaskQuene<T> where T : ZTask
{
    #region 配置
    public Func<T> TaskCreateFunc { get; set; }
    public Func<bool> CanRunNextFunc { get; set; }
    public Func<string> NextIdFunc { get; set; }
    #endregion
    #region 运行上下文

    public List<T> AllList { get; set; } = new();
    public List<T> WaitingList { get; set; } = new();
    public List<T> RunningList { get; set; } = new();
    public List<T> HoldingList { get; set; } = new();
    public List<T> FinishedList { get; set; } = new();
    public List<T> CrackedList { get; set; } = new();

    bool isRequireToRelease = false;
    public ZTaskQuene()
    {
        CanRunNextFunc = () => WaitingList.Count > 0;
    }
    /// <summary>
    /// 每当有事件将触发
    /// </summary>
    AutoResetEvent processLock = new AutoResetEvent(true);
    #endregion
    #region 信息
    public List<string> GetAllID()
    {
        return AllList.Select(t => t.Id).ToList();
    }
    /// <summary>
    /// 返回空则表示id不正确
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ZTaskStatu? GetTaskStatu(string id)
    {
        return AllList.Where(t => t.Id == id).FirstOrDefault()?.Statu;
    }
    public T? GetTask(string id)
    {
        return AllList.FirstOrDefault(t => t.Id == id);
    }
    #endregion


    #region 操作
    public string AddTask()
    {
        return AddTask(TaskCreateFunc());
    }
    public string AddTask(T task)
    {
        task.Id = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        task.Statu = ZTaskStatu.Waiting;
        task.OnFinished += (t) =>
        {
            RunningList.Remove(task);
            FinishedList.Add(task);
            processLock.Set();
        };
        task.OnCracked += (t) =>
        {
            RunningList.Remove(task);
            CrackedList.Add(task);
            processLock.Set();
        };

        AllList.Add(task);
        WaitingList.Add(task);
        processLock.Set();

        return task.Id;
    }
    /// <summary>
    /// 开启独立线程线程
    /// </summary>
    public void Start()
    {
        Thread thread = new Thread(() =>
        {
            while (true)
            {
                if (isRequireToRelease)
                    break;
                if (CheckIfCanRunNext())
                    RunNext();
                processLock.WaitOne();
            }
        });
        thread.Start();
    }
    protected virtual bool CheckIfCanRunNext()
    {
        return CanRunNextFunc();
    }

    protected virtual void RunNext()
    {
        var nextTask = WaitingList[0];
        nextTask.Statu = ZTaskStatu.Running;
        WaitingList.Remove(nextTask);
        RunningList.Add(nextTask);
        Thread thread = new Thread(() =>
        {
            try
            {
                nextTask.Run();
                nextTask.Statu = ZTaskStatu.Finished;
            }
            catch (Exception ex)
            {
                nextTask.Statu = ZTaskStatu.Cracked;
            }

        });
        thread.Start();
    }

    public void Stop()
    {
        isRequireToRelease = true;
        processLock.Set();
    }
    #endregion



}
