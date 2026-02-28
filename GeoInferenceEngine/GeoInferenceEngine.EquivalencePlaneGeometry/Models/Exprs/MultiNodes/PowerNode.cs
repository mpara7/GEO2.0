using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    public class PowerNode : Expr
    {
        /// <summary>
        /// 指数
        /// </summary>
        public Expr Exponent { get; set; }
        /// <summary>
        /// 底数
        /// </summary>
        public Expr Base { get; set; }

        public override Expr Simplify()
        {
            //if (IsSimplified) return this;
            //IsSimplified = true;
            if (Base is MutNode && Exponent == Expr.One) return Base;

            Base = Base.Simplify();
            Exponent = Exponent.Simplify();
            var intBase = Base as IntNode;
            var powerBase = Base as PowerNode;
            var sumBase = Base as SumNode;
            var productBase = Base as ProductNode;
            var fractionBase = Base as FractionNode;

            var intExp = Exponent as IntNode;
            var productExp = Exponent as FractionNode;
            //指数为负数 调整为除法
            if (Exponent.CompareTo(0) == ExprCompareResult.Less)
            {
                ProductNode product = new();
                Expr inner = new PowerNode()
                {
                    Base = Base,
                    Exponent = (ARationalNode)Exponent.Opposite()
                };
                inner = inner.Simplify();
                product.Divisors.Add(inner);
                return product;
            }
            //底数与指数都是整形 直接计算
            if (intBase is not null && intExp is not null)
            {
                return Let((int)Math.Pow(intBase.Value, intExp.Value));
            }
            //底数是整形 开根号 尽可能提取指数 
            else if (intBase is not null && productExp is not null)
            {
                var factorDict = intBase.GetPrimeFactors();
                //没有可以提取的
                if (factorDict.ToList().TrueForAll(kv => kv.Value < productExp.Denominator))
                    return this;
                ProductNode product = new();
                foreach (var factor in factorDict)
                {
                    var times = factor.Value / productExp.Denominator;
                    var mod = factor.Value % productExp.Denominator;
                    product.Constant = (RealNode)product.Constant.Mul(factor.Key * times);
                    product.Multipliers.Add(new PowerNode() { Exponent = (ARationalNode)Exponent.Clone(), Base = Let((int)Math.Pow(factor.Key, int.Parse(mod.ToString()))) });
                }
                var sim = product.Simplify();
                return sim;
            }
            //底数是次方节点
            else if (powerBase is not null)
            {
                Base = powerBase.Base;
                Exponent = (ARationalNode)Exponent.Mul(powerBase.Exponent).Simplify();
                return this;
            }
            //指数分配
            else if (productBase is not null)
            {
                ProductNode product = new();
                foreach (var mul in productBase.Multipliers)
                {
                    PowerNode powerNode = new PowerNode() { Exponent = (ARationalNode)Exponent.Clone(), Base = mul };
                    product.Multipliers.Add(powerNode);
                }
                foreach (var mul in productBase.Divisors)
                {
                    PowerNode powerNode = new PowerNode() { Exponent = (ARationalNode)Exponent.Clone(), Base = mul };
                    product.Divisors.Add(powerNode);
                }
                var result = product.Simplify();
                return result;
            }
            else if (fractionBase is not null)
            {
                ProductNode product = new();
                PowerNode powerNode1 = new PowerNode() { Exponent = Exponent.Clone(), Base = fractionBase.Numerator };
                product.Multipliers.Add(powerNode1);
                PowerNode powerNode2 = new PowerNode() { Exponent = Exponent.Clone(), Base = fractionBase.Denominator };
                product.Multipliers.Add(powerNode2);
                var result = product.Simplify();
                return result;
            }
            //展开次方
            else if (sumBase is not null && intExp is not null)
            {
                ProductNode product = new();
                for (int i = 0; i < intExp.Value; i++)
                {
                    product.Multipliers.Add(sumBase.Clone());
                }
                return product.Simplify();
            }

            var check = CheckDegeneration();
            if (check.Item1) { return check.Item2; }
            return this;
        }
        public (bool, Expr) CheckDegeneration()
        {

            if (Exponent.Equals(0)) return (true, 1);
            if (Exponent.Equals(1)) return (true, Base);
            return (false, null);
        }
        public override Expr Clone()
        {
            return new PowerNode() { Base = Base, Exponent = Exponent };
        }
        public override string ToString()
        {
            var front = "";
            if (Base is SumNode or ProductNode)
                front += $"({Base})";
            else
                front += Base.ToString();

            front += "^";

            if (Exponent is FractionNode)
                front += $"({Exponent})";
            else
                front += Exponent.ToString();
            return front;
        }
    }
}
