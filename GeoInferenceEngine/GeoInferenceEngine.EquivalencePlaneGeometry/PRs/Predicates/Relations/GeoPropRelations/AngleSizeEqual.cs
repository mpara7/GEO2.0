using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("AngleSizeEqual ", "角的大小相等")]

public class AngleSizeEqual : Knowledge
{
    public Angle Angle1 { get => (Angle)Properties[0]; }
    public Angle Angle2 { get => (Angle)Properties[1]; }
    public AngleSizeEqual(Angle angle1, Angle angle2)
    {
        Add(angle1, angle2);
        Normalize();
        SetHashCode();
    }

    public override string ToString() => $"{Properties[0]}与{Properties[1]}的大小相等";


    public override void Normalize()
    {
        Sort();
    }
}
