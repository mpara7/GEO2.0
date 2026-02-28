using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal;

using GeoInferenceEngine.Knowledges;
using GeoInferenceEngine.Knowledges.Imps.Componments;

using GeoInferenceEngine.PredicateShared.Models;
using System.Reflection.Metadata;
using YamlDotNet.Core.Tokens;

namespace GeoInferenceEngine.PlaneKnowledges.Engine.Comps.GeoCal;

public class CalHanderStrategy : IInferenceComponent
{
    [ZDI]
    private FormularBase _formularBase { get; set; }

    [ZDI]
    private CalAddProcessor calAddProcessor;

    [ZDI]
    private ExprService AExprService { get; set; }

    public CalHanderStrategy()
    {
        //初始化
        for (int i = 0; i < Level; i++)
            AngleAdditionDict.Add(i, new List<GeoEquation>());
        for (int i = 0; i < Level; i++)
            AngleMultiplicationDict.Add(i, new List<GeoEquation>());
        for (int i = 0; i < Level; i++)
            DistanceAdditionDict.Add(i, new List<GeoEquation>());
        for (int i = 0; i < Level; i++)
            DistanceMultiplicationDict.Add(i, new List<GeoEquation>());

        for (int i = 0; i < Level; i++)
            AngleComplexAdditionDict.Add(i, new List<GeoEquation>());
        for (int i = 0; i < Level; i++)
            AngleComplexMultiplicationDict.Add(i, new List<GeoEquation>());
        for (int i = 0; i < Level; i++)
            DistanceComplexAdditionDict.Add(i, new List<GeoEquation>());
        for (int i = 0; i < Level; i++)
            DistanceComplexMultiplicationDict.Add(i, new List<GeoEquation>());

    }

    private int Level = 4;
    private Dictionary<int, List<GeoEquation>> AngleAdditionDict = new Dictionary<int, List<GeoEquation>>();
    private Dictionary<int, List<GeoEquation>> AngleMultiplicationDict = new Dictionary<int, List<GeoEquation>>();
    private Dictionary<int, List<GeoEquation>> DistanceAdditionDict = new Dictionary<int, List<GeoEquation>>();
    private Dictionary<int, List<GeoEquation>> DistanceMultiplicationDict = new Dictionary<int, List<GeoEquation>>();

    private Dictionary<int, List<GeoEquation>> AngleComplexAdditionDict = new Dictionary<int, List<GeoEquation>>();
    private Dictionary<int, List<GeoEquation>> AngleComplexMultiplicationDict = new Dictionary<int, List<GeoEquation>>();
    private Dictionary<int, List<GeoEquation>> DistanceComplexAdditionDict = new Dictionary<int, List<GeoEquation>>();
    private Dictionary<int, List<GeoEquation>> DistanceComplexMultiplicationDict = new Dictionary<int, List<GeoEquation>>();

    private int AngleAdditionGeoEquationInfosLastEquationCoun = 0;
    private int DistanceAdditionGeoEquationInfosLastEquationCoun = 0;

    private int AngleComplexGeoEquationInfosLastEquationCoun = 0;
    private int DistanceComplexGeoEquationInfosLastEquationCoun = 0;

    private int AngleMultiplicationGeoEquationInfosLastEquationCoun = 0;
    private int DistanceMultiplicationGeoEquationInfosLastEquationCoun = 0;


    // private List<GeoEquationInfo> replaceProcessing = new List<GeoEquationInfo>();

    //falg用来记录每次新产生等式的数量,yong
    //private int addinFlag = 0;

    //private int multingFlag = 0;

    public const int MaxNodeNumbers = 10;

    #region 连加

