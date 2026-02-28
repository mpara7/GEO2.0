using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("角的大小", "角大小")]

public class AngleSize : Knowledge
{
    public Angle Angle { get => (Angle)Properties[0]; }
    public AngleSize(Angle angle, Expr expr)
    {
        Add(angle, expr);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => $"{Properties[0]}的大小是{Expr}度";

    public override void Normalize()
    {


    }
}
