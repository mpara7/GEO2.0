using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("弧的长度", "弧长度")]
public class ArcLength : Knowledge
{
    public ArcLength(Arc arc, Expr expr)
    {
        Add(arc, expr);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => $"{Properties[0]}的长度是{Expr}";

    public override void Normalize()
    {

    }
}