    /// <summary>
    /// 连加策略，组合N(level-1)个等式，a+b在第一层，a+b+c在第二层.....
    /// </summary>
    /// <param name="level"></param>
    public void AddingEquations(int level = 4)//BUG--->AngleAdditionGeoEquationInfos和addingDict的问题冲突
    {
        //自身为第一层
        level = level - 1;

        //组合处理
        GenerateAddingEquations(_formularBase.AngleAdditionGeoEquationInfos, AngleAdditionGeoEquationInfosLastEquationCoun, AngleAdditionDict, 0, level);
        GenerateAddingEquations(_formularBase.DistanceAdditionGeoEquationInfos, DistanceAdditionGeoEquationInfosLastEquationCoun, DistanceAdditionDict, 0, level);
        // GenerateAddingEquations(_formularBase.AngleComplexGeoEquationInfos, AngleComplexGeoEquationInfosLastEquationCoun, AngleComplexAdditionDict, 0, level);
        //GenerateAddingEquations(_formularBase.DistanceComplexGeoEquationInfos, DistanceComplexGeoEquationInfosLastEquationCoun, DistanceComplexAdditionDict, 0, level);

        AngleAdditionGeoEquationInfosLastEquationCoun = _formularBase.AngleAdditionGeoEquationInfos.Count;
        HandleResult(AngleAdditionDict);

        DistanceAdditionGeoEquationInfosLastEquationCoun = _formularBase.DistanceAdditionGeoEquationInfos.Count;
        HandleResult(DistanceAdditionDict);

        //AngleComplexGeoEquationInfosLastEquationCoun = _formularBase.AngleComplexGeoEquationInfos.Count;
        //HandleResult(AngleComplexAdditionDict);

        //DistanceComplexGeoEquationInfosLastEquationCoun = _formularBase.DistanceComplexGeoEquationInfos.Count;
        //HandleResult(DistanceComplexAdditionDict);
    }

    /// <summary>
    /// 进行相加减组合处理
    /// </summary>
    /// <param name="equationListInfo">待处理的等式表</param>
    /// <param name="currentDic">存放相加结果</param>
    /// <param name="index">当前在哪一层</param>
    /// <param name="level">最高层数</param>
    /// 
    //private void GenerateAddingEquations(Dictionary<ulong, GeoEquationInfo> equationListInfo, int LastEquationCount, Dictionary<int, List<GeoEquation>> currentDic, int index, int level)
    //{
    //    if (index == level) return;

    //    var equationList = equationListInfo.Values.Select(v => v.GeoEquation).ToList();
    //    int count = equationList.Count;
    //    Dictionary<int, HashSet<int>> skipPairs = new();

    //    if (index == 0)
    //    {
    //        for (int i = 0; i < count; i++)
    //        {
    //            for (int j = i + 1; j < count; j++)
    //            {
    //                if (!skipPairs.ContainsKey(i))
    //                    skipPairs[i] = new HashSet<int>();

    //                if (!skipPairs[i].Contains(j) && !equationList[j].hasCodition(equationList[i]))
    //                {
    //                    CombinationAddingEquations(equationList[i], equationList[j], currentDic, index);
    //                    skipPairs[i].Add(j);
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        for (int i = LastEquationCount; i < count; i++)
    //        {
    //            var lastLevel = currentDic[index - 1];
    //            HashSet<int> seenHashes = new();
    //            List<GeoEquation> uniqueLastLevel = new();

    //            for (int j = 0; j < lastLevel.Count; j++)
    //            {
    //                if (i == j || lastLevel[j].hasCodition(equationList[i])) continue;

    //                CombinationAddingEquations(equationList[i], lastLevel[j], currentDic, index);

    //                if (seenHashes.Add((int)lastLevel[j].HashCode))
    //                    uniqueLastLevel.Add(lastLevel[j]);
    //            }
    //            currentDic[index - 1] = uniqueLastLevel; // 更新去重后的列表
    //        }
    //    }

    //    GenerateAddingEquations(equationListInfo, LastEquationCount, currentDic, ++index, level);
    //}

