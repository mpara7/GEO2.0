
using ZTool.Databases;

namespace GeoDatabase.Shared.Models;
public class GeoProblemDAO : ADatabaseObject
{
    /// <summary>
    /// 避免误删题目
    /// </summary>
    public bool IsDeleted { get; set; } = false;
    public string NetpadID { get; set; } = "";
    public string Title { get; set; } = "";
    /// <summary>
    /// 题目类型
    /// </summary>
    public string Type { get; set; } = "2D";
    /// <summary>
    /// 缩略图
    /// </summary>
    public List<string> ThumbNails { get; set; } = new();
    /// <summary>
    /// 原始题目图片
    /// </summary>
    public List<string> OrgProblemPictures { get; set; } = new();
    /// <summary>
    /// 原始答案图片
    /// </summary>
    public List<string> OrgAnswerPictures { get; set; } = new();
    public DateTime LastUpdateTime { get; set; }
    public string LastOperateUser { get; set; } = "unknow";
    /// <summary>
    /// 标签
    /// </summary>
    public Dictionary<string, string> Labels { get; set; } = new();
    /// <summary>
    /// 题干
    /// </summary>
    public string QuestionStem { get; set; } = "";
    /// <summary>
    /// 题目图片
    /// </summary>
    public List<string> ProblemPictures { get; set; } = new();
    /// <summary>
    /// 小问
    /// </summary>
    public List<GeoQuestionDAO> Questions { get; set; } = new();
    //额外信息字典
    public Dictionary<string, string> AuxInfos { get; set; } = new();

}
