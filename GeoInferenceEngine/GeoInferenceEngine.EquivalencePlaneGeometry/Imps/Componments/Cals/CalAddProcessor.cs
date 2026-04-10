using GeoInferenceEngine.Knowledges.Imps.Componments;
using GeoInferenceEngine.PredicateShared.Models;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal
{
    [Description("等式知识添加处理")]
    public class CalAddProcessor : IInferenceComponent
    {
        [ZDI]
        ExprService ExprService;
        [ZDI]
        KnowledgeAddProcessor KnowledgeAddProcessor;
        [ZDI]
        Logger logger;
        [ZDI]
        FormularBase FormularBase { get; set; }
        public Action<GeoEquationInfo> DistanceRatioAdding;
        public Action<GeoEquationInfo> DistanceAdditionAdding;
        public Action<GeoEquationInfo> DistanceMultiplicationAdding;
        public Action<GeoEquationInfo> DistanceComplexAdding;
        public Action<GeoEquationInfo> AngleRatioAdding;
        public Action<GeoEquationInfo> AngleAdditionAdding;
        public Action<GeoEquationInfo> AngleMultiplicationAdding;
        public Action<GeoEquationInfo> AngleComplexAdding;
        public void Add(GeoEquation equation)
        {
            var equationInfo = ExprService.GetEquationInfo(equation);
            equationInfo.SimplifyByRationInfos();
            equationInfo.ExtractInfo();
            equationInfo.TryMakeNewKnowledge();

            if (FormularBase.AllGeoEquationInfos.ContainsKey(equationInfo.HashCode)) return;
            if (equationInfo.Type == GeoEquationTypes.Value)
            {
                if (equationInfo.Unit == GeoEquationUnits.Distance)
                {
                    if (equationInfo.EquivalenceKnowledge is not null)
                    {
                        var a=  KnowledgeAddProcessor.Add(equationInfo.EquivalenceKnowledge);
                        UpdateRatioInfo(equationInfo.Mut, equationInfo.Value, a, GeoEquationUnits.Distance);
                    }
                    else
                    {
                        UpdateRatioInfo(equationInfo.Mut, equationInfo.Value, equation, GeoEquationUnits.Distance);
                    }
                    FormularBase.DistanceSimpleGeoEquationInfos
                           .Add(equationInfo.HashCode, equationInfo);
                }
                else if (equationInfo.Unit == GeoEquationUnits.Angle)
                {
                    if (equationInfo.EquivalenceKnowledge is not null)
                    {
                        var a = KnowledgeAddProcessor.Add(equationInfo.EquivalenceKnowledge);
                        UpdateRatioInfo(equationInfo.Mut, equationInfo.Value, a, GeoEquationUnits.Angle);
                    }
                    else
                    {
                        UpdateRatioInfo(equationInfo.Mut, equationInfo.Value, equation, GeoEquationUnits.Angle);
                    }
                    FormularBase.AngleSimpleGeoEquationInfos
                        .Add(equationInfo.HashCode, equationInfo);
                }
                FormularBase.AllGeoEquationInfos.Add(equationInfo.HashCode, equationInfo);
            }
            else if (equationInfo.Type == GeoEquationTypes.Ratio)
            {
                if (equationInfo.Unit == GeoEquationUnits.Distance)
                {
                    if (equationInfo.EquivalenceKnowledge is not null)
                    {
                        var a = KnowledgeAddProcessor.Add(equationInfo.EquivalenceKnowledge);
                        UpdateRatioInfo(equationInfo.LeftMut, equationInfo.RightMut, equationInfo.Ratio, a, equationInfo.Unit);
                    }
                    else
                    {
                        UpdateRatioInfo(equationInfo.LeftMut, equationInfo.RightMut, equationInfo.Ratio, equation, equationInfo.Unit);
                    }
                    FormularBase.DistanceSimpleGeoEquationInfos
                           .Add(equationInfo.HashCode, equationInfo);
                }
                else if (equationInfo.Unit == GeoEquationUnits.Angle)
                {
                    if (equationInfo.EquivalenceKnowledge is not null)
                    {
                        var a = KnowledgeAddProcessor.Add(equationInfo.EquivalenceKnowledge);
                        UpdateRatioInfo(equationInfo.LeftMut, equationInfo.RightMut, equationInfo.Ratio, a, equationInfo.Unit);
                    }
                    else
                    {
                        UpdateRatioInfo(equationInfo.LeftMut, equationInfo.RightMut, equationInfo.Ratio, equation, equationInfo.Unit);
                    }
                    FormularBase.AngleSimpleGeoEquationInfos
                        .Add(equationInfo.HashCode, equationInfo);
                }
                FormularBase.AllGeoEquationInfos.Add(equationInfo.HashCode, equationInfo);
            }
            else if (equationInfo.Type == GeoEquationTypes.Addition)
            {
                if(equationInfo.SimplifiedGeoequation is not null)
                {
                    Add(equationInfo.SimplifiedGeoequation);
                }
                else
                {
                    if (equationInfo.Unit == GeoEquationUnits.Distance)
                    {
                        FormularBase.DistanceAdditionGeoEquationInfos
                        .Add(equationInfo.HashCode, equationInfo);
                    }
                    else if (equationInfo.Unit == GeoEquationUnits.Angle)
                    {
                        FormularBase.AngleAdditionGeoEquationInfos
                        .Add(equationInfo.HashCode, equationInfo);
                    }
                }
                FormularBase.AllGeoEquationInfos.Add(equationInfo.HashCode, equationInfo);
                
            }
            else if (equationInfo.Type == GeoEquationTypes.Multiplication)
            {
                if (equationInfo.SimplifiedGeoequation is not null)
                {
                    Add(equationInfo.SimplifiedGeoequation);
                }
                else
                {
                    if (equationInfo.Unit == GeoEquationUnits.Distance)
                    {
                        FormularBase.DistanceMultiplicationGeoEquationInfos
                            .Add(equationInfo.HashCode, equationInfo);
                    }
                    else if (equationInfo.Unit == GeoEquationUnits.Angle)
                    {
                        FormularBase.AngleMultiplicationGeoEquationInfos
                            .Add(equationInfo.HashCode, equationInfo);
                    }
                }
                FormularBase.AllGeoEquationInfos.Add(equationInfo.HashCode, equationInfo);
            }
            else if (equationInfo.Type == GeoEquationTypes.Complex)
            {
                if (equationInfo.Unit == GeoEquationUnits.Distance)
                {
                    FormularBase.DistanceComplexGeoEquationInfos
                    .Add(equationInfo.HashCode, equationInfo);
                }
                else if (equationInfo.Unit == GeoEquationUnits.Angle)
                {
                    FormularBase.AngleComplexGeoEquationInfos
                    .Add(equationInfo.HashCode, equationInfo);
                }
                FormularBase.AllGeoEquationInfos.Add(equationInfo.HashCode, equationInfo);
            }
        }
        public void UpdateRatioInfo(Mut mut, Expr value, Knowledge knowledge, GeoEquationUnits unit= GeoEquationUnits.Distance)
        {
            var ratioInfo = FormularBase.TryGetRatioInfo(mut);
            if (ratioInfo is null)
            {
                if (unit == GeoEquationUnits.Angle)
                    ratioInfo = FormularBase.AngleRatioInfos.FirstOrDefault(r => r.ActualValue is not null);
                else if (unit == GeoEquationUnits.Distance)
                    ratioInfo = FormularBase.DistanceRatioInfos.FirstOrDefault(r => r.ActualValue is not null);

                //如果存在节点有具体值
                if (ratioInfo is not null)
                {
                    ratioInfo.AddByValue(mut, value, knowledge);
                    FormularBase.RatioInfoIndex.Add(mut, ratioInfo);
                }
                else
                {
                    ratioInfo = new RatioInfo();
                    ratioInfo.AddValue(mut, value, knowledge);

                    FormularBase.RatioInfoIndex.Add(mut, ratioInfo);
                    if (unit == GeoEquationUnits.Angle)
                        FormularBase.AngleRatioInfos.Add(ratioInfo);
                    else if (unit == GeoEquationUnits.Distance)
                        FormularBase.DistanceRatioInfos.Add(ratioInfo);
                }
            }
            else
            {
                //已有具体值就不再添加了
                if (ratioInfo.ActualValue is null)
                {
                    ratioInfo.AddValue(mut, value, knowledge);
                    //检查是否可以合并比值节点
                    CheckIfCanBeAbsordByValue(unit);
                }
            }
        }
        public void UpdateRatioInfo(Mut expr1, Mut expr2, Expr ratio, Knowledge knowledge, GeoEquationUnits unit= GeoEquationUnits.Distance)
        {
            var ratioInfo1 = FormularBase.TryGetRatioInfo(expr1);
            var ratioInfo2 = FormularBase.TryGetRatioInfo(expr2);
            //根据可变变量是否存在对于比例关系进行不同操作
            if (ratioInfo1 is null && ratioInfo2 is null)
            {
                var ratioInfo = new RatioInfo();
                ratioInfo.AddFirst(expr1, expr2, ratio, knowledge);
                if (unit == GeoEquationUnits.Angle)
                {
                    FormularBase.AngleRatioInfos.Add(ratioInfo);
                    FormularBase.RatioInfoIndex.Add(expr1, ratioInfo);
                    FormularBase.RatioInfoIndex.Add(expr2, ratioInfo);
                }
                else
                {
                    FormularBase.DistanceRatioInfos.Add(ratioInfo);
                    FormularBase.RatioInfoIndex.Add(expr1, ratioInfo);
                    FormularBase.RatioInfoIndex.Add(expr2, ratioInfo);
                }
            }
            else if (ratioInfo1 is not null && ratioInfo2 is null)
            {
                ratioInfo1.Add(expr1, expr2, ratio, knowledge);
                FormularBase.RatioInfoIndex.Add(expr2, ratioInfo1);
            }
            else if (ratioInfo1 is null && ratioInfo2 is not null)
            {
                ratioInfo2.Add(expr2, expr1, ratio.Invert(), knowledge);
                FormularBase.RatioInfoIndex.Add(expr1, ratioInfo2);
            }
            else if (ratioInfo1 is not null && ratioInfo2 is not null)
            {
                //如果已经在同一个比值信息内
                if (ratioInfo1 == ratioInfo2) return;
                //Debug.Assert(ratioInfo2.ToString() == "(90=1*角B-A-PD的大小=1*角B-C-ED的大小)");
                ratioInfo1.AbsorbByMutableRatio(ratioInfo2, expr2, expr1, ratio, knowledge);
                foreach (var item in ratioInfo2.CoffDict.Keys)
                {
                    logger.Debug($"更新{item}所在的RatioInfo{FormularBase.RatioInfoIndex[item]}->{ratioInfo1}");
                    //Debug.Assert(item.ToString()== "角B-C-ED");
                    FormularBase.RatioInfoIndex[item] = ratioInfo1;
                }
            }
        }
        public void CheckIfCanBeAbsordByValue(GeoEquationUnits unit)
        {
            List<RatioInfo> hasActualValue = new List<RatioInfo>();
            //收集存在实际值的比例信息
            if (unit == GeoEquationUnits.Angle)
            {
                foreach (var item in FormularBase.AngleRatioInfos)
                {
                    if (item.ActualValue is not null)
                    {
                        hasActualValue.Add(item);
                    }
                }
            }
            else if (unit == GeoEquationUnits.Distance)
            {
                foreach (var item in FormularBase.DistanceRatioInfos)
                {
                    if (item.ActualValue is not null)
                    {
                        hasActualValue.Add(item);
                    }
                }
            }
            //以第一个为基础吸收其他的比例信息
            if (hasActualValue.Count > 1)
            {
                var baseRatioInfo = hasActualValue[0];
                foreach (var item in hasActualValue.Skip(1))
                {
                    baseRatioInfo.AbsordByValue(item);
                }
            }

        }
    }
}