    private void GenerateAddingEquations(Dictionary<ulong, GeoEquationInfo> equationListInfo, int LastEquationCount, Dictionary<int, List<GeoEquation>> currentDic, int index, int level)
    {
        if (index == level) return;

        //初始化
        if (index == 0)
        {
            for (int i = 0; i < equationListInfo.Count; i++)
            {
                if (LastEquationCount == 0)
                {
                    for (int j = i + 1; j < equationListInfo.Count; j++)
                    {
                        CombinationAddingEquations(equationListInfo.ElementAt(i).Value.GeoEquation, equationListInfo.ElementAt(j).Value.GeoEquation, currentDic, index);
                    }
                }
                else
                {
                    int j = LastEquationCount;
                    if (i >= LastEquationCount)
                        j = i + 1;

                    while (j < equationListInfo.Count)
                    {
                        if (i >= j)
                        {
                            j++; continue;
                        }
                        if (!(equationListInfo.ElementAt(j).Value.GeoEquation.hasCodition(equationListInfo.ElementAt(i).Value.GeoEquation)))
                            CombinationAddingEquations(equationListInfo.ElementAt(i).Value.GeoEquation, equationListInfo.ElementAt(j).Value.GeoEquation, currentDic, index);
                        j++;
                    }
                }
            }
        }
        else
            for (int i = LastEquationCount; i < equationListInfo.Count; i++)
            {
                //上一层
                var lastLevel = currentDic[index - 1];
                //全组合
                for (int j = 0; j < lastLevel.Count; j++)
                {
                    if (i == j
                         // ||lastLevel[j].HasCondition(equationListInfo[i].equation) //避免重复相加
                         || lastLevel[j].hasCodition(equationListInfo.ElementAt(i).Value.GeoEquation)//避免重复相加
                        )
                        continue;

                    //构建
                    CombinationAddingEquations(equationListInfo.ElementAt(i).Value.GeoEquation, lastLevel[j], currentDic, index);

                    //排除重复（会影响一题多解）
                    lastLevel = lastLevel.DistinctBy(x => x.HashCode).ToList();
                }
            }

        GenerateAddingEquations(equationListInfo, LastEquationCount, currentDic, ++index, level);
    }

    /// <summary>
    /// 生成a+b，a-b，b-a
    /// </summary>
    /// <param name="equationListInfo">condition</param>
    /// <param name="equation_0">a</param>
    /// <param name="equation_1">b</param>
    /// <param name="currentDic">已处理列表</param>
    /// <param name="index">获取下一层</param>
    /// <param name="flag">若是flase进行初始化处理，不用添加condition</param>
    private void CombinationAddingEquations(GeoEquation equation_0, GeoEquation equation_1, Dictionary<int, List<GeoEquation>> currentDic, int index)
    {
        List<GeoEquation> next = currentDic[index];

        //a+b
        {
            GeoEquation equation = equation_0.Add(equation_1);

            var nodeNumbers = equation.NodeNumber;

            if (nodeNumbers < MaxNodeNumbers)
            {
                equation.AddCondition("相加", equation_0);
                equation.AddCondition("相加", equation_1);
                next.Add(equation);
            }
        }
        //a-b
        {
            GeoEquation equation = equation_0.Sub(equation_1);

            var nodeNumbers = equation.NodeNumber;

            if (nodeNumbers < MaxNodeNumbers)
            {
                equation.AddCondition("相减", equation_0);
                equation.AddCondition("相减", equation_1);
                next.Add(equation);
            }
        }
        //b-a
        {
            GeoEquation equation = equation_1.Sub(equation_0);

            var nodeNumbers = equation.NodeNumber;

            if (nodeNumbers < MaxNodeNumbers)
            {
                equation.AddCondition("相减", equation_0);
                equation.AddCondition("相减", equation_1);
                next.Add(equation);
            }
        }
    }

    private void HandleResult(Dictionary<int, List<GeoEquation>> dict)
    {
        List<GeoEquation> result = dict.Values.SelectMany(list => list).ToList();
        foreach (var item in result)
        {
            //是否有判断机制？
            calAddProcessor.Add(item);
        }
    }

    #endregion 连加

    #region 连乘

    public void MultiplyingEquations(int level = 4)
    {
        //自身为第一层
        level = level - 1;

        //连乘
        GenerateMultiplyingEquations(_formularBase.AngleMultiplicationGeoEquationInfos, AngleMultiplicationGeoEquationInfosLastEquationCoun, AngleMultiplicationDict, 0, level);
        GenerateMultiplyingEquations(_formularBase.DistanceMultiplicationGeoEquationInfos, DistanceMultiplicationGeoEquationInfosLastEquationCoun, DistanceMultiplicationDict, 0, level);
        // GenerateMultiplyingEquations(_formularBase.AngleComplexGeoEquationInfos, AngleComplexGeoEquationInfosLastEquationCoun, AngleComplexMultiplicationDict, 0, level);
        //GenerateMultiplyingEquations(_formularBase.DistanceComplexGeoEquationInfos, DistanceComplexGeoEquationInfosLastEquationCoun, DistanceComplexMultiplicationDict, 0, level);
        AngleMultiplicationGeoEquationInfosLastEquationCoun = _formularBase.AngleMultiplicationGeoEquationInfos.Count;
        HandleResult(AngleMultiplicationDict);
        DistanceMultiplicationGeoEquationInfosLastEquationCoun = _formularBase.DistanceMultiplicationGeoEquationInfos.Count;
        HandleResult(DistanceMultiplicationDict);
        // HandleResult(AngleComplexMultiplicationDict);
        //HandleResult(DistanceComplexMultiplicationDict);

        // multingFlag = result.Count - multingFlag;

        //筛选处理
        //筛选的标准；（1）太长的等式可以不要，GeoEquation里面加一个结点计算函数
    }

