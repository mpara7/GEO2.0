using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    public class RealProductNode : RealNode
    {
        /// <summary>
        /// 正负号
        /// </summary>
        public bool IsPositive { get; set; } = true;
        public ARationalNode Rational { get; set; } = FromInt(1);
        public List<RealNode> Multipliers { get; set; } = new();
        public List<RealNode> Divisors { get; set; } = new();

        public (ARationalNode, RealNode) SplitRational()
        {
            if (Multipliers.Count == 1 && Divisors.Count == 0)
            {
                return ((ARationalNode)Rational.Clone(), Multipliers[0]);
            }
            else
            {
                var temp = (RealProductNode)Clone();
                temp.Rational = FromInt(1);
                return ((ARationalNode)Rational.Clone(), temp);
            }

        }
        public override string ToString()
        {
            if (Multipliers.Count == 1 && Divisors.Count == 0)
            {
                return $"{Rational}*{Multipliers[0]}";
            }

            var str = "";
            bool first = true;
            if (!IsPositive) { str += "-"; }

            if (Multipliers.Count == 0)
            {
                str += $"{Rational}";
            }
            else
            {
                if (Rational != One)
                {
                    str += $"{Rational}*";
                }
                if (Multipliers.Count > 1)
                {
                    str += "(";
                }
                foreach (Expr e in Multipliers)
                {
                    if (first)
                        first = false;
                    else
                        str += "*";
                    str += e.ToString();

                }
                if (Multipliers.Count > 1)
                {
                    str += ")";
                }
            }
            if (Divisors.Count > 0)
            {
                str += "/";
                first = true;
                if (Divisors.Count > 1)
                {
                    str += "(";
                }
                foreach (Expr e in Divisors)
                {
                    if (first)
                        first = false;
                    else
                        str += "*";
                    str += e.ToString(); ;
                }
                if (Divisors.Count > 1)
                {
                    str += ")";
                }
            }
            return str;
        }
        public override RealProductNode Clone()
        {
            RealProductNode node = new RealProductNode();
            node.IsPositive = IsPositive;
            node.Rational = (ARationalNode)Rational.Clone();
            node.Multipliers = Multipliers.Select(s => s.Clone()).ToList();
            node.Divisors = Divisors.Select(s => s.Clone()).ToList();
            return node;
        }
        public override RealNode Simplify()
        {

            if (IsSimplified) return this;
            IsSimplified = true;
            for (int i = 0; i < Multipliers.Count; i++)
            {
                Multipliers[i] = Multipliers[i].Simplify();
            }
            for (int i = 0; i < Divisors.Count; i++)
            {
                Divisors[i] = Divisors[i].Simplify();
            }
            //子节点是积节点合并();
            //有理数合并();
            //相同节点抵消();
            Rational = (ARationalNode)Rational.Simplify();
            var check = CheckDegeneration();
            if (check.Item1) { return check.Item2; }
            return this;
        }
        public Dictionary<RealNode, Expr> CoffDict { get; set; } = new(new ExprValueComparer());
        void 子节点是积节点合并()
        {
            var mulRationalNodes = Multipliers.Where(node => node is RealProductNode).Select(n => (RealProductNode)n).ToList();
            var divRationalNodes = Divisors.Where(node => node is RealProductNode).Select(n => (RealProductNode)n).ToList();
            foreach (var productNode in mulRationalNodes)
            {
                if (!productNode.IsPositive) IsPositive = !IsPositive;
                Rational = (ARationalNode)Rational.Mul(productNode.Rational);
                Multipliers.AddRange(productNode.Multipliers);
                Divisors.AddRange(productNode.Divisors);
            }
            foreach (var productNode in divRationalNodes)
            {
                if (!productNode.IsPositive) IsPositive = !IsPositive;
                Rational = (ARationalNode)Rational.Div(productNode.Rational);
                Divisors.AddRange(productNode.Multipliers);
                Multipliers.AddRange(productNode.Divisors);
            }
            for (int i = 0; i < mulRationalNodes.Count(); i++)
                Multipliers.Remove(mulRationalNodes[i]);
            for (int i = 0; i < divRationalNodes.Count(); i++)
                Divisors.Remove(divRationalNodes[i]);
        }
        private bool _isMerging = false; // 类级别变量


        void 有理数合并()
        {
            for (int i = 0; i < Multipliers.Count(); i++)
            {
                if (Multipliers[i] is ARationalNode v)
                {
                    Rational = (ARationalNode)Rational.Mul(v);
                    Multipliers.Remove(v);
                    i -= 1;
                }
            }
            for (int i = 0; i < Divisors.Count(); i++)
            {
                if (Divisors[i] is ARationalNode v)
                {
                    Rational = (ARationalNode)Rational.Div(v);
                    Divisors.Remove(v);
                    i -= 1;
                }
            }
        }
        void 相同节点抵消()
        {
            CoffDict.Clear();

            for (int i = 0; i < Multipliers.Count; i++)
            {
                if (Multipliers[i] is RealPowerNode p)
                {
                    if (CoffDict.ContainsKey(p.Base))
                        CoffDict[p.Base] = CoffDict[p.Base].Add(p.Exponent);
                    else CoffDict.Add(p.Base, p.Exponent);
                }
                else
                {
                    if (CoffDict.ContainsKey(Multipliers[i]))
                        CoffDict[Multipliers[i]] = CoffDict[Multipliers[i]].Add(1);
                    else CoffDict.Add(Multipliers[i], 1);
                }
            }
            for (int i = 0; i < Divisors.Count; i++)
            {
                if (Divisors[i] is RealPowerNode p)
                {
                    if (CoffDict.ContainsKey(p.Base))
                        CoffDict[p.Base] = CoffDict[p.Base].Sub(p.Exponent);
                    else CoffDict.Add(p.Base, -p.Exponent);
                }
                else
                {
                    if (CoffDict.ContainsKey(Divisors[i]))
                        CoffDict[Divisors[i]] = CoffDict[Divisors[i]].Add(-1);
                    else CoffDict.Add(Divisors[i], -1);
                }
            }
            Multipliers.Clear();
            Divisors.Clear();
            foreach (var kv in CoffDict.ToList())
            {
                if (kv.Value == Zero)
                {
                    continue;
                }
                if (kv.Value == Expr.One)
                {
                    Multipliers.Add(kv.Key); continue;
                }
                if (kv.Value == Expr.NegativeOne)
                {
                    Divisors.Add(kv.Key); continue;
                }
                if (kv.Value > 0)
                {
                    var p = kv.Key.Pow(kv.Value);
                    p = p.Simplify();
                    Multipliers.Add(p);
                }
                else
                {
                    if (kv.Key is ARationalNode)
                    {
                        Rational = (ARationalNode)(Rational / kv.Key);
                    }
                    else
                    {
                        Divisors.Add(kv.Key);
                    }
                    var p = kv.Key.Pow(1 - kv.Value);
                    Multipliers.Add(p);
                }

            }
        }


        public (bool, RealNode) CheckDegeneration()
        {
            if (Multipliers.Count == 0 && Divisors.Count == 0) return (true, Rational);
            if (Multipliers.Count == 1 && Divisors.Count == 0 && Rational == Expr.One) return (true, (RealNode)Multipliers[0]);
            return (false, null);
        }


        public override Expr Mul(Expr expr)
        {
            if (expr is IntNode i)
            {
                if (i.Value == 0)
                {
                    return 0;
                }
                var temp = (RealProductNode)this.Clone();
                temp.Rational = (FractionNode)(temp.Rational * i);
                var result = temp.Simplify();
                return result;
            }
            else if (expr is RealProductNode product)
            {
                var temp = (RealProductNode)this.Clone();
                temp.IsPositive = this.IsPositive == product.IsPositive;
                temp.Rational = (ARationalNode)(temp.Rational * product.Rational);
                temp.Multipliers.AddRange(product.Multipliers.Select(e => e.Clone()));
                temp.Divisors.AddRange(product.Divisors.Select(e => e.Clone()));
                var result = temp.Simplify();
                return result;
            }
            return base.Mul(expr);
        }
        public override Expr Div(Expr expr)
        {
            if (expr is IntNode i)
            {
                if (i.Value == 0)
                {
                    throw new ArgumentException("不能除以0");
                }
                var temp = (RealProductNode)this.Clone();
                temp.Rational = (FractionNode)(temp.Rational / i);
                var result = temp.Simplify();
                return result;
            }
            else if (expr is RealProductNode product)
            {
                var temp = (RealProductNode)this.Clone();
                temp.Rational = (FractionNode)(temp.Rational / product.Rational);
                temp.IsPositive = this.IsPositive == product.IsPositive;
                temp.Multipliers.AddRange(product.Divisors.Select(e => e.Clone()));
                temp.Divisors.AddRange(product.Multipliers.Select(e => e.Clone()));
                var result = temp.Simplify();
                return result;
            }
            return base.Div(expr);
        }

        public override RealNode Opposite()
        {
            var temp = this.Clone();
            temp.IsPositive = !temp.IsPositive;
            return temp;
        }
        public override RealNode Invert()
        {
            var temp = new RealProductNode();
            temp.IsPositive = IsPositive;
            temp.Rational = (ARationalNode)temp.Rational.Invert();
            temp.Multipliers.AddRange(temp.Divisors.Select(e => e.Clone()));
            temp.Divisors.AddRange(temp.Multipliers.Select(e => e.Clone()));
            var result = temp.Simplify();
            return result;
        }

        public override double GetApproximation()
        {
            var temp = Rational.GetApproximation();
            foreach (var item in Multipliers)
            {
                temp *= item.GetApproximation();
            }
            foreach (var item in Divisors)
            {
                temp /= item.GetApproximation();
            }
            return temp;
        }
    }
}
