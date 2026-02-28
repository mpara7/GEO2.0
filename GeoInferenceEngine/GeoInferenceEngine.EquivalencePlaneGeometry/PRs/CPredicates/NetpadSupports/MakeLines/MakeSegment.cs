using GeoInferenceEngine.PlaneKnowledges.CKnowledges;

namespace GeoInferenceEngine.PlaneKnowledges.KP.CKnowledges.Primitives;
[Description("作线段")]
public class MakeSegment : ConstructiveKnowledge
{
    /// <summary>
    /// N A B
    /// </summary>
    /// <param name="points"></param>
    public MakeSegment(Point point1, Point point2)
    {
        Add(point1, point2);
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"连接{Properties[0]}{Properties[1]}";

    public override void Normalize()
    {
    }
}
