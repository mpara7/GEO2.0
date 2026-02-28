using GeoInferenceEngine.Backbone.AppConfiger;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using System.Reflection;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Configs.ConfigerPlugIns;
/// <summary>
/// 从RuleClass中搜索所需要的配置
/// </summary>
public class RuleRequireConfigConfigerPlugIn : IInferenceEngineBuilderConfigerPlugIn
{
    PRAssembliesBase _AAssemblyBase;
    List<string> hints = new List<string>();
    public List<string> Hints => hints;
    bool isEnable;
    public bool IsEnable { get => isEnable; set => isEnable = value; }
    public InferenceEngineConfiger Configer { set => configer = value; }
    InferenceEngineConfiger configer;
    public bool CheckIsEnable()
    {
        return configer.ConfigerDI.CanGet<PRAssembliesBase>();
    }

    public void Disable()
    {
        foreach (var item in aConfigInputs)
        {
            configer.ExConfigs.Remove(item);
            configer.UnusedConfigs.Add(item);
        }
    }

    public void Enable()
    {
        Update();
    }
    public void Init()
    {

    }
    List<AInferenceSetting> aConfigInputs = new List<AInferenceSetting>();
    public void Update()
    {
        if (!configer.ConfigerDI.HasSet<PRAssembliesBase>())
            return;
        var AAssemblyBase = configer.ConfigerDI.Get<PRAssembliesBase>();
        if (_AAssemblyBase == AAssemblyBase)
            return;
        else
            _AAssemblyBase = AAssemblyBase;
        foreach (var item in aConfigInputs)
        {
            configer.ExConfigs.Remove(item);
        }

        List<Type> configTypes = new List<Type>();
        foreach (var assembly in _AAssemblyBase.RuleAssemblies)
        {
            var ruleClass = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(PlaneRuleClass)));
            var configs = _findAssignable(ruleClass, typeof(AInferenceSetting));
            configTypes.AddRange(configs);
        }
        configTypes = configTypes.Distinct().ToList();
        foreach (var type in configTypes)
        {
            if (configer.ExConfigs.Any(e => e.GetType() == type))
                continue;
            var history = configer.UnusedConfigs.FirstOrDefault(c => c.GetType() == type);

            if (history is not null)
            {
                configer.ExConfigs.Add(history);
                configer.UnusedConfigs.Remove(history);
                aConfigInputs.Add(history);
            }
            else
            {
                var thisHistoryConfig = aConfigInputs.FirstOrDefault(c => c.GetType() == type);
                if (thisHistoryConfig is not null)
                {
                    configer.ExConfigs.Add(thisHistoryConfig);
                }
                else
                {
                    AInferenceSetting config = (AInferenceSetting)Activator.CreateInstance(type);
                    configer.ExConfigs.Add(config);
                    aConfigInputs.Add(config);
                }
            }
        }

    }

    List<Type> _findAssignable(IEnumerable<Type> types, Type target)
    {
        List<Type> list = new();
        //搜索组件的包含关系
        foreach (var type in types)
        {
            //判断具有DI特性的字段/属性是否继承/实现于对应类型
            var fileds = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var di = prop.GetCustomAttribute<ZDIAttribute>();
                if (di is not null)
                {
                    if (prop.PropertyType.IsAssignableTo(target))
                        list.Add(prop.PropertyType);
                }
            }
            foreach (var field in fileds)
            {
                var di = field.GetCustomAttribute<ZDIAttribute>();
                if (di is not null)
                {
                    if (field.FieldType.IsAssignableTo(target))
                        list.Add(field.FieldType);
                }
            }
        }
        list = list.Distinct().ToList();
        return list;
    }
}
