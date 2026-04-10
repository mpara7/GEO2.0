namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[OrdinaryKnowledge]
[Description("点在圆上")]
public class PointOnCircle : PlainRelation
{
    public PointOnCircle(Point point, Circle circle)
    {
        Add(point, circle);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => $"{Properties[0]}在{Properties[1]}上";

    public override void Normalize()
    {
    }
}
