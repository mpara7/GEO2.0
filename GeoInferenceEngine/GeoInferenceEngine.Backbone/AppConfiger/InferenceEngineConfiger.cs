

using System.Collections.ObjectModel;
using System.Reflection;

using ZTool.UsefulTypes;



namespace GeoInferenceEngine.Backbone.AppConfiger;

public static class StringTypeTool
{
    public static string ToQName(this Type type)
    {
        return type.AssemblyQualifiedName;
    }
    public static Type ToType(this string typeName)
    {
        return Type.GetType(typeName);
    }
}
/// <summary>
/// 为了方便更新界面
/// </summary>
public class TypeInfo
{
    public virtual Type Type { get; set; }
    public TypeInfo Pump(Type target)
    {
        Type = target;
        return this;
    }
}
public class SelectionInfo
{
    public virtual Type TargetType { get; set; }
    public virtual Type SelectedType { get; set; }
    /// <summary>
    /// 可选的类型列表
    /// </summary>
    public virtual Collection<Type> OptionTypes { get; set; } = new();
    public SelectionInfo Pump(Type target, Type selected, IList<Type> options)
    {
        TargetType = target;
        SelectedType = selected;
        options.ToList().ForEach(t => OptionTypes.Add(t));
        if (OptionTypes.Count == 1)
            SelectedType = OptionTypes[0];
        return this;
    }
    public override string ToString()
    {
        return $"目标：{TargetType},选择：{SelectedType}";
    }
}
public class CheckSelectionInfo
{
    public virtual Type TargetType { get; set; }
    public virtual bool IsChecked { get; set; } = false;
    public CheckSelectionInfo Pump(Type target, bool isChecked)
    {
        TargetType = target;
        IsChecked = isChecked;
        return this;
    }
    public override string ToString()
    {
        return $"{TargetType},选择：{IsChecked}";
    }
}
public class GetterInfo
{
    public virtual Type Type { get; set; }
    public virtual Collection<Type> PlugInTypes { get; set; } = new Collection<Type>() { };
    public virtual Collection<AInferenceSetting> Configs { get; set; } = new Collection<AInferenceSetting>() { };
}

/// <summary>
/// DI计算思路
/// 1推理引擎 根据用户选择 
/// 然后确定引擎准备器
/// 2确定组件 根据选择的引擎以及组件递归确定
/// 3确定数据类 根据已选择的引擎与组件确定
/// 4确定数据获取类  
/// 5确定插件 根据用户选择的独立插件与数据获取者中的定义确定
/// 6确定输入类型 根据引擎、组件、数据类、插件、数据获取者确定
/// 7确定配置 根据引擎、组件、数据类、插件、数据获取者、输入类型确定

