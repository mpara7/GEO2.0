using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives;
[Description("作三点圆")]
public class MakeThreePointCircle : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakeThreePointCircle(Point point1, Point point2, Point point3, Circle circle)
    {
        Add(point1, point2, point3, circle);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"过{Properties[0]}{Properties[1]}{Properties[2]}作{Properties[3]}";

    public override void Normalize()
    {
        Sort(0, 1, 2);
    }
}
