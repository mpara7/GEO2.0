using System.Reflection;
namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models;
public class RuleInfo
{
    public string Name { get; init; }
    public string Description { get; init; }
    public Type DefineType { get; init; }
    public List<Type> LeftTypes { get; init; } = new List<Type>();
    public List<Type> RightTypes { get; init; } = new List<Type>();
    public RuleInfo(MethodInfo ruleMethod)
    {
        RuleMethod = ruleMethod;
        Name = ruleMethod.Name;
        DefineType = RuleMethod.DeclaringType;
        LeftTypes = RuleMethod.GetParameters().Select(p => p.ParameterType).ToList();
    }
    public MethodInfo RuleMethod { get; set; }
    public PlaneRuleClass RuleClass { get; set; }
    public override string ToString()
    {
        return Name;
    }
}
