namespace GeoInferenceEngine.Backbone.AppConfiger;
public class ConfigRequirePlugInsAttribute : Attribute
{
    public Type[] RequirePlugIns { get; set; }
    public ConfigRequirePlugInsAttribute(params Type[] requirePlugIns)
    {
        RequirePlugIns = requirePlugIns;
    }
}
public interface IInferenceEngineBuilderConfigerPlugIn
{
    /// <summary>
    /// 不能用的提示信息
    /// </summary>
    public List<string> Hints { get; }
    public bool IsEnable { get; set; }
    /// <summary>
    /// 在更新前 检查能否使用 
    /// </summary>
    /// <returns></returns>
    public bool CheckIsEnable();
    public void Enable();
    public void Disable();
    public void UpdateEnableState()
    {
        bool isNowEnable = CheckIsEnable();
        if (!IsEnable && isNowEnable)
        {
            Enable();
            IsEnable = isNowEnable;
        }
        else if (IsEnable && !isNowEnable)
        {
            Disable();
            IsEnable = isNowEnable;
        }
    }

    public InferenceEngineConfiger Configer { set; }

    /// <summary>
    /// 更新
    /// </summary>
    public void Update();
    /// <summary>
    /// 初始时 一般 会后接一个Update()
    /// </summary>
    public void Init();
}
