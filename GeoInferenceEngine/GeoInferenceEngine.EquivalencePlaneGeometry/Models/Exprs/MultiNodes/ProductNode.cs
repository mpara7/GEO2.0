using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs.MutNodes;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    public class ProductNode : Expr
    {
        public Dictionary<Expr, Expr> PowerDict = new(new ExprValueComparer());
        /// <summary>
        /// 只有一个乘法节点
        /// </summary>
        public Expr Single { get; set; } = null;
        public bool IsSingle { get; set; }
        bool isPositive = true;
        public bool IsPositive
        {
            get { return isPositive; }
            set
            {
                if (ToString().Contains("BC"))
                    Console.WriteLine();
                isPositive = value;
            }
        }
        public Expr Constant { get; set; } = Let(1);
        public List<Expr> Multipliers { get; set; } = new();
        public List<Expr> Divisors { get; set; } = new();
        public void AddToMultipliers(Expr expr)
        {
            Multipliers.Add(expr);
            IsSimplified = false;
        }
        public void AddToDivisors(Expr expr)
        {
            Divisors.Add(expr);
            IsSimplified = false;
        }

        public override Expr Simplify()
        {
            //if (IsSimplified) return this;
            //IsSimplified = true;
            for (int i = 0; i < Multipliers.Count; i++)
            {
                Multipliers[i] = Multipliers[i].Simplify();
            }
            for (int i = 0; i < Divisors.Count; i++)
            {
                Divisors[i] = Divisors[i].Simplify();
            }
            实数合并();
            子节点是积节点合并();
            相同节点抵消();
            Constant = Constant.Simplify();
            if (Constant < 0)
            {
                IsPositive = !IsPositive;
                Constant = Constant.Opposite();
            }

            var check = CheckDegeneration();
            if (check.Item1) { return check.Item2; }

            //检查信息
            if (Multipliers.Count() == 1 && Divisors.Count == 0 && Multipliers.First() is MutNode or NameNode) { IsSingle = true; Single = Multipliers.First(); }

            else { IsSingle = false; Single = null; }
            return this;
        }

        public (bool success, SumNode sum) 分配律()
        {
            void Combine(List<Expr> exprs, List<SumNode> sums)
            {
                if (sums.Count == 0) return;
                var sum = sums[0];
                sums.Remove(sum);
                List<Expr> tmp = new List<Expr>(exprs);
                exprs.Clear();
                if (sum.Constant != Zero)
                {
                    foreach (var expr in tmp)
                    {
                        exprs.Add(expr.Clone().Mul(sum.Constant));
                    }
                }
                foreach (Expr e in sum.Addends)
                {
                    foreach (var expr in tmp)
                    {
                        exprs.Add(expr.Clone().Mul(e));
                    }
                }
                foreach (Expr e in sum.Subtrahends)
                {
                    foreach (var expr in tmp)
                    {
                        exprs.Add(expr.Clone().Mul(e).Opposite());
                    }
                }
                Combine(exprs, sums);
            }
            bool flag = false;
            ProductNode left = new ProductNode();
            left.Constant = (RealNode)Constant.Clone();
            left.Divisors = Divisors.ToList();
            List<SumNode> MulSums = new List<SumNode>();

            foreach (var e in Multipliers)
            {
                if (e is SumNode)
                {
                    flag = true;
                    MulSums.Add((SumNode)e);
                }
                else
                {
                    left.Multipliers.Add(e);
                }
            }
            if (flag)
            {
                List<Expr> lefts = new List<Expr>() { left };
                Combine(lefts, MulSums);
                SumNode sumNode = new SumNode();
                foreach (var l in lefts)
                {
                    var result = l.Simplify();
                    if (IsPositive)
                    {
                        sumNode.Addends.Add(result);
                    }

                    else
                    {
                        sumNode.Subtrahends.Add(result);
                    }
                }
                sumNode.Simplify();
                return (true, sumNode);
            }
            return (false, null);

        }

        void 子节点是积节点合并()
        {
            var addRationalNodes = Multipliers.Where(node => node is ProductNode).Select(s => (ProductNode)s).ToList();
            var subRationalNodes = Divisors.Where(node => node is ProductNode).Select(s => (ProductNode)s).ToList();
            foreach (var product in addRationalNodes)
            {
                if (!product.IsPositive) IsPositive = !IsPositive;
                Constant = Constant.Mul(product.Constant);
                Multipliers.AddRange(product.Multipliers);
                Divisors.AddRange(product.Divisors);
            }
            foreach (var product in subRationalNodes)
            {
                if (!product.IsPositive) IsPositive = !IsPositive;
                Constant = Constant.Div(product.Constant);
                Multipliers.AddRange(product.Divisors);
                Divisors.AddRange(product.Multipliers);
            }
            for (int i = 0; i < addRationalNodes.Count(); i++)
                Multipliers.Remove(addRationalNodes[i]);
            for (int i = 0; i < subRationalNodes.Count(); i++)
                Divisors.Remove(subRationalNodes[i]);
        }
        void 实数合并()
        {
            var addRationalNodes = Multipliers.Where(node => node is RealNode).Select(s => (RealNode)s).ToList();
            var subRationalNodes = Divisors.Where(node => node is RealNode).Select(s => (RealNode)s).ToList();

            foreach (var rationalNode in addRationalNodes)
            {
                Constant = Constant.Mul(rationalNode);
                Multipliers.Remove(rationalNode);
            }
            foreach (var rationalNode in subRationalNodes)
            {
                Constant = Constant.Div(rationalNode);
                Divisors.Remove(rationalNode);
            }

        }
        void 相同节点抵消()
        {
            PowerDict.Clear();

            for (int i = 0; i < Multipliers.Count; i++)
            {
                if (Multipliers[i] is PowerNode powerNode)
                {
                    var inner = powerNode.Base;
                    if (PowerDict.ContainsKey(inner))
                        PowerDict[inner] = (ARationalNode)PowerDict[inner].Add(powerNode.Exponent);
                    else PowerDict.Add(inner, (ARationalNode)powerNode.Exponent);
                }
                else
                {
                    if (PowerDict.ContainsKey(Multipliers[i]))
                        PowerDict[Multipliers[i]] = (ARationalNode)PowerDict[Multipliers[i]].Add(1);
                    else PowerDict.Add(Multipliers[i], 1);
                }
            }
            for (int i = 0; i < Divisors.Count; i++)
            {
                if (Divisors[i] is RealPowerNode powerNode)
                {
                    var inner = powerNode.Base;
                    if (PowerDict.ContainsKey(inner))
                        PowerDict[inner] = (ARationalNode)PowerDict[inner].Sub(powerNode.Exponent);
                    else PowerDict.Add(inner, (ARationalNode)powerNode.Exponent.Opposite());
                }
                else
                {
                    if (PowerDict.ContainsKey(Divisors[i]))
                        PowerDict[Divisors[i]] = (ARationalNode)PowerDict[Divisors[i]].Add(-1);
                    else PowerDict.Add(Divisors[i], -1);
                }
            }

            Multipliers.Clear();
            Divisors.Clear();
            //重新组合
            foreach (var kv in PowerDict)
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
                var p = kv.Key.Pow(kv.Value).Simplify();
                if (p is IntNode)
                {
                    Constant = (RealNode)Constant.Mul(p);
                }
                else if (p is ProductNode pp)
                {
                    Constant = (RealNode)Constant.Mul(pp.Constant);
                    for (int i = 0; i < pp.Multipliers.Count; i++)
                    {
                        Multipliers.Add(pp.Multipliers[i]);
                    }
                    for (int i = 0; i < pp.Divisors.Count; i++)
                    {
                        Divisors.Add(pp.Divisors[i]);
                    }
                }
                else
                {
                    Multipliers.Add(p);
                }
            }
            Multipliers.Sort(new ExprOrderComparer());
            Divisors.Sort(new ExprOrderComparer());
        }
        public override string ToString()
        {
            var str = "";
            bool first = true;
            if (!IsPositive) { str += "-"; }

            if (Multipliers.Count == 0)
            {
                str += $"{Constant}";
            }
            else
            {
                if (Constant.CompareTo(1) != ExprCompareResult.Equal)
                {
                    str += $"{Constant}*";
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
        public (bool, Expr) CheckDegeneration()
        {
            if (!IsPositive && Multipliers.Count == 1 && Divisors.Count == 0 && Multipliers[0] is SumNode sum)
            { return (true, sum.Opposite()); }
            if (Multipliers.Count == 0 && Divisors.Count == 0)
            {
                if (IsPositive) return (true, Constant);
                else return (true, Constant.Opposite());
            }

            if (Constant == One && Multipliers.Count == 1 && Divisors.Count == 0 && IsPositive)
                return (true, Multipliers[0]);
            return (false, null);
        }
        public override ProductNode Clone()
        {
            ProductNode node = new ProductNode();
            node.IsPositive = IsPositive;

            node.Constant = (RealNode)Constant.Clone();
            node.Multipliers = Multipliers.Select(s => s.Clone()).ToList();
            node.Divisors = Divisors.Select(s => s.Clone()).ToList();
            return node;
        }


    }
}
