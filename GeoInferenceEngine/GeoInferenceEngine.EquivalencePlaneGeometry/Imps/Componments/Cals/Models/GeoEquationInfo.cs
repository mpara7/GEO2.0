using GeoInferenceEngine.Knowledges.Imps.Componments;

namespace GeoInferenceEngine.PredicateShared.Models
{
    public enum GeoEquationUnits
    {
        Unknow,
        Angle,
        Distance,
    }

    public enum GeoEquationTypes
    {
        //其他的
        Unknow,

        //Mut具体值
        Value,

        //两个Prop或Var Single Product的SumNode
        Ratio,

        //能表示成SumNode 其中子节点是（Prop或Var或Single Product）
        Addition,

        Multiplication,

        //复杂的
        Complex,
    }

    public class GeoEquationInfo
    {
        public GeoEquation GeoEquation { get; set; }
        ExprService exprService;
        FormularBase formularBase;

        public GeoEquationInfo(GeoEquation geoEquation, ExprService exprService)
        {
            GeoEquation = geoEquation;
            this.exprService = exprService;
            this.formularBase = exprService.FormularBase;
            this.HashCode = geoEquation.HashCode;
        }
        public bool IsSimplifyedByRationInfos { get; set; }
        public override string ToString()
        {
            return GeoEquation.ToString();
        }
        public Expr CoLeft { get; set; }
        public Expr CoRight { get; set; }
        public List<Knowledge> SimplifyReason { get; set; } = new List<Knowledge>();


