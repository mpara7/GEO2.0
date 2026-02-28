
namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("Segment ")]
[PrimitiveKnowledge]
public class Segment : Figure
{
    GeoProp length;
    public GeoProp Length
    {
        get
        {
            if (ReferenceEquals(length, null))
            {
                length = new GeoProp(this, GeoProp.Length);
            }

            return length;
        }
    }
    public Segment(Point p1, Point p2)
    {
        Add(p1, p2);
        Normalize();
        SetHashCode();
    }


    public override void Normalize()
    {
        Sort();
    }

    public override string ToString() => $"{Properties[0]}{Properties[1]}";

    public static explicit operator Segment((Knowledge, Knowledge) v)
    {
        throw new NotImplementedException();
    }
}