    private void GenerateMultiplyingEquations(Dictionary<ulong, GeoEquationInfo> equationListInfo, int LastEquationCount, Dictionary<int, List<GeoEquation>> currentDic, int index, int level)
    {
        if (index == level) return;

        //初始化
        if (index == 0)
        {
            for (int i = 0; i < equationListInfo.Count; i++)
            {
                if (LastEquationCount == 0)
                {
                    for (int j = i + 1; j < equationListInfo.Count; j++)
                    {
                        CombinationMultiplyingEquations(equationListInfo.ElementAt(i).Value.GeoEquation, equationListInfo.ElementAt(j).Value.GeoEquation, currentDic, index);
                    }
                }
                else
                {
                    int j = LastEquationCount;
                    if (i >= LastEquationCount)
                        j = i + 1;

                    while (j < equationListInfo.Count)
                    {
                        if (i >= j)
                        {
                            j++; continue;
                        }
                        if (!(equationListInfo.ElementAt(j).Value.GeoEquation.hasCodition(equationListInfo.ElementAt(i).Value.GeoEquation)))
                            CombinationMultiplyingEquations(equationListInfo.ElementAt(i).Value.GeoEquation, equationListInfo.ElementAt(j).Value.GeoEquation, currentDic, index);

                        j++;
                    }
                }
            }
        }
        else
            for (int i = LastEquationCount; i < equationListInfo.Count; i++)
            {
                //上一层
                var lastLevel = currentDic[index - 1];
                //全组合
                for (int j = 0; j < lastLevel.Count; j++)
                {
                    if (i == j
                        || lastLevel[j].hasCodition(equationListInfo.ElementAt(i).Value.GeoEquation)//避免重复相加
                        )
                        continue;

                    //构建
                    CombinationMultiplyingEquations(equationListInfo.ElementAt(i).Value.GeoEquation, lastLevel[j], currentDic, index);

                    //排除重复（会影响一题多解）
                    lastLevel = lastLevel.DistinctBy(x => x.HashCode).ToList();
                }
            }
        GenerateMultiplyingEquations(equationListInfo, LastEquationCount, currentDic, ++index, level);
    }

    public void CombinationMultiplyingEquations(GeoEquation equation_0, GeoEquation equation_1, Dictionary<int, List<GeoEquation>> currentDic, int index)
    {
        //还可以改动，不要直接访问_formularBase，来增加可扩张性

        //目前：左部或者右部为0则没有意义
        if (equation_0.LeftPart.ToString() == "0" || equation_1.LeftPart.ToString() == "0" ||
            equation_0.RightPart.ToString() == "0" || equation_1.RightPart.ToString() == "0")
            return;

        List<GeoEquation> next = currentDic[index];

        //a*b
        {
            GeoEquation equation = equation_0.Mul(equation_1);

            var nodeNumbers = equation.NodeNumber;

            if (nodeNumbers < MaxNodeNumbers)
            {
                equation.AddCondition("相乘", equation_0);
                equation.AddCondition("相乘", equation_1);
                next.Add(equation);
            }
        }
        //a/b
        {
            GeoEquation equation = equation_0.Div(equation_1);

            var nodeNumbers = equation.NodeNumber;

            if (nodeNumbers < MaxNodeNumbers)
            {
                equation.AddCondition("相除", equation_0);
                equation.AddCondition("相除", equation_1);
                next.Add(equation);
            }
        }
        //b/a
        {
            GeoEquation equation = equation_1.Div(equation_0);

            var nodeNumbers = equation.NodeNumber;

            if (nodeNumbers < MaxNodeNumbers)
            {
                equation.AddCondition("相除", equation_0);
                equation.AddCondition("相除", equation_1);
                next.Add(equation);
            }
        }
    }

