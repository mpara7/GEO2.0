using OneOf.Types;
using System.Collections.Generic;
using System.Linq;

namespace GeoInferenceEngine.Knowledges.Models
{
    [Description("Equation")]
    public class GeoEquation : Knowledge
    {
        public Expr LeftPart { get; set; }
        public Expr RightPart { get; set; }
        public int NodeNumber { get; set; } = 0;

        public List<ulong> AllConditionHashCode { get; set; } = new List<ulong> { };

        public GeoEquation(Expr leftPart, Expr rightPart)
        {
            LeftPart = leftPart;
            RightPart = rightPart;
            Normalize();
            SetHashCode();
            //NodeNubmers();
        }

        public override void SetHashCode()
        {
            
            HashCode = (ulong)ToString().GetHashCode();
        }

        public override string ToString() => $"{LeftPart}={RightPart}";

        public override void Normalize()
        {
        }

        public void AddCondition(string reason, params GeoEquation[] conditionPreds)
        {
            Reason = reason;
            foreach (var condition in conditionPreds)
            {
                Conditions.Add(condition);
                AllConditionHashCode.Add(condition.HashCode);
                AllConditionHashCode.AddRange(condition.AllConditionHashCode);
            }
        }

        public bool hasCodition(GeoEquation conditionPreds)
        {
            bool areEqual = new HashSet<ulong>(AllConditionHashCode).SetEquals(AllConditionHashCode);
            return areEqual;
        }

        #region 统计数量

        public void NodeNubmers()
        {
            CalNodeNubmers(Expr.FromString(this.LeftPart.ToString().Replace("`", "")));
            CalNodeNubmers(Expr.FromString(this.RightPart.ToString().Replace("`", "")));
        }

        private void CalNodeNubmers(Expr expr)
        {
            if (expr is SumNode sum)
            {
                foreach (var item in sum.Addends)
                    CalNodeNubmers(item);
                foreach (var item in sum.Subtrahends)
                    CalNodeNubmers(item);
            }
            else if (expr is ProductNode product)
            {
                foreach (var item in product.Multipliers)
                    CalNodeNubmers(item);
                foreach (var item in product.Divisors)
                    CalNodeNubmers(item);
            }
            else
            {
                NodeNumber++;
            }
        }

        #endregion 统计数量

        public GeoEquation Mul(Expr realNode)
        {
            GeoEquation pred = new GeoEquation(LeftPart.Clone().Mul(realNode), RightPart.Clone().Mul(realNode));
            return pred;
        }

        public GeoEquation Div(Expr realNode)
        {
            GeoEquation pred = new GeoEquation(LeftPart.Clone().Div(realNode), RightPart.Clone().Div(realNode));
            return pred;
        }

        public GeoEquation Add(GeoEquation geoEquation)
        {
            GeoEquation pred = new GeoEquation(LeftPart.Clone().Add(geoEquation.LeftPart).Simplify(),
                RightPart.Clone().Add(geoEquation.RightPart).Simplify());
            return pred;
        }

        public GeoEquation Sub(GeoEquation geoEquation)
        {
            GeoEquation pred = new GeoEquation(LeftPart.Clone().Sub(geoEquation.LeftPart).Simplify(),
                RightPart.Clone().Sub(geoEquation.RightPart).Simplify());
            
            return pred;
        }

        public GeoEquation Mul(GeoEquation geoEquation)
        {
            GeoEquation pred = new GeoEquation(LeftPart.Clone().Mul(geoEquation.LeftPart).Simplify(),
                RightPart.Clone().Mul(geoEquation.RightPart).Simplify());
            return pred;
        }

        public GeoEquation Div(GeoEquation geoEquation)
        {
            GeoEquation pred = new GeoEquation(LeftPart.Clone().Div(geoEquation.LeftPart).Simplify()
                , RightPart.Clone().Div(geoEquation.RightPart).Simplify());
            return pred;
        }
    }
}