        public void SimplifyByRationInfos()
        {
            CoLeft = GeoEquation.LeftPart.Clone();
            CoRight = GeoEquation.RightPart.Clone();

            var leftMuts = exprService.FindMuts(CoLeft);
            var rightMuts = exprService.FindMuts(CoRight);
            leftMuts.AddRange(rightMuts);
            var mutList = leftMuts.Distinct();
            Dictionary<RatioInfo, List<Mut>> replaceDict = FindReplaceItem(mutList);
            Dictionary<RatioInfo, List<Mut>> FindReplaceItem(IEnumerable<Mut> muts)
            {
                Dictionary<RatioInfo, List<Mut>> replaceDict = new Dictionary<RatioInfo, List<Mut>>();
                foreach (var mut in muts)
                {
                    var ratio = formularBase.TryGetRatioInfo(mut);
                    if (ratio != null)
                    {
                        if (replaceDict.ContainsKey(ratio))
                        {
                            replaceDict[ratio].Add(mut);
                        }
                        else
                        {
                            replaceDict.Add(ratio, [mut]);
                        }
                    }
                }
                return replaceDict;
            }
            void Replace(Expr target, MutNode from, MutNode to)
            {
                if (target is SumNode sum)
                {
                    for (int i = 0; i < sum.Addends.Count; i++)
                    {
                        if (sum.Addends[i] == from)
                        {
                            sum.Addends[i] = to;
                        }
                        else
                        {
                            Replace(sum.Addends[i], from, to);
                        }
                    }
                    for (int i = 0; i < sum.Subtrahends.Count; i++)
                    {
                        if (sum.Subtrahends[i] == from)
                        {
                            sum.Subtrahends[i] = to;
                        }
                        else
                        {
                            Replace(sum.Subtrahends[i], from, to);
                        }
                    }
                }
                else if (target is ProductNode product)
                {
                    for (int i = 0; i < product.Multipliers.Count; i++)
                    {
                        if (product.Multipliers[i] == from)
                        {
                            product.Multipliers[i] = to;
                        }
                        else
                        {
                            Replace(product.Multipliers[i], from, to);
                        }
                    }
                    for (int i = 0; i < product.Divisors.Count; i++)
                    {
                        if (product.Divisors[i] == from)
                        {
                            product.Divisors[i] = to;
                        }
                        else
                        {
                            Replace(product.Divisors[i], from, to);
                        }
                    }
                }
                else if (target is PowerNode power)
                {
                    if (power.Base == from)
                    {
                        power.Base = to;
                    }
                }
            }

            foreach (var item in replaceDict.Where(v => v.Value.Count > 1))
            {
                foreach (var mut in item.Value.Skip(1))
                {
                    Replace(CoLeft, mut, item.Value.First());
                    Replace(CoRight, mut, item.Value.First());
                    SimplifyReason.AddRange(item.Key.SimpleFindReason(mut, item.Value.First()));
                    IsSimplifyedByRationInfos = true;
                }
            }

            CoLeft = CoLeft.Simplify();
            CoRight = CoRight.Simplify();
        }
        void doSubSumNode(SumNode tempSum)
        {
            if (tempSum.Subtrahends.Count > 0 && tempSum.Addends.Count == 0)
            {
                tempSum = (SumNode)tempSum.Opposite();
            }
            var coffs = exprService.FindCoffs(tempSum);
            if (coffs.Count == 0)
            {
                Type = GeoEquationTypes.Unknow;
            }
            else if (coffs.Count == 1)
            {
                Type = GeoEquationTypes.Value;
                Mut = coffs.First().Key;
                Value = tempSum.Constant.Opposite() / coffs.First().Value;
            }
            else if (coffs.Count == 2)
            {
                if (tempSum.Constant == 0)
                {
                    Type = GeoEquationTypes.Ratio;
                    LeftMut = coffs.First().Key;
                    RightMut = coffs.Skip(1).First().Key;
                    Value = coffs.Skip(1).First().Value / coffs.First().Value;
                }
                else
                {
                    Type = GeoEquationTypes.Addition;
                    AddictionRealValue = tempSum.Constant;
                    AdditionCoefficients = coffs;
                }
            }
            else
            {
                Type = GeoEquationTypes.Addition;
                AddictionRealValue = tempSum.Constant;
                AdditionCoefficients = coffs;
            }
        }
        void doDivdeProductNode(ProductNode tempProduct)
        {
            if (tempProduct.Divisors.Count > 0 && tempProduct.Multipliers.Count == 0)
            {
                tempProduct = (ProductNode)tempProduct.Invert();
            }
            var expDict = exprService.FindExponents(tempProduct);
            if (expDict.Count == 0)
            {
                Type = GeoEquationTypes.Unknow;
            }
            else if (expDict.Count == 1)
            {
                if (expDict.First().Value < 0)
                {
                    Type = GeoEquationTypes.Value;
                    Mut = expDict.First().Key;
                    Value = tempProduct.Constant.Pow(expDict.First().Value.Opposite().Invert());
                }
                else
                {
                    Type = GeoEquationTypes.Value;
                    Mut = expDict.First().Key;
                    Value = tempProduct.Constant.Pow(expDict.First().Value.Invert());
                }
            }
            else if (expDict.Count == 2)
            {
                //2*a/b=1
                if (expDict.First().Value == 1 && expDict.Skip(1).First().Value == -1)
                {
                    Type = GeoEquationTypes.Ratio;
                    LeftMut = expDict.First().Key;
                    RightMut = expDict.Skip(1).First().Key;
                    Ratio = tempProduct.Constant.Invert();
                }
                else if (expDict.First().Value == -1 && expDict.Skip(1).First().Value == 1)
                {
                    Type = GeoEquationTypes.Ratio;
                    LeftMut = expDict.First().Key;
                    RightMut = expDict.Skip(1).First().Key;
                    Ratio = tempProduct.Constant;
                }
                else
                {
                    Type = GeoEquationTypes.Multiplication;
                    MultiplicatioRealValue = tempProduct.Constant;
                    MultiplicationExps = expDict;
                }

            }
            else
            {
                Type = GeoEquationTypes.Multiplication;
                MultiplicatioRealValue = tempProduct.Constant;
                MultiplicationExps = expDict;
            }
        }
        void doExtract(Expr expr1, Expr expr2)
        {
            if (expr1 is RealNode real1)
            {
                if (expr2 is RealNode real2) { Type = GeoEquationTypes.Unknow; }
                else if (expr2 is MutNode mut2)
                {
                    Type = GeoEquationTypes.Value;
                    Mut = mut2.Mut;
                    Value = expr1;

                    var g = mut2.Mut as GeoProp;
                    Unit = g?.PropName != GeoProp.Size ? GeoEquationUnits.Distance : GeoEquationUnits.Angle;
                }
                else if (expr2 is SumNode sum2)
                {
                    var temp = sum2.Sub(expr1);
                    temp = temp.Simplify();
                    if (temp is SumNode tempSum)
                    {
                        //如果能找到字典
                        try
                        {
                            var coffs = exprService.FindCoffs(tempSum);
                            if (coffs.Count == 0)
                            {
                                Type = GeoEquationTypes.Unknow;
                            }
                            else if (coffs.Count == 1)
                            {
                                Type = GeoEquationTypes.Value;
                                Mut = coffs.First().Key;
                                Value = tempSum.Constant.Opposite() / coffs.First().Value;
                            }
                            else if (coffs.Count == 2)
                            {
                                if (tempSum.Constant == 0)
                                {
                                    Type = GeoEquationTypes.Ratio;
                                    LeftMut = coffs.First().Key;
                                    RightMut = coffs.Skip(1).First().Key;
                                    Ratio = coffs.Skip(1).First().Value / coffs.First().Value;
                                }
                                else
                                {
                                    Type = GeoEquationTypes.Addition;
                                    AddictionRealValue = tempSum.Constant;
                                    AdditionCoefficients = coffs;
                                }
                            }
                            else
                            {
                                Type = GeoEquationTypes.Addition;
                                AddictionRealValue = tempSum.Constant;
                                AdditionCoefficients = coffs;
                            }
                        }
                        catch (Exception)
                        {
                            Type = GeoEquationTypes.Complex;
                        }
                    }
                    else
                    {
                        Type = GeoEquationTypes.Unknow;
                    }
                }
                else if (expr2 is ProductNode product2)
                {
                    var temp = product2.Div(expr1);
                    temp = temp.Simplify();
                    if (temp is ProductNode tempProduct)
                    {
                        if (tempProduct.Divisors.Count > 0 && tempProduct.Multipliers.Count == 0)
                        {
                            tempProduct = (ProductNode)tempProduct.Invert();
                        }
                        try
                        {
                            var expDict = exprService.FindExponents(tempProduct);
                            if (expDict.Count == 0)
                            {
                                Type = GeoEquationTypes.Unknow;
                            }
                            else if (expDict.Count == 1)
                            {
                                if (expDict.First().Value < 0)
                                {
                                    Type = GeoEquationTypes.Value;
                                    Mut = expDict.First().Key;
                                    Value = tempProduct.Constant.Pow(expDict.First().Value.Opposite().Invert());
                                }
                                else
                                {
                                    Type = GeoEquationTypes.Value;
                                    Mut = expDict.First().Key;
                                    Value = tempProduct.Constant.Pow(expDict.First().Value.Invert());
                                }
                            }
                            else if (expDict.Count == 2)
                            {
                                //2*a/b=1
                                if (expDict.First().Value == 1 && expDict.Skip(1).First().Value == -1)
                                {
                                    Type = GeoEquationTypes.Ratio;
                                    LeftMut = expDict.First().Key;
                                    RightMut = expDict.Skip(1).First().Key;
                                    Ratio = tempProduct.Constant.Invert();
                                }
                                else if (expDict.First().Value == -1 && expDict.Skip(1).First().Value == 1)
                                {
                                    Type = GeoEquationTypes.Ratio;
                                    LeftMut = expDict.First().Key;
                                    RightMut = expDict.Skip(1).First().Key;
                                    Ratio = tempProduct.Constant;
                                }
                                else
                                {
                                    Type = GeoEquationTypes.Multiplication;
                                    MultiplicatioRealValue = tempProduct.Constant;
                                    MultiplicationExps = expDict;
                                }

                            }
                            else
                            {
                                Type = GeoEquationTypes.Multiplication;
                                MultiplicatioRealValue = tempProduct.Constant;
                                MultiplicationExps = expDict;
                            }
                        }
                        catch
                        {
                            Type = GeoEquationTypes.Complex;
                        }
                    }
                }
            }
            else if (expr1 is MutNode mut1)
            {
                if (expr2 is MutNode mut2)
                {
                    Type = GeoEquationTypes.Ratio;
                    LeftMut = mut1.Mut;
                    RightMut = mut2.Mut;
                    Ratio = Expr.One;
                }
                else if (expr2 is SumNode sum2)
                {
                    var temp = sum2.Sub(expr1);
                    temp = temp.Simplify();
                    if (temp is SumNode tempSum)
                    {
                        //如果能找到字典
                        try
                        {
                            var coffs = exprService.FindCoffs(tempSum);
                            if (coffs.Count == 0)
                            {
                                Type = GeoEquationTypes.Unknow;
                            }
                            else if (coffs.Count == 1)
                            {
                                Type = GeoEquationTypes.Value;
                                Mut = coffs.First().Key;
                                Value = tempSum.Constant.Opposite() / coffs.First().Value;
                            }
                            else if (coffs.Count == 2)
                            {
                                if (tempSum.Constant == 0)
                                {
                                    Type = GeoEquationTypes.Ratio;
                                    LeftMut = coffs.First().Key;
                                    RightMut = coffs.Skip(1).First().Key;
                                    Ratio = coffs.Skip(1).First().Value / coffs.First().Value;
                                }
                                else
                                {
                                    Type = GeoEquationTypes.Addition;
                                    AddictionRealValue = tempSum.Constant;
                                    AdditionCoefficients = coffs;
                                }
                            }
                            else
                            {
                                Type = GeoEquationTypes.Addition;
                                AddictionRealValue = tempSum.Constant;
                                AdditionCoefficients = coffs;
                            }
                        }
                        catch (Exception)
                        {
                            Type = GeoEquationTypes.Complex;
                        }
                    }
                    else
                    {
                        Type = GeoEquationTypes.Unknow;
                    }
                }
                else if (expr2 is ProductNode product2)
                {
                    var temp = product2.Div(expr1);
                    temp = temp.Simplify();
                    if (temp is ProductNode tempProduct)
                    {
                        if (tempProduct.Divisors.Count > 0 && tempProduct.Multipliers.Count == 0)
                        {
                            tempProduct = (ProductNode)tempProduct.Invert();
                        }
                        try
                        {
                            var expDict = exprService.FindExponents(tempProduct);
                            if (expDict.Count == 0)
                            {
                                Type = GeoEquationTypes.Unknow;
                            }
                            else if (expDict.Count == 1)
                            {
                                if (expDict.First().Value < 0)
                                {
                                    Type = GeoEquationTypes.Value;
                                    Mut = expDict.First().Key;
                                    Value = tempProduct.Constant.Pow(expDict.First().Value.Opposite().Invert());
                                }
                                else
                                {
                                    Type = GeoEquationTypes.Value;
                                    Mut = expDict.First().Key;
                                    Value = tempProduct.Constant.Pow(expDict.First().Value.Invert());
                                }
                            }
                            else if (expDict.Count == 2)
                            {
                                //2*a/b=1
                                if (expDict.First().Value == 1 && expDict.Skip(1).First().Value == -1)
                                {
                                    Type = GeoEquationTypes.Ratio;
                                    LeftMut = expDict.First().Key;
                                    RightMut = expDict.Skip(1).First().Key;
                                    Ratio = tempProduct.Constant.Invert();
                                }
                                else if (expDict.First().Value == -1 && expDict.Skip(1).First().Value == 1)
                                {
                                    Type = GeoEquationTypes.Ratio;
                                    LeftMut = expDict.First().Key;
                                    RightMut = expDict.Skip(1).First().Key;
                                    Ratio = tempProduct.Constant;
                                }
                                else
                                {
                                    Type = GeoEquationTypes.Multiplication;
                                    MultiplicatioRealValue = tempProduct.Constant;
                                    MultiplicationExps = expDict;
                                }

                            }
                            else
                            {
                                Type = GeoEquationTypes.Multiplication;
                                MultiplicatioRealValue = tempProduct.Constant;
                                MultiplicationExps = expDict;
                            }
                        }
                        catch
                        {
                            Type = GeoEquationTypes.Complex;
                        }
                    }
                }
            }
            else if (expr1 is SumNode sum1)
            {
                if (expr2 is SumNode sum2)
                {
                    var temp = sum2.Sub(expr1);
                    temp = temp.Simplify();
                    if (temp is SumNode tempSum)
                    {
                        //如果能找到字典
                        try
                        {
                            var coffs = exprService.FindCoffs(tempSum);
                            if (coffs.Count == 0)
                            {
                                Type = GeoEquationTypes.Unknow;
                            }
                            else if (coffs.Count == 1)
                            {
                                Type = GeoEquationTypes.Value;
                                Mut = coffs.First().Key;
                                Value = tempSum.Constant.Opposite() / coffs.First().Value;
                            }
                            else if (coffs.Count == 2)
                            {
                                if (tempSum.Constant == 0)
                                {
                                    Type = GeoEquationTypes.Ratio;
                                    LeftMut = coffs.First().Key;
                                    RightMut = coffs.Skip(1).First().Key;
                                    Ratio = coffs.Skip(1).First().Value / coffs.First().Value;
                                }
                                else
                                {
                                    Type = GeoEquationTypes.Addition;
                                    AddictionRealValue = tempSum.Constant;
                                    AdditionCoefficients = coffs;
                                }
                            }
                            else
                            {
                                Type = GeoEquationTypes.Addition;
                                AddictionRealValue = tempSum.Constant;
                                AdditionCoefficients = coffs;
                            }
                        }
                        catch (Exception)
                        {
                            Type = GeoEquationTypes.Complex;
                        }
                    }
                    else
                    {
                        Type = GeoEquationTypes.Unknow;
                    }
                }
            }
            else if (expr1 is ProductNode product1)
            {
                var temp = expr1 / expr2;
                doDivdeProductNode((ProductNode)temp);
            }
        }
        public void ExtractInfo()
        {
            List<Type> order = new List<Type>() { typeof(RealNode), typeof(MutNode), typeof(SumNode), typeof(ProductNode), };
            if (order.IndexOf(CoLeft.GetType()) < order.IndexOf(CoRight.GetType()))
            {
                
                doExtract(CoLeft, CoRight);
            }
            else
            {
                doExtract(CoRight, CoLeft);
            }
            var muts= exprService.FindMuts(CoLeft).Union(exprService.FindMuts(CoRight));
            if (muts.Count() > 0)
            {

                var a= muts.First() as GeoProp;
                Unit = a.PropName == GeoProp.Size ? GeoEquationUnits.Angle : GeoEquationUnits.Distance;
            }

        }
        public void TryMakeNewKnowledge()
        {
            if (Type == GeoEquationTypes.Value)
            {
                if (Mut is GeoProp geoProp)
                {
                    var knowledge = geoProp.ToSolveKnowledge();
                    knowledge.Expr = Value;
                    EquivalenceKnowledge = knowledge;
                }
            }
            else if (Type == GeoEquationTypes.Ratio)
            {
                if (Ratio == 1)
                {
                    if (LeftMut is GeoProp left && RightMut is GeoProp right)
                    {
                        if (left.Knowledge is Angle angle1 && right.Knowledge is Angle angle2)
                        {
                            Knowledge temp = new AngleSizeEqual(angle1, angle2);
                            temp.AddReason("等式化简");
                            temp.AddCondition(SimplifyReason);
                            EquivalenceKnowledge = temp;
                        }
                        else if (left.Knowledge is Segment segment1 && right.Knowledge is Segment segment2)
                        {
                            Knowledge temp = new SegmentLengthEqual(segment1, segment2);
                            temp.AddReason("等式化简");
                            temp.AddCondition(SimplifyReason);
                            EquivalenceKnowledge = temp;
                        }
                        else if (left.Knowledge is PlaneFigure planeFigure && left.Knowledge.GetType() == right.Knowledge.GetType())
                        {
                        }
                    }
                }
                else
                {
                    if (LeftMut is GeoProp left && RightMut is GeoProp right)
                    {
                        if (left.Knowledge is Angle angle1 && right.Knowledge is Angle angle2)
                        {
                            Knowledge temp = new AngleSizeRatio(angle1, angle2, Ratio);
                            temp.AddReason("等式化简");
                            temp.AddCondition(SimplifyReason);
                            EquivalenceKnowledge = temp;

                        }
                        else if (left.Knowledge is Segment segment1 && right.Knowledge is Segment segment2)
                        {
                            Knowledge temp = new SegmentLengthRatio(segment1, segment2, Ratio);
                            temp.AddReason("等式化简");
                            temp.AddCondition(SimplifyReason);
                            EquivalenceKnowledge = temp;
                        }
                    }
                }
            }
            else if (IsSimplifyedByRationInfos)
            {
                if (Type == GeoEquationTypes.Addition)
                {
                    Expr temp = Expr.Zero;
                    foreach (var kv in AdditionCoefficients)
                    {
                        if (kv.Value == 1)
                        {
                            temp += kv.Key;
                        }
                        else
                        {
                            temp += kv.Value * kv.Key;
                        }
                    }
                    temp += AddictionRealValue;
                    GeoEquation geoEquation = new GeoEquation(temp, 0);
                    geoEquation.AddReason("连加等式化简");
                    geoEquation.AddCondition(SimplifyReason);
                    SimplifiedGeoequation = geoEquation;
                }
                else if (Type == GeoEquationTypes.Multiplication)
                {
                    Expr temp = Expr.Zero;
                    foreach (var kv in MultiplicationExps)
                    {
                        if (kv.Value == 1)
                        {
                            temp *= kv.Key;
                        }
                        else
                        {
                            temp *= kv.Key.Pow(kv.Value);
                        }
                    }
                    temp *= AddictionRealValue;
                    GeoEquation geoEquation = new GeoEquation(temp, 0);
                    geoEquation.AddReason("连乘等式化简");
                    geoEquation.AddCondition(SimplifyReason);
                    SimplifiedGeoequation = geoEquation;
                }
            }
        }

        public GeoEquationUnits Unit { get; set; }
        public GeoEquationTypes Type { get; set; }
        public ulong HashCode { get; set; }

        public Knowledge? EquivalenceKnowledge { get; set; }
        #region ValueInfo

        public Mut Mut { get; set; }
        public Expr Value { get; set; }

        #endregion ValueInfo

        #region RatioInfo

        public Mut LeftMut { get; set; }
        public Mut RightMut { get; set; }
        public Expr Ratio { get; set; }

        #endregion RatioInfo


        public GeoEquation? SimplifiedGeoequation { get; set; }
        #region Addiction
        public Expr AddictionRealValue { get; set; }
        public Dictionary<Mut, Expr> AdditionCoefficients { get; set; } = new();

        #endregion Addiction

        #region Mutilation
        public Expr MultiplicatioRealValue { get; set; }
        public Dictionary<Mut, Expr> MultiplicationExps { get; set; } = new();

        #endregion Mutilation
    }
}