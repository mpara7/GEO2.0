using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.CKnowledges;
public abstract class ConstructiveKnowledge : Knowledge
{
    protected void AddResult(params object[] objs)
    {
        foreach (var obj in objs)
        {
            if (obj is null)
                throw new ArgumentNullException();
            if (obj is Knowledge pred)
                Properties.Add(pred);
            else if (obj is Expr expr)
                throw new ArgumentNullException();
        }
    }
    public List<Knowledge> Result = new List<Knowledge>();
}
