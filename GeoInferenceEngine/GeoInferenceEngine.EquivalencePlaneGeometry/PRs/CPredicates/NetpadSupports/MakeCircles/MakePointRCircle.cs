using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives;
[Description("作点径圆")]
public class MakePointRCircle : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakePointRCircle(Point point1, Expr expr, Circle circle)
    {
        Add(point1, expr, circle);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"以{Properties[0]}为圆心半径为{Expr}作{Properties[1]}";

    public override void Normalize()
    {
    }
}
