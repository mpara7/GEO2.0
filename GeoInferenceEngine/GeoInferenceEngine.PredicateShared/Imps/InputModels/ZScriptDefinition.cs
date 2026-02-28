using System.Text.RegularExpressions;

namespace GeoInferenceEngine.Knowledges.Imps.IOs.Inputs
{
    public class ZscriptParseException : Exception
    {
        public string Detail { get; set; }
        public int CurLineIndex { get; set; }
        public string CurLine { get; set; }
        public ZscriptParseException(string detail,int curLineIndex,string curLine)
        {
            Detail = detail;
            CurLineIndex = curLineIndex;
            CurLine = curLine;
        }
        public override string ToString()
        {
            return $"解析出错，在{CurLineIndex}行 {CurLine}：{Detail}";
        }
    }
    public static class ZScriptDefinition
    {
        #region 工具函数
        /// <summary>
        /// 标准格式化
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public static string FormatStandard(string script)
        {
            script = script.Trim('\n');
            script = Regex.Replace(script, @" +", " ");
            script = script.Replace("！", "");
            script = script.Replace("\r", "");
            script = script.Replace('）', ')').Replace('（', '(');
            script = script.Replace('，', ',');
            script = script.Replace("pi", "Pi");
            script = script.Replace('：', ':');
            script = script.Replace(" : ", ":");
            script = script.Replace(": ", ":");
            script = script.Replace(" :", ":");
            string standardScript = string.Empty;
            foreach (var line in script.Split('\n'))
            {
                standardScript += $"{line.Trim()}\n";
            }
            return standardScript;
        }
        #endregion

        #region 关键词
        /// <summary>
        /// 定义点(包括伪坐标)
        /// </summary>
        public const string PointsDef = "Points:";

        /// <summary>
        /// 定义辅助线
        /// </summary>
        public const string AuxDef = "+";

        /// <summary>
        /// 证伪 不可能成立
        /// </summary>
        public const string DisproveDef = "Disprove:";
        /// <summary>
        /// 求证
        /// </summary>
        public const string ProveDef = "Prove:";
        /// <summary>
        /// 求解数值
        /// </summary>
        public const string SolveDef = "Solve:";
        /// <summary>
        /// 确定指定对象的目标类型知识
        /// </summary>
        public const string DetermineDef = "Determine:";
        /// <summary>
        /// 待满足信息 当xx取最大值时 如果toSatisfy中包含max min 则先推出max，min 当。。。时做。。。
        /// </summary>
        public const string SatisfyDef = "Satisfy:";

        /// <summary>
        /// 用于指代不明确的值，且不需要设置 Solve中要求的值或者最大值\最小值中
        /// </summary>
        public const string Value = "value";
        #endregion
    }
}
