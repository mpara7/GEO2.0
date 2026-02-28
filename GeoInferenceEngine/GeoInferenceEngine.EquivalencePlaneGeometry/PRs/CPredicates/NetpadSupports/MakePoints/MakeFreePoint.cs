namespace GeoInferenceEngine.PlaneKnowledges.CKnowledges;
[Description("作自由点")]
public class MakeFreePoint : ConstructiveKnowledge
{
    public string Name { get; set; } = "?";
    /// <summary>
    /// 多边形
    /// </summary>
    /// <param name="points"></param>
    public MakeFreePoint(string name)
    {
        Name = name;
        Normalize();
        SetHashCode();
    }
    public override string ToString() => $"作自由点{Name}";

    public override void SetHashCode()
    {
        SetNameHashCode(Name);
    }
    public override void Normalize()
    {
    }

}
