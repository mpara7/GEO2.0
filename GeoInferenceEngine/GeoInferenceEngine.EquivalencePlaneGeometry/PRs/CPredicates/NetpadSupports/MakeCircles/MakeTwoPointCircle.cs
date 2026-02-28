using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives;
[Description("作两点圆")]
public class MakeTwoPointCircle : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakeTwoPointCircle(Point point1, Point point2, Circle circle)
    {
        Add(point1, point2, circle);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"以{Properties[0]}为圆心，以{Properties[0]}{Properties[1]}为半径作{Properties[2]}";

    public override void Normalize()
    {
    }
}
