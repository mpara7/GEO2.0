using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("弧的大小", "弧大小")]

public class ArcSize : Knowledge
{
    public ArcSize(Arc arc, Expr expr)
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
