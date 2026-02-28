using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives;
[Description("作两圆切线", "作两圆的切线")]
public class MakeCirclesTangent : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakeCirclesTangent(Circle circle1, Circle circle2, Point rPoint1, Point rPoint2, Point rPoint3, Point rPoint4, Segment rSegment1, Segment rSegment2)
    {
        Add(circle1, circle2, rPoint1, rPoint2, rPoint3, rPoint4, rSegment1, rSegment2);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作{Properties[0]}上{Properties[1]}的切线{Properties[2]}{Properties[3]}";

    public override void Normalize()
    {
    }
}
