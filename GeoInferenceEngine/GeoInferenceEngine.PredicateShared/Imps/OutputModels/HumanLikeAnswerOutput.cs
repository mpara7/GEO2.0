global using ZTool.UsefulTypes;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs;
using System.Text;

namespace GeoInferenceEngine.Knowledges.Imps.IOs.Outputs;
public class QuestionHumanLikeAnswer
{
    public TimeSpan RunTime { get; set; }

    public int Index { get; set; }
    
    public bool IsSuccess { get; set; }
    public string Question { get; set; } = "";
    public string Answer { get; set; } = "未成功解题";
    public override string ToString()
    {
        RunTime = GlobalTimer.Elapsed;
        var builder = new StringBuilder();
        if (RunTime.Hours > 0) builder.Append($"{RunTime.Hours}小时");
        if (RunTime.Minutes > 0) builder.Append($"{RunTime.Minutes}分");
        builder.Append($"{RunTime.Seconds}.{RunTime.Milliseconds}s");
        int methodARecords = GlobalRecorder.Instance.GetRecords("A").Sum();
        GlobalRecorder.Instance.Clear("A");
        if (IsSuccess) return $"第{Index}小问：{Question}，证明完成\n花费时间:{builder}\n当前推理信息总数:{methodARecords}\n{Answer}";
        return $"第{Index}问：{Question}，未解决";
    }
}
[Description("类人答题输出")]
public class HumanLikeAnswerOutput : AInferenceOutput
{
    public List<QuestionHumanLikeAnswer> Answers { get; set; } = new();

    

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var item in Answers)
        {
            
            sb.Append(item.ToString() + "\n");
        }
        
        return sb.ToString();
    }
}
