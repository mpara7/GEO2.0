using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Executors;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.PRs.PairMakers;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.PRs.RuleProviders;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns;
public enum SupplementingProcesses
{
    Start,
    Constructive,//构造转约束
    Internal,//特殊图形与几何关系生成一般
    CoLine,//生成共线
    AutoGenerate,//从共线生成线段和角再
}

public class GraphicsCompletePlugInConfig
{
    public bool IsFullConnect { get; set; }
}
/// <summary>
/// 根据构造性语句生成
/// 根据基础关系生成一般图形以及对应的连线
/// 根据基于点在线上
/// （可选）自动生成剩余的
/// </summary>
[Description("直线自动推断插件")]
[InferenceStandPlugIn]
internal class LineAutoCompletePlugIn : IInferencePlugIn
{
    [ZDI]
    IEnginePreparer _enginePreparer;
    [ZDI]
    PlaneExecutor planeExecutor;
    public bool Finished { get; set; }
    [ZDI]
    KnowledgeBase knowledgeBase;

    [ZDI]
    protected EngineInfo EngineInfo;
    [ZDI]
    PairMaker PairMaker { get; set; }
    [ZDI]
    RuleProvider RuleProvider { get; set; }
    [ZDI]
    Logger Logger { get; set; }
    [ZDI]
    AppInfo AppInfo { get; set; }

    SupplementingProcesses State = SupplementingProcesses.Constructive;
    public void Init()
    {
        planeExecutor.DoingAutoGen += Complete;
    }
    public virtual void Complete()
    {
        RuleProvider.BuildRules(PlaneRuleType.CoLine);
        PairMaker.Reload();
        Logger.Info("准备自动推导直线");
        EngineInfo.IsOutOfPair = false;

        while (!EngineInfo.IsOutOfPair)
        {
            if (PairMaker.HasNextPair())
            {
                var pair = PairMaker.GetNextPair();
                if (pair.args.ToList().TrueForAll(p => p.IsAvailable))
                {
                    pair.rule.Invoke(pair.@class, pair.args);
                }
            }
            else
            {
                EngineInfo.IsOutOfPair = true;
            }
        }
        Logger.Info("自动推导直线完成");
    }
}
