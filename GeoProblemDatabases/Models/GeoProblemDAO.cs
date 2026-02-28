using ZTool.Databases;

namespace GeoProblemDatabases.Models
{
    public class GeoProblemDAO : ADatabaseObject
    {
        #region 题目信息
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = "";
        /// <summary>
        /// 题干
        /// </summary>
        public string QuestionStem { get; set; } = "";
        /// <summary>
        /// 小问
        /// </summary>
        public List<GeoQuestionDAO> Questions { get; set; } = new();
        /// <summary>
        /// 题目图片
        /// </summary>
        public List<string> ProblemPictureIDs { get; set; } = new();
        #endregion

        #region 原始记录
        /// <summary>
        /// 原始题目图片
        /// </summary>
        public List<string> OrgPictureIDs { get; set; } = new();
        #endregion

        #region 附加信息
        /// <summary>
        /// 带值标签 比如 年份/题目类型
        /// </summary>
        public Dictionary<string, string> ValueLabels { get; set; } = new();
        /// <summary>
        /// 不带值标签 比如 高考
        /// </summary>
        public List<string> Labels { get; set; } = new();
        public DateTime LastUpdateTime { get; set; }
        #endregion
    }

    public class GeoQuestionDAO
    {
        /// <summary>
        /// 问题
        /// </summary>
        public string Question { get; set; } = "";
        /// <summary>
        /// 题目原始答案 一题多解
        /// </summary>
        public List<GeoAnswerDAO> Answers { get; set; } = new List<GeoAnswerDAO>();
    }

    public class GeoAnswerDAO
    {
        /// <summary>
        /// 答案
        /// </summary>
        public string Answer { get; set; } = "";
        /// <summary>
        /// 答案图片以base64形式存放
        /// </summary>
        public List<string> AnswerPicIDs { get; set; } = new List<string>();
    }
}
