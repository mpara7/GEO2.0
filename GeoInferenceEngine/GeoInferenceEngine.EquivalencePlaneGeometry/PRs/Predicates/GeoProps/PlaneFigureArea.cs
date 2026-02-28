using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("<T>的面积")]
public class PlaneFigureArea<T> : Knowledge where T : PlaneFigure
{
    public PlaneFigureArea(T planeFigure, Expr value)
    {
        Add(planeFigure, value);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"{Properties[0]}的面积是{Expr}";

    public override void Normalize()
    {
    }
}