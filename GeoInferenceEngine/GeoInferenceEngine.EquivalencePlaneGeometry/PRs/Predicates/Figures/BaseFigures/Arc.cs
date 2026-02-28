namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("弧")]
[PrimitiveKnowledge]
public class Arc : Figure
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
    GeoProp size;
    public GeoProp Size
    {
        get
        {
            if (size is null)
            {
                size = new GeoProp(this, GeoProp.Size);
            }

            return size;
        }
    }
    /// <summary>
    /// 第一个点为圆心
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public Arc(Point p1, Point p2, Point p3)
    {
        Add(p1, p2, p3);
        Normalize();
        SetHashCode();
    }

    GeoProp measure;
    //指圆心角大小
    public GeoProp Measure
    {
        get
        {
            if (ReferenceEquals(measure, null))
            {
                measure = new GeoProp(this, GeoProp.Size);
            }

            return measure;
        }
    }

    public override string ToString() => $"弧{Properties[0]}{Properties[1]}{Properties[2]}";

    public override void Normalize()
    {

    }
}
