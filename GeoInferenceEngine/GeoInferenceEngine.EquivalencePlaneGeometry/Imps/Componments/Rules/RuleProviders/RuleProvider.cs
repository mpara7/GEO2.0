using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using System.Reflection;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.PRs.RuleProviders
{
    [Description("规则提供器")]
    public class RuleProvider : IInferenceComponent
    {
        public Action ReBuilded { get; set; }
        [ZDI]
        protected ZDIContainer container;
        [ZDI]
        protected PRAssembliesBase AssemblyBase { get; set; }
        [ZDI]
        protected RuleBase RuleBase { get; set; }
        public virtual void BuildRules(params PlaneRuleType[] ruleTypes)
        {
            RuleBase.RuleInfos.Clear();
            foreach (var assembly in AssemblyBase.RuleAssemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(PlaneRuleClass)) && !type.IsAbstract)
                    {
                        var t = type.GetCustomAttribute<PlaneRuleTypeAttribute>();
                        foreach (var ruleType in ruleTypes)
                        {
                            if (t is not null && t.RuleType == ruleType)
                            {
                                container.SetSingleton(type);
                                var ruleClass = container.Get(type);

                                foreach (var methodInfo in type.GetMethods())
                                {
                                    if (methodInfo.DeclaringType == type)
                                    {
                                        RuleInfo runningRule = new RuleInfo(methodInfo);
                                        runningRule.RuleClass = (PlaneRuleClass)ruleClass;
                                        RuleBase.RuleInfos.Add(runningRule);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            ReBuilded?.Invoke();
        }
    }
}
