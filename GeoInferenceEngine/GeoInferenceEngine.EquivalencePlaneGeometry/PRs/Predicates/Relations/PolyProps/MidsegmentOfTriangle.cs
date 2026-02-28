namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("三角形的中位线")]


public class MidsegmentOfTriangle : Knowledge
{
    public MidsegmentOfTriangle(Segment segment, Segment segment1, Triangle triangle)
    {
        Add(segment,segment1, triangle);
        Normalize();
        SetHashCode();

    }

    public override string ToString() => $"{Properties[2]}的中位线是{Properties[0]}";

    public override void Normalize()
    {

    }
}
