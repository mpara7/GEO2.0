using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs;
using GeoInferenceEngine.Knowledges.Imps.Componments;
using GeoInferenceEngine.PredicateShared.Models;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.CalByCombination.GeoCalHandler
{
    internal class GaussianElimination
    {
        //传进来是需要对字典进行预处理！（）
        [ZDI]
        private ExprService AExprService { get; set; }

        [ZDI]
        public FormularBase _formularBase { get; set; }

        [ZDI]
        private CalAddProcessor calAddProcessor;

        public GaussianElimination()
        {
            foreach (var item in _formularBase.DistanceAdditionGeoEquationInfos.Values)
                disMatrixList.Add(item.AdditionCoefficients);
            foreach (var item in _formularBase.AngleMultiplicationGeoEquationInfos.Values)
                angleMatrixList.Add(item.AdditionCoefficients);
            Solve(disMatrixList);
            Solve(angleMatrixList);
        }

        public List<Dictionary<Mut, Expr>> disMatrixList { get; set; } = new List<Dictionary<Mut, Expr>>();
        public List<Dictionary<Mut, Expr>> angleMatrixList { get; set; } = new List<Dictionary<Mut, Expr>>();

        private void Solve(List<Dictionary<Mut, Expr>> matrix)
        {
            int m = matrix.Count; // 行数
            int n = matrix.Max(dict => dict.Count) - 1; // 列数（去掉结果列）
            // 记录自由变量和主元变量
            bool[] isFreeVariable = new bool[n];
            List<int> freeVariables = new List<int>();
            for (int i = 0; i < Math.Min(m, n); i++)
            {
                // 找到主元
                var max = matrix[i].Values.ElementAt(i).Abs();
                int maxRow = i;
                for (int k = i + 1; k < m; k++)
                {
                    if (matrix[k].Values.ElementAt(i).Abs().CompareTo(max) == ExprCompareResult.Greater)
                    {
                        max = matrix[k].Values.ElementAt(i).Abs();
                        maxRow = k;
                    }
                }
                // 交换行
                SwapRows(matrix, i, maxRow);
                // 检查无解条件：若某行的所有系数为0，结果列不为0，则无解
                if (IsZeroRow(matrix, i, n) && matrix[i].Values.ElementAt(n).Abs() == Expr.Zero)
                {
                    Console.WriteLine("无解");
                    return;
                }
                // 检查主元是否为0，判断为自由变量
                if (matrix[i].Values.ElementAt(i).Abs() == Expr.Zero)
                {
                    isFreeVariable[i] = true;
                    freeVariables.Add(i); // 记录自由变量
                    continue;
                }
                // 消元过程
                for (int k = i + 1; k < m; k++)
                {
                    var factor = matrix[k].Values.ElementAt(i).Div(matrix[i].Values.ElementAt(i));
                    for (int j = i; j < n + 1; j++) // 包括最后一列（结果列）
                    {
                        //matrix[k][j] = matrix[k].Values.ElementAt(j).Sub(matrix[i].Values.ElementAt(j).Mul(factor));
                        var newValue = matrix[k].Values.ElementAt(j).Sub(matrix[i].Values.ElementAt(j).Mul(factor));
                        var key = matrix[k].Keys.ElementAt(j);
                        matrix[k][key] = newValue;
                    }
                }
            }

            //返回一个通过高斯消元法处理后的增广矩阵
            ChangeToEquation(matrix);
        }

        // 检查是否为零行
        private bool IsZeroRow(List<Dictionary<Mut, Expr>> matrix, int row, int n)
        {
            for (int j = 0; j < n; j++)
            {
                if (matrix[row].Values.ElementAt(j).Abs() == Expr.Zero)
                {
                    return false;
                }
            }
            return true;
        }

        // 行交换的辅助函数
        private void SwapRows(List<Dictionary<Mut, Expr>> matrix, int row1, int row2)
        {
            //行数
            int n = matrix.Count;
            for (int k = 0; k < n; k++)
            {
                var tep = matrix[row1];
                matrix[row1] = matrix[row2];
                matrix[row2] = tep;
            }
        }

        //把矩阵组装成Equation
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
    }
}