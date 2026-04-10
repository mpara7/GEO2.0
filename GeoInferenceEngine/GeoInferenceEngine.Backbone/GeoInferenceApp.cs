using System.Diagnostics;
using GeoInferenceEngine.Backbone.AppBuilder;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs;
namespace GeoInferenceEngine.Backbone;
/// <summary>
/// 应用状态
/// </summary>
public enum AppStatus
{
    /// <summary>
    /// 准备状态
    /// </summary>
    Waiting,
    /// <summary>
    /// 运行中
    /// </summary>
    Running,
    /// <summary>
    /// 暂停中
    /// </summary>
    Holding,
    /// <summary>
    /// 已崩溃——抛出异常
    /// </summary>
    Cracked,
    /// <summary>
    /// 已结束
    /// </summary>
    Finished,
}
/// <summary>
/// 应用流程信息
/// </summary>
public class AppInfo
{
    // 新增运行时间属性
    public TimeSpan RunTime { get; set; }
    public AppStatus AppStatu { get; set; } = AppStatus.Waiting;
    
    #region 外部流程控制
    public bool IsRequirePause { get; set; }
    public bool IsRequireStop { get; set; }
    #endregion

    #region 内部流程控制与Debug信息
    public string CurAction { get; set; } = "无行动";
    public bool IsActivedPause { get; set; }
    public List<string> ActivedPauseReasons { get; set; } = new();

    public bool IsActivedStop { get; set; }
    public List<string> ActivedStopReasons { get; set; } = new();
   
    public bool HasWarning { get => WarningInfo.Warnings.Count > 0; }
    public InferenceWarningInfo WarningInfo { get; set; } = new InferenceWarningInfo();

    public bool IsCracked { get; set; }
    public InferenceCrackedInfo CreckedInfo { get; set; } = new InferenceCrackedInfo();
    #endregion
}
public class GeoInferenceApp
{

    

    public bool IsThrowExection { get; set; } = true;
    public bool IsRunByAsync { get; set; } = false;

    public AppInfo AppInfo { get; set; } = new AppInfo();
    IEnginePreparer preparer { get; set; }
    IInferenceEngine engine { get; set; }
    IEngineOutputGetter outputGetter { get; set; }
    public Action OnStarting { get; set; }
    public Action OnFinished { get; set; }
    public void SetInput(InferenceUniversalInputs input)
    {
        EngineBuilder builder = new EngineBuilder();
        builder.container.SetSingleton(AppInfo);
        builder
            .SetInput(input.Inputs)
            .SetEngineConfig(input.EngineConfig);
        var (preparer, engine, outputGetter) = builder.Build();
        this.preparer = preparer;
        this.engine = engine;
        this.outputGetter = outputGetter;
    }
    /// <summary>
    /// 准备推理
    /// </summary>
    public void Prepare()
    {
        preparer.Prepare();
        AppInfo.AppStatu = AppStatus.Waiting;
    }

    /// <summary>
    /// 开始推理，直到结束
    /// </summary>
    public virtual void Start()
    {

        GlobalTimer.Start();
        AppInfo.CurAction = "开始推理";
        if (IsThrowExection)
        {
            if (IsRunByAsync)
            {
                Task.Run(() =>
                {
                    OnStarting?.Invoke();
                    _startAsync();
                    if (AppInfo.AppStatu == AppStatus.Finished || AppInfo.AppStatu == AppStatus.Cracked)
                    {
                        OnFinished?.Invoke();
                    }
                });
            }
            else
            {
                OnStarting?.Invoke();
                _start();
                if (AppInfo.AppStatu == AppStatus.Finished || AppInfo.AppStatu == AppStatus.Cracked)
                {
                    OnFinished?.Invoke();
                }
            }

        }
        else
        {
            if (IsRunByAsync)
            {
                Task.Run(() =>
                {
                    try
                    {
                        OnStarting?.Invoke();
                        _startAsync();
                    }
                    catch (NullReferenceException ex)
                    {
                        AppInfo.IsCracked = true;
                        AppInfo.CreckedInfo.Detail = ex.Message;
                        AppInfo.CreckedInfo.CurAction = AppInfo.CurAction;
                        AppInfo.AppStatu = AppStatus.Cracked;
                    }
                    catch (Exception ex)
                    {
                        AppInfo.IsCracked = true;
                        AppInfo.CreckedInfo.Detail = ex.Message;
                        AppInfo.CreckedInfo.CurAction = AppInfo.CurAction;
                        AppInfo.AppStatu = AppStatus.Cracked;
                    }
                    finally
                    {
                        if (AppInfo.AppStatu == AppStatus.Finished || AppInfo.AppStatu == AppStatus.Cracked)
                        {
                            OnFinished?.Invoke();
                        }
                    }
                });
            }
            else
            {
                try
                {
                    OnStarting?.Invoke();
                    _start();
                }
                catch (NullReferenceException ex)
                {
                    AppInfo.IsCracked = true;
                    AppInfo.CreckedInfo.Detail = ex.Message;
                    AppInfo.CreckedInfo.CurAction = AppInfo.CurAction;
                    AppInfo.AppStatu = AppStatus.Cracked;
                }
                catch (Exception ex)
                {
                    AppInfo.IsCracked = true;
                    AppInfo.CreckedInfo.Detail = ex.Message;
                    AppInfo.CreckedInfo.CurAction = AppInfo.CurAction;
                    AppInfo.AppStatu = AppStatus.Cracked;
                }
                finally
                {
                    if (AppInfo.AppStatu == AppStatus.Finished || AppInfo.AppStatu == AppStatus.Cracked)
                    {
                        OnFinished?.Invoke();
                    }
                }
            }
        }
    }