    #endregion 连乘

    private bool select(GeoEquation equation)
    {
        ////常数=常数
        //if (equation.LeftPart is Mut && equation.RightPart is Mut)
        //    return false;
        ////0=A
        //if (equation.LeftPart == Expr.Zero && equation.RightPart.)
        //    return false;
        //if (equation.RightPart == Expr.Zero && equation.RightPart.)
        //    return false;

        return true;
    }

    #region 高斯消元法

    public List<Dictionary<Mut, Expr>> disMatrixList { get; set; } = new List<Dictionary<Mut, Expr>>();
    public List<Dictionary<Mut, Expr>> angleMatrixList { get; set; } = new List<Dictionary<Mut, Expr>>();
    public List<Expr> disConstList { get; set; } = new();
    public List<Expr> angleCosntList { get; set; } = new();

    public void CalGaussianElimination()
    {
        foreach (var item in _formularBase.DistanceAdditionGeoEquationInfos.Values)
        {
            disMatrixList.Add(item.AdditionCoefficients);
            disConstList.Add(item.AddictionRealValue);
        }
        foreach (var item in _formularBase.AngleMultiplicationGeoEquationInfos.Values)
        {
            angleMatrixList.Add(item.AdditionCoefficients);
            angleCosntList.Add(item.AddictionRealValue);
        }
        //需要结构化处理行列式

        StructurationMatrixList(disMatrixList);
        StructurationMatrixList(angleMatrixList);

        Solve(disMatrixList, disConstList);
        Solve(angleMatrixList, angleCosntList);

        disMatrixList.Clear();
        disConstList.Clear();

        angleMatrixList.Clear();
        angleCosntList.Clear(); 

    }

    private void Solve(List<Dictionary<Mut, Expr>> matrix, List<Expr> constList)
    {
        int row = matrix.Count; // 行数
        if (row == 0) return;
        int maxcolumn = matrix[0].Count; // 列数（包含常数列）

        //遍历每一列
        for (int i = 0; i < Math.Min(row, maxcolumn); i++)
        {
            // 找到这一列中最大的一行
            var maxkey = matrix[i].Keys.ElementAt(i);
            var maxvalue = matrix[i][maxkey].Abs();
            int maxRow = i;

            for (int k = i + 1; k < row; k++)
            {
                if (matrix[k][maxkey].Abs().CompareTo(maxvalue) == ExprCompareResult.Greater)
                {
                    maxvalue = matrix[k][maxkey].Abs();
                    maxRow = k;
                }
            }
            // 交换行
            SwapRows(matrix, constList, i, maxRow);

            // 检查：若某行的所有系数为0(或只有常数项不为0)，则删除
            //  IsZeroRow(matrix, constList, i);

            // 检查主元是否为0，判断为自由变量
            if (matrix[i][maxkey].Abs() == Expr.Zero)
            {
                continue;
            }

            ////该行归一化(行最简)
            foreach (var key in matrix[i].Keys)
            {
                matrix[i][key] = matrix[i][key].Div(maxvalue);
            }
            constList[i] = constList[i].Div(maxvalue);

            // 消元过程(行最简)
            for (int k = 0; k < row; k++)
            {
                if (k == i) continue;
                //加一个0判断 为0的话就不用除了
                if (matrix[k][maxkey].CompareTo(Expr.Zero) == ExprCompareResult.Equal)
                    continue;

                var factor = matrix[k][maxkey].Div(matrix[i][maxkey]);

                for (int j = 0; j < maxcolumn; j++)
                {
                    var newValue = matrix[k].Values.ElementAt(j).Sub(matrix[i].Values.ElementAt(j).Mul(factor));
                    var key = matrix[k].Keys.ElementAt(j);
                    matrix[k][key] = newValue;
                }
                //包括常数项
                constList[k] = constList[k].Sub(constList[k].Mul(factor));
            }
        }

        知识发现(matrix, constList);
    }

    //若某行的所有系数为0，则删除
    private void IsZeroRow(List<Dictionary<Mut, Expr>> matrix, List<Expr> ConstList, int row)
    {
        //列数
        var n = matrix[row].Count();
        for (int j = 0; j < n; j++)
        {
            if (matrix[row].Values.ElementAt(j).CompareTo(Expr.Zero) != ExprCompareResult.Equal)
            {
                return;
            }
        }
        matrix.RemoveAt(row);
        ConstList.RemoveAt(row);
    }