/// 7配置器插件 根据配置确定配置器插件 
/// 8检查 配置器插件 是否满足引用
/// 9运行配置器插件
/// 10检查 引擎已选择 组件数、据库已选择 插件、数据获取者 是否满足引用 若不能 给出提示不允许获取
/// 结束
/// </summary>
public class InferenceEngineConfiger
{
    public static ZDIContainer DefaultConfigerDI()
    {
        ZDIContainer dIContainer = new();

        dIContainer.Set<TypeInfo>();
        dIContainer.Set<SelectionInfo>();
        dIContainer.Set<CheckSelectionInfo>();
        dIContainer.Set<GetterInfo>();


        dIContainer.Set<Collection<SelectionInfo>>();
        dIContainer.Set<Collection<CheckSelectionInfo>>();
        dIContainer.Set<Collection<AInferenceInput>>();
        dIContainer.Set<Collection<AInferenceSetting>>();
        dIContainer.Set<Collection<Type>>();
        return dIContainer;
    }
    public static InferenceEngineConfiger ReadFromEngineConfig(EngineConfig engineConfig, List<Assembly> inferenceAssemblies, ZDIContainer DI)
    {
        var configer = new InferenceEngineConfiger(inferenceAssemblies, DI);
        var dIContainer = configer.ConfigerDI;
        #region 读取配置
        configer.EnginePreparerSelection = dIContainer.Get<SelectionInfo>().Pump(typeof(IEnginePreparer), engineConfig.EnginePreparer.ToType(), configer.EnginePrparerTypes);
        configer.EngineSelection = dIContainer.Get<SelectionInfo>().Pump(typeof(IInferenceEngine), engineConfig.Engine.ToType(), configer.EngineTypes);
        configer.EngineOutputSelection = dIContainer.Get<SelectionInfo>().Pump(typeof(IEngineOutputGetter), engineConfig.EngineOutputGetter.ToType(), configer.EngineOutputGetterTypes);

        foreach (var kv in engineConfig.Components)
        {
            Type targetType = kv.targetType.ToType();
            Type actualType = kv.actualType.ToType();
            SelectionInfo selectionInfo = dIContainer.Get<SelectionInfo>().Pump(targetType, actualType, configer.InheritanceDictionary[targetType]);

            configer.Components.Add(selectionInfo);
        }
        foreach (var kv in engineConfig.DataBases)
        {
            Type targetType = kv.targetType.ToType();
            Type actualType = kv.actualType.ToType();
            SelectionInfo selectionInfo = dIContainer.Get<SelectionInfo>().Pump(targetType, actualType, configer.InheritanceDictionary[targetType]);

            configer.DataBases.Add(selectionInfo);
        }
        foreach (var typeStr in engineConfig.PlugIns)
        {
            Type type = typeStr.ToType();
            if (type.GetCustomAttribute<InferenceStandPlugInAttribute>() is not null)
            {
                configer.StandPlugIns.Add(dIContainer.Get<CheckSelectionInfo>().Pump(type, true));
            }
            else
            {
                configer.WithPlugIns.Add(type);
            }
        }
        foreach (var configKV in engineConfig.Configs)
        {
            var config = YAML.Deserialize(configKV.type, configKV.yaml);
            configer.UnusedConfigs.Add((AInferenceSetting)config);
        }
        foreach (var getterConfig in engineConfig.ProcessInfoGetters)
        {
            GetterInfo signletonGetter = dIContainer.Get<GetterInfo>();
            signletonGetter.Type = getterConfig.TypeName.ToType();
            foreach (var plugIn in getterConfig.PlugIns)
            {
                signletonGetter.PlugInTypes.Add(plugIn.ToType());
            }
            foreach (var configKV in getterConfig.Configs)
            {
                Type configType = configKV.Key.ToType();
                var config = YAML.Deserialize(configType, configKV.Value);
                signletonGetter.Configs.Add((AInferenceSetting)config);
            }
            configer.ProcessingInfoGetters.Add(signletonGetter);
        }
        foreach (var getterConfig in engineConfig.ResultGetters)
        {
            GetterInfo signletonGetter = dIContainer.Get<GetterInfo>();
            signletonGetter.Type = getterConfig.TypeName.ToType();
            foreach (var plugIn in getterConfig.PlugIns)
            {
                signletonGetter.PlugInTypes.Add(plugIn.ToType());
            }
            foreach (var configKV in getterConfig.Configs)
            {
                Type configType = configKV.Key.ToType();
                var config = YAML.Deserialize(configType, configKV.Value);
                signletonGetter.Configs.Add((AInferenceSetting)config);
            }
            configer.ResultGetters.Add(signletonGetter);
        }
        #endregion

        #region 补齐可选项
        foreach (var type in configer.StandPlugInTypes)
        {
            if (!configer.StandPlugIns.ToList().Exists(ci => ci.TargetType == type))
            {
                configer.StandPlugIns.Add(dIContainer.Get<CheckSelectionInfo>().Pump(type, false));
            }
        }
        #endregion
        configer.CalDI();
        return configer;
    }

    /// <summary>
    /// 推理组件程序集
    /// </summary>
    public List<Assembly> InferenceAssemblies { get; init; }
    /// <summary>
    /// 配置器的DI
    /// </summary>
    public ZDIContainer ConfigerDI { get; init; }
    /// <summary>
    /// 配置器的插件
    /// </summary>
    public List<IInferenceEngineBuilderConfigerPlugIn> ConfigerPlugIns { get; set; } = new();
    /// <summary>
    /// 错误提示
    /// </summary>

    List<string> ErrorHints = new List<string>();

    #region 类型集合
    /// <summary>
    /// 引擎类型
    /// </summary>
    public List<Type> EnginePrparerTypes { get; set; } = new();
    /// <summary>
    /// 引擎类型
    /// </summary>
    public List<Type> EngineTypes { get; set; } = new();
    /// <summary>
    /// 引擎类型
    /// </summary>
    public List<Type> EngineOutputGetterTypes { get; set; } = new();

    public List<Type> DataBaseTypes { get; set; } = new();
    public List<Type> ComponentTypes { get; set; } = new();
    /// <summary>
    /// 插件
    /// </summary>
    public List<Type> PlugInTypes { get; set; } = new();
    /// <summary>
    /// 独立插件
    /// </summary>
    public List<Type> StandPlugInTypes { get; set; } = new();
    /// <summary>
    /// 输入配置
    /// </summary>
    public List<Type> ConfigTypes { get; set; } = new();
    public List<Type> InputTypes { get; set; } = new();
    public List<Type> OutputMakerTypes { get; set; } = new();
    #endregion

    #region 类型关系字典
    /// <summary>
    /// 组件与数据库 之间继承关系的字典
    /// </summary>
    public Dictionary<Type, List<Type>> InheritanceDictionary { get; set; } = new();
    /// <summary>
    /// 组件之间相互包含的关系
    /// </summary>
    public Dictionary<Type, List<Type>> ComponentContainmentDictionary { get; set; } = new();

