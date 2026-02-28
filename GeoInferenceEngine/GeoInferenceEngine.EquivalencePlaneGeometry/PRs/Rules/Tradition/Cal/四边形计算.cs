namespace GeoInferenceEngine.PlaneGeometry.Rules.Tradition.Cal
{
    //[PlaneRuleType(PlaneRuleType.Tradition)]
    internal class 四边形计算 : PlaneRuleClass
    {
        public void RuleCQ001四边形边长关系(Quadriliateral quad)
        {
            var seg1 = KnowledgeGetter.GetSegment(quad[0], quad[1]);
            var seg2 = KnowledgeGetter.GetSegment(quad[1], quad[2]);
            var seg3 = KnowledgeGetter.GetSegment(quad[2], quad[3]);
            var seg4 = KnowledgeGetter.GetSegment(quad[3], quad[0]);
            var pred = new GeoEquation(seg1.Length.Add(seg2.Length).Add(seg3.Length).Add(seg4.Length), quad.Perimeter);
            pred.AddReason();
            pred.AddCondition(quad);
            AddProcessor.Add(pred);
        }
        public void RuleCQ002四边形角大小关系(Quadriliateral quad)
        {
            var a1 = KnowledgeGetter.GetAngle(quad[0], quad[1], quad[2]);
            var a2 = KnowledgeGetter.GetAngle(quad[1], quad[2], quad[3]);
            var a3 = KnowledgeGetter.GetAngle(quad[2], quad[3], quad[0]);
            var a4 = KnowledgeGetter.GetAngle(quad[3], quad[0], quad[1]);
            //var pred = new GeoEquation(a1.Size.Add(a2.Size).Add(a3.Size).Add(a4.Size), 360);
            //pred.AddReason();
            //pred.AddCondition(quad);
            //AddProcessor.Add(pred);
        }

        public void RuleCQ003平行四边形面积计算(Parallelogram quad, LinePerpendicular perpendicular)
        {
            for (var i = 0; i < 4; i++)
            {
                var bottomLine = KnowledgeGetter.GetLine(quad[i % 4], quad[(i + 1) % 4]);
                if (perpendicular.Line1 == bottomLine)
                {
                    foreach (var point in perpendicular.Line2.Properties)
                    {
                        var line = KnowledgeGetter.GetColine(point, quad[i % 4], quad[(i + 1) % 4]);
                        if (line is not NegatingKnowledge)
                        {
                            var bottomSeg = KnowledgeGetter.GetSegment(quad[i % 4], quad[(i + 1) % 4]);
                            var cross = FindIntersection(perpendicular.Line1, perpendicular.Line2).intersection[0];
                            var heightSeg = KnowledgeGetter.GetSegment(cross, point);
                            var bquad = KnowledgeGetter.GetQuadriliateral((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);

                            //var pred = new GeoEquation(bottomSeg.Length.Mul(heightSeg.Length), bquad.Area);
                            //pred.AddReason();
                            //pred.AddCondition(quad, perpendicular);
                            //AddProcessor.Add(pred);

                        }
                    }
                }
                else if (perpendicular.Line2 == bottomLine)
                {
                    foreach (var point in perpendicular.Line1.Properties)
                    {
                        var line = KnowledgeGetter.GetColine(point, quad[i % 4], quad[(i + 1) % 4]);
                        if (line is not NegatingKnowledge)
                        {
                            var bottomSeg = KnowledgeGetter.GetSegment(quad[i % 4], quad[(i + 1) % 4]);
                            var cross = FindIntersection(perpendicular.Line1, perpendicular.Line2).intersection[0];
                            var heightSeg = KnowledgeGetter.GetSegment(cross, point);
                            var bquad = KnowledgeGetter.GetQuadriliateral((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);

                            //var pred = new GeoEquation(bottomSeg.Length.Mul(heightSeg.Length), bquad.Area);
                            //pred.AddReason();
                            //pred.AddCondition(quad, perpendicular);
                            //AddProcessor.Add(pred);

                        }
                    }
                }
            }
        }
        public void RuleCQ004平行四边形面积计算sin(Parallelogram para)
        {
            var ab = KnowledgeGetter.GetSegment(para[0], para[1]);
            var bc = KnowledgeGetter.GetSegment(para[1], para[2]);
            var cd = KnowledgeGetter.GetSegment(para[2], para[3]);
            var da = KnowledgeGetter.GetSegment(para[3], para[0]);

            var angleA = KnowledgeGetter.GetAngle((Point)para[3], (Point)para[0], (Point)para[1]);
            var angleB = KnowledgeGetter.GetAngle((Point)para[0], (Point)para[1], (Point)para[2]);
            var angleC = KnowledgeGetter.GetAngle((Point)para[1], (Point)para[2], (Point)para[3]);
            var angleD = KnowledgeGetter.GetAngle((Point)para[2], (Point)para[3], (Point)para[0]);
            var bquad = KnowledgeGetter.GetQuadriliateral((Point)para[0], (Point)para[1], (Point)para[2], (Point)para[3]);
            List<GeoEquation> geoEquations = new List<GeoEquation>();
            //geoEquations.Add(new GeoEquation(ab.Length.Mul(da.Length).Mul(angleA.Size.Sin()), bquad.Area));
            //geoEquations.Add(new GeoEquation(ab.Length.Mul(bc.Length).Mul(angleB.Size.Sin()), bquad.Area));
            //geoEquations.Add(new GeoEquation(bc.Length.Mul(cd.Length).Mul(angleC.Size.Sin()), bquad.Area));
            //geoEquations.Add(new GeoEquation(cd.Length.Mul(da.Length).Mul(angleD.Size.Sin()), bquad.Area));
            foreach (var pred in geoEquations)
            {
                pred.AddReason();
                pred.AddCondition(para);
                AddProcessor.Add(pred);
            }
        }

        public void RuleCQ005风筝形面积计算(Kite para)
        {
            var ac = KnowledgeGetter.GetSegment(para[0], para[2]);
            var bd = KnowledgeGetter.GetSegment(para[1], para[3]);
            if (ac is null && bd is null) return;

            var bquad = KnowledgeGetter.GetQuadriliateral((Point)para[0], (Point)para[1], (Point)para[2], (Point)para[3]);
            List<GeoEquation> geoEquations = new List<GeoEquation>();
            //geoEquations.Add(new GeoEquation(ac.Length.Mul(bd.Length).Div(2), bquad.Area));
            foreach (var pred in geoEquations)
            {
                pred.AddReason();
                pred.AddCondition(para);
                AddProcessor.Add(pred);
            }
        }

        public void RuleCQ006风筝形面积计算sin(Kite para)
        {
            var ab = KnowledgeGetter.GetSegment(para[0], para[1]);
            var bc = KnowledgeGetter.GetSegment(para[1], para[2]);
            var cd = KnowledgeGetter.GetSegment(para[2], para[3]);
            var da = KnowledgeGetter.GetSegment(para[3], para[0]);

            var angleB = KnowledgeGetter.GetAngle((Point)para[0], (Point)para[1], (Point)para[2]);
            var angleD = KnowledgeGetter.GetAngle((Point)para[2], (Point)para[3], (Point)para[0]);
            var bquad = KnowledgeGetter.GetQuadriliateral((Point)para[0], (Point)para[1], (Point)para[2], (Point)para[3]);
            List<GeoEquation> geoEquations = new List<GeoEquation>();
            //geoEquations.Add(new GeoEquation(ab.Length.Mul(bc.Length).Mul(angleB.Size.Sin()), bquad.Area));
            //geoEquations.Add(new GeoEquation(cd.Length.Mul(da.Length).Mul(angleD.Size.Sin()), bquad.Area));
            foreach (var pred in geoEquations)
            {
                pred.AddReason();
                pred.AddCondition(para);
                AddProcessor.Add(pred);
            }
        }

        public void RuleCQ007梯形面积计算(Trapezoid quad, LinePerpendicular perpendicular)
        {
            //for (var i = 0; i < 4; i += 2)
            //{
            //    var bottomLine = KnowledgeGetter.GetLine(quad[i % 4], quad[(i + 1) % 4]);
            //    if (perpendicular.Line1 == bottomLine)
            //    {
            //        foreach (var point in perpendicular.Line2.Properties)
            //        {
            //            var line = KnowledgeGetter.GetColine(point, quad[i % 4], quad[(i + 1) % 4]);
            //            if (line is not NegatingKnowledge)
            //            {
            //                var topSeg = KnowledgeGetter.GetSegment(quad[(i + 2) % 4], quad[(i + 3) % 4]);
            //                var bottomSeg = KnowledgeGetter.GetSegment(quad[i % 4], quad[(i + 1) % 4]);
            //                var cross = FindIntersection(perpendicular.Line1, perpendicular.Line2).intersection[0];
            //                var heightSeg = KnowledgeGetter.GetSegment(cross, point);
            //                var bquad = KnowledgeGetter.GetQuadriliateral((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);

            //                var pred = new GeoEquation((topSeg.Length + bottomSeg.Length) * heightSeg.Length / 2, bquad.Area);
            //                pred.AddReason();
            //                pred.AddCondition(quad, perpendicular);
            //                AddProcessor.AddNew(pred);

            //            }
            //        }
            //    }
            //    else if (perpendicular.Line2 == bottomLine)
            //    {
            //        foreach (var point in perpendicular.Line1.Properties)
            //        {
            //            var line = KnowledgeGetter.GetColine(point, quad[i % 4], quad[(i + 1) % 4]);
            //            if (line is not NegatingKnowledge)
            //            {
            //                var topSeg = KnowledgeGetter.GetSegment(quad[(i + 2) % 4], quad[(i + 3) % 4]);
            //                var bottomSeg = KnowledgeGetter.GetSegment(quad[i % 4], quad[(i + 1) % 4]);
            //                var cross = FindIntersection(perpendicular.Line1, perpendicular.Line2).intersection[0];
            //                var heightSeg = KnowledgeGetter.GetSegment(cross, point);
            //                var bquad = KnowledgeGetter.GetQuadriliateral((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);

            //                var pred = new GeoEquation((topSeg.Length + bottomSeg.Length) * heightSeg.Length / 2, bquad.Area);
            //                pred.AddReason();
            //                pred.AddCondition(quad, perpendicular);
            //                AddProcessor.AddNew(pred);

            //            }
            //        }
            //    }
            //}
        }
        public void RuleCQ008梯形面积计算(RightTrapezoid quad)
        {
            //var topSeg = KnowledgeGetter.GetSegment(quad[2], quad[3]);
            //var bottomSeg = KnowledgeGetter.GetSegment(quad[0], quad[1]);
            //var heightSeg = KnowledgeGetter.GetSegment(quad[1], quad[2]);
            //var bquad = KnowledgeGetter.GetQuadriliateral((Point)quad[0], (Point)quad[1], (Point)quad[2], (Point)quad[3]);

            //var pred = new GeoEquation((topSeg.Length + bottomSeg.Length) * heightSeg.Length / 2, bquad.Area);
            //pred.AddReason();
            //pred.AddCondition(quad);
            //AddProcessor.AddNew(pred);
        }
    }
}
