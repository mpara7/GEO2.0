using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs
{
    

    public abstract class RealNode : Expr
    {
        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="i"></param>
        public static implicit operator RealNode(int i)
        {
            return FromInt(i);
        }
        public abstract double GetApproximation();

        #region 重载方法
        public static RealNode operator +(RealNode ZExpr1, RealNode ZExpr2) => (RealNode)ZExpr1.Add(ZExpr2);

        public static RealNode operator -(RealNode ZExpr1, RealNode ZExpr2) => (RealNode)ZExpr1.Sub(ZExpr2);

        public static RealNode operator *(RealNode ZExpr1, RealNode ZExpr2) => (RealNode)ZExpr1.Mul(ZExpr2);

        public static RealNode operator /(RealNode ZExpr1, RealNode ZExpr2) => (RealNode)ZExpr1.Div(ZExpr2);

        public static RealNode operator -(RealNode ZExpr) => ZExpr.Opposite();
        #endregion

        #region 辅助方法
        /// <summary>
        /// 找最大公因子
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int FindGCD(int num1, int num2)
        {
            // 处理负数，取绝对值
            //num1 = Math.Abs(num1);
            //num2 = Math.Abs(num2);

            while (num2 != 0)
            {
                int remainder = num1 % num2;
                num1 = num2;
                num2 = remainder;
            }

            return num1;
        }
        /// <summary>
        /// 找最大公因子
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Expr FindGCD(Expr a, Expr b)
        {
            if (b == 0)
            {
                return a;
            }
            return FindGCD(b, a % b);
        }
        #endregion

        public override abstract RealNode Clone();
        public abstract override RealNode Simplify();

        #region 基础运算
        #region 基础运算
        public override Expr Add(Expr r)
        {
            if (r is RealNode real)
            {
                RealSumNode realSum = new RealSumNode();
                realSum.Addends.Add(this.Clone());
                realSum.Addends.Add(real.Clone());
                var result = realSum.Simplify();
                return result;
            }
            else if (r is SumNode sum)
            {
                var temp = sum.Clone();
                temp.Constant = temp.Constant + this;
                var result = temp.Simplify();
                return temp;
            }
            return base.Add(r);
        }
        public override Expr Sub(Expr r)
        {
            if (r is RealNode real)
            {
                RealSumNode realSum = new RealSumNode();
                realSum.Addends.Add(this.Clone());
                realSum.Subtrahends.Add(real.Clone());
                var result = realSum.Simplify();
                return result;
            }
            else if (r is SumNode sum)
            {
                var temp = sum.Clone();
                temp.Constant = temp.Constant- this;
                var result = temp.Simplify();
                return temp;
            }
            return base.Add(r);
        }
        public override Expr Mul(Expr r)
        {
            if (r is RealNode real)
            {
                RealProductNode realSum = new RealProductNode();
                realSum.Multipliers.Add(this.Clone());
                realSum.Multipliers.Add(real.Clone());
                var result = realSum.Simplify();
                return result;
            }
            else if (r is ProductNode product)
            {
                var temp = product.Clone();
                temp.Constant = temp.Constant* this;
                var result = temp.Simplify();
                return temp;
            }
            return base.Mul(r);
        }
        public override Expr Div(Expr r)
        {
            if (r is RealNode real)
            {
                RealProductNode realSum = new RealProductNode();
                realSum.Multipliers.Add(this.Clone());
                realSum.Divisors.Add(real.Clone());
                var result = realSum.Simplify();
                return result;
            }
            else if (r is ProductNode product)
            {
                var temp = product.Clone();
                temp.Constant = temp.Constant / this;
                var result = temp.Simplify();
                return temp;
            }
            return base.Mul(r);
        }


        public override RealNode Pow(Expr r)
        {
            if (r is RealNode real)
            {
                RealPowerNode powerNode = new RealPowerNode();
                powerNode.Base = this.Clone();
                powerNode.Exponent = real.Clone();
                return powerNode;
            }
            throw new NotImplementedException("暂不支持指数为非有理数");
        }
        public override RealNode Sqrt()
        {
            return (RealNode)this.Pow(AHalf);
        }
        #endregion

        #region 常用函数
        public override RealNode Opposite()
        {
            RealProductNode result = new RealProductNode();
            result.IsPositive = false;
            result.Multipliers.Add(this);
            return result;
        }
        public override RealNode Invert()
        {
            RealProductNode result = new RealProductNode();
            result.Divisors.Add(this);
            return result;
        }


        #endregion

        #endregion
    }
}
