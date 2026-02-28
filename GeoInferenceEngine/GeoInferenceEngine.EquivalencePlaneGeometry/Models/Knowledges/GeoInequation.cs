namespace GeoInferenceEngine.Knowledges.Models;
public enum InequationSign
{
    Greater,
    Greater_OR_Equal,
    Equal,
    Less_OR_Equal,
    Less,
}
public class GeoInequation : Knowledge
{
    public Expr LeftPart { get; set; }
    public Expr RightPart { get; set; }

    public bool NormalizeFlag = false;
    public InequationSign Sign { get; set; }
    public GeoInequation(Expr leftPart, Expr rightPart, InequationSign inequationSign)
    {
        LeftPart = leftPart;
        RightPart = rightPart;
        Sign = inequationSign;
        Normalize();
        SetHashCode();
    }

    public override void Normalize()
    {

    }

    public override void SetHashCode()
    {
        HashCode = (ulong)ToString().GetHashCode();
    }

    public override string ToString() => $"{LeftPart}{Sign}{RightPart}";

}