    // 行交换的辅助函数
    private void SwapRows(List<Dictionary<Mut, Expr>> matrix, List<Expr> Const, int row1, int row2)
    {
        var temp = matrix[row1];
        matrix[row1] = matrix[row2];
        matrix[row2] = temp;

        var constTemp = Const[row1];
        Const[row1] = Const[row2];
        Const[row2] = constTemp;
    }

    //预处理结构化MatrixList
    private void StructurationMatrixList(List<Dictionary<Mut, Expr>> MatrixList)
    {
        var allKeys = new HashSet<Mut>();
        //获得所有存在的变量
        foreach (var dict in MatrixList)
            foreach (var key in dict.Keys)
                allKeys.Add(key);

        //对不存在的进行填充
        foreach (var dict in MatrixList)
        {
            foreach (var key in allKeys)
            {
                if (!dict.ContainsKey(key))
                    dict[key] = Expr.Zero;
            }
        }

        foreach (var dict in MatrixList)
        {
            var sortedDict = new SortedDictionary<Mut, Expr>(dict);
            dict.Clear();
            foreach (var kvp in sortedDict)
            {
                dict.Add(kvp.Key, kvp.Value);
            }
        }
    }

    //组装成Equation?还是知识发现？
    private void ChangeToEquation(List<Dictionary<Mut, Expr>> matrix)
    {
        foreach (var additionCoefficients in matrix)
        {
            SumNode sumNode = new SumNode();
            //组装sumNode
            foreach (var coefficients in additionCoefficients)
            {
                Expr value = coefficients.Value;
                Mut mut = coefficients.Key;
                //常数
                if (mut == Expr.Zero)
                {
                    if (value.CompareTo(Expr.Zero) == ExprCompareResult.Greater)
                        sumNode.Addends.Add(value);
                    else if (value.CompareTo(Expr.Zero) == ExprCompareResult.Less)
                        sumNode.Subtrahends.Add(value.Opposite());
                }
                //系数
                else if (value.CompareTo(Expr.One) == ExprCompareResult.Equal)
                {
                    sumNode.Addends.Add(mut);
                }
                else if (value.CompareTo(Expr.Zero) == ExprCompareResult.Greater)
                {
                    ProductNode productNode = new ProductNode();
                    productNode.Multipliers.Add(mut);
                    productNode.Multipliers.Add(value);
                    sumNode.Addends.Add(productNode);
                }
                else if (value.CompareTo(Expr.Zero) == ExprCompareResult.Less)
                {
                    ProductNode productNode = new ProductNode();
                    productNode.Multipliers.Add(mut);
                    //负数加入减节点是 正还是负？
                    productNode.Multipliers.Add(value.Opposite());
                    sumNode.Subtrahends.Add(productNode);
                }
            }
            //组装Equation
            GeoEquation geoEquation = new(sumNode, 0);
            calAddProcessor.Add(geoEquation);
        }
    }

    private void 知识发现(List<Dictionary<Mut, Expr>> matrix, List<Expr> constList)
    {
        for (int i = 0; i < matrix.Count; i++)
        {
            var dict = matrix[i]; // 获取第 i 行的字典
            var contExpr = constList[i];
            // 获取所有非零的键
            List<Mut> nonZeroKeys = dict.Where(kvp => kvp.Value != Expr.Zero)
                                         .Select(kvp => kvp.Key).ToList();

            //值
            if (nonZeroKeys.Count == 1 && contExpr != Expr.Zero)
            {
                GeoEquation equation = new(nonZeroKeys[0], contExpr);
                calAddProcessor.Add(equation);
            }
            //比例或相等
            else if (nonZeroKeys.Count == 2 && contExpr == Expr.Zero)
            {
                GeoEquation equation = new(nonZeroKeys[0], nonZeroKeys[1]);
                calAddProcessor.Add(equation);
            }

        }
    }

    #endregion 高斯消元法

    //判断是否重复生成
    public bool HasCondition(GeoEquation geoEquationA, GeoEquation geoEquationB)
    {
        if (geoEquationA.Conditions.Any(info => info.HashCode == geoEquationB.HashCode))
            return true;
        else
            return false;
    }
}