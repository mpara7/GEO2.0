global using GeoInferenceEngine.Backbone.Abstractions.IOs.Inputs;
global using GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs;
global using GeoInferenceEngine.Backbone.Abstractions.Modules;
global using GeoInferenceEngine.Backbone.Abstractions.Parts;
global using GeoInferenceEngine.EquivalencePlaneGeometry.Engine.Modules;
global using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.Models;
global using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.PRs;
global using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.StopJudges;
global using GeoInferenceEngine.PlaneKnowledges.Knowledges;
global using GeoInferenceEngine.Knowledges.Imps.IOs.Inputs;
global using GeoInferenceEngine.Knowledges.Imps.IOs.Outputs;
global using ZTool.Infrastructures.ZDI;
using System.Reflection;
namespace GeoInferenceEngine.EquivalencePlaneGeometry;
public static class EquivalencePlaneGeometryDLLInfo
{
    public static List<string> GetAllRuleNames()
    {
        var RuleAssemblies = new Assembly[] { typeof(EquivalencePlaneGeometryDLLInfo).Assembly, };
        List<string> names = new List<string>();

        foreach (var assembly in RuleAssemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(PlaneRuleClass)) && !type.IsAbstract)
                {

                    foreach (var methodInfo in type.GetMethods())
                    {
                        if (methodInfo.DeclaringType == type)
                        {
                            names.Add(methodInfo.Name);
                        }
                    }
                }
            }
        }
        return names;
    }
}
