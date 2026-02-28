using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("垂足")]

public class Pedal : Knowledge
{

    //public Point point { get =>(Point) Properties[0]; }
    /// <summary>
    /// 垂足
    /// </summary>
    /// <param name="point">垂足</param>
    /// <param name="line"></param>
    /// <param name="line1"></param>

    public Pedal(Point point, Line line, Line line1)
    {
        Add(point, line, line1);
        Normalize();
        SetHashCode();
    }
    public override void Normalize()
    {
        Sort(1, 2);
    }

    public override string ToString() => $"{Properties[1]}和{Properties[2]}的垂足是{Properties[0]}";
}

