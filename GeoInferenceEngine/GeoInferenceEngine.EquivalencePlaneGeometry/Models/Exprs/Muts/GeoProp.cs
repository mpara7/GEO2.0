namespace GeoInferenceEngine.Knowledges
{
    public class GeoProp : Mut
    {
        public const string Size = "大小";

        public const string Length = "长度";
        public const string Area = "面积";
        public const string Perimeter = "周长";
        public const string Radius = "半径";
        public const string Diameter = "直径";

        public GeoProp(Knowledge knowledge, string propName) : base()
        {
            Knowledge = knowledge;
            PropName = propName;
            Mut.Record.Invoke(this);
        }
        public Knowledge Knowledge { get; set; }
        public string PropName { get; init; }


        public override string ToString()
        {
            return $"{Knowledge}";
            // return $"{Knowledge}的{PropName}";
        }
        public Knowledge ToSolveKnowledge()
        {

            if (Knowledge is Angle angle)
            {
                return new AngleSize(angle, ZScriptDefinition.Value);
            }
            else if (Knowledge is Segment segment)
            {
                return new SegmentLength(segment, ZScriptDefinition.Value);
            }
            else if (Knowledge is PlaneFigure planeFigure)
            {
                if (PropName is Area)
                {
                    var type = typeof(PlaneFigureArea<>).MakeGenericType(planeFigure.GetType());
                    var knowledge = type.GetConstructors()[0].Invoke(new object[] { planeFigure, Expr.FromString(ZScriptDefinition.Value) });
                    return (Knowledge)knowledge;
                }
                else if (PropName is Perimeter)
                {
                    var type = typeof(PlaneFigurePerimeter<>).MakeGenericType(planeFigure.GetType());
                    var knowledge = type.GetConstructors()[0].Invoke(new object[] { planeFigure, Expr.FromString(ZScriptDefinition.Value) });
                    return (Knowledge)knowledge;
                }
            }
            throw new ArgumentException();
        }
    }
}
