namespace GeoInferenceEngine.PlaneGeometry.Rules.Tradition.Cal
{
    [PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 三角形计算 : PlaneRuleClass
    {
        public void RuleCT001三角形边长关系(Triangle triangle)
        {
            var seg1 = KnowledgeGetter.GetSegment(triangle[0], triangle[1]);
            var seg2 = KnowledgeGetter.GetSegment(triangle[0], triangle[2]);
            var seg3 = KnowledgeGetter.GetSegment(triangle[1], triangle[2]);
            var pred = new GeoEquation(seg1.Length.Add(seg2.Length).Add(seg3.Length), triangle.Perimeter);
            pred.AddReason();
            pred.AddCondition(triangle);
            AddProcessor.Add(pred);
        }
        public void RuleCT002三角形角大小关系(Triangle triangle)
        {
            var a1 = KnowledgeGetter.GetAngle(triangle[0], triangle[1], triangle[2]);
            var a2 = KnowledgeGetter.GetAngle(triangle[1], triangle[0], triangle[2]);
            var a3 = KnowledgeGetter.GetAngle(triangle[0], triangle[2], triangle[1]);
            if (a1 == null || a2 == null || a3 == null) return;
            var pred = new GeoEquation(a1.Size.Add(a2.Size).Add(a3.Size), 180);
            pred.AddReason();
            pred.AddCondition(triangle);
            AddProcessor.Add(pred);
        }
        public void RuleCT003三角形角余弦定理(Triangle triangle)
        {
            var ab = KnowledgeGetter.GetSegment(triangle[0], triangle[1]);
            var bc = KnowledgeGetter.GetSegment(triangle[1], triangle[2]);
            var ca = KnowledgeGetter.GetSegment(triangle[2], triangle[0]);

            var angleA = KnowledgeGetter.GetAngle((Point)triangle[1], (Point)triangle[0], (Point)triangle[2]);
            var angleB = KnowledgeGetter.GetAngle((Point)triangle[0], (Point)triangle[1], (Point)triangle[2]);
            var angleC = KnowledgeGetter.GetAngle((Point)triangle[0], (Point)triangle[2], (Point)triangle[1]);
            List<GeoEquation> geoEquations = new List<GeoEquation>();
            //geoEquations.Add(new GeoEquation( ca.Length.Mul(ca.Length).Add(ab.Length .Mul( ab.Length)).Sub(bc.Length.Mul(bc.Length)).Div(2).Mul(ca.Length).Mul(ab.Length),angleA.Size.Cos()));
            //geoEquations.Add(new GeoEquation(ab.Length.Mul(ca.Length).Add(bc.Length.Mul(bc.Length)).Sub(ca.Length.Mul(bc.Length)).Div(2).Mul(ca.Length).Mul(ab.Length), angleA.Size.Cos()));
            //geoEquations.Add(new GeoEquation(ca.Length.Mul(ca.Length).Add(ab.Length.Mul(ab.Length)).Sub(bc.Length.Mul(bc.Length)).Div(2).Mul(ca.Length).Mul(ab.Length), angleA.Size.Cos()));
            //geoEquations.Add(new GeoEquation( ab.Length * ca.Length + bc.Length * ab.Length - ca.Length * bc.Length / 2 * bc.Length * ab.Length, angleB.Size.Cos()));
            //geoEquations.Add(new GeoEquation( ca.Length * ca.Length + bc.Length * bc.Length - ab.Length * ab.Length / 2 * ca.Length * bc.Length, angleC.Size.Cos()));
            foreach (var pred in geoEquations)
            {
                pred.AddReason();
                pred.AddCondition(triangle);
                AddProcessor.Add(pred);
            }
        }

        void RuleCT004三角形正弦定理公式(Triangle triangle)
        {
            var ab = KnowledgeGetter.GetSegment(triangle[0], triangle[1]);
            var bc = KnowledgeGetter.GetSegment(triangle[1], triangle[2]);
            var ca = KnowledgeGetter.GetSegment(triangle[2], triangle[0]);

            var angleA = KnowledgeGetter.GetAngle((Point)triangle[1], (Point)triangle[0], (Point)triangle[2]);
            var angleB = KnowledgeGetter.GetAngle((Point)triangle[0], (Point)triangle[1], (Point)triangle[2]);
            var angleC = KnowledgeGetter.GetAngle((Point)triangle[0], (Point)triangle[2], (Point)triangle[1]);

            //List<GeoEquation> geoEquations = new List<GeoEquation>();
            //geoEquations.Add(new GeoEquation(bc.Length.Div(angleA.Size.Sin()), ca.Length.Div(angleB.Size.Sin())));
            //geoEquations.Add(new GeoEquation(ca.Length.Div(angleB.Size.Sin()), ab.Length.Div(angleC.Size.Sin())));
            //foreach (var pred in geoEquations)
            //{
            //    pred.AddReason();
            //    pred.AddCondition(triangle);
            //    AddProcessor.Add(pred);
            //}
        }
        void RuleCT005三角形面积_垂直(Triangle triangle, LinePerpendicular perpendicular)
        {
            var cross = FindIntersection(perpendicular.Line1, perpendicular.Line2).intersection[0];
            for (var i = 0; i < 3; i++)
            {

                if (perpendicular.Line1.Properties.Contains(triangle[i % 3]) && perpendicular.Line1.Properties.Contains(triangle[i + 1 % 3]))
                {
                    if (perpendicular.Line2.Properties.Contains(triangle[i + 2 % 3]))
                    {
                        var ad = KnowledgeGetter.GetSegment(triangle[i + 2 % 3], cross);
                        var bc = KnowledgeGetter.GetSegment(triangle[i % 3], triangle[i + 1 % 3]);
                        var pred = new GeoEquation(ad.Length.Mul(bc.Length).Div(2), triangle.Area);
                        pred.AddReason();
                        pred.AddCondition(triangle);
                        AddProcessor.Add(pred);
                    }
                }
                else if (perpendicular.Line2.Properties.Contains(triangle[i % 3]) && perpendicular.Line2.Properties.Contains(triangle[i + 1 % 3]))
                {
                    if (perpendicular.Line1.Properties.Contains(triangle[i + 2 % 3]))
                    {
                        var ad = KnowledgeGetter.GetSegment(triangle[i + 2 % 3], cross);
                        var bc = KnowledgeGetter.GetSegment(triangle[i % 3], triangle[i + 1 % 3]);
                        var pred = new GeoEquation(ad.Length.Mul(bc.Length).Div(2), triangle.Area);
                        pred.AddReason();
                        pred.AddCondition(triangle);
                        AddProcessor.Add(pred);
                    }
                }
            }
        }

        void RuleCT006三角形面积_sin(Triangle triangle)
        {
            var ab = KnowledgeGetter.GetSegment(triangle[0], triangle[1]);
            var bc = KnowledgeGetter.GetSegment(triangle[1], triangle[2]);
            var ca = KnowledgeGetter.GetSegment(triangle[2], triangle[0]);

            var angleA = KnowledgeGetter.GetAngle((Point)triangle[1], (Point)triangle[0], (Point)triangle[2]);
            var angleB = KnowledgeGetter.GetAngle((Point)triangle[0], (Point)triangle[1], (Point)triangle[2]);
            var angleC = KnowledgeGetter.GetAngle((Point)triangle[0], (Point)triangle[2], (Point)triangle[1]);

            List<GeoEquation> geoEquations = new List<GeoEquation>();
            //geoEquations.Add(new GeoEquation(ab.Length.Mul(ca.Length).Mul(angleA.Size.Sin()).Div(2), triangle.Area));
            //geoEquations.Add(new GeoEquation(ab.Length.Mul(bc.Length).Mul(angleB.Size.Sin()).Div(2), triangle.Area));
            //geoEquations.Add(new GeoEquation(bc.Length.Mul(ca.Length).Mul(angleC.Size.Sin()).Div(2), triangle.Area));
            foreach (var pred in geoEquations)
            {
                pred.AddReason();
                pred.AddCondition(triangle);
                AddProcessor.Add(pred);
            }
        }

    }
}
