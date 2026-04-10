using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Executors;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.PRs.RuleProviders;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.Knowledges;
using GeoInferenceEngine.PredicateShared.Models;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns.CompleteQuadrilateralInferencePlugIns
{
    [InferenceStandPlugIn]
    [Description("完全四边形推理插件")]
    public class CompleteQuadInferencePlugIn : IInferencePlugIn
    {
        //[ZDI]
        //EquationToKnowledgeHandler equationToKnowledgeHandler;
        [ZDI]
        KnowledgeBase knowledgeGetter;
        [ZDI]
        RuleProvider RuleProvider;
        [ZDI]
        RuleBase RuleBase { get; set; }
        [ZDI]
        ZDIContainer container { get; set; }
        [ZDI]
        PlaneExecutor PlaneExecutor { get; set; }
        [ZDI]
        KnowledgeAddProcessor AddProcessor;
        bool isInUse = false;
        public void Init()
        {
            PlaneExecutor.Initing += () =>
            {
                isInUse = true;
            };
            RuleProvider.ReBuilded += () =>
            {
                if (!isInUse) return;
                List<Type> types = new List<Type>()
                { typeof(CompleteQuadInferenceRule)};
                foreach (var type in types)
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
            };
            //equationToKnowledgeHandler.OnMultiplicationGeoEquationGenerrated += GenerateCUAD;
        }


        private void GenerateCUAD(GeoEquationInfo equationInfo)
        {

            List<Segment> m = new List<Segment>();
            List<Segment> d = new List<Segment>();
            List<Point> CQ0 = new List<Point>();
            List<Point> cq2 = new List<Point>();
            var M = equationInfo.GeoEquation.LeftPart;

            //if (M is ProductNode product)
            //{
            //    if (product.Multipliers.Count != 3 ||
            //        product.Divisors.Count != 3)
            //        return;

            //    foreach (var item in product.Multipliers)
            //    {
            //        if (item is GeoPropNode proNode)
            //        {
            //            if (proNode.Knowledge is Segment segment)
            //            {
            //                m.Add(segment);
            //            }
            //            else return;
            //        }
            //        else return;
            //    }
            //    foreach (var item in product.Divisors)
            //    {
            //        if (item is GeoPropNode proNode)
            //        {
            //            if (proNode.Knowledge is Segment segment)
            //            {
            //                d.Add(segment);
            //            }
            //            else return;
            //        }
            //        else return;
            //    }
            //    var mpoint = m.SelectMany(segment => segment.Properties);
            //    var dpoint = d.SelectMany(segment => segment.Properties);
            //    mpoint = mpoint.Distinct();
            //    dpoint = dpoint.Distinct();
            //    if (mpoint.Count() != 6 || dpoint.Count() != 6) return;

            //    var start = m[0][0];
            //    CQ0.Add((Point)start);
            //    var travel = m[0][1];
            //    cq2.Add((Point)travel);

            //    var d1 = d.FirstOrDefault(segment => segment.Properties.Contains(travel));
            //    if (d1 == null) { return; }
            //    travel = d1.Properties.FirstOrDefault(Point => Point != travel);
            //    CQ0.Add((Point)travel);

            //    var m1 = m.FirstOrDefault(segment => segment.Properties.Contains(travel));
            //    if (m1 == null) { return; }
            //    travel = m1.Properties.FirstOrDefault(Point => Point != travel);
            //    cq2.Add((Point)travel);

            //    var d2 = d.FirstOrDefault(segment => segment.Properties.Contains(travel));
            //    if (d2 == null) { return; }
            //    travel = d2.Properties.FirstOrDefault(Point => Point != travel);
            //    CQ0.Add((Point)travel);

            //    var m2 = m.FirstOrDefault(segment => segment.Properties.Contains(travel));
            //    if (m2 == null) { return; }
            //    travel = m2.Properties.FirstOrDefault(Point => Point != travel);
            //    cq2.Add((Point)travel);

            //    var d3 = d.FirstOrDefault(segment => segment.Properties.Contains(travel));
            //    if (d3 == null) { return; }
            //    travel = d3.Properties.FirstOrDefault(Point => Point != travel);

            //    if (travel != start) return;

            //    if (knowledgeGetter.HasColine(CQ0[0], CQ0[1], CQ0[2]))
            //    {
            //        var temp = CQ0;
            //        CQ0 = cq2;
            //        cq2 = temp;
            //    }
            //    CQ0.Sort((a, b) => a.PosIndex.CompareTo(b.PosIndex));
            //    CQ0.AddRange(cq2);
            //    if (!(knowledgeGetter.HasColine(CQ0[0], CQ0[1], CQ0[3]))) // 如果0 1 3不共线，那么判断0 1 4，如果014还不共线，那么015一定共线，交换3和5
            //    {
            //        if (!(knowledgeGetter.HasColine(CQ0[0], CQ0[1], CQ0[4])))
            //        {
            //            Point SP = CQ0[5];
            //            CQ0[5] = CQ0[3];
            //            CQ0[3] = SP;
            //        }
            //        else
            //        {
            //            Point SP = CQ0[4];
            //            CQ0[4] = CQ0[3];
            //            CQ0[3] = SP;
            //        }
            //        if (!(knowledgeGetter.HasColine(CQ0[1], CQ0[2], CQ0[4])))
            //        {
            //            Point SP = CQ0[5];
            //            CQ0[5] = CQ0[4];
            //            CQ0[4] = SP;
            //        }
            //    }
            //    else
            //    {
            //        if (!(knowledgeGetter.HasColine(CQ0[1], CQ0[2], CQ0[4])))
            //        {
            //            Point SP = CQ0[5];
            //            CQ0[5] = CQ0[4];
            //            CQ0[4] = SP;
            //        }
            //    }

            //    //if (!(knowledgeGetter.HasCollinear(CQ1[1], CQ1[2], CQ1[5]))) return;

            //    CompleteQuadriliateral pred0 = new CompleteQuadriliateral(CQ0[0], CQ0[1], CQ0[2], CQ0[3], CQ0[4], CQ0[5]);

            //    pred0.AddReason("由梅涅劳斯定理生成完全四边形");
            //    pred0.AddCondition(equationInfo.GeoEquation);
            //    AddProcessor.Add(equationInfo.GeoEquation);
            //    AddProcessor.Add(pred0);
            //}
        }
    }
}