    void _start()
    {

        AppInfo.AppStatu = AppStatus.Running;

        while (AppInfo.AppStatu == AppStatus.Running)
        {
            if (AppInfo.IsRequirePause || AppInfo.IsActivedPause)
            {
                AppInfo.AppStatu = AppStatus.Holding;
                break;
            }
            else if (AppInfo.IsRequireStop || AppInfo.IsActivedStop)
            {
                AppInfo.AppStatu = AppStatus.Finished;
                break;
            }
            engine.StepForward();
        }
        if (AppInfo.AppStatu == AppStatus.Finished)
        {
            engine.Release();
        }
    }
    AutoResetEvent autoResetEvent { get; set; } = new AutoResetEvent(false);
    void _startAsync()
    {
        AppInfo.AppStatu = AppStatus.Running;

        while (AppInfo.AppStatu == AppStatus.Running)
        {
            if (AppInfo.IsRequirePause || AppInfo.IsActivedPause)
            {
                AppInfo.AppStatu = AppStatus.Holding;
                autoResetEvent.WaitOne();
            }
            else if (AppInfo.IsRequireStop || AppInfo.IsActivedStop)
            {
                AppInfo.AppStatu = AppStatus.Finished;
                break;
            }
            engine.StepForward();
        }
        if (AppInfo.AppStatu == AppStatus.Finished)
        {
            engine.Release();
        }
    }
    /// <summary>
    /// 单步推理
    /// </summary>
    public void Step()
    {
        if (AppInfo.AppStatu == AppStatus.Running)
        {
            engine.StepForward();
        }
    }
    /// <summary>
    /// 暂停推理
    /// </summary>
    public void Pause()
    {
        AppInfo.IsRequirePause = true;
    }
    /// <summary>
    /// 恢复推理，直到结束
    /// </summary>
    public void Continue()
    {
        AppInfo.AppStatu = AppStatus.Running;
        AppInfo.IsRequirePause = false;
        if (IsRunByAsync)
        {
            autoResetEvent.Set();
        }
        else
        {
            _start();
        }

    }
    /// <summary>
    /// 停止推理,停止后不可恢复
    /// </summary>
    public void Stop()
    {
        // 停止计时并记录运行时间
       
        AppInfo.IsRequireStop = true;
    }


    public Dictionary<string, AInferenceOutput> GetProcessingInfos()
    {
        return outputGetter.GetProcessingInfos();
    }
    public InferenceUniversalOutputs GetResults()
    {
        InferenceUniversalOutputs outputs = new InferenceUniversalOutputs();
        
        if (AppInfo.WarningInfo.Warnings.Count > 0)
        {
            outputs.HasWarning = true;
            AppInfo.WarningInfo.Warnings.ForEach(outputs.WarningInfo.Warnings.Add);
        }
        if (AppInfo.IsActivedStop)
        {
            outputs.IsActiveStop = true;
            AppInfo.ActivedStopReasons.ForEach(outputs.ActiveStopReasons.Add);
            outputs.Outputs = outputGetter.GetResults();
            return outputs;
        }
        var results = outputGetter.GetResults();
        outputs.Outputs = results;
        return outputs;
    }
    public AInferenceOutput GetProcessingInfo<T>(string name = null) where T : AInferenceOutput
    {
        return outputGetter.GetProcessingInfo<T>(name);
    }
    public AInferenceOutput GetResult<T>(string name = null) where T : AInferenceOutput
    {
        return outputGetter.GetResult<T>(name);
    }
}
