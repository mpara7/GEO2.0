namespace ZTool.Infrastructures.TaskQuene;
/// <summary>
/// 在ZTask中修改Statu即可进入
/// </summary>
public abstract class ZTask
{
    public event Action<ZTask> OnWaiting;
    public event Action<ZTask> OnFinished;
    public event Action<ZTask> OnRunning;
    public event Action<ZTask> OnCracked;
    protected virtual void EventInvoke(ZTaskStatu newStatu)
    {
        if (newStatu == ZTaskStatu.Finished)
        {
            OnFinished?.Invoke(this);
        }
        else if (newStatu == ZTaskStatu.Cracked)
        {
            OnCracked?.Invoke(this);
        }
        else if (newStatu == ZTaskStatu.Running)
        {
            OnRunning?.Invoke(this);
        }
        else if (newStatu == ZTaskStatu.Waiting)
        {
            OnWaiting?.Invoke(this);
        }


    }
    ZTaskStatu statu;
    public ZTaskStatu Statu { get => statu; set { EventInvoke(value); statu = value; } }
    public string Id { get; set; }
    public virtual void Run() { Statu = ZTaskStatu.Finished; }
}
