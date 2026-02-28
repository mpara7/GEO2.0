using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs.MutNodes;

using System.Collections.Generic;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    public class SumNode : Expr
    {
        public Expr Constant { get; set; } = Expr.Zero;
        
        public List<Expr> Addends { get; internal set; } = new();
        public List<Expr> Subtrahends { get; set; } = new();
        public void AddToAddends(Expr expr)
        {
            Addends.Add(expr);
            IsSimplified = false;
        }
        public void AddToSubtrahends(Expr expr)
        {
            Subtrahends.Add(expr);
            IsSimplified = false;
        }
        public override SumNode Clone()
        {
            SumNode node = new SumNode();
            node.Constant = (RealNode)Constant.Clone();
            node.Addends = Addends.Select(s => s.Clone()).ToList();
            node.Subtrahends = Subtrahends.Select(s => s.Clone()).ToList();
            return node;
        }

        public override Expr Simplify()
        {
            //if (IsSimplified) return this;
            //IsSimplified = true;
            //子积节点正负调整();
            for (int i = 0; i < Addends.Count; i++)
            {
                Addends[i] = Addends[i].Simplify();
            }
            for (int i = 0; i < Subtrahends.Count; i++)
            {
                Subtrahends[i] = Subtrahends[i].Simplify();
            }
            实数合并();
            子节点是和节点合并();

            子积节点正负调整();
            相同节点抵消();
            Constant = (RealNode)Constant.Simplify();
            var check = CheckDegeneration();
            if (check.Item1) { return check.Item2; }
            return this;
        }

        private void 子节点是和节点合并()
        {
            var addRationalNodes = Addends.Where(node => node is SumNode).ToList();
            var subRationalNodes = Subtrahends.Where(node => node is SumNode).ToList();
            if (addRationalNodes.Count == 0 && subRationalNodes.Count == 0) return;
            foreach (var rationalNode in addRationalNodes)
            {
                Constant = (RealNode)Constant.Add(((SumNode)rationalNode).Constant);
                Addends.AddRange(((SumNode)rationalNode).Addends);
                Subtrahends.AddRange(((SumNode)rationalNode).Subtrahends);
            }
            foreach (var rationalNode in subRationalNodes)
            {
                Constant = (RealNode)Constant.Sub(((SumNode)rationalNode).Constant);
                Addends.AddRange(((SumNode)rationalNode).Subtrahends);
                Subtrahends.AddRange(((SumNode)rationalNode).Addends);
            }
            for (int i = 0; i < addRationalNodes.Count(); i++)
            {
                Addends.Remove(addRationalNodes[i]);
            }
            for (int i = 0; i < subRationalNodes.Count(); i++)
            {
                Subtrahends.Remove(subRationalNodes[i]);
            }
        }

        private void 子积节点正负调整()
        {
            for (int i = 0; i < Addends.Count; i++)
            {
                if (Addends[i] is ProductNode product)
                {
                    if (!product.IsPositive)
                    {
                        product.IsPositive = true;
                        Addends.RemoveAt(i);
                        Subtrahends.Add(product);
                        i--;
                    }
                }
            }
            for (int i = 0; i < Subtrahends.Count; i++)
            {
                if (Subtrahends[i] is ProductNode product)
                {
                    if (!product.IsPositive)
                    {
                        product.IsPositive = true;
                        Subtrahends.RemoveAt(i);
                        Addends.Add(product);
                        i--;
                    }
                }
            }
        }

        private void 实数合并()
        {
            for (int i = 0; i < Addends.Count(); i++)
            {
                if (Addends[i] is RealNode v)
                {
                    Constant = Constant.Add(v);
                    Addends.Remove(v);
                    i -= 1;
                }
            }
            for (int i = 0; i < Subtrahends.Count(); i++)
            {
                if (Subtrahends[i] is RealNode v)
                {
                    Constant = Constant.Sub(v);
                    Subtrahends.Remove(v);
                    i -= 1;
                }
            }
        }

        private void 相同节点抵消()
        {
            Dictionary<Expr, Expr> dict = new(new ExprValueComparer());
            for (int i = 0; i < Addends.Count; i++)
            {
                if (Addends[i] is MutNode atomExpr)
                {
                    if (dict.ContainsKey(atomExpr))
                        dict[atomExpr] = dict[atomExpr].Add(1);
                    else dict.Add(atomExpr, 1);
                    Addends.RemoveAt(i);
                    i -= 1;
                    continue;
                }
                else if (Addends[i] is NameNode name)
                {
                    if (dict.ContainsKey(name))
                        dict[name] = dict[name].Add(1);
                    else dict.Add(name, 1);
                    Addends.RemoveAt(i);
                    i -= 1;
                    continue;
                }
                //处理类似2*a/3
                else if (Addends[i] is ProductNode productNode)
                {
                    if (productNode.Multipliers.Count == 1 && productNode.Divisors.Count == 0)
                    {
                        if (productNode.Multipliers[0] is MutNode a)
                        {
                            if (dict.ContainsKey(a))
                                dict[a] = dict[a].Add(productNode.Constant);
                            else dict.Add(a, productNode.Constant);
                            Addends.RemoveAt(i);
                            i -= 1;
                            continue;
                        }
                        else if (productNode.Multipliers[0] is NameNode name2)
                        {
                            if (dict.ContainsKey(name2))
                                dict[name2] = dict[name2].Add(productNode.Constant);
                            else dict.Add(name2, productNode.Constant);
                            Addends.RemoveAt(i);
                            i -= 1;
                            continue;
                        }
                    }
                }
                //最终操作
                if (dict.ContainsKey(Addends[i]))
                    dict[Addends[i]] = dict[Addends[i]].Add(1);
                else dict.Add(Addends[i], 1);
                Addends.RemoveAt(i);
                i -= 1;
            }
            for (int i = 0; i < Subtrahends.Count; i++)
            {
                if (Subtrahends[i] is MutNode atomExpr)
                {
                    if (dict.ContainsKey(atomExpr))
                        dict[atomExpr] = dict[atomExpr].Sub(1);
                    else dict.Add(atomExpr, -1);
                    Subtrahends.RemoveAt(i);
                    i -= 1;
                    continue;
                }
                else if (Subtrahends[i] is NameNode name)
                {
                    if (dict.ContainsKey(name))
                        dict[name] = dict[name].Sub(1);
                    else dict.Add(name, -1);
                    Subtrahends.RemoveAt(i);
                    i -= 1;
                    continue;
                }
                //处理类似2*a/3
                else if (Subtrahends[i] is ProductNode productNode)
                {
                    if (productNode.Multipliers.Count == 1 && productNode.Divisors.Count == 0)
                    {
                        if (productNode.Multipliers[0] is MutNode a)
                        {
                            if (dict.ContainsKey(a))
                                dict[a] = dict[a].Sub(productNode.Constant);
                            else dict.Add(a, productNode.Constant.Opposite());
                            Subtrahends.RemoveAt(i);
                            i -= 1;
                            continue;
                        }
                        else if (productNode.Multipliers[0] is NameNode name2)
                        {
                            if (dict.ContainsKey(name2))
                                dict[name2] = dict[name2].Sub(productNode.Constant);
                            else dict.Add(name2, productNode.Constant.Opposite());
                            Subtrahends.RemoveAt(i);
                            i -= 1;
                            continue;
                        }
                    }
                }
                if (dict.ContainsKey(Subtrahends[i]))
                    dict[Subtrahends[i]] = dict[Subtrahends[i]].Sub(1);
                else dict.Add(Subtrahends[i], -1);
                Subtrahends.RemoveAt(i);
                i -= 1;
            }
            foreach (var kv in dict)
            {
                if (kv.Value == Zero)
                {
                    continue;
                }
                if (kv.Value == Expr.One)
                {
                    Addends.Add(kv.Key); continue;
                }

                if (kv.Value == Expr.NegativeOne)
                {
                    Subtrahends.Add(kv.Key); continue;
                }

                var p = kv.Key.Mul(kv.Value).Simplify();
                if (p is RealNode)
                {
                    Constant = (RealNode)Constant.Add(p);
                }
                else if (p is ProductNode pp)
                {
                    if (pp.IsPositive)
                    {
                        Addends.Add(pp);
                    }
                    else
                    {
                        pp.IsPositive = true;
                        Subtrahends.Add(pp);
                    }
                }
                else
                {
                    Addends.Add(p);
                }
            }
            //非原子节点计算
            for (int i = 0; i < Subtrahends.Count; i++)
            {
                for (int j = 0; j < Addends.Count; j++)
                {
                    if (Addends[j] == Subtrahends[i])
                    {
                        Addends.RemoveAt(j);
                        Subtrahends.RemoveAt(i);
                        i -= 1;
                        break;
                    }
                }
            }
        }

        public (bool, Expr) 提取公因式()
        {
            if (Constant != Expr.Zero) return (false, (Expr?)null);
            Dictionary<Expr, Expr> DDict = new();
            bool first = true;

            foreach (var item in Addends.Union(Subtrahends))
            {
                if (first)
                {
                    first = false;
                    if (item is MutNode)
                    {
                        DDict.Add(item, 1);
                    }
                    else if (item is ProductNode productNode)
                    {
                        DDict = new Dictionary<Expr, Expr>(productNode.PowerDict);
                    }
                    else if (item is PowerNode powerNode)
                    {
                        DDict.Add(powerNode.Base, (Expr)powerNode.Exponent);
                    }
                }
                else
                {
                    if (item is MutNode)
                    {
                        foreach (var e in DDict.Where(kv => !kv.Key.Equals(item)).ToArray())
                        {
                            DDict.Remove(e.Key);
                        }
                    }
                    else if (item is ProductNode productNode)
                    {
                        foreach (var e in DDict.Keys.Where(kv => !productNode.PowerDict.Keys.Contains(kv)).ToArray())
                        {
                            DDict.Remove(e);
                        }
                        foreach (var e in DDict.Keys.Where(productNode.PowerDict.Keys.Contains).ToArray())
                        {
                            if (DDict[e].CompareTo(productNode.PowerDict[e]) == ExprCompareResult.Greater)
                                DDict[e] = productNode.PowerDict[e];
                        }
                    }
                }
            }
            if (DDict.Count == 0) return (false, null);
            ProductNode result = new ProductNode();
            foreach (var kv in DDict)
            {
                if (kv.Value == Zero)
                {
                    continue;
                }
                if (kv.Value == Expr.One)
                {
                    result.Multipliers.Add(kv.Key); continue;
                }

                if (kv.Value == Expr.NegativeOne)
                {
                    result.Divisors.Add(kv.Key); continue;
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
                        result.Multipliers.Add(pp.Multipliers[i]);
                    }
                    for (int i = 0; i < pp.Divisors.Count; i++)
                    {
                        result.Divisors.Add(pp.Divisors[i]);
                    }
                }
                else
                {
                    result.Multipliers.Add(p);
                }
            }

            SumNode sumNode = new SumNode();
            foreach (var add in Addends)
            {
                sumNode.Addends.Add(add.Div(result).Simplify());
            }
            foreach (var add in Subtrahends)
            {
                sumNode.Subtrahends.Add(add.Div(result).Simplify());
            }
            result.Multipliers.Add(sumNode);
            Addends.Sort(new ExprOrderComparer());
            Subtrahends.Sort(new ExprOrderComparer());
            return (true, result);
        }

        public (bool, Expr) 通分()
        {
            bool havD = false;
            Dictionary<Expr, int> DDict = new();
            foreach (var addend in Addends.Union(Subtrahends))
            {
                if (addend is ProductNode product)
                {
                    if (product.Divisors.Count > 0)
                    {
                        havD = true;
                        foreach (var divisor in product.Divisors)
                        {
                            if (divisor is PowerNode powerNode)
                            {
                                if (DDict.ContainsKey(powerNode.Base))
                                {
                                    if (DDict[powerNode.Base] < ((IntNode)powerNode.Exponent).Value)
                                        DDict[powerNode.Base] = ((IntNode)powerNode.Exponent).Value;
                                }
                                else
                                {
                                    DDict.Add(powerNode.Base, ((IntNode)powerNode.Exponent).Value);
                                }
                            }
                            else
                            {
                                if (DDict.ContainsKey(divisor))
                                {
                                }
                                else
                                {
                                    DDict.Add(divisor, 1);
                                }
                            }
                        }
                    }
                    else if (product.Constant is FractionNode fractionNode)
                    {
                        havD = true;
                        throw new NotImplementedException();
                        //var factorDict = fractionNode.Denominator.GetPrimeFactors();
                        //foreach (var factor in factorDict)
                        //{
                        //    if (DDict.ContainsKey(Let(factor.Key)))
                        //    {
                        //        if (DDict[Let(factor.Key)] < factor.Value)
                        //            DDict[Let(factor.Key)] = factor.Value;
                        //    }
                        //    else
                        //    {
                        //        DDict.Add(Let(factor.Key), factor.Value);
                        //    }
                        //}
                    }
                }
            }
            if (Constant is FractionNode ff)
            {
                havD = true;
                throw new NotImplementedException();
                //var factorDict = ff.Denominator.GetPrimeFactors();
                //foreach (var factor in factorDict)
                //{
                //    if (DDict.ContainsKey(Let(factor.Key)))
                //    {
                //        if (DDict[Let(factor.Key)] < factor.Value)
                //            DDict[Let(factor.Key)] = factor.Value;
                //    }
                //    else
                //    {
                //        DDict.Add(Let(factor.Key), factor.Value);
                //    }
                //}
            }
            if (!havD) { return (false, null); }
            SumNode 分子 = new SumNode();
            ProductNode 分母 = new ProductNode();
            foreach (var kv in DDict)
            {
                if (kv.Value == 1)
                {
                    分母.Multipliers.Add(kv.Key.Clone());
                }
                else
                {
                    分母.Multipliers.Add(kv.Key.Clone().Pow(kv.Value));
                }
            }

            var d = 分母.Simplify();
            foreach (var add in Addends)
            {
                var a = add.Clone().Mul(d).Simplify();
                分子.Addends.Add(a);
            }
            foreach (var sub in Subtrahends)
            {
                var a = sub.Clone().Mul(d);
                a = a.Simplify();
                分子.Subtrahends.Add(a);
            }
            if (Constant != Expr.Zero)
            {
                var a = Constant.Clone().Mul(d).Simplify();
                分子.Addends.Add(a);
                //if (Constant is IntNode i)
                //{
                //    var a = i.Mul(d).Simplify();
                //    分子.Addends.Add(a);
                //}
                //else if (Constant is RealFractionNode f)
                //{
                //    var a = f.Mul(d).Simplify();
                //    分子.Addends.Add(a);
                //}
            }

            var N = 分子.Simplify();
            var result = N.Div(分母);
            return (true, result);
        }

        public (bool, Expr) CheckDegeneration()
        {
            if (Addends.Count == 0 && Subtrahends.Count == 0) return (true, Constant);
            if (Addends.Count == 1 && Subtrahends.Count == 0 && Constant == Expr.Zero) return (true, Addends[0]);
            if (Addends.Count == 0 && Subtrahends.Count == 1 && Constant == Expr.Zero) return (true, (Expr)new ProductNode() { IsPositive = false, Multipliers = new() { Subtrahends[0] } });
            return (false, null);
        }

        public override string ToString()
        {
            var str = "(";
            bool first = true;
            if (Constant.CompareTo(0) != ExprCompareResult.Equal)
            {
                str += $"{Constant}";
                first = false;
            }
            foreach (Expr e in Addends)
            {
                if (first)
                    first = false;
                else
                    str += "+";
                str += e.ToString();
            }
            if (Subtrahends.Count > 0)
            {
                str += "-";
                first = true;
                foreach (Expr e in Subtrahends)
                {
                    if (first)
                        first = false;
                    else
                        str += "-";
                    str += e.ToString();
                }
            }
            return str + ")";
        }


        public override SumNode Opposite()
        {
            Constant = (RealNode)Constant.Opposite();
            var temp = Addends;
            Addends = Subtrahends;
            Subtrahends = temp;
            return this;
        }
    }
}