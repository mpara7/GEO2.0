using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.PRs.Rules.GraphicsComplete.Internal
{
    [PlaneRuleType(PlaneRuleType.AutoGenerate)]
    internal class LineGenInternalRules : PlaneRuleClass
    {
        #region 生成几何对象
        public void RuleLi001共线生成所有线段(Line line)
        {
            foreach (var item in ListTool.GetCombination(line.Properties, 2))
            {
                Segment pred = new Segment((Point)item[0], (Point)item[1]);
                pred.AddReason();
                pred.AddCondition(line);
                AddProcessor.Add(pred);
            }
        }
        public void RuleLi002根据交点生成角(Line line1, Line line2)
        {
            if (line1.ToString() == "AO0B" && line2.ToString() == "AEC" || line1.ToString() == "AEC" && line2.ToString() == "AO0B")
            {
                var (i, l1, l2) = FindIntersection(line1, line2);
                if (i.Count() == 0 || i.Count() >= 2) return;
                //找到交点在line1和line2点集合里面的索引
                var pos1 = line1.Properties.IndexOf(i[0]);
                var pos2 = line2.Properties.IndexOf(i[0]);
                List<List<Knowledge>> linepart1 = new List<List<Knowledge>>();
                List<List<Knowledge>> linepart2 = new List<List<Knowledge>>();
                if (pos1 == 0)
                {
                    linepart1.Add(line1.Properties.Skip(pos1 + 1).ToList());
                }
                else if (pos1 == line1.Properties.Count - 1)
                {
                    linepart1.Add(line1.Properties.Take(pos1).Reverse().ToList());
                }
                else
                {
                    linepart1.Add(line1.Properties.Skip(pos1 + 1).ToList());
                    linepart1.Add(line1.Properties.Take(pos1).Reverse().ToList());
                }
                if (pos2 == 0)
                {
                    linepart2.Add(line2.Properties.Skip(pos2 + 1).ToList());
                }
                else if (pos2 == line2.Properties.Count - 1)
                {
                    linepart2.Add(line2.Properties.Take(pos2).Reverse().ToList());
                }
                else
                {
                    linepart2.Add(line2.Properties.Skip(pos2 + 1).ToList());
                    linepart2.Add(line2.Properties.Take(pos2).Reverse().ToList());
                }
                foreach (var part1 in linepart1)
                {
                    foreach (var part2 in linepart2)
                    {
                        //产生新知识
                        Angle pred = new Angle(part1.Select(p => (Point)p).ToList(), (Point)i[0], part2.Select(p => (Point)p).ToList());
                        pred.AddReason();
                        pred.AddCondition(line1, line2);
                        AddProcessor.Add(pred);
                    }
                }

            }
            else
            {
                var (i, l1, l2) = FindIntersection(line1, line2);
                if (i.Count() == 0 || i.Count() >= 2) return;
                //找到交点在line1和line2点集合里面的索引
                var pos1 = line1.Properties.IndexOf(i[0]);
                var pos2 = line2.Properties.IndexOf(i[0]);
                List<List<Knowledge>> linepart1 = new List<List<Knowledge>>();
                List<List<Knowledge>> linepart2 = new List<List<Knowledge>>();
                if (pos1 == 0)
                {
                    linepart1.Add(line1.Properties.Skip(pos1 + 1).ToList());
                }
                else if (pos1 == line1.Properties.Count - 1)
                {
                    linepart1.Add(line1.Properties.Take(pos1).Reverse().ToList());
                }
                else
                {
                    linepart1.Add(line1.Properties.Skip(pos1 + 1).ToList());
                    linepart1.Add(line1.Properties.Take(pos1).Reverse().ToList());
                }
                if (pos2 == 0)
                {
                    linepart2.Add(line2.Properties.Skip(pos2 + 1).ToList());
                }
                else if (pos2 == line2.Properties.Count - 1)
                {
                    linepart2.Add(line2.Properties.Take(pos2).Reverse().ToList());
                }
                else
                {
                    linepart2.Add(line2.Properties.Skip(pos2 + 1).ToList());
                    linepart2.Add(line2.Properties.Take(pos2).Reverse().ToList());
                }
                foreach (var part1 in linepart1)
                {
                    foreach (var part2 in linepart2)
                    {
                        //产生新知识
                        Angle pred = new Angle(part1.Select(p => (Point)p).ToList(), (Point)i[0], part2.Select(p => (Point)p).ToList());
                        pred.AddReason();
                        pred.AddCondition(line1, line2);
                        AddProcessor.Add(pred);
                    }
                }
            }
        }
            
        public void RuleLi003生成三角形(Point p1, Point p2, Point p3)
        {
            RuleLi012生成三点不共线(p1, p2, p3);
            if (KnowledgeGetter.HasAllSegment([p1, p2, p3]))
            {

                var coline = KnowledgeGetter.GetColine(p1, p2, p3);
                if (coline is NegatingKnowledge negating)
                {
                    Triangle pred = new Triangle(p1, p2, p3);
                    pred.AddReason();
                    pred.AddCondition(negating);
                    AddProcessor.Add(pred);
                }
            }
        }
        public void RuleLi004生成四边形(Point p1, Point p2, Point p3, Point p4)
        {
            if (KnowledgeGetter.HasSegment(p1, p2) && KnowledgeGetter.HasSegment(p2, p3) &&
                KnowledgeGetter.HasSegment(p3, p4) && KnowledgeGetter.HasSegment(p4, p1))
            {
                var conditions = new List<Knowledge>();
                ///任意三点共线就不生成
                foreach (var combo in ListTool.GetCombination(new List<Point>() { p1, p2, p3, p4 }, 3))
                {
                    var result = KnowledgeGetter.GetLine(combo.ToArray());
                    if (result is null)
                    {
                    }
                    else
                    {
                        //conditions.Add(result);
                        return;
                    }
                }
                var judge = KnowledgeGetter.GetJudgeIpsilateral(p1, p3, p2, p4);
                if (judge is PointsOnLineDifferentSide p)
                {
                    //后续可以添加不共线信息
                    Quadriliateral pred = new Quadriliateral(p1, p2, p3, p4);
                    pred.AddReason();
                    //conditions.AddRange([p1, p2, p3, p4]);
                    pred.AddCondition(conditions.ToArray());
                    AddProcessor.Add(pred);
                }
            }
        }
        #endregion

        #region 生成平凡几何关系
        #region 工具
        public Knowledge GetPointWithin(Point point1, Point point2, Point point3)
        {
            Knowledge pred = new PointWithInPoints(point1, point2, point3);
            if (point1.X == point2.X || point1.X == point3.X)
            {
                if (point2.Y > point1.Y && point1.Y > point3.Y)
                {
                    return pred;
                }
                else if (point3.Y > point1.Y && point1.Y > point2.Y)
                {
                    return pred;
                }
            }
            else
            {
                if (point2.X > point1.X && point1.X > point3.X)
                {
                    return pred;
                }
                else if (point3.X > point1.X && point1.X > point2.X)
                {
                    return pred;
                }
            }
            Knowledge negapred = new NegatingKnowledge(pred);
            negapred.AddReason("假设不成立");
            return negapred;
        }
        public Knowledge GetJudgeIpsilateral(Point point1, Point point2, Point point3, Point point4)
        {
            var (X1, Y1) = (point1.X, point1.Y);
            var (X2, Y2) = (point2.X, point2.Y);
            var (X3, Y3) = (point3.X, point3.Y);
            var (X4, Y4) = (point4.X, point4.Y);
            if (X1 - X2 == 0)
            {
                if (X3 > X1 && X4 > X1 || X3 < X1 && X4 < X1)
                {
                    Knowledge pred = new PointsOnLineSameSide(point1, point2, point3, point4);
                    return pred;
                }
                else if (X3 < X1 && X4 > X1 || X3 > X1 && X4 < X1)
                {
                    Knowledge pred = new PointsOnLineDifferentSide(point1, point2, point3, point4);
                    return pred;
                }
                else
                {
                    return null;
                }
            }
            else if (Y1 - Y2 == 0)
            {
                if (Y3 > Y1 && Y4 > Y1 || Y3 < Y1 && Y4 < Y1)
                {
                    Knowledge pred = new PointsOnLineSameSide(point1, point2, point3, point4);
                    return pred;
                }
                else if (Y3 < Y1 && Y4 > Y1 || Y3 > Y1 && Y4 < Y1)
                {
                    Knowledge pred = new PointsOnLineDifferentSide(point1, point2, point3, point4);
                    return pred;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                //得到x3在该方程上的函数值y3'
                double F_X3 = Fx(X1, X2, Y1, Y2, X3);
                double F_X4 = Fx(X1, X2, Y1, Y2, X4);
                if (F_X3 > Y3 && F_X4 > Y4 || F_X3 < Y3 && F_X4 < Y4)
                {
                    Knowledge pred = new PointsOnLineSameSide(point1, point2, point3, point4);
                    return pred;
                }
                else
                {
                    double F_2 = Fx(X1, X2, Y1, Y2, X3);
                    double F_3 = Fx(X1, X2, Y1, Y2, X4);
                    Knowledge pred = new PointsOnLineDifferentSide(point1, point2, point3, point4);
                    return pred;
                }
            }
        }
        public double Fx(double x1, double x2, double y1, double y2, double x)
        {
            return (y2 - y1) / (x2 - x1) * (x - x1) + y1;
        }
        #endregion
        public void RuleLi011生成点在两点之间(Line collinear)
        {
            if (collinear.Properties.Count <= 2)
                return;
            for (int i = 0; i < collinear.Properties.Count; i++)
            {
                for (int j = i + 1; j < collinear.Properties.Count; j++)
                {
                    for (int k = j + 1; k < collinear.Properties.Count; k++)
                    {
                        Knowledge pred = new PointWithInPoints((Point)collinear[j], (Point)collinear[i], (Point)collinear[k]);
                        pred.AddReason();
                        pred.AddCondition(collinear);
                        AddProcessor.Add(pred);
                    }
                }
            }
        }
        public void RuleLi012生成三点不共线(Point point1, Point point2, Point point3)
        {
            if (KnowledgeGetter.GetLine(point1, point2, point3) is null)
            {
                Line line = new Line(point1, point2, point3);
                NegatingKnowledge pred = new NegatingKnowledge(line);
                pred.AddReason();
                AddProcessor.Add(pred);
            }
        }

        //public void RuleLi014根据交点生成共线(LineIntersection i1)
        //{
        //    Line line1 = new Line((Point)i1[0], (Point)i1[1][0], (Point)i1[1][1]);
        //    Line line2 = new Line((Point)i1[0], (Point)i1[2][0], (Point)i1[2][1]);
        //    line1.AddReason();
        //    line1.AddCondition(i1);
        //    AddProcessor.Add(line1);
        //    line2.AddReason();
        //    line2.AddCondition(i1);
        //    AddProcessor.Add(line2);
        //}
            [ZDI]
        Logger logger;
        public void RuleLi013生成两点分别在同侧或异侧(Point point1, Point point2, Point point3, Point point4)
        {
            var pred = GetJudgeIpsilateral(point1, point2, point3, point4);
            if(pred is not null)
            {
                pred.AddReason();
                pred = AddProcessor.Add(pred);
            }
            

        }
        #endregion


    }
}
