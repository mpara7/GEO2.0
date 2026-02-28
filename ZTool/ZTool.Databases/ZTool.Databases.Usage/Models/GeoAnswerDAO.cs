namespace GeoDatabase.Shared.Models;
public class GeoAnswerDAO
{
    /// <summary>
    /// 答案
    /// </summary>
    public string Answer { get; set; } = "";
    /// <summary>
    /// 答案图片以base64形式存放
    /// </summary>
    public List<string> AnswerPics { get; set; } = new List<string>();
}
