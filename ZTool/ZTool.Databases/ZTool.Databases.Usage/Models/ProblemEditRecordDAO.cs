using ZTool.Databases;

namespace GeoDatabase.Shared.Models;
public class ProblemEditRecordDAO : ADatabaseObject
{
    /// <summary>
    /// 题目id
    /// </summary>
    public string ProblemID { get; set; }
    /// <summary>
    /// 操作内容
    /// </summary>
    public string OperateDetail { get; set; }
    /// <summary>
    /// 操作 创建或删除
    /// </summary>
    public string Operate { get; set; }
    /// <summary>
    /// 操作者
    /// </summary>
    public DateTime OperateTime { get; set; }
    /// <summary>
    /// 操作者
    /// </summary>
    public string Operator { get; set; }

    public override string ToString() => $"{Operator} 在 {OperateTime} 进行了 {Operate} 具体： {OperateDetail}";
}
