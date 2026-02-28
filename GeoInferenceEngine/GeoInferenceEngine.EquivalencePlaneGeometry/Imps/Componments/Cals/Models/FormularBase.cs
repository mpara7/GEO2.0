using GeoInferenceEngine.PredicateShared.Models;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal.Models;

public class FormularBase : IInferenceDatabase
{
    #region 等式
    //A=B A=3B A=90会直接生成比值知识不会作为等式
    public Dictionary<ulong, GeoEquationInfo> AllGeoEquationInfos { get; set; } = new();
    public Dictionary<ulong, GeoEquationInfo> DistanceAdditionGeoEquationInfos { get; set; } = new();
    public Dictionary<ulong, GeoEquationInfo> DistanceMultiplicationGeoEquationInfos { get; set; } = new();
    public Dictionary<ulong, GeoEquationInfo> DistanceComplexGeoEquationInfos { get; set; } = new();
    public Dictionary<ulong, GeoEquationInfo> AngleAdditionGeoEquationInfos { get; set; } = new();
    public Dictionary<ulong, GeoEquationInfo> AngleMultiplicationGeoEquationInfos { get; set; } = new();
    public Dictionary<ulong, GeoEquationInfo> AngleComplexGeoEquationInfos { get; set; } = new();

    public Dictionary<ulong, GeoEquationInfo> DistanceSimpleGeoEquationInfos { get; set; } = new();
    public Dictionary<ulong, GeoEquationInfo> AngleSimpleGeoEquationInfos { get; set; } = new();
    #endregion
    //a = b
    #region 比值信息
    public Dictionary<Mut, RatioInfo> RatioInfoIndex { get; set; } = new();
    public RatioInfo? TryGetRatioInfo(Mut index)
    {
        if (RatioInfoIndex.ContainsKey(index))
            return RatioInfoIndex[index];
        else
            return null;
    }
    public List<RatioInfo> DistanceRatioInfos { get; set; } = new();
    public List<RatioInfo> AngleRatioInfos { get; set; } = new();
    #endregion
}