    /// <summary>
    /// 结果获取者中包含的插件
    /// </summary>
    public Dictionary<Type, List<Type>> PlugInContainmentDictionary { get; set; } = new();

    /// <summary>
    /// 包含所需的配置
    /// </summary>
    public Dictionary<Type, List<Type>> ConfigContainmentDictionary { get; set; } = new();

    /// <summary>
    /// 数据库包含关系
    /// </summary>
    public Dictionary<Type, List<Type>> DataBaseContainmentDictionary { get; set; } = new();
    /// <summary>
    /// 包含所需配置器插件
    /// </summary>
    public Dictionary<Type, List<Type>> ConfigRequirePlugInContainmentDictionary { get; set; } = new();

    /// <summary>
    /// 输入依赖字典
    /// </summary>
    public Dictionary<Type, List<Type>> InputContainmentDictionary { get; set; } = new();

    /// <summary>
    /// 同组关联字典
    /// </summary>
    public Dictionary<Type, List<Type>> GroupDictionary { get; set; } = new();
    #endregion

    #region 寻找类型以及依赖关系
    void _findTypes()
    {
        //找推理引擎
        foreach (var assembly in InferenceAssemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                //模块
                if (type.IsAssignableTo(typeof(IEnginePreparer)) && type != typeof(IEnginePreparer))
                {
                    EnginePrparerTypes.Add(type);
                }
                else if (type.IsAssignableTo(typeof(IInferenceEngine)) && type != typeof(IInferenceEngine))
                {
                    EngineTypes.Add(type);
                }
                else if (type.IsAssignableTo(typeof(IEngineOutputGetter)) && type != typeof(IEngineOutputGetter))
                {
                    EngineOutputGetterTypes.Add(type);
                }

                //组件
                else if (type.IsAssignableTo(typeof(IInferenceComponent)))
                {
                    ComponentTypes.Add(type);
                }
                //数据库
                else if (type.IsAssignableTo(typeof(IInferenceDatabase)))
                {
                    DataBaseTypes.Add(type);
                }
                //输入配置
                else if (type.IsAssignableTo(typeof(AInferenceInput)))
                {
                    ConfigTypes.Add(type);
                }
                //输入
                else if (type.IsAssignableTo(typeof(AInferenceInput)))
                {
                    if (type != typeof(AInferenceInput))
                        InputTypes.Add(type);
                }
                //输出
                else if (type.IsAssignableTo(typeof(IInferenceOutputMaker<AInferenceOutput>)))
                {
                    if (type != typeof(IInferenceOutputMaker<>))
                        OutputMakerTypes.Add(type);
                }


                //插件
                if (type.IsAssignableTo(typeof(IInferencePlugIn)))
                {
                    if (type.GetCustomAttribute<InferenceStandPlugInAttribute>() is not null)
                    {
                        StandPlugInTypes.Add(type);
                    }
                    else if (!type.IsAbstract)
                    {
                        PlugInTypes.Add(type);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 找关系
    /// </summary>
    void _findDIRelation()
    {
        _fillComponentANDDatabaseInheritanceDictionary();
        ComponentContainmentDictionary = _findAssignable(EngineTypes.Union(EnginePrparerTypes).Union(EngineOutputGetterTypes).Union(ComponentTypes),
            typeof(IInferenceComponent));
        PlugInContainmentDictionary = _findAssignable(EngineTypes.Union(ComponentTypes).Union(OutputMakerTypes), typeof(IInferencePlugIn));
        ConfigContainmentDictionary = _findAssignable(EngineTypes
            .Union(EnginePrparerTypes)
            .Union(EngineOutputGetterTypes)
            .Union(ComponentTypes)
            .Union(PlugInTypes)
            .Union(OutputMakerTypes)
            .Union(StandPlugInTypes), typeof(AInferenceSetting));
        DataBaseContainmentDictionary = _findAssignable(EngineTypes.Union(ComponentTypes).Union(PlugInTypes).Union(OutputMakerTypes), typeof(IInferenceDatabase));
        InputContainmentDictionary = _findAssignable(EnginePrparerTypes, typeof(AInferenceInput));
        _fillConfigRequirePlugInContainmentDictionary();
        _fillGroupDictionary();
    }
    void _fillComponentANDDatabaseInheritanceDictionary()
    {
        InheritanceDictionary.Clear();
        foreach (var item in ComponentTypes.Union(DataBaseTypes))
        {
            if (item.IsAbstract)
                InheritanceDictionary.Add(item, new());
            else
                InheritanceDictionary.Add(item, new() { item });
        }
        foreach (var item in InheritanceDictionary)
        {
            foreach (var component in ComponentTypes.Union(DataBaseTypes))
            {
                if (component.IsSubclassOf(item.Key) && !component.IsAbstract)
                {
                    item.Value.Add(component);
                }
            }
        }
    }
    Dictionary<Type, List<Type>> _findAssignable(IEnumerable<Type> types, Type target)
    {
        Dictionary<Type, List<Type>> dict = new();
        //搜索组件的包含关系
        foreach (var type in types)
        {
            dict.Add(type, new());
            //判断具有DI特性的字段/属性是否继承/实现于对应类型
            var fileds = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var di = prop.GetCustomAttribute<ZDIAttribute>();
                if (di is not null)
                {
                    if (prop.PropertyType.IsAssignableTo(target))
                        dict[type].Add(prop.PropertyType);
                }
            }
            foreach (var field in fileds)
            {
                var di = field.GetCustomAttribute<ZDIAttribute>();
                if (di is not null)
                {
                    if (field.FieldType.IsAssignableTo(target))
                        dict[type].Add(field.FieldType);
                }
            }
        }
        return dict;
    }
    void _fillConfigRequirePlugInContainmentDictionary()
    {
        foreach (var type in ConfigTypes)
        {
            var attri = type.GetCustomAttribute<ConfigRequirePlugInsAttribute>();
            if (attri is not null)
            {
                ConfigRequirePlugInContainmentDictionary.Add(type, attri.RequirePlugIns.ToList());
            }
        }
    }

    void _fillGroupDictionary()
    {
        foreach (var type in EngineTypes.Union(ComponentTypes)
            .Union(PlugInTypes).Union(StandPlugInTypes).Union(OutputMakerTypes))
        {
            var attriType = typeof(InferenceComponentGroupAttribute<>);
            Attribute attri = type.GetCustomAttribute(attriType);
            if (attri is null) continue;
            var groupType = attri.GetType().GetGenericArguments()[0];
            if (GroupDictionary.ContainsKey(groupType))
            {
                GroupDictionary[groupType].Add(type);
            }
            else
            {
                GroupDictionary.Add(groupType, new List<Type>() { type });
            }
        }
    }
    #endregion

    public InferenceEngineConfiger(List<Assembly> inferenceAssemblies, ZDIContainer DI)
    {
        ConfigerDI = DI;
        ConfigerDI.SetSingleton(this);
        InferenceAssemblies = inferenceAssemblies;

        _findTypes();
        _findDIRelation();

        EnginePreparerSelection = ConfigerDI.Get<SelectionInfo>();
        EngineSelection = ConfigerDI.Get<SelectionInfo>();
        EngineOutputSelection = ConfigerDI.Get<SelectionInfo>();

        Components = ConfigerDI.Get<Collection<SelectionInfo>>();
        DataBases = ConfigerDI.Get<Collection<SelectionInfo>>();
        StandPlugIns = ConfigerDI.Get<Collection<CheckSelectionInfo>>();
        ProcessingInfoGetters = ConfigerDI.Get<Collection<GetterInfo>>();
        ResultGetters = ConfigerDI.Get<Collection<GetterInfo>>();

        Configs = ConfigerDI.Get<Collection<AInferenceSetting>>();
        UnusedConfigs = ConfigerDI.Get<Collection<AInferenceSetting>>();
        ExConfigs = ConfigerDI.Get<Collection<AInferenceSetting>>();

        WithInputs = ConfigerDI.Get<Collection<Type>>();
        WithOutputs = ConfigerDI.Get<Collection<Type>>();
        WithPlugIns = ConfigerDI.Get<Collection<Type>>();
    }

    #region DI注册内容
    public SelectionInfo EnginePreparerSelection { get; set; }
    public SelectionInfo EngineSelection { get; set; }
    public SelectionInfo EngineOutputSelection { get; set; }

    public Collection<SelectionInfo> Components { get; set; }
    public Collection<SelectionInfo> DataBases { get; set; }
    /// <summary>
    /// 独立插件
    /// </summary>
    public Collection<CheckSelectionInfo> StandPlugIns { get; set; }
    public Collection<GetterInfo> ProcessingInfoGetters { get; set; }
    public Collection<GetterInfo> ResultGetters { get; set; }

    public Collection<AInferenceSetting> Configs { get; set; }
    public Collection<AInferenceSetting> UnusedConfigs { get; set; }
    public Collection<AInferenceSetting> ExConfigs { get; set; }

    /// <summary>
    /// 输入类型
    /// </summary>
    public Collection<Type> WithInputs { get; set; }
    /// <summary>
    /// 输入类型
    /// </summary>
    public Collection<Type> WithOutputs { get; set; }
    /// <summary>
    /// 包含的非独立插件
    /// </summary>
    public Collection<Type> WithPlugIns { get; set; }
    #endregion

    #region DI计算
    /// <summary>
    /// 用于循环判断
    /// </summary>
    bool compHasChanged = false;
    void addSelectionInfo(IList<SelectionInfo> list, Type targetType)
    {
        SelectionInfo selectionInfo = ConfigerDI.Get<SelectionInfo>();
        selectionInfo.Pump(targetType, null, InheritanceDictionary[targetType]);
        if (selectionInfo.OptionTypes.Count == 1)
            selectionInfo.SelectedType = selectionInfo.OptionTypes[0];
        list.Add(selectionInfo);
    }


    /// <summary>
    /// 先把所有的加进去，再分出不要的
    /// </summary>
    public void updateComp()
    {
        //标志列表有无改变
        var hasChange = false;
        List<Type> havingTypes = Components.Select(s => s.TargetType).ToList();
        List<Type> actualTypes = Components.Where(s => s.SelectedType != null).Select(s => s.SelectedType).ToList();
        List<Type> requiredTypes = new List<Type>();
        if (EnginePreparerSelection.SelectedType is not null)
        {
            foreach (Type type in ComponentContainmentDictionary[EnginePreparerSelection.SelectedType])
            {
                requiredTypes.Add(type);
            };

        }
        if (EngineSelection.SelectedType is not null)
        {
            foreach (Type type in ComponentContainmentDictionary[EngineSelection.SelectedType])
            {
                requiredTypes.Add(type);
            };
        }
        if (EngineOutputSelection.SelectedType is not null)
        {
            foreach (Type type in ComponentContainmentDictionary[EngineOutputSelection.SelectedType])
            {
                requiredTypes.Add(type);
            };
        }

        foreach (var type in actualTypes)
        {
            foreach (var containedType in ComponentContainmentDictionary[type])
            {
                if (!requiredTypes.Contains(containedType))
                {
                    requiredTypes.Add(containedType);
                }
            }
        }
        requiredTypes = requiredTypes.Distinct().ToList();
        //多余的删掉
        for (int i = Components.Count - 1; i >= 0; i--)
        {

            if (!requiredTypes.Contains(Components[i].TargetType))
            {
                hasChange = true;
                Components.RemoveAt(i);
            }
        }
        //少的补上
        var addingTypes = requiredTypes.Except(havingTypes);
        foreach (var item in addingTypes)
        {
            hasChange = true;
            addSelectionInfo(Components, item);
        }

        if (!hasChange)
            compHasChanged = false;
        else
            compHasChanged = true;
        //排序
        var sorted = Components.ToList();
        sorted.Sort((a, b) => -a.OptionTypes.Count.CompareTo(b.OptionTypes.Count));
        Components.Clear();
        foreach (var item in sorted)
        {
            Components.Add(item);
        }
    }
    void updateDataBases()
    {
        List<Type> havingTypes = DataBases.Select(s => s.TargetType).ToList();
        List<Type> requiredTypes = new List<Type>();

        //引擎
        if (EngineSelection.SelectedType is not null)
        {
            foreach (Type type in DataBaseContainmentDictionary[EngineSelection.SelectedType])
            {
                requiredTypes.Add(type);
            };
        }
        //组件
        foreach (var compType in Components)
        {
            if (compType.SelectedType is null) continue;
            foreach (var requiredType in DataBaseContainmentDictionary[compType.SelectedType])
            {
                if (!requiredTypes.Contains(requiredType))
                {
                    requiredTypes.Add(requiredType);
                }
            }
        }
        requiredTypes = requiredTypes.Distinct().ToList();
        //多余的删掉
        for (int i = DataBases.Count - 1; i >= 0; i--)
        {
            if (!requiredTypes.Contains(DataBases[i].TargetType))
            {
                DataBases.RemoveAt(i);
            }
        }
        //少的补上
        var addingTypes = requiredTypes.Except(havingTypes);
        foreach (var item in addingTypes)
        {
            addSelectionInfo(DataBases, item);
        }
        //排序
        var sorted = DataBases.ToList();
        sorted.Sort((a, b) => -a.OptionTypes.Count.CompareTo(b.OptionTypes.Count));
        DataBases.Clear();
        foreach (var item in sorted)
        {
            DataBases.Add(item);
        }
    }
    void updatePlugIns()
    {
        List<Type> havingTypes = WithPlugIns.ToList();
        List<Type> requiredTypes = new List<Type>();
        //直接选择
        requiredTypes.AddRange(StandPlugIns.Where(c => c.IsChecked).Select(c => c.TargetType));
        //结果输出
        foreach (var selectionInfo in Components.Where(c => c.SelectedType is not null))
        {
            requiredTypes.AddRange(PlugInContainmentDictionary[selectionInfo.SelectedType]);
        }
        requiredTypes = requiredTypes.Distinct().ToList();
        //多余的删掉
        for (int i = WithPlugIns.Count - 1; i >= 0; i--)
        {
            if (!requiredTypes.Contains(WithPlugIns[i]))
                WithPlugIns.RemoveAt(i);
        }
        //少的补上
        var addingTypes = requiredTypes.Except(havingTypes);
        foreach (var item in addingTypes)
        {
            WithPlugIns.Add(item);
        }

    }
    void updateConfigs()
    {
        //配置文件---------------------------
        List<Type> configsHavingTypes = Configs.Select(c => c.GetType()).ToList();
        List<Type> requiredTypes = new List<Type>();
        //模块
        if (EngineSelection.SelectedType != null)
        {
            foreach (var comp in ConfigContainmentDictionary[EngineSelection.SelectedType])
            {
                requiredTypes.Add(comp);
            }
        }
        if (EnginePreparerSelection.SelectedType != null)
        {
            foreach (var comp in ConfigContainmentDictionary[EnginePreparerSelection.SelectedType])
            {
                requiredTypes.Add(comp);
            }
        }
        if (EngineOutputSelection.SelectedType != null)
        {
            foreach (var comp in ConfigContainmentDictionary[EngineOutputSelection.SelectedType])
            {
                requiredTypes.Add(comp);
            }
        }
        //组件
        foreach (var item in Components.Where(s => s.SelectedType != null).Select(s => s.SelectedType))
        {
            requiredTypes.AddRange(ConfigContainmentDictionary[item]);
        }
        //插件
        foreach (var item in WithPlugIns)
        {
            requiredTypes.AddRange(ConfigContainmentDictionary[item]);
        }
        //多余的删掉
        for (int i = Configs.Count - 1; i >= 0; i--)
        {
            if (!requiredTypes.Contains(Configs[i].GetType()))
            {
                UnusedConfigs.Add(Configs[i]);
                Configs.RemoveAt(i);
            }
        }
        //少的补上
        var addingTypes = requiredTypes.Except(configsHavingTypes);
        foreach (var type in addingTypes)
        {
            var history = UnusedConfigs.Where(obj => obj.GetType() == type).FirstOrDefault();
            if (history is not null)
            {
                Configs.Add(history);
                UnusedConfigs.Remove(history);
            }
            else
            {
                ConfigerDI.SetSingleton(type);
                Configs.Add((AInferenceSetting)ConfigerDI.Get(type));
            }
        }
    }
    void updateInputs()
    {
        List<Type> havingTypes = WithInputs.ToList();
        List<Type> requiredTypes = new List<Type>();
        //结果输出
        if (EnginePreparerSelection.SelectedType is not null)
        {
            requiredTypes.AddRange(InputContainmentDictionary[EnginePreparerSelection.SelectedType]);
        }

        requiredTypes = requiredTypes.Distinct().ToList();
        //多余的删掉
        for (int i = WithInputs.Count - 1; i >= 0; i--)
        {
            if (!requiredTypes.Contains(WithInputs[i]))
                WithInputs.RemoveAt(i);
        }
        //少的补上
        var addingTypes = requiredTypes.Except(havingTypes);
        foreach (var item in addingTypes)
        {
            WithInputs.Add(item);
        }
    }
    void updateOutputs()
    {
        List<Type> havingTypes = WithOutputs.ToList();
        List<Type> requiredTypes = new List<Type>();
        //直接选择
        requiredTypes.AddRange(ResultGetters.Select(c => c.Type));

        requiredTypes = requiredTypes.Distinct().ToList();
        //多余的删掉
        for (int i = WithOutputs.Count - 1; i >= 0; i--)
        {
            if (!requiredTypes.Contains(WithOutputs[i]))
                WithOutputs.RemoveAt(i);
        }
        //少的补上
        var addingTypes = requiredTypes.Except(havingTypes);
        foreach (var item in addingTypes)
        {
            WithOutputs.Add(item);
        }
    }
    void updateConfigerPlugIns()
    {
        //更新数据库
        List<AGetter> unsetGetters = new();
        foreach (var getter in ConfigerDI.Getters)
        {
            if (getter.TargetType.IsAssignableTo(typeof(IInferenceDatabase)))
            {
                unsetGetters.Add(getter);
            }
        }
        foreach (var getter in unsetGetters)
        {
            ConfigerDI.Unset(getter);
        }
        foreach (var selection in DataBases.Where(s => s.SelectedType is not null))
        {
            ConfigerDI.SetSingleton(selection.SelectedType, selection.TargetType);
        }
        List<Type> forCheckRequiredTypes = new List<Type>();
        forCheckRequiredTypes.AddRange(Configs.Select(c => c.GetType()));
        if (EngineSelection.SelectedType is not null)
            forCheckRequiredTypes.Add(EngineSelection.SelectedType);
        forCheckRequiredTypes.AddRange(Components.Select(c => c.SelectedType?.GetType()));
        forCheckRequiredTypes = forCheckRequiredTypes.Where(t => t is not null).ToList();
        var requiredTypes = forCheckRequiredTypes.SelectMany(c =>
        {
            var attri = c.GetCustomAttribute<ConfigRequirePlugInsAttribute>();
            if (attri is not null)
            {
                return attri.RequirePlugIns.ToList();
            }
            else
            {
                return new List<Type>();
            }
        }).ToList();

        var havingTypes = ConfigerPlugIns.Select(p => p.GetType()).ToList();
        //多余的删掉
        for (int i = ConfigerPlugIns.Count - 1; i >= 0; i--)
        {
            if (!requiredTypes.Contains(ConfigerPlugIns[i].GetType()))
                ConfigerPlugIns.RemoveAt(i);
        }
        //少的补上
        var addingTypes = requiredTypes.Except(havingTypes);
        foreach (var type in addingTypes)
        {
            if (ConfigerDI.CanGet(type))
            {
                ConfigerDI.SetSingleton(type);
                IInferenceEngineBuilderConfigerPlugIn plugIn = (IInferenceEngineBuilderConfigerPlugIn)ConfigerDI.Get(type);
                plugIn.Configer = this;
                plugIn.UpdateEnableState();
                plugIn.Init();
                plugIn.Update();
                ConfigerPlugIns.Add(plugIn);
            }
        }
    }

    public void doConfigerPlugIns()
    {
        foreach (var plugIn in ConfigerPlugIns)
        {
            plugIn.UpdateEnableState();
            if (plugIn.IsEnable)
            {
                plugIn.Update();
            }
        }
    }

    public void CalDI()
    {
        if (EngineSelection.SelectedType == null) return;
        ErrorHints.Clear();
        compHasChanged = true;
        while (compHasChanged)
        {
            updateComp();
        }
        updateDataBases();
        updatePlugIns();
        updateConfigs();

        updateConfigerPlugIns();
        updateInputs();
        updateOutputs();
        doConfigerPlugIns();
    }
    void CheckDI()
    {
        ErrorHints.Clear();
        //引擎
        if (EngineSelection.SelectedType == null)
        {
            ErrorHints.Add("未选择推理引擎");
        }
        foreach (var c in Components)
        {
            if (c.SelectedType is null)
            {
                ErrorHints.Add($"未为{c.TargetType.GetCustomAttribute<DescriptionAttribute>()?.Description}选择具体类型");
            }
        }

        //检查同组 先找组别

        List<Type> groupTypes = new List<Type>();
        List<Type> toCheckGroupTypes = new List<Type>();
        toCheckGroupTypes
            .AddRange(Components.Where(c => c.SelectedType != null).Select(c => c.SelectedType));
        foreach (var c in Components.Where(c => c.SelectedType != null).Select(c => c.SelectedType))
        {
            var attriType = typeof(InferenceComponentGroupAttribute<>);
            Attribute attri = c.GetCustomAttribute(attriType);
            if (attri is null) continue;
            var groupType = attri.GetType().GetGenericArguments()[0];
            if (!groupTypes.Contains(attriType))
            {
                groupTypes.Add(groupType);
            }
        }
        foreach (var c in Components.Union(DataBases).Where(c => c.SelectedType != null))
        {
            foreach (var groupType in groupTypes)
            {
                bool hasGroupOption = false;
                foreach (var item in c.OptionTypes)
                {
                    var ts = InferenceComponentGroupAttribute.GetTIfExist(item);
                    if (ts.Count == 0) continue;
                    foreach (var t in ts)
                    {
                        if (t == groupType)
                        {
                            hasGroupOption = true;
                        }
                    }
                }

                if (hasGroupOption)
                {
                    var ts = InferenceComponentGroupAttribute.GetTIfExist(c.SelectedType);
                    if (!ts.Contains(groupType))
                    {
                        ErrorHints.Add($"{DescriptionAttribute.GetDescription(c.TargetType)}中{DescriptionAttribute.GetDescription(c.SelectedType)}不满足{DescriptionAttribute.GetDescription(groupType)}的要求");
                    }
                }
            }
        }

        foreach (var d in DataBases)
        {
            if (d.SelectedType is null)
            {
                ErrorHints.Add($"未为{d.TargetType.GetCustomAttribute<DescriptionAttribute>()?.Description}选择具体类型");
            }
        }
        foreach (var p in WithPlugIns.Union(StandPlugIns.Where(c => c.IsChecked).Select(c => c.TargetType)).Union(ProcessingInfoGetters.Select(g => g.Type).Union(ResultGetters.Select(g => g.Type))))
        {
            if (p == null) return;
            List<Type> dis = new List<Type>();
            foreach (var propertyInfo in p.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (propertyInfo.GetCustomAttribute<ZDIAttribute>() is not null)
                {
                    dis.Add(propertyInfo.PropertyType);
                }
            }
            foreach (var fieldInfo in p.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (fieldInfo.GetCustomAttribute<ZDIAttribute>() is not null)
                {
                    dis.Add(fieldInfo.FieldType);
                }
            }
            foreach (var type in dis)
            {
                if (type.IsAssignableTo(typeof(IInferenceEngine)))
                {
                    if (!Components.Union(DataBases).Any(c => c.SelectedType == type || c.TargetType == type))
                    {
                        ErrorHints.Add($"{p}缺少{type}引擎");
                    }
                }
                if (type.IsAssignableTo(typeof(IInferenceComponent)))
                {
                    if (!Components.Union(DataBases).Any(c => c.SelectedType == type || c.TargetType == type))
                    {
                        ErrorHints.Add($"{p}缺少{type}组件");
                    }
                }
                if (type.IsAssignableTo(typeof(IInferenceDatabase)))
                {
                    if (!Components.Union(DataBases).Any(c => c.SelectedType == type || c.TargetType == type))
                    {
                        ErrorHints.Add($"{p}缺少{type}数据库");
                    }
                }
            }
        }
        //配置器插件也应该全部启用
        foreach (var configerPlugIn in ConfigerPlugIns)
        {
            if (!configerPlugIn.IsEnable)
            {
                ErrorHints.AddRange(configerPlugIn.Hints);
            }
        }
    }

    #endregion


    #region 操作
    public List<string> GetErrorHints()
    {
        return ErrorHints;
    }

    public bool CanBuild()
    {
        CheckDI();
        return ErrorHints.Count == 0;
    }
    public EngineConfig BuildEngineConfig()
    {
        EngineConfig config = new EngineConfig();
        config.Engine = EngineSelection.SelectedType.AssemblyQualifiedName;
        config.EnginePreparer = EnginePreparerSelection.SelectedType.AssemblyQualifiedName;
        config.EngineOutputGetter = EngineOutputSelection.SelectedType.AssemblyQualifiedName;

        config.Components = Components.Select(selectInfo => (selectInfo.SelectedType.AssemblyQualifiedName, selectInfo.TargetType.AssemblyQualifiedName)).ToList();
        config.WithInferenceInputs = WithInputs.Select(t => t.AssemblyQualifiedName).ToList();
        config.PlugIns = WithPlugIns
            .Union(StandPlugIns.Where(checkInfo => checkInfo.IsChecked).Select(checkInfo => checkInfo.TargetType))
            .Select(t => t.AssemblyQualifiedName).ToList();
        config.DataBases = DataBases.Select(selectInfo => (selectInfo.SelectedType.AssemblyQualifiedName, selectInfo.TargetType.AssemblyQualifiedName)).ToList();
        config.Configs = Configs.Union(ExConfigs).Select(kv => (kv.GetType().AssemblyQualifiedName, YAML.Serialize(kv))).ToList();
        config.ProcessInfoGetters = ProcessingInfoGetters
            .Select(pi =>
            {

                GetterConfig getterConfig = new GetterConfig();
                if (pi.Type == null) return getterConfig;
                getterConfig.TypeName = pi.Type.AssemblyQualifiedName;
                foreach (var config in pi.Configs)
                {
                    getterConfig.Configs.Add(config.GetType().AssemblyQualifiedName, YAML.Serialize(config));
                }
                foreach (var plugInType in pi.PlugInTypes)
                {
                    getterConfig.PlugIns.Add(plugInType.AssemblyQualifiedName);
                }
                return getterConfig;
            }).ToList();
        config.ResultGetters = ResultGetters
            .Select(pi =>
            {
                GetterConfig getterConfig = new GetterConfig();
                if (pi.Type == null) return getterConfig;
                getterConfig.TypeName = pi.Type.AssemblyQualifiedName;
                foreach (var config in pi.Configs)
                {
                    getterConfig.Configs.Add(config.GetType().AssemblyQualifiedName, YAML.Serialize(config));
                }
                foreach (var plugInType in pi.PlugInTypes)
                {
                    getterConfig.PlugIns.Add(plugInType.AssemblyQualifiedName);
                }
                return getterConfig;
            }).ToList();
        return config;
    }

    public void AddProcessingInfoGetter(Type getterType)
    {
        GetterInfo getterInfo = new GetterInfo();
        getterInfo.Type = getterType;
        foreach (var plugInType in PlugInContainmentDictionary[getterType])
        {
            getterInfo.PlugInTypes.Add(plugInType);
        }
        foreach (var configType in ConfigContainmentDictionary[getterType])
        {
            var config = (AInferenceSetting)Activator.CreateInstance(configType);
            getterInfo.Configs.Add(config);
        }
        ProcessingInfoGetters.Add(getterInfo);
    }
    public void AddResultGetter(Type getterType)
    {
        GetterInfo getterInfo = new GetterInfo();
        getterInfo.Type = getterType;
        foreach (var plugInType in PlugInContainmentDictionary[getterType])
        {
            getterInfo.PlugInTypes.Add(plugInType);
        }
        foreach (var configType in ConfigContainmentDictionary[getterType])
        {
            var config = (AInferenceSetting)Activator.CreateInstance(configType);
            getterInfo.Configs.Add(config);
        }
        ResultGetters.Add(getterInfo);
    }
    public void DelProcessingInfoGetter(GetterInfo getterInfo)
    {
        ProcessingInfoGetters.Remove(getterInfo);
    }
    public void DelResultGetter(GetterInfo getterInfo)
    {
        ResultGetters.Remove(getterInfo);
    }
    #endregion

}
