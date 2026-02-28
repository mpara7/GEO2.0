using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("线段互相垂直平分")]

public class VerticalBisectorEachOther : Knowledge
{
    public VerticalBisectorEachOther(Segment segment, Segment segment1)
    {
        Add(segment, segment1);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => $"{Properties[0]}和{Properties[1]}互相垂直平分";


    public override void Normalize()
    {
        Sort(0, 1);
    }
}