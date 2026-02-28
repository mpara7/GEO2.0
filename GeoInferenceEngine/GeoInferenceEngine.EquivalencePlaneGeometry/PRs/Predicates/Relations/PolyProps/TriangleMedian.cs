namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("三角形中线", "三角形的中线")]

public class TriangleMedian : Knowledge
{
    public TriangleMedian(Segment seg, Triangle triangle)
    {
        Add(seg, triangle);
        Normalize();
        SetHashCode();

    }


    public override string ToString() => $"{Properties[1]}的中线是{Properties[0]}";

    public override void Normalize()
    {

    }
}