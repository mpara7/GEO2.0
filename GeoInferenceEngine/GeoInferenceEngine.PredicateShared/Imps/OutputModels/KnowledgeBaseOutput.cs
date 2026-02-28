using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs;

using System.Text;
using ZTool.Infrastructures.ZDI;

namespace GeoInferenceEngine.Knowledges.Imps.IOs.Outputs;
public class CondictionInfo
{
    public int Index { get; set; }
    public string Type { get; set; }
}
public class KnowledgeInfo
{
    public string Content { get; set; }
    public string Type { get; set; }
    public string Reason { get; set; }
    public bool IsAvailable { get; set; } = true;
    public override string ToString()
    {
        return Content;
    }
    public List<CondictionInfo> Conditions { get; set; } = new List<CondictionInfo>();
}
//public enum CategoryType
//{
//    Figure,
//    SpecialFigure,
//    Relations,
//    Equations
//}
//public class CategoryInfo
//{
//    public CategoryType CategoryType { get; set; }
//    public string TypeName { get; set; }
//    public List<KnowledgeInfo> KnowledgeInfos { get; set; }=new List<KnowledgeInfo>();
//}
[Description("知识库输出")]
public class KnowledgeBaseOutput : AInferenceOutput
{
    public static bool FigureToString = false;
    public static bool ConstrcutivesToString = false;
    /// <summary>
    /// 分类知识库，string:类型名字
    /// </summary>
    public Dictionary<string, List<KnowledgeInfo>> Figures { get; set; } = new();
    /// <summary>
    /// 分类知识库，string:类型名字
    /// </summary>
    public Dictionary<string, List<KnowledgeInfo>> SpecialFigures { get; set; } = new();
    /// <summary>
    /// 分类知识库，string:类型名字
    /// </summary>
    public Dictionary<string, List<KnowledgeInfo>> Relations { get; set; } = new();
    /// <summary>
    /// 分类知识库，string:类型名字
    /// </summary>
    public Dictionary<string, List<KnowledgeInfo>> Constrcutives { get; set; } = new();
    /// <summary>
    /// 分类知识库，string:类型名字
    /// </summary>
    public Dictionary<string, List<KnowledgeInfo>> PlainRelations { get; set; } = new();

    public Dictionary<string, List<KnowledgeInfo>> Equations { get; set; } = new();
    public Dictionary<string, List<KnowledgeInfo>> RatioInfos { get; set; } = new();
    
    public List<KnowledgeInfo> FindKnowledgeInfoList(string typeName)
    {
        if (Figures.ContainsKey(typeName))
            return Figures[typeName];
        else if (SpecialFigures.ContainsKey(typeName))
            return SpecialFigures[typeName];
        else if (Relations.ContainsKey(typeName))
            return Relations[typeName];
        else if (Constrcutives.ContainsKey(typeName))
            return Constrcutives[typeName];
        else if (PlainRelations.ContainsKey(typeName))
            return PlainRelations[typeName];
        else if (Equations.ContainsKey(typeName))
            return Equations[typeName];
        throw new NotImplementedException();
    }
    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        int totalCount = 0; // 新增：总条目计数器
        var allCategories = new[]
{
    Figures,
    SpecialFigures,
    Relations,
    Constrcutives,
    PlainRelations,
    Equations
};
        // 遍历所有分类计算总数量
        foreach (var category in allCategories)
        {
            foreach (var kv in category)
            {
                totalCount += kv.Value.Count; // 累加每个分类的条目数
            }
        }

        if (FigureToString)
        {
            foreach (var kv in Figures)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(kv.Key);
                stringBuilder.Append($"({kv.Value.Count})");
                stringBuilder.AppendLine();
                foreach (var knowledge in kv.Value)
                {
                    if (!knowledge.IsAvailable)
                        stringBuilder.Append("~");
                    if (kv.Key == "点")
                    {
                        stringBuilder.Append($"{knowledge.Content}\t");
                    }
                    else
                    {
                        stringBuilder.Append($"{knowledge.Content}({knowledge.Reason})\n");
                    }
                    foreach (var condiction in knowledge.Conditions)
                    {
                        try
                        {
                            stringBuilder.Append("\t" + FindKnowledgeInfoList(condiction.Type)[condiction.Index] + "\n");
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }
        foreach (var kv in SpecialFigures)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append(kv.Key);
            stringBuilder.Append($"({kv.Value.Count})");
            stringBuilder.AppendLine();
            foreach (var knowledge in kv.Value)
            {
                if (!knowledge.IsAvailable)
                    stringBuilder.Append("~");
                stringBuilder.Append(knowledge.Content + "\n");

                foreach (var condiction in knowledge.Conditions)
                {

                    stringBuilder.Append("\t" + FindKnowledgeInfoList(condiction.Type)[condiction.Index] + "\n");
                }
            }
        }
        foreach (var kv in Relations)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append(kv.Key);
            stringBuilder.Append($"({kv.Value.Count})");
            stringBuilder.AppendLine();
            foreach (var knowledge in kv.Value)
            {
                if (!knowledge.IsAvailable)
                    stringBuilder.Append("~");
                stringBuilder.Append(knowledge.Content + "\t" + knowledge.Reason + "\n");

                foreach (var condiction in knowledge.Conditions)
                {
                    try
                    {

                        stringBuilder.Append("\t" + FindKnowledgeInfoList(condiction.Type)[condiction.Index] + "\n");
                    }
                    catch { }
                }
            }
        }
        if (ConstrcutivesToString)
        {
            foreach (var kv in Constrcutives)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(kv.Key);
                stringBuilder.Append($"({kv.Value.Count})");
                stringBuilder.AppendLine();
                foreach (var knowledge in kv.Value)
                {
                    if (!knowledge.IsAvailable)
                        stringBuilder.Append("~");
                    stringBuilder.Append(knowledge.Content + "\t" + knowledge.Reason + "\n");

                    foreach (var condiction in knowledge.Conditions)
                    {
                        try
                        {

                            stringBuilder.Append("\t" + FindKnowledgeInfoList(condiction.Type)[condiction.Index] + "\n");
                        }
                        catch { }


                    }
                }
            }
        }
        
        return stringBuilder.ToString();
    }
}