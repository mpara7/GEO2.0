namespace GeoInferenceEngine.Backbone.Abstractions.IOs.Inputs;
public class GetterConfig
{
    public string Name { get; set; } = "default";
    public string TypeName { get; set; }
    public Dictionary<string, string> Configs { get; set; } = new Dictionary<string, string>();
    public List<string> PlugIns { get; set; } = new List<string>();
}
/// <summary>
/// 推理引擎的配置信息
/// 考虑网络传输本地保存等问题需要序列化，记录类型在程序集中的名称
/// 由于后加入的组件会覆盖前者，可以获取默认的再按需求直接调用Add函数覆盖
/// </summary>
public class EngineConfig
{
    /// <summary>
    /// 引擎准备者类型
    /// </summary>
    public string EnginePreparer { get; set; } = "";
    /// <summary>
    /// 引擎类型
    /// </summary>
    public string Engine { get; set; } = "";
    /// <summary>
    /// 引擎输出类型
    /// </summary>
    public string EngineOutputGetter { get; set; } = "";

    /// <summary>
    /// 数据类型
    /// </summary>
    public List<(string actualType, string targetType)> DataBases { get; set; } = new();
    /// <summary>
    /// 配置信息（需要输入，反序列化）
    /// </summary>
    public List<(string type, string yaml)> Configs { get; set; } = new();
    /// <summary>
    /// 组件类型
    /// </summary>
    public List<(string actualType, string targetType)> Components { get; set; } = new();
    /// <summary>
    /// 插件类型-虽然与组件很像但是plugIn就不允许继承了
    /// </summary>
    public List<string> PlugIns { get; set; } = new();

    /// <summary>
    /// 中间结果获取类型
    /// </summary>
    public List<GetterConfig> ProcessInfoGetters { get; set; } = new();
    /// <summary>
    /// 结果类型
    /// </summary>
    public List<GetterConfig> ResultGetters { get; set; } = new();

    /// <summary>
    /// 题目信息输入类型 主要为了显示
    /// </summary>
    public List<string> WithInferenceInputs { get; set; } = new();

    public static EngineConfig FormFile(string filePath)
    {
        var input = File.ReadAllText(filePath);
        var engineConfig = YAML.Deserialize<EngineConfig>(input);
        return engineConfig;
    }

    #region Modules
    public void SetEnginePreparer<T>() where T : class, IEnginePreparer
    {
        EnginePreparer = typeof(T).AssemblyQualifiedName;
    }
    public void SetEngine<T>() where T : class, IInferenceEngine
    {
        Engine = typeof(T).AssemblyQualifiedName;
    }
    public void SetEngineOutputGetter<T>() where T : class, IEngineOutputGetter
    {
        EngineOutputGetter = typeof(T).AssemblyQualifiedName;
    }
    #endregion

    #region Parts
    public void AddDataBase<T>() where T : IInferenceDatabase
    {
        DataBases.Add((typeof(T).AssemblyQualifiedName, typeof(T).AssemblyQualifiedName));
    }
    public void AddConfig(AInferenceSetting aInferenceConfig)
    {
        Configs.Add((aInferenceConfig.GetType().AssemblyQualifiedName, YAML.Serialize(aInferenceConfig)));
    }
    public void ReplaceComponent(Type actual, Type target)
    {
        var a = Components.Find(x => x.targetType == target.AssemblyQualifiedName);
        Components.Remove(a);
        Components.Add((actual.AssemblyQualifiedName, target.AssemblyQualifiedName));
    }
    public void ReplaceComponent<T, Target>() where T : IInferenceComponent, Target
    {
        var a = Components.Find(x => x.targetType == typeof(Target).AssemblyQualifiedName);
        Components.Remove(a);
        Components.Add((typeof(T).AssemblyQualifiedName, typeof(Target).AssemblyQualifiedName));
    }
    public void AddComponent<T, Target>() where T : IInferenceComponent, Target
    {
        Components.Add((typeof(T).AssemblyQualifiedName, typeof(Target).AssemblyQualifiedName));
    }
    public void AddComponent<T>() where T : IInferenceComponent
    {
        Components.Add((typeof(T).AssemblyQualifiedName, typeof(T).AssemblyQualifiedName));
    }
    public void AddPlugIn<T>() where T : IInferencePlugIn
    {
        PlugIns.Add(typeof(T).AssemblyQualifiedName);
    }
    #endregion

    #region Outputs
    public void AddProcessInfoGetter<T>(params AInferenceSetting[] aInferenceConfig)
    {
        GetterConfig getterConfig = new GetterConfig();
        foreach (AInferenceSetting config in aInferenceConfig)
        {
            getterConfig.Configs.Add(config.GetType().AssemblyQualifiedName, YAML.Serialize(config));
        }
        getterConfig.TypeName = typeof(T).AssemblyQualifiedName;

        ProcessInfoGetters.Add(getterConfig);
    }
    public void AddResultGetter<T>(params AInferenceSetting[] aInferenceConfig)
    {
        GetterConfig getterConfig = new GetterConfig();
        foreach (AInferenceSetting config in aInferenceConfig)
        {
            getterConfig.Configs.Add(config.GetType().AssemblyQualifiedName, YAML.Serialize(config));
        }
        getterConfig.TypeName = typeof(T).AssemblyQualifiedName;

        ResultGetters.Add(getterConfig);
    }
    #endregion
}
