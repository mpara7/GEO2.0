using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.PlaneKnowledges.CKnowledges;
using GeoInferenceEngine.Knowledges;
using System.Text;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.IO.Outputs
{
    public class KnowledgeBaseOutputMakerConfig : AInferenceSetting
    {
        public bool IsSlim { get; set; } = true;
    }
    [Description("知识库")]
    public class KnowledgeBaseOutputMaker : IInferenceOutputMaker<KnowledgeBaseOutput>
    {
        [ZDI]
        KnowledgeBaseOutputMakerConfig config;
        [ZDI]
        FormularBase formularBase { get; set; }
        [ZDI]
        KnowledgeBase knowledgeBase { get; set; }
        string name;
        public string Name { get => name; set => name = value; }
        public List<string> RatioInfos { get; set; } = new();
        public List<string> NormalGeoEquations { get; set; } = new();
        public (string, List<KnowledgeInfo>) makeInfos(Type type, List<Knowledge> knowledges)
        {
            var name = DescriptionAttribute.GetDescription(type);
            var list = new List<KnowledgeInfo>();

            for (int i = 0; i < knowledges.Count; i++)
            {

                var knowledge = knowledges[i];
                if (config.IsSlim)
                {
                    if (!knowledge.IsAvailable) continue;
                }
                KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
                knowledgeInfo.Content = knowledge.ToString();
                knowledgeInfo.Reason = knowledge.Reason;
                knowledgeInfo.IsAvailable = knowledge.IsAvailable;

                for (int j = 0; j < knowledge.Conditions.Count; j++)
                {
                    var condition = knowledge.Conditions[j];
                    CondictionInfo condictionInfo = new CondictionInfo();

                    var conditionTypeName = "";
                    if (condition is GeoEquation e)
                    {
                        if (formularBase.DistanceSimpleGeoEquationInfos.ContainsKey(e.HashCode))
                        {
                            conditionTypeName = nameof(formularBase.DistanceSimpleGeoEquationInfos);
                        }
                    }
                    else
                    {
                        conditionTypeName = DescriptionAttribute.GetDescription(condition.GetType());
                    }

                    condictionInfo.Type = conditionTypeName;
                    condictionInfo.Index = (int)condition.PosIndex;
                    knowledgeInfo.Conditions.Add(condictionInfo);
                }
                list.Add(knowledgeInfo);
            }
            return (name, list);
        }
        public KnowledgeBaseOutput Make()
        {
            var output = new KnowledgeBaseOutput();
            lock (knowledgeBase)
            {
                foreach (var cate in knowledgeBase.Categories)
                {
                    if (cate.Value.Count == 0) { continue; }

                    var type = cate.Key;
                    if (type.IsAssignableTo(typeof(ConstructiveKnowledge)))
                    {
                        var result = makeInfos(cate.Key, cate.Value);
                        output.Constrcutives.Add(result.Item1, result.Item2);
                    }
                    else if (type.IsAssignableTo(typeof(PlaneFigure))
                        && type != typeof(Triangle) && type != typeof(Quadriliateral))
                    {
                        var result = makeInfos(cate.Key, cate.Value);
                        output.SpecialFigures.Add(result.Item1, result.Item2);
                    }
                    else if (type.IsAssignableTo(typeof(Figure)))
                    {
                        var result = makeInfos(cate.Key, cate.Value);
                        output.Figures.Add(result.Item1, result.Item2);
                    }
                    else if (type.IsAssignableTo(typeof(PlainRelation)) || type.IsAssignableTo(typeof(NegatingKnowledge)))
                    {
                        var result = makeInfos(cate.Key, cate.Value);
                        output.PlainRelations.Add(result.Item1, result.Item2);
                    }
                    else
                    {
                        var result = makeInfos(cate.Key, cate.Value);
                        output.Relations.Add(result.Item1, result.Item2);
                    }
                }
            }
            lock (formularBase)
            {
                #region RatioInfos
                output.RatioInfos.Add(nameof(formularBase.DistanceRatioInfos), new());
                foreach (var item in formularBase.DistanceRatioInfos)
                {
                    KnowledgeInfo knowledgeInfo = new KnowledgeInfo();

                    knowledgeInfo.Content = item.ToString();
                    output.RatioInfos[nameof(formularBase.DistanceRatioInfos)].Add(knowledgeInfo);
                }

                output.RatioInfos.Add(nameof(formularBase.AngleRatioInfos), new());
                foreach (var item in formularBase.AngleRatioInfos)
                {
                    KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
                    knowledgeInfo.Content = item.ToString();
                    output.RatioInfos[nameof(formularBase.AngleRatioInfos)].Add(knowledgeInfo);
                }
                #endregion
                #region equations

                //length
                output.Equations.Add(nameof(formularBase.DistanceSimpleGeoEquationInfos), new());
                foreach (var item in formularBase.DistanceSimpleGeoEquationInfos)
                {
                    KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
                    knowledgeInfo.Reason = item.Value.GeoEquation.Reason;
                    knowledgeInfo.Content = item.Value.GeoEquation.ToString();
                    output.Equations[nameof(formularBase.DistanceSimpleGeoEquationInfos)].Add(knowledgeInfo);
                }

                output.Equations.Add(nameof(formularBase.DistanceAdditionGeoEquationInfos), new());
                foreach (var item in formularBase.DistanceAdditionGeoEquationInfos)
                {
                    KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
                    knowledgeInfo.Reason = item.Value.GeoEquation.Reason;
                    knowledgeInfo.Content = item.Value.GeoEquation.ToString();
                    output.Equations[nameof(formularBase.DistanceAdditionGeoEquationInfos)].Add(knowledgeInfo);
                }
                output.Equations.Add(nameof(formularBase.DistanceMultiplicationGeoEquationInfos), new());
                foreach (var item in formularBase.DistanceMultiplicationGeoEquationInfos)
                {
                    KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
                    knowledgeInfo.Reason = item.Value.GeoEquation.Reason;
                    knowledgeInfo.Content = item.Value.GeoEquation.ToString();
                    output.Equations[nameof(formularBase.DistanceMultiplicationGeoEquationInfos)].Add(knowledgeInfo);
                }
                output.Equations.Add(nameof(formularBase.DistanceComplexGeoEquationInfos), new());
                foreach (var item in formularBase.DistanceComplexGeoEquationInfos)
                {
                    KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
                    knowledgeInfo.Reason = item.Value.GeoEquation.Reason;
                    knowledgeInfo.Content = item.Value.GeoEquation.ToString();
                    output.Equations[nameof(formularBase.DistanceComplexGeoEquationInfos)].Add(knowledgeInfo);
                }


                //angle
                output.Equations.Add(nameof(formularBase.AngleAdditionGeoEquationInfos), new());
                foreach (var item in formularBase.AngleAdditionGeoEquationInfos)
                {
                    KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
                    knowledgeInfo.Reason = item.Value.GeoEquation.Reason;
                    knowledgeInfo.Content = item.Value.GeoEquation.ToString();
                    output.Equations[nameof(formularBase.AngleAdditionGeoEquationInfos)].Add(knowledgeInfo);
                }
                output.Equations.Add(nameof(formularBase.AngleMultiplicationGeoEquationInfos), new());
                foreach (var item in formularBase.AngleMultiplicationGeoEquationInfos)
                {
                    KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
                    knowledgeInfo.Reason = item.Value.GeoEquation.Reason;
                    knowledgeInfo.Content = item.Value.GeoEquation.ToString();
                    output.Equations[nameof(formularBase.AngleMultiplicationGeoEquationInfos)].Add(knowledgeInfo);
                }
                output.Equations.Add(nameof(formularBase.AngleComplexGeoEquationInfos), new());
                foreach (var item in formularBase.AngleComplexGeoEquationInfos)
                {
                    KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
                    knowledgeInfo.Reason = item.Value.GeoEquation.Reason;
                    knowledgeInfo.Content = item.Value.GeoEquation.ToString();
                    output.Equations[nameof(formularBase.AngleComplexGeoEquationInfos)].Add(knowledgeInfo);
                }

                

                output.Equations.Add(nameof(formularBase.AngleSimpleGeoEquationInfos), new());
                foreach (var item in formularBase.AngleSimpleGeoEquationInfos)
                {
                    KnowledgeInfo knowledgeInfo = new KnowledgeInfo();
                    knowledgeInfo.Reason = item.Value.GeoEquation.Reason;
                    knowledgeInfo.Content = item.Value.GeoEquation.ToString();
                    output.Equations[nameof(formularBase.AngleSimpleGeoEquationInfos)].Add(knowledgeInfo);
                }
                #endregion
            }
            return output;
        }
    }
}
