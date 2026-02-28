namespace GeoDatabase.Shared.Models;
public class GeoQuestionDAO
{
    /// <summary>
    /// 问题
    /// </summary>
    public string Question { get; set; } = "";
    /// <summary>
    /// 谓词形式
    /// </summary>
    public string QuestionInPred { get; set; } = "";
    /// <summary>
    /// 题目原始图片
    /// </summary>
    public List<GeoAnswerDAO> OrgAnswers { get; set; } = new List<GeoAnswerDAO>();
    /// <summary>
    /// 自动推理答案
    /// </summary>
    public List<GeoAnswerDAO> AutoAnswers { get; set; } = new List<GeoAnswerDAO>();
}
