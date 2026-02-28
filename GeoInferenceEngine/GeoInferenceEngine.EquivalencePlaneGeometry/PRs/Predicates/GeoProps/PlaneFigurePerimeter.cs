using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("<T>的周长")]
public class PlaneFigurePerimeter<T> : Knowledge where T : PlaneFigure
{
    public PlaneFigurePerimeter(T planeFigure, Expr value)
    {
        Add(planeFigure, value);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"{Properties[0]}的周长是{Expr}";

    public override void Normalize()
    {
    }
}
