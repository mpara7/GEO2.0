
using GeoInferenceEngine.PredicateShared.Models;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.Models
{
    /// <summary>
    /// 比例表
    /// </summary>
    public class RatioInfo
    {
        public GeoEquationUnits Unit { get; set; }
        public Expr ActualValue { get; set; } = null;
        public Dictionary<Mut, Expr> CoffDict { get; } = new();
        public (Mut mut, Knowledge reason) ToValueReason { get; set; }
        public List<(Mut, Mut, Knowledge Reasons)> Reasons { get; set; } = new();
        List<RatioInfo> Absorbed = new();
        /// <summary>
        /// 已知Mut在列表中
        /// </summary>
        /// <param name="mut"></param>
        /// <param name="value"></param>
        /// <param name="reason"></param>
        public void AddValue(Mut mut, Expr value, Knowledge reason)
        {
            if (!CoffDict.ContainsKey(mut)) { CoffDict.Add(mut, Expr.One); ActualValue = value; }
            else { ActualValue = value.Mul(CoffDict[mut]).Simplify(); }
            
            ToValueReason = (mut, reason);
        }
        /// <summary>
        /// 基于值直接添加
        /// </summary>
        /// <param name="expr"></param>
        /// <param name="value"></param>
        /// <param name="reason"></param>
        public void AddByValue(Mut expr1, Expr value, Knowledge reason)
        {
            CoffDict.Add(expr1, ((ActualValue.Clone() / value).Simplify()));
            SimpleAddReason(expr1, ToValueReason.mut, reason);
        }
        public void AddFirst(Mut mut1, Mut mut2, Expr ratio, Knowledge reason)
        {
            var eRatio = ratio.Clone().Simplify();
            if (mut1.Index < mut2.Index)
            {
                CoffDict.Add(mut1, Expr.One);
                CoffDict.Add(mut2, eRatio);
                SimpleAddReason(mut1, mut2, reason);
            }
            else
            {
                CoffDict.Add(mut2, Expr.One);
                CoffDict.Add(mut1, eRatio.Invert().Simplify());
                SimpleAddReason(mut1, mut2, reason);
            }
        }
        public void Add(Mut hadMut, Mut newMut, Expr ratio, Knowledge reason)
        {
            CoffDict.Add(newMut, CoffDict[hadMut].Clone().Mul(ratio).Simplify());
            SimpleAddReason(newMut, hadMut, reason);
        }
        public void AbsordByValue(RatioInfo ratioInfo)
        {
            var bridgeValue = ActualValue.Clone().Div(CoffDict[ToValueReason.Item1]);
            var absorbbridgeValue = ratioInfo.ActualValue.Div(ratioInfo.CoffDict[ratioInfo.ToValueReason.Item1]);
            var ratio = absorbbridgeValue.Clone().Div(bridgeValue).Simplify();

            GeoEquation geoEquation = new GeoEquation((ToValueReason.Item1 / ratioInfo.ToValueReason.Item1), ratio);
            geoEquation.AddReason();
            geoEquation.AddCondition(ToValueReason.Item2, ratioInfo.ToValueReason.Item2);

            SimpleAddReason(ToValueReason.Item1, ratioInfo.ToValueReason.Item1, geoEquation);
            AbsorbByMutableRatio(ratioInfo, ratioInfo.ToValueReason.Item1, ToValueReason.Item1, ratio, geoEquation);

            Absorbed.Add(ratioInfo);
        }
        public void AbsorbByMutableRatio(RatioInfo ratioInfo, Mut absorbedBridge, Mut thisBridge, Expr ratio, Knowledge reason)
        {
            foreach (var kv in ratioInfo.CoffDict.ToList())
            {
                var absorbedToBridgeRatio = kv.Value.Clone().Div(ratioInfo.CoffDict[absorbedBridge]);
                var innerRatio = absorbedToBridgeRatio.Clone().Div(ratio).Simplify();
                if (CoffDict.ContainsKey(kv.Key)) { continue; }
                Add(thisBridge, kv.Key, innerRatio, reason);
            }
            Absorbed.Add(ratioInfo);
        }

        #region 工具
        /// <summary>
        /// 调用者保证都加上
        /// </summary>
        /// <param name="mut1"></param>
        /// <param name="mut2"></param>
        /// <param name="reasons"></param>
        public void SimpleAddReason(Mut mut1, Mut mut2, Knowledge reasons)
        {
            if (mut1.Index > mut2.Index)
            {
                var temp = mut1; mut1 = mut2; mut2 = temp;
            }
            Reasons.Add((mut1, mut2, reasons));
        }
        public List<Knowledge> SimpleFindReason(Mut mut1, Mut mut2)
        {
            Dictionary<Mut,List<(Mut,Knowledge)>> graph=new ();
            foreach (var item in Reasons)
            {
                if (!graph.ContainsKey(item.Item1))
                {
                    graph.Add(item.Item1, new() {(item.Item2,item.Reasons) });
                }
                else
                {
                    graph[item.Item1].Add((item.Item2, item.Reasons));
                }

                if (!graph.ContainsKey(item.Item2))
                {
                    graph.Add(item.Item2, new() { (item.Item1, item.Reasons) });
                }
                else
                {
                    graph[item.Item2].Add((item.Item1, item.Reasons));
                }
            }
            List<Knowledge> reasons=new List<Knowledge>();
            List<Mut> visited=new List<Mut>();
            BFS(reasons,graph, visited, mut1,mut2);
            return reasons;

        }
        bool BFS(List<Knowledge> reasons, Dictionary<Mut, List<(Mut, Knowledge)>> graph,List<Mut> visited, Mut mut1, Mut mut2)
        {
            visited.Add(mut1);
            foreach (var item in graph[mut1])
            {
                if (visited.Contains(item.Item1))
                    continue;
                if (item.Item1 == mut2)
                {
                    reasons.Add(item.Item2);
                    return true;
                }
                else
                {
                    reasons.Add(item.Item2);
                    if (BFS(reasons, graph,visited, item.Item1, mut2))
                    {
                        return true;
                    }
                    else
                    {
                        reasons.Remove(item.Item2);
                    }
                   
                }
            }
            return false;
        }

        public override string ToString()
        {
            string str = "(";
            if (ActualValue is not null)
            {
                str += $"{ActualValue}=";
            }
            foreach (var kv in CoffDict)
            {
                str += $"{kv.Value}*{kv.Key}=";
            }
            str = str.Remove(str.Length - 1);

            str += $")";
            return str;
        }
        #endregion
    }
}
