namespace GeoInferenceEngine.PlaneKnowledges.Knowledges
{
    [Description("共圆")]
    public class CoCirle : PlainRelation
    {
        public CoCirle(params Point[] points)
        {
            Add(points);
            Normalize();
            SetHashCode();
        }

        public override string ToString() => $"{StringTool.ComposeList(Properties)}共圆";

        public override void Normalize()
        {

        }
    }
}
