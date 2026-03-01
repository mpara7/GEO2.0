using System;
using System.Linq;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.PRs.PairMakers;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns.CompleteQuadrilateralInferencePlugIns
{
    public class CompleteQuadInferenceRule : PlaneRuleClass
    {

        [Combination]
        public void 判定完全四边形(LineIntersectionPoint lineIntersectionPoint1, LineIntersectionPoint lineIntersectionPoint2)
        {
            //判断两条相交知识所包含的交点是不是一致的，不一致就继续
            if (lineIntersectionPoint1[0] == lineIntersectionPoint2[0]) return;
            //如果传进来的line的多个点的
            // 获取第一个 LineIntersectionPoint 的两条 Line
            Line line1_1 = (Line)lineIntersectionPoint1.Properties[1];
            Line line1_2 = (Line)lineIntersectionPoint1.Properties[2];

            // 获取第二个 LineIntersectionPoint 的两条 Line
            Line line2_1 = (Line)lineIntersectionPoint2.Properties[1];
            Line line2_2 = (Line)lineIntersectionPoint2.Properties[2];

            //

            var points1_1 = new HashSet<Point>(line1_1.Points);
            var points1_2 = new HashSet<Point>(line1_2.Points);
            var points2_1 = new HashSet<Point>(line2_1.Points);
            var points2_2 = new HashSet<Point>(line2_2.Points);
            bool hasCommon = (points1_1.Overlaps(points2_1) && points1_1.Overlaps(points2_2)) &&
                             (points1_2.Overlaps(points2_1) && points1_2.Overlaps(points2_2));
            if (!hasCommon) return;





            //获取11和21与22的相同点，12也一样
            Line lineinsert1 = GetCommonPointsLine(line2_1, line1_1, line1_2);
            Line lineinsert2 = GetCommonPointsLine(line2_2, line1_1, line1_2);
            if (lineinsert1 == null || lineinsert2 == null) return;


            if (lineinsert1.Points.Count != 2 || lineinsert2.Points.Count != 2) return;

            //判断完就开始赋点，六个点的位置都确定了
            Point p1 = (Point)lineIntersectionPoint1[0];
            Point p5 = (Point)lineIntersectionPoint2[0];
            Point p2 = (Point)lineinsert1[0];
            Point p3 = (Point)lineinsert1[1];
            Point p4 = (Point)lineinsert2[0];
            Point p6 = (Point)lineinsert2[1];

            //判断1 2 4是否共线，不共线就把4和6换位置
            if (!(KnowledgeGetter.HasColine(p1, p2, p4)))
            {
                Point temp = p4;
                p4 = p6;
                p6 = temp;
            }

            CompleteQuadriliateral pred = new CompleteQuadriliateral(p1, p2, p3, p4, p5, p6);
            pred.AddReason();
            pred.AddCondition(lineIntersectionPoint1, lineIntersectionPoint2);
            AddProcessor.Add(pred);

            CompleteQuadriliateral pred2 = CqNormaliza(p3, p5, p6, p1, p2, p4);

            pred2.AddReason();
            pred2.AddCondition(lineIntersectionPoint1, lineIntersectionPoint2);
            AddProcessor.Add(pred2);

            CompleteQuadriliateral pred3 = CqNormaliza(p1, p4, p6, p2, p3, p5);
            pred3.AddReason();
            pred3.AddCondition(lineIntersectionPoint1, lineIntersectionPoint2);
            AddProcessor.Add(pred3);

            CompleteQuadriliateral pred4 = CqNormaliza(p2, p4, p5, p3, p1, p6);
            pred4.AddReason();
            pred4.AddCondition(lineIntersectionPoint1, lineIntersectionPoint2);
            AddProcessor.Add(pred4);
        }
        [Combination]
        public CompleteQuadriliateral CqNormaliza(Point p1, Point p2, Point p3, Point p4, Point p5, Point p6)
        {
            //
            if (!(KnowledgeGetter.HasColine(p1, p2, p4)))
            {
                if (KnowledgeGetter.HasColine(p1, p2, p5))
                {
                    Point temp = p4;
                    p4 = p5;
                    p5 = temp;
                }
                else
                {
                    Point temp = p4;
                    p4 = p6;
                    p6 = temp;

                }
            }
            if (!(KnowledgeGetter.HasColine(p2, p3, p5)))
            {
                Point temp = p5;
                p5 = p6;
                p6 = temp;
            }

            if (!(KnowledgeGetter.HasColine(p1, p3, p6)))
            {
                return null;
            }
            CompleteQuadriliateral pred = new CompleteQuadriliateral(p1, p2, p3, p4, p5, p6);
            return pred;

        }

        
        [Combination]
        public static Line GetCommonPointsLine(Line line1_1, Line line2_1, Line line2_2)
        {
            // 获取每条 Line 的 Point 集合
            var points1 = new HashSet<Point>(line1_1.Points);
            var points2 = new HashSet<Point>(line2_1.Points);
            var points3 = new HashSet<Point>(line2_2.Points);

            // 找出 line1_1 与 line2_1 之间的共有点
            var commonWithLine2_1 = points1.Intersect(points2);

            // 找出 line1_1 与 line2_2 之间的共有点
            var commonWithLine2_2 = points1.Intersect(points3);

            // 合并两个集合（去重）
            var commonPoints = commonWithLine2_1.Union(commonWithLine2_2).ToList();

            // 共同点少于 2 个，则返回 null
            if (commonPoints.Count < 2)
            {
                return null;
            }

            // 创建新 Line
            return new Line(commonPoints.ToArray());
        }

        public void 共线线段比例扩展(SLR segmentLengthRatio, PointWithInPoints pointOnSeg)
        {
            var expr1 = segmentLengthRatio.Expr.Clone();
            var expr9 = expr1.Clone();
            if (expr1 != 1)
            {
                expr9 = segmentLengthRatio.Expr.Invert().Clone();
            }
            
            SLR pred9 = new SLR((Point)segmentLengthRatio[2], (Point)segmentLengthRatio[1], (Point)segmentLengthRatio[0], expr9);
            pred9.AddCondition(segmentLengthRatio);
            pred9.AddReason();
            AddProcessor.Add(pred9);

            
            //把expr数值转成int再来算


            //首先确保pointOnSeg是segmentLengthRatio的

            //比较法，例如AB:AR=1/a，R在AB上 AC:CQ = 1/c,Q在AC上
            List<Segment> segOfpointOnSeg = new List<Segment>()
            {
                KnowledgeGetter.GetSegment(pointOnSeg[1], pointOnSeg[2]),
                KnowledgeGetter.GetSegment(pointOnSeg[0], pointOnSeg[1]),
                KnowledgeGetter.GetSegment(pointOnSeg[0], pointOnSeg[2])
            };
            List<Segment> segOfsegmentLengthRatio = new List<Segment>()
            {
                KnowledgeGetter.GetSegment(segmentLengthRatio[0], segmentLengthRatio[1]),
                KnowledgeGetter.GetSegment(segmentLengthRatio[1], segmentLengthRatio[2]), 
            };

            foreach (Segment line in segOfsegmentLengthRatio)
            {
                segOfpointOnSeg.Remove(line);
            }
            if (!(segOfpointOnSeg.Count == 1)) return;
            //不一定是最长的，默认点在线上不在端点上就可以了
            Segment longestseg = KnowledgeGetter.GetSegment(pointOnSeg[1], pointOnSeg[2]);
            

            //如果最长的线段在第一位，不需要倒过来，仅需要x-1
            if (segOfsegmentLengthRatio[0] == longestseg)
            {
                Expr expr4;
                //全部变为乘看看
                expr4 = expr1 - (Expr)1;
                var expr7 = expr4.Clone();
                if (expr4 != 1)
                {
                    expr7 = expr4.Invert().Clone();
                }
                

                //SegmentLengthRatio pred3 = new SegmentLengthRatio(segOfsegmentLengthRatio[1], segOfpointOnSeg[0], expr4);
                SLR pred3 = new SLR((Point)segmentLengthRatio[1], (Point)segmentLengthRatio[2], (Point)segmentLengthRatio[0], expr7);
                pred3.AddCondition(segmentLengthRatio);
                pred3.AddReason();
                AddProcessor.Add(pred3);

                SLR pred4 = new SLR((Point)segmentLengthRatio[0], (Point)segmentLengthRatio[2], (Point)segmentLengthRatio[1], expr4);
                pred4.AddCondition(segmentLengthRatio);
                pred4.AddReason();
                AddProcessor.Add(pred4);

                var expr5 = expr7.Clone();
                Expr expr6 = expr1 * expr5;
                //SegmentLengthRatio pred2 = new SegmentLengthRatio(longestseg, segOfpointOnSeg[0], expr6);
                SLR pred2 = new SLR((Point)segmentLengthRatio[1], (Point)segmentLengthRatio[0], (Point)segmentLengthRatio[2], expr6);
                pred2.AddCondition(segmentLengthRatio);
                pred2.AddReason();
                AddProcessor.Add(pred2);
                var expr8 = expr6.Clone();
                if (expr6 != 1)
                {
                    expr8 = expr6.Invert().Clone();
                }
                
                SLR pred5 = new SLR((Point)segmentLengthRatio[2], (Point)segmentLengthRatio[0], (Point)segmentLengthRatio[1], expr8);
                pred5.AddCondition(segmentLengthRatio);
                pred5.AddReason();
                AddProcessor.Add(pred5);
                //短的两个相比


            }
            else if (segOfsegmentLengthRatio[1] == longestseg)//最长的线段在第二位，
            {

                Expr expr4 = 1 - expr1;
                var expr5 = expr4.Clone();
                if (expr4 != 1)
                {
                    expr5 = expr4.Invert().Clone();
                }
                
                //SegmentLengthRatio pred2 = new SegmentLengthRatio(longestseg, segOfpointOnSeg[0], expr5);
                SLR pred2 = new SLR((Point)segmentLengthRatio[1], (Point)segmentLengthRatio[2], (Point)segmentLengthRatio[0], expr5);
                pred2.AddCondition(segmentLengthRatio);
                pred2.AddReason();
                AddProcessor.Add(pred2);

                SLR pred4 = new SLR((Point)segmentLengthRatio[0], (Point)segmentLengthRatio[2], (Point)segmentLengthRatio[1], expr4);
                pred4.AddCondition(segmentLengthRatio);
                pred4.AddReason();
                AddProcessor.Add(pred4);



                //短的两个相比
                Expr expr6 = expr1 * expr5;
                //SegmentLengthRatio pred3 = new SegmentLengthRatio(segOfsegmentLengthRatio[0], segOfpointOnSeg[0], expr6);
                SLR pred3 = new SLR((Point)segmentLengthRatio[1], (Point)segmentLengthRatio[0], (Point)segmentLengthRatio[2], expr6);
                pred3.AddCondition(segmentLengthRatio);
                pred3.AddReason();
                AddProcessor.Add(pred3);
                var expr7 = expr6.Clone();
                if (expr6 != 1)
                {
                    expr7 = expr6.Invert().Clone();
                }
                

                SLR pred5 = new SLR((Point)segmentLengthRatio[2], (Point)segmentLengthRatio[0], (Point)segmentLengthRatio[1], expr7);
                pred5.AddCondition(segmentLengthRatio);
                pred5.AddReason();
                AddProcessor.Add(pred5);

            }
            else //没有最长的线段,加起来,生成最长的线段比上其他线段的比值
            {
                Expr expr4;
                expr4 = expr1 + (Expr)1;

                var expr2 = expr1.Clone();
                if (expr1 != 1)
                {
                    expr2 = expr1.Invert().Clone();
                }
                var expr6 = expr4.Clone();
                SLR pred4 = new SLR((Point)segmentLengthRatio[0], (Point)segmentLengthRatio[2], (Point)segmentLengthRatio[1], expr6);
                //SegmentLengthRatio pred4 = new SegmentLengthRatio(longestseg, segOfsegmentLengthRatio[1], expr6);
                pred4.AddCondition(segmentLengthRatio);
                pred4.AddReason();
                AddProcessor.Add(pred4);

                var expr7 = expr6.Clone();
                if (expr6 != 1)
                {
                    expr7 = expr6.Invert().Clone();
                }
                SLR pred6 = new SLR((Point)segmentLengthRatio[1], (Point)segmentLengthRatio[2], (Point)segmentLengthRatio[0], expr7);
                //SegmentLengthRatio pred4 = new SegmentLengthRatio(longestseg, segOfsegmentLengthRatio[1], expr6);
                pred6.AddCondition(segmentLengthRatio);
                pred6.AddReason();
                AddProcessor.Add(pred6);

                Expr expr5 = expr4 * expr2;
                SLR pred3 = new SLR((Point)segmentLengthRatio[2], (Point)segmentLengthRatio[0], (Point)segmentLengthRatio[1], expr5);
                //SegmentLengthRatio pred3 = new SegmentLengthRatio(longestseg, segOfsegmentLengthRatio[0], expr5);
                pred3.AddCondition(segmentLengthRatio);
                pred3.AddReason();
                AddProcessor.Add(pred3);
                
                    var expr8 = expr5.Clone();
                if (expr5 != 1)
                {
                    expr8 = expr5.Invert().Clone();
                }
                SLR pred8 = new SLR((Point)segmentLengthRatio[1], (Point)segmentLengthRatio[0], (Point)segmentLengthRatio[2], expr8);
                    //SegmentLengthRatio pred3 = new SegmentLengthRatio(longestseg, segOfsegmentLengthRatio[0], expr5);
                    pred8.AddCondition(segmentLengthRatio);
                    pred8.AddReason();
                    AddProcessor.Add(pred8);
               

            }

        }

        

        public void 梅涅劳斯定理(CompleteQuadriliateral completeQuadriliateral)
        {
            
            SLR ABBC = KnowledgeGetter.GetSegmentLengthRatio1((Point)completeQuadriliateral[0], (Point)completeQuadriliateral[3], (Point)completeQuadriliateral[1]);
            SLR CDDF = KnowledgeGetter.GetSegmentLengthRatio1((Point)completeQuadriliateral[1], (Point)completeQuadriliateral[4], (Point)completeQuadriliateral[2]);
            SLR FEEA = KnowledgeGetter.GetSegmentLengthRatio1((Point)completeQuadriliateral[2], (Point)completeQuadriliateral[5], (Point)completeQuadriliateral[0]);

            //创建等式，创建一个新知识
            Expr i = 1;
            SREE equation = new SREE(i, ABBC, CDDF, FEEA);
            equation.AddReason();
            equation.AddCondition(completeQuadriliateral);
            AddProcessor.Add(equation);


        }

        //public void 梅涅劳斯逆定理(SREE eq1)
        //{

        //    //（1）要先判断这个等式的形式好吧
        //    if (eq1.count != 3) return;

        //    //（2）符合条件后，判断一下是不是只有六个点
        //    List<Point> points = new List<Point>
        //    {
        //        eq1.SegLR1.point1,
        //        eq1.SegLR1.point2,
        //        eq1.SegLR1.point3,

        //        eq1.SegLR2.point1,
        //        eq1.SegLR2.point2,
        //        eq1.SegLR2.point3,

        //        eq1.SegLR3.point1,
        //        eq1.SegLR3.point2,
        //        eq1.SegLR3.point3,


        //    };
        //    List<Point> uniquePoints = points.Distinct().ToList();
        //    if (uniquePoints.Count != 6) return;

        //    //（3）每条等式是不是都有重复的一个点，拿出来


        //    uniquePoints.Remove(eq1.SegLR1.point2);
        //    uniquePoints.Remove(eq1.SegLR2.point2);
        //    uniquePoints.Remove(eq1.SegLR3.point2);
        //    //（4）判断这个完全四边形是不是生成过的，没生成就生成一个new的
        //    Point p1 = uniquePoints[0];
        //    Point p2 = uniquePoints[1];
        //    Point p3 = uniquePoints[2];


        //    Point p4 = eq1.SegLR1.point2;
        //    Point p5 = eq1.SegLR2.point2;
        //    Point p6 = eq1.SegLR3.point2;

        //    CompleteQuadriliateral pred = CqNormaliza(p1, p2, p3, p4, p5, p6);
        //    pred.AddReason();
        //    pred.AddCondition(eq1);
        //    AddProcessor.Add(pred);

        //    //getter里面取
        //    Line line1 = KnowledgeGetter.GetLine((Point)pred[0], (Point)pred[1], (Point)pred[3]);
        //    Line line2 = KnowledgeGetter.GetLine((Point)pred[1], (Point)pred[2], (Point)pred[4]);
        //    Line line3 = KnowledgeGetter.GetLine((Point)pred[2], (Point)pred[0], (Point)pred[5]);
        //    Line line4 = KnowledgeGetter.GetLine((Point)pred[3], (Point)pred[4], (Point)pred[5]);
        //    if (line1 == null)
        //    {
        //        line1 = new Line((Point)pred[0], (Point)pred[1], (Point)pred[3]);
        //        line1.AddReason();
        //        line1.AddCondition(pred);
        //        AddProcessor.Add(line1);
        //    }

        //    if (line2 == null)
        //    {
        //        line2 = new Line((Point)pred[1], (Point)pred[2], (Point)pred[4]);
        //        line2.AddReason();
        //        line2.AddCondition(pred);
        //        AddProcessor.Add(line2);
        //    }
        //    if (line3 == null)
        //    {
        //        line3 = new Line((Point)pred[2], (Point)pred[0], (Point)pred[5]);
        //        line3.AddReason();
        //        line3.AddCondition(pred);
        //        AddProcessor.Add(line3);
        //    }


        //    if (line4 == null)
        //    {
        //        line4 = new Line((Point)pred[3], (Point)pred[4], (Point)pred[5]);
        //        line4.AddReason();
        //        line4.AddCondition(pred);
        //        AddProcessor.Add(line4);
        //    }

        //}
        public void 比例式化简(SREE equation1, SLR segmentLengthRatio1)
        {
            
                if ((segmentLengthRatio1.point1.ToString() == "A" && segmentLengthRatio1.point2.ToString() == "N" && segmentLengthRatio1.point3.ToString() == "C")
                       
                       )
                {
                    int i = 1;
                }
                   
            if(equation1.count == 4)
            {
                int i = 1;
            }
            var expr = 1 / 99;
            //算比值的
            if (segmentLengthRatio1.Expr == 99 || segmentLengthRatio1.Expr == expr) return;
            if (equation1.count >= 5 && equation1.count == 0) return;
            bool flag = false;
            List<SLR> pequtaion = new List<SLR>();//存放与segmentLengthRatio1不相等的SLR
            if (equation1.count >= 3)
            {
            
                foreach (var segment1 in equation1.Properties)
                {
                    if (segment1 == segmentLengthRatio1)
                    {
                        flag = true;
                        continue;
                    }
                    pequtaion.Add((SLR)segment1);
                }
                if (!flag) return;

                var v1 = segmentLengthRatio1.Expr;
                if (segmentLengthRatio1.Expr == 1)
                {
                    v1 = segmentLengthRatio1.Expr;
                }
                else
                {
                    v1 = segmentLengthRatio1.Expr.Invert();
                }
                var value = equation1.Expr * v1;
                if (segmentLengthRatio1.Expr == 1 && equation1.Expr == 1)
                {
                    value = 1;
                }

                  SREE sREE = new SREE(value, pequtaion.ToArray());
                sREE.AddReason();
                sREE.AddCondition(equation1, segmentLengthRatio1);
                AddProcessor.Add(sREE);
                
                if (sREE.count == 2)
                {
                    GeoEquation equation3 = new GeoEquation(1, 1);
                    //if (value is FractionNode )
                    //{
                        
                    //    foreach (var item in pequtaion)
                    //    {
                    //        Segment seg1 = KnowledgeGetter.GetSegment((Point)item.Properties[0], (Point)item.Properties[1]);
                    //        Segment seg2 = KnowledgeGetter.GetSegment((Point)item.Properties[1], (Point)item.Properties[2]);
                    //        var equation2 = new GeoEquation(seg2.Length / seg1.Length, 1);
                    //        equation3 = equation3.Mul(equation2);
                    //    }
                    //    equation3.RightPart = value.Invert();
                    //    equation3.AddReason();
                    //    equation3.AddCondition(equation1, segmentLengthRatio1);
                    //    AddProcessor.Add(equation3);
                    //}
                    //else
                    //{
                        foreach (var item in pequtaion)
                        {
                            Segment seg1 = KnowledgeGetter.GetSegment((Point)item.Properties[0], (Point)item.Properties[1]);
                            Segment seg2 = KnowledgeGetter.GetSegment((Point)item.Properties[1], (Point)item.Properties[2]);
                            var equation2 = new GeoEquation(seg1.Length / seg2.Length, 1);
                            equation3 = equation3.Mul(equation2);
                        }
                    
                        equation3.RightPart = value;
                    if ((pequtaion[0][0].ToString() == "A" && pequtaion[0][1].ToString() == "M" && pequtaion[0][2].ToString() == "B")
                        || (pequtaion[1][0].ToString() == "A" && pequtaion[1][1].ToString() == "M" && pequtaion[1][2].ToString() == "B")
                       )
                    {
                        int i = 1;
                    }
                    GeoEquation equation5 = new GeoEquation(equation3.LeftPart, value);
                    equation5.AddReason();
                    equation5.AddCondition(equation1, segmentLengthRatio1);
                    AddProcessor.Add(equation5);
                    //}



                }

            }
            else
            {
                foreach (var segment1 in equation1.Properties)
                {
                    if (segment1 == segmentLengthRatio1)
                    {
                        flag = true;
                        continue;
                    }
                    pequtaion.Add((SLR)segment1);
                }
                if (pequtaion.Count == 0) return;
                if (!flag) return;
                var value = equation1.Expr * segmentLengthRatio1.Expr.Invert();
                SLR segmentLengthRatio =KnowledgeGetter.GetSegmentLengthRatio1((Point)pequtaion[0].Properties[0], (Point)pequtaion[0].Properties[1], (Point)pequtaion[0].Properties[2]);
                if(segmentLengthRatio.Expr == 99 || segmentLengthRatio.Expr == expr)
                {
                    segmentLengthRatio.Expr = value;
                    segmentLengthRatio.AddReason();
                    segmentLengthRatio.AddCondition(equation1, segmentLengthRatio1);
                    AddProcessor.Add(segmentLengthRatio);
                }
                
                
            }
            

        }

        //public void 比例式计算(SREE eq1, SREE eq2)
        //{

        //    if (eq1.count != 3 || eq2.count != 3) return; //避免等式太长导致无限循环
        //    if (eq1.Expr != 1 || eq2.Expr != 1) return;
        //    if (eq1.count >= 5 || eq2.count >= 5) return; //避免等式太长导致无限循环
        //    if (eq1.count == 0 || eq2.count == 0) return;
        //    if (eq1.count < 3 || eq2.count < 3) return;
        //    int index1 = 0;
        //    int index2 = 0;
        //    bool flag = false;

        //    if (eq1.Properties[0].Properties[0].ToString().Equals("A")
        //        && eq1.Properties[0].Properties[2].ToString().Equals("D"))
        //    {
        //        if (eq2.Properties[0].Properties[0].ToString().Equals("C")
        //       && eq2.Properties[0].Properties[2].ToString().Equals("D"))
        //        {

        //            GeoEquation equation1 = new GeoEquation(1, 1);
        //            Segment AF = KnowledgeGetter.GetSegment((Point)eq1.Properties[0].Properties[0], (Point)eq1.Properties[2].Properties[1]);
        //            Segment FB = KnowledgeGetter.GetSegment((Point)eq1.Properties[2].Properties[1], (Point)eq2.Properties[0].Properties[1]);
        //            Segment BD = KnowledgeGetter.GetSegment((Point)eq2.Properties[0].Properties[1], (Point)eq2.Properties[0].Properties[2]);
        //            Segment CD = KnowledgeGetter.GetSegment((Point)eq2.Properties[0].Properties[0], (Point)eq2.Properties[0].Properties[2]);
        //            Segment CE = KnowledgeGetter.GetSegment((Point)eq2.Properties[0].Properties[0], (Point)eq2.Properties[2].Properties[1]);
        //            Segment EA = KnowledgeGetter.GetSegment((Point)eq2.Properties[2].Properties[1], (Point)eq1.Properties[0].Properties[0]);
        //            var equation2 = new GeoEquation(AF.Length / FB.Length, 1);
        //            var equation3 = new GeoEquation(BD.Length / CD.Length, 1);
        //            var equation4 = new GeoEquation(CE.Length / EA.Length, 1);
        //            equation1 = equation1.Mul(equation2);
        //            equation1 = equation1.Mul(equation3);
        //            equation1 = equation1.Mul(equation4);
        //            equation1.AddCondition(eq1, eq2);
        //            equation1.AddReason();
        //            AddProcessor.Add(equation1);
        //            return;

        //        }
        //    }
        //    List<SLR> commonSegment = new List<SLR>();
        //    List<SLR> commonSegmentInvert = new List<SLR>();
        //    SREE eq1invert = null;
        //    // 找到两个 SegmentRatioEquation 中相同的 SegmentLengthRatio
        //    foreach (var segment1 in eq1.Properties)
        //    {
        //        index1++;
        //        foreach (var segment2 in eq2.Properties)
        //        {
        //            index2++;
        //            // 判断两个 SegmentLengthRatio 是否相等,这里是以线段比较
        //            if (segment1.Properties[1] == segment2.Properties[1])
        //            {
        //                if (segment1.Properties[0] == segment2.Properties[0] || segment1.Properties[0] == segment2.Properties[2])
        //                {
        //                    if (segment1.Properties[2] == segment2.Properties[0] || segment1.Properties[2] == segment2.Properties[2])
        //                    {
        //                        //以eq1为正
        //                        commonSegment.Add((SLR)segment1);
        //                        var expr = segment1.Expr.Clone();
        //                        if (segment1.Expr != 1)
        //                        {
        //                            expr = segment1.Expr.Invert().Clone();

        //                        }
        //                        SLR segmentLengthRatio = new SLR((Point)segment1.Properties[2], (Point)segment1.Properties[1], (Point)segment1.Properties[0], expr);
        //                        commonSegmentInvert.Add(segmentLengthRatio);
        //                        flag = true;
        //                        continue;
        //                    }

        //                }

        //            }
        //        }
        //    }

        //    //判断是否相反
        //    if (flag)
        //    {
        //        List<SLR> invertedSegments = new List<SLR>();
        //        List<SLR> zhengfan = new List<SLR>();
        //        bool zheng = false;
        //        foreach (var segment in eq2.Properties)
        //        {
        //            foreach (var segment2 in commonSegment)
        //            {
        //                if (segment == segment2)
        //                {
        //                    zheng = true;
        //                    break;
        //                }
        //            }
        //        }
        //        foreach (var segment in eq1.Properties)
        //        {
        //            if (!commonSegment.Any(s => s.Properties[1] == segment.Properties[1] && s.Properties[0] == segment.Properties[0]
        //            && s.Properties[2] == segment.Properties[2]))
        //            {
        //                if (!commonSegmentInvert.Any(s => s.Properties[1] == segment.Properties[1] && s.Properties[0] == segment.Properties[0]
        //            && s.Properties[2] == segment.Properties[2]))
        //                {
        //                    invertedSegments.Add((SLR)segment);

        //                }
        //            }
        //        }
        //        // 遍历 eq2.Properties，将不等于 commonSegment 的部分添加到 eq1invert
        //        foreach (var segment in eq2.Properties)
        //        {


        //            //判断eq2是正还是反，正要倒，反不用倒
        //            if (!commonSegment.Any(s => s.Properties[1] == segment.Properties[1] && s.Properties[0] == segment.Properties[0]
        //            && s.Properties[2] == segment.Properties[2]))
        //            {
        //                //这是不等于正的
        //                if (!commonSegmentInvert.Any(s => s.Properties[1] == segment.Properties[1] && s.Properties[0] == segment.Properties[0]
        //            && s.Properties[2] == segment.Properties[2]))
        //                {
        //                    if (zheng)
        //                    {
        //                        SLR segmentLengthRatio = new SLR((Point)segment.Properties[2], (Point)segment.Properties[1], (Point)segment.Properties[0], segment.Expr.Invert());
        //                        invertedSegments.Add(segmentLengthRatio);
        //                    }
        //                    else
        //                    {
        //                        SLR segmentLengthRatio = new SLR((Point)segment.Properties[0], (Point)segment.Properties[1], (Point)segment.Properties[2], segment.Expr.Invert());
        //                        invertedSegments.Add(segmentLengthRatio);
        //                    }

        //                }
        //            }
        //        }

        //        //进行替换




        //        if (invertedSegments.Count == 0 || invertedSegments.Count >= 5) return;
        //        //GeoEquation equation1 = new GeoEquation(1, 1);


        //        //foreach (var item in invertedSegments)
        //        //{

        //        //    Segment seg1 = KnowledgeGetter.GetSegment((Point)item.Properties[0], (Point)item.Properties[1]);
        //        //    Segment seg2 = KnowledgeGetter.GetSegment((Point)item.Properties[1], (Point)item.Properties[2]);
        //        //    var equation2 = new GeoEquation(seg1.Length / seg2.Length, 1);
        //        //    equation1 = equation1.Mul(equation2);
        //        //}

        //        //equation1.AddCondition(eq1, eq2);
        //        //equation1.AddReason();
        //        //AddProcessor.Add(equation1);
        //        SREE sreInstance = new SREE(eq1.Expr, invertedSegments.ToArray());
        //        sreInstance.AddReason();
        //        sreInstance.AddCondition(eq1, eq2);
        //        AddProcessor.Add(sreInstance);

        //    }
        //    else
        //    {
        //        return;
        //    }

        //}


        //public void Cal(SREE equation1, SLR segmentLengthRatio1, SLR segmentLengthRatio2)
        //{
        //    //算比值的
        //    if (equation1.count >= 5) return;

        //    if (!(equation1.SegLR1 == segmentLengthRatio1 || equation1.SegLR1 == segmentLengthRatio2))
        //    {
        //        if (!(equation1.SegLR2 == segmentLengthRatio1 || equation1.SegLR2 == segmentLengthRatio2))
        //        {
        //            return;
        //        }
        //    }

        //    int zeroCount = 0;
        //    int index = 0;

        //    Expr nine1 = (Expr)1 / (Expr)99;

        //    SLR zeroRatio = null;
        //    List<SLR> nonZeroList = new List<SLR>();

        //    // 检查每个 SegmentLengthRatio 的比值是否为 0
        //    if (equation1.SegLR1.Expr == 99 || equation1.SegLR1.Expr == nine1)
        //    {
        //        index = 1;
        //        zeroCount++;
        //        zeroRatio = equation1.SegLR1;
        //    }
        //    else
        //    {
        //        nonZeroList.Add(equation1.SegLR1);
        //    }

        //    if (equation1.SegLR2.Expr == 99 || equation1.SegLR2.Expr == nine1)
        //    {
        //        index = 2;
        //        zeroCount++;
        //        zeroRatio = equation1.SegLR2;
        //    }
        //    else
        //    {
        //        nonZeroList.Add(equation1.SegLR2);
        //    }

        //    if (equation1.SegLR3.Expr == 99 || equation1.SegLR3.Expr == nine1)
        //    {
        //        index = 3;
        //        zeroCount++;
        //        zeroRatio = equation1.SegLR3;
        //    }
        //    else
        //    {
        //        nonZeroList.Add(equation1.SegLR3);
        //    }

        //    // 若有2个或以上的比值为99，则直接返回，不做计算
        //    if (zeroCount >= 2)
        //    {
        //        return;
        //    }


        //    // 此时仅有一个比值为 0，根据等式：r1 * r2 * r3 = 1，
        //    // 可知缺失的比值为 1 除以另外两个比值的乘积
        //    Expr product = nonZeroList[0].Expr * nonZeroList[1].Expr;
        //    Expr newValue = product.Invert();
        //    if (index == 0) return;
        //    // 用计算得到的新值替换原来为0的比值,我该怎么知道哪个是0
        //    if (index == 1)
        //    {
        //        equation1.SegLR1.Expr = newValue;
        //        //SegmentLengthRatio segmentLengthRatio = equation1.SegLR1;
        //        SLR segmentLengthRatio = equation1.SegLR1;
        //        segmentLengthRatio.AddReason();
        //        segmentLengthRatio.AddCondition(equation1, segmentLengthRatio1, segmentLengthRatio2);
        //        AddProcessor.Add(segmentLengthRatio);

        //    }

        //    if (index == 2)
        //    {
        //        equation1.SegLR2.Expr = newValue;
        //        //SegmentLengthRatio segmentLengthRatio = equation1.SegLR2;
        //        SLR segmentLengthRatio = equation1.SegLR2;
        //        segmentLengthRatio.AddReason();
        //        segmentLengthRatio.AddCondition(equation1, segmentLengthRatio1, segmentLengthRatio2);
        //        AddProcessor.Add(segmentLengthRatio);
        //    }

        //    if (index == 3)
        //    {
        //        equation1.SegLR3.Expr = newValue;
        //        //SegmentLengthRatio segmentLengthRatio = equation1.SegLR3;
        //        SLR segmentLengthRatio = equation1.SegLR3;
        //        segmentLengthRatio.AddReason();
        //        segmentLengthRatio.AddCondition(equation1, segmentLengthRatio1, segmentLengthRatio2);
        //        AddProcessor.Add(segmentLengthRatio);
        //    }

        //}



        //public bool JudgeCommonPoints(Line line1_1, Line line1_2, Line line2_1, Line line2_2)
        //{
        //    // 获取每条 Line 的 Point 列表
        //    var points1_1 = new HashSet<Point>(line1_1.Points);
        //    var points1_2 = new HashSet<Point>(line1_2.Points);
        //    var points2_1 = new HashSet<Point>(line2_1.Points);
        //    var points2_2 = new HashSet<Point>(line2_2.Points);

        //    // 检查是否有相同的 Point
        //    bool hasCommon = (points1_1.Overlaps(points2_1) && points1_1.Overlaps(points2_2)) &&
        //                     (points1_2.Overlaps(points2_1) && points1_2.Overlaps(points2_2));
        //    if (!hasCommon) return false;

        //    return true;
        //}

        //public void SegmentProportionalityExtension(SegmentLengthRatio segmentLengthRatio, PointWithInPoints pointOnSeg)
        //{
        //    //把expr数值转成int再来算


        //    //首先确保pointOnSeg是segmentLengthRatio的

        //    //比较法，例如AB:AR=1/a，R在AB上 AC:CQ = 1/c,Q在AC上
        //    List<Segment> segOfpointOnSeg = new List<Segment>()
        //    {
        //        KnowledgeGetter.GetSegment(pointOnSeg[1], pointOnSeg[2]),
        //        KnowledgeGetter.GetSegment(pointOnSeg[0], pointOnSeg[1]),
        //        KnowledgeGetter.GetSegment(pointOnSeg[0], pointOnSeg[2])
        //    };
        //    List<Segment> segOfsegmentLengthRatio = new List<Segment>()
        //    {
        //        segmentLengthRatio.Seg1,
        //        segmentLengthRatio.Seg2
        //    };

        //    foreach (Segment line in segOfsegmentLengthRatio)
        //    {
        //        segOfpointOnSeg.Remove(line);
        //    }
        //    if (!(segOfpointOnSeg.Count == 1)) return;
        //    //不一定是最长的，默认点在线上不在端点上就可以了
        //    Segment longestseg = KnowledgeGetter.GetSegment(pointOnSeg[1], pointOnSeg[2]);
        //    var expr1 = segmentLengthRatio.Expr.Clone();


        //    //如果最长的线段在第一位，不需要倒过来，仅需要x-1
        //    if (segOfsegmentLengthRatio[0] == longestseg)
        //    {
        //        Expr expr4;
        //        //全部变为乘看看
        //        expr4 = expr1 - (Expr)1;
        //        expr4 = expr4.Invert();

        //        SegmentLengthRatio pred3 = new SegmentLengthRatio(segmentLengthRatio.Seg2, segOfpointOnSeg[0], expr4);
        //        pred3.AddCondition(segmentLengthRatio);
        //        pred3.AddReason();
        //        AddProcessor.Add(pred3);
        //        var expr5 = expr4.Clone();
        //        Expr expr6 = expr1 * expr5;
        //        SegmentLengthRatio pred2 = new SegmentLengthRatio(longestseg, segOfpointOnSeg[0], expr6);
        //        pred2.AddCondition(segmentLengthRatio);
        //        pred2.AddReason();
        //        AddProcessor.Add(pred2);
        //        //短的两个相比


        //    }
        //    else if (segOfsegmentLengthRatio[1] == longestseg)//最长的线段在第二位，
        //    {

        //        Expr expr4 = 1 - expr1;
        //        var expr5 = expr4.Invert().Clone();
        //        SegmentLengthRatio pred2 = new SegmentLengthRatio(longestseg, segOfpointOnSeg[0], expr5);
        //        pred2.AddCondition(segmentLengthRatio);
        //        pred2.AddReason();
        //        AddProcessor.Add(pred2);

        //        //短的两个相比
        //        Expr expr6 = expr1 * expr5;
        //        SegmentLengthRatio pred3 = new SegmentLengthRatio(segmentLengthRatio.Seg1, segOfpointOnSeg[0], expr6);
        //        pred3.AddCondition(segmentLengthRatio);
        //        pred3.AddReason();
        //        AddProcessor.Add(pred3);

        //    }
        //    else //没有最长的线段,加起来,生成最长的线段比上其他线段的比值
        //    {
        //        Expr expr4;
        //        expr4 = expr1 + (Expr)1;
        //        var expr2 = expr1.Invert().Clone();
        //        var expr6 = expr4.Clone();

        //        SegmentLengthRatio pred4 = new SegmentLengthRatio(longestseg, segmentLengthRatio.Seg2, expr6);
        //        pred4.AddCondition(segmentLengthRatio);
        //        pred4.AddReason();
        //        AddProcessor.Add(pred4);
        //        Expr expr5 = expr4 * expr2;
        //        SegmentLengthRatio pred3 = new SegmentLengthRatio(longestseg, segmentLengthRatio.Seg1, expr5);
        //        pred3.AddCondition(segmentLengthRatio);
        //        pred3.AddReason();
        //        AddProcessor.Add(pred3);


        //    }

        //}
        //public void Cal(SREE eq1, SREE eq2)
        //{

        //    if (eq1.count >= 5 || eq2.count >= 5) return; //避免等式太长导致无限循环
        //    int index1 = 0;
        //    int index2 = 0;
        //    bool flag = false;

        //    List<SegmentLengthRatio> commonSegment = new List<SegmentLengthRatio>();
        //    List<SegmentLengthRatio> commonSegmentInvert = new List<SegmentLengthRatio>();
        //    SRE eq1invert = null;
        //    // 找到两个 SegmentRatioEquation 中相同的 SegmentLengthRatio
        //    foreach (var segment1 in eq1.Properties)
        //    {
        //        index1++;
        //        foreach (var segment2 in eq2.Properties)
        //        {
        //            index2++;
        //            // 判断两个 SegmentLengthRatio 是否相等
        //            if (segment1.Properties[0] == segment2.Properties[0] || segment1.Properties[0] == segment2.Properties[1])
        //            {
        //                if (segment1.Properties[1] == segment2.Properties[0] || segment1.Properties[1] == segment2.Properties[1])
        //                {
        //                    commonSegment.Add((SegmentLengthRatio)segment1);
        //                    Segment seg1 = (Segment)segment1.Properties[0];
        //                    Segment seg2 = (Segment)segment1.Properties[1];
        //                    SegmentLengthRatio segmentLengthRatio = new SegmentLengthRatio(seg2, seg1, segment1.Expr.Invert());
        //                    commonSegmentInvert.Add(segmentLengthRatio);
        //                    flag = true;
        //                }

        //            }
        //        }
        //    }

        //    if (flag)
        //    {
        //        List<SegmentLengthRatio> invertedSegments = new List<SegmentLengthRatio>();

        //        // 遍历 eq2.Properties，将不等于 commonSegment 的部分添加到 eq1invert
        //        foreach (var segment in eq2.Properties)
        //        {
        //            if (!commonSegment.Any(s => s.Properties[0] == segment.Properties[0] && s.Properties[1] == segment.Properties[1]))
        //            {
        //                if (!commonSegmentInvert.Any(s => s.Properties[0] == segment.Properties[0] && s.Properties[1] == segment.Properties[1]))
        //                {

        //                    Segment seg1 = (Segment)segment.Properties[0];
        //                    Segment seg2 = (Segment)segment.Properties[1];
        //                    SegmentLengthRatio segmentLengthRatio = new SegmentLengthRatio(seg2, seg1, segment.Expr.Invert());
        //                    invertedSegments.Add(segmentLengthRatio);
        //                }
        //            }


        //            //foreach (var segment1 in commonSegment) //这个判定条件不太对
        //            //{

        //            //    if (!(segment == segment1))
        //            //    {
        //            //        if (segment.Properties[0] == segment1.Properties[1] && segment.Properties[1] == segment1.Properties[0]
        //            //            && segment.Expr.Invert().Equals(segment1.Expr))
        //            //        {
        //            //            //这是倒置的情况，直接跳过
        //            //            if (eq1.Properties[0].Properties[0].ToString() == "AA1" &&
        //            //                eq1.Properties[0].Properties[1].ToString() == "SA1")
        //            //            {
        //            //                index1 = 1;
        //            //            }
        //            //            continue;

        //            //        }
        //            //        //将每个segment倒置，然后赋给新的
        //            //        Segment seg1 = (Segment)segment.Properties[0];
        //            //        Segment seg2 = (Segment)segment.Properties[1];
        //            //        SegmentLengthRatio segmentLengthRatio = new SegmentLengthRatio(seg2, seg1, segment.Expr.Invert());
        //            //        invertedSegments.Add(segmentLengthRatio);
        //            //    }
        //            //}

        //        }


        //        foreach (var segment in eq1.Properties)
        //        {
        //            if (!commonSegment.Any(s => s.Properties[0] == segment.Properties[0] && s.Properties[1] == segment.Properties[1]))
        //            {
        //                if (!commonSegmentInvert.Any(s => s.Properties[0] == segment.Properties[0] && s.Properties[1] == segment.Properties[1]))
        //                {
        //                    invertedSegments.Add((SegmentLengthRatio)segment);

        //                }
        //            }
        //        }
        //        //}
        //        //foreach (var segment in eq1.Properties)
        //        //{
        //        //    foreach (var segment1 in commonSegment)
        //        //    {
        //        //        if (!(segment == segment1))
        //        //        {
        //        //            invertedSegments.Add((SegmentLengthRatio)segment);

        //        //        }
        //        //    }

        //        //}

        //        //再把得到的代入到eq1里面去
        //        //foreach (var segment in eq1.Properties)
        //        //{
        //        //    foreach (var segment1 in commonSegment)
        //        //    {
        //        //        if (!(segment == segment1))
        //        //        {
        //        //            invertedSegments.Add((SegmentLengthRatio)segment);

        //        //        }
        //        //    }

        //        //}



        //        if (invertedSegments.Count == 0 || invertedSegments.Count >= 5) return;
        //        //GeoEquation equation1 = new GeoEquation(1, 1);

        //        //var equation3 = new GeoEquation(1, 1);
        //        //foreach (var item in invertedSegments)
        //        //{

        //        //    var equation2 = new GeoEquation(item.Seg1.Length / item.Seg2.Length, 1);
        //        //    equation1 = equation1.Mul(equation2);
        //        //}

        //        //equation1.AddCondition(eq1, eq2);
        //        //equation1.AddReason();
        //        //AddProcessor.Add(equation1);
        //        SRE sreInstance = new SRE(invertedSegments.ToArray());
        //        sreInstance.AddReason();
        //        sreInstance.AddCondition(eq1, eq2);
        //        AddProcessor.Add(sreInstance);

        //    }
        //    else
        //    {
        //        return;
        //    }

        //}






        //public void 梅涅劳斯定理(CompleteQuadriliateral completeQuadriliateral)
        //{


        //Segment AB = new Segment((Point)completeQuadriliateral[0], (Point)completeQuadriliateral[3]);
        //Segment BC = new Segment((Point)completeQuadriliateral[3], (Point)completeQuadriliateral[1]);
        //Segment CD = new Segment((Point)completeQuadriliateral[1], (Point)completeQuadriliateral[4]);
        //Segment DF = new Segment((Point)completeQuadriliateral[4], (Point)completeQuadriliateral[2]);
        //Segment FE = new Segment((Point)completeQuadriliateral[2], (Point)completeQuadriliateral[5]);
        //Segment EA = new Segment((Point)completeQuadriliateral[5], (Point)completeQuadriliateral[0]);
        //List<Knowledge> tp = completeQuadriliateral.Properties.Take(3).ToList();
        //List<Knowledge> sp = completeQuadriliateral.Properties.Skip(3).ToList();


        //14/42/25/53/36/61
        //Segment AB = KnowledgeGetter.GetSegment((Point)completeQuadriliateral[0], (Point)completeQuadriliateral[3]);
        //Segment BC = KnowledgeGetter.GetSegment((Point)completeQuadriliateral[3], (Point)completeQuadriliateral[1]);
        //Segment CD = KnowledgeGetter.GetSegment((Point)completeQuadriliateral[1], (Point)completeQuadriliateral[4]);
        //Segment DF = KnowledgeGetter.GetSegment((Point)completeQuadriliateral[4], (Point)completeQuadriliateral[2]);
        //Segment FE = KnowledgeGetter.GetSegment((Point)completeQuadriliateral[2], (Point)completeQuadriliateral[5]);
        //Segment EA = KnowledgeGetter.GetSegment((Point)completeQuadriliateral[5], (Point)completeQuadriliateral[0]);

        //Segment AF = KnowledgeGetter.GetSegment(tp[0], tp[2]);
        //Segment ED = KnowledgeGetter.GetSegment(sp[2], sp[1]);
        //Segment DB = KnowledgeGetter.GetSegment(sp[1], sp[0]);
        //Segment CA = KnowledgeGetter.GetSegment(tp[1], sp[0]);

        //Segment BE = KnowledgeGetter.GetSegment(sp[0], sp[2]);
        //Segment FC = KnowledgeGetter.GetSegment(tp[2], tp[1]);



        //Expr c = AB.Length.Sub(BC.Length);
        //var equation = new GeoEquation(AB.Length / BC.Length * CD.Length / DF.Length * FE.Length / EA.Length, 1);
        //equation.AddCondition(completeQuadriliateral);
        //equation.AddReason();
        //AddProcessor.Add(equation);

        //var equation1 = new GeoEquation(AF.Length / FE.Length * ED.Length / DB.Length * BC.Length / CA.Length, 1);
        //equation1.AddCondition(completeQuadriliateral);
        //equation1.AddReason();
        //AddProcessor.Add(equation1);

        //var equation2 = new GeoEquation(CA.Length / AB.Length * BE.Length / ED.Length * DF.Length / FE.Length, 1);
        //equation2.AddCondition(completeQuadriliateral);
        //equation2.AddReason();
        //AddProcessor.Add(equation2);

        //var equation3 = new GeoEquation(EA.Length / AF.Length * FC.Length / CD.Length * DB.Length / BE.Length, 1);
        //equation3.AddCondition(completeQuadriliateral);
        //equation3.AddReason();
        //AddProcessor.Add(equation3);


        ////梅涅劳斯逆定理，14，42,25，53 13,36

        //var equationni = new GeoEquation(AB.Length / BC.Length * CD.Length / DF.Length, 1 + AF.Length / FE.Length);
        //equationni.AddCondition(completeQuadriliateral);
        //equationni.AddReason();
        //AddProcessor.Add(equationni);

        //}



        //public void MenelausTheoremNi(SREE eq1)
        //{

        //    //（1）要先判断这个等式的形式好吧
        //    if (eq1.count != 3) return;

        //    //（2）符合条件后，判断一下是不是只有六个点
        //    List<Point> points = new List<Point>
        //    {
        //        eq1.SegLR1.point1,
        //        eq1.SegLR1.point2,
        //        eq1.SegLR1.point3,

        //        eq1.SegLR2.point1,
        //        eq1.SegLR2.point2,
        //        eq1.SegLR2.point3,

        //        eq1.SegLR3.point1,
        //        eq1.SegLR3.point2,
        //        eq1.SegLR3.point3,


        //    };
        //    List<Point> uniquePoints = points.Distinct().ToList();
        //    if (uniquePoints.Count != 6) return;

        //    //（3）每条等式是不是都有重复的一个点，拿出来


        //    uniquePoints.Remove(eq1.SegLR1.point2);
        //    uniquePoints.Remove(eq1.SegLR2.point2);
        //    uniquePoints.Remove(eq1.SegLR3.point2);
        //    //（4）判断这个完全四边形是不是生成过的，没生成就生成一个new的
        //    Point p1 = uniquePoints[0];
        //    Point p2 = uniquePoints[1];
        //    Point p3 = uniquePoints[2];


        //    Point p4 = eq1.SegLR1.point2;
        //    Point p5 = eq1.SegLR2.point2;
        //    Point p6 = eq1.SegLR3.point2;

        //    CompleteQuadriliateral pred = CqNormaliza(p1, p2, p3, p4, p5, p6);
        //    pred.AddReason();
        //    pred.AddCondition(eq1);
        //    AddProcessor.Add(pred);

        //    //getter里面取
        //    Line line1 = KnowledgeGetter.GetLine((Point)pred[0], (Point)pred[1], (Point)pred[3]);
        //    Line line2 = KnowledgeGetter.GetLine((Point)pred[1], (Point)pred[2], (Point)pred[4]);
        //    Line line3 = KnowledgeGetter.GetLine((Point)pred[2], (Point)pred[0], (Point)pred[5]);
        //    Line line4 = KnowledgeGetter.GetLine((Point)pred[3], (Point)pred[4], (Point)pred[5]);
        //    if (line1 == null)
        //    {
        //        line1 = new Line((Point)pred[0], (Point)pred[1], (Point)pred[3]);
        //        line1.AddReason();
        //        line1.AddCondition(pred);
        //        AddProcessor.Add(line1);
        //    }

        //    if (line2 == null)
        //    {
        //        line2 = new Line((Point)pred[1], (Point)pred[2], (Point)pred[4]);
        //        line2.AddReason();
        //        line2.AddCondition(pred);
        //        AddProcessor.Add(line2);
        //    }
        //    if (line3 == null)
        //    {
        //        line3 = new Line((Point)pred[2], (Point)pred[0], (Point)pred[5]);
        //        line3.AddReason();
        //        line3.AddCondition(pred);
        //        AddProcessor.Add(line3);
        //    }


        //    if (line4 == null)
        //    {
        //        line4 = new Line((Point)pred[3], (Point)pred[4], (Point)pred[5]);
        //        line4.AddReason();
        //        line4.AddCondition(pred);
        //        AddProcessor.Add(line4);
        //    }

        //}

        //public void 共边定理(SegmentLengthRatio segmentLengthRatio, Triangle triangle, PointWithInPoints pointOnSeg)

        //{
        //    //    //首先确保pointOnSeg是segmentLengthRatio的

        //    //    //比较法，例如AB:AR=1/a，R在AB上 AC:CQ = 1/c,Q在AC上
        //    //    List<Segment> segOfpointOnSeg = new List<Segment>()
        //    //{
        //    //    KnowledgeGetter.GetSegment(pointOnSeg[1], pointOnSeg[1][1]),
        //    //    KnowledgeGetter.GetSegment(pointOnSeg[0], pointOnSeg[1][0]),
        //    //    KnowledgeGetter.GetSegment(pointOnSeg[0], pointOnSeg[1][1])
        //    //};
        //    //    List<Segment> segOfsegmentLengthRatio = new List<Segment>()
        //    //{
        //    //        segmentLengthRatio.Seg1,
        //    //        segmentLengthRatio.Seg2,

        //    //};

        //    //    foreach (Segment line in segOfsegmentLengthRatio)
        //    //    {
        //    //        segOfpointOnSeg.Remove(line);
        //    //    }


        //    //    if (!(segOfpointOnSeg.Count == 1)) return;
        //    //    //再确保PointOnSegment是Triangle的 BCD F在BD中

        //    //    List<Segment> triSeg = new List<Segment>
        //    //{
        //    //    KnowledgeGetter.GetSegment(triangle[0], triangle[1]),
        //    //    KnowledgeGetter.GetSegment(triangle[1], triangle[2]),
        //    //    KnowledgeGetter.GetSegment(triangle[2], triangle[0]),
        //    //};
        //    //    bool flag = false;
        //    //    foreach (Segment line in triSeg)
        //    //    {
        //    //        if (pointOnSeg[1] == line)
        //    //        {
        //    //            flag = true;
        //    //        }
        //    //    }
        //    //    if (!flag) return;

        //    //    List<Point> tripoint = new List<Point>()
        //    //{
        //    //    (Point)triangle[0],(Point)triangle[1],(Point)triangle[2]
        //    //};

        //    //    List<Point> pointOfsegmentLengthRatio1 = new List<Point>()
        //    //{
        //    //     (Point)segmentLengthRatio[0][0],  (Point)segmentLengthRatio[0][1]
        //    //};

        //    //    List<Point> pointOfsegmentLengthRatio2 = new List<Point>()
        //    //{
        //    //     (Point)segmentLengthRatio[1][0],  (Point)segmentLengthRatio[1][1]
        //    //};

        //    //    var expr1 = segmentLengthRatio.Expr.Clone();

        //    //    //BD:FD = 5/3 
        //    //    if (segmentLengthRatio[0] == pointOnSeg[1])//最长的在前
        //    //    {
        //    //        //比例等式调转过来

        //    //        expr1 = expr1.Invert();
        //    //        foreach (Point point in pointOfsegmentLengthRatio1)
        //    //        {
        //    //            tripoint.Remove(point);
        //    //        }
        //    //    }
        //    //    else //FD: BD = 3 / 5
        //    //    {
        //    //        foreach (Point point in pointOfsegmentLengthRatio2)
        //    //        {
        //    //            tripoint.Remove(point);
        //    //        }
        //    //    }


        //    //    if (tripoint.Count != 1) return;
        //    //    //创建三角形
        //    //    Triangle triangle2 = KnowledgeGetter.GetTriangle((Point)segmentLengthRatio[1][0], (Point)segmentLengthRatio[1][1], tripoint[0]);
        //    //    //创建等式
        //    //    if (triangle2 == null) return;
        //    //    //var equation = new GeoEquation(expr1 * triangle2.Area, triangle.Area);

        //    //    //equation.AddCondition(segmentLengthRatio, triangle, pointOnSeg);
        //    //    //equation.AddReason();
        //    //    //AddProcessor.Add(equation);
        //}



        //    public void MenelausTheoremNi(SRE eq1)
        //    {
        //        int index1;
        //        //（1）要先判断这个等式的形式好吧
        //        if (eq1.count != 3) return;




        //        //（2）符合条件后，判断一下是不是只有六个点
        //        List<Point> points = new List<Point>
        //        {
        //            (Point)eq1.SegLR1.Seg1[0],
        //            (Point)eq1.SegLR1.Seg1[1],
        //            (Point)eq1.SegLR1.Seg2[0],
        //            (Point)eq1.SegLR1.Seg2[1],
        //            (Point)eq1.SegLR2.Seg1[0],
        //            (Point)eq1.SegLR2.Seg1[1],
        //            (Point)eq1.SegLR2.Seg2[0],
        //            (Point)eq1.SegLR2.Seg2[1],
        //            (Point)eq1.SegLR3.Seg1[0],
        //            (Point)eq1.SegLR3.Seg1[1],
        //            (Point)eq1.SegLR3.Seg2[0],
        //            (Point)eq1.SegLR3.Seg2[1],

        //        };
        //        List<Point> uniquePoints = points.Distinct().ToList();
        //        if (uniquePoints.Count != 6) return;

        //        //（3）每条等式是不是都有重复的一个点，拿出来
        //        List<Point> points1 = new List<Point>
        //        {
        //            (Point)eq1.SegLR1.Seg1[0],
        //            (Point)eq1.SegLR1.Seg1[1],
        //            (Point)eq1.SegLR1.Seg2[0],
        //            (Point)eq1.SegLR1.Seg2[1],
        //        };

        //        List<Point> points2 = new List<Point>
        //        {
        //            (Point)eq1.SegLR2.Seg1[0],
        //            (Point)eq1.SegLR2.Seg1[1],
        //            (Point)eq1.SegLR2.Seg2[0],
        //            (Point)eq1.SegLR2.Seg2[1],
        //        };

        //        List<Point> points3 = new List<Point>
        //        {
        //            (Point)eq1.SegLR3.Seg1[0],
        //            (Point)eq1.SegLR3.Seg1[1],
        //            (Point)eq1.SegLR3.Seg2[0],
        //            (Point)eq1.SegLR3.Seg2[1],
        //        };

        //        List<Point> duplicates1 = points1
        //.GroupBy(p => p.Name)  // 按 Name 分组
        //.Where(g => g.Count() > 1) // 只取出现次数 >1 的
        //.Select(g => g.First()) // 取出重复的 Point
        //.ToList();
        //        List<Point> duplicates2 = points2
        //.GroupBy(p => p.Name)  // 按 Name 分组
        //.Where(g => g.Count() > 1) // 只取出现次数 >1 的
        //.Select(g => g.First()) // 取出重复的 Point
        //.ToList();
        //        List<Point> duplicates3 = points3
        //.GroupBy(p => p.Name)  // 按 Name 分组
        //.Where(g => g.Count() > 1) // 只取出现次数 >1 的
        //.Select(g => g.First()) // 取出重复的 Point
        //.ToList();

        //        if (duplicates1.Count != 1 || duplicates2.Count != 1 || duplicates3.Count != 1) return;

        //        uniquePoints.Remove(duplicates1[0]);
        //        uniquePoints.Remove(duplicates2[0]);
        //        uniquePoints.Remove(duplicates3[0]);
        //        //（4）判断这个完全四边形是不是生成过的，没生成就生成一个new的
        //        Point p1 = uniquePoints[0];
        //        Point p2 = uniquePoints[1];
        //        Point p3 = uniquePoints[2];
        //        Point p4 = duplicates1[0];
        //        Point p5 = duplicates2[0];
        //        Point p6 = duplicates3[0];

        //        CompleteQuadriliateral pred = CqNormaliza(p1, p2, p3, p4, p5, p6);
        //        pred.AddReason();
        //        pred.AddCondition(eq1);
        //        AddProcessor.Add(pred);

        //        //getter里面取
        //        Line line1 = KnowledgeGetter.GetLine((Point)pred[0], (Point)pred[1], (Point)pred[3]);
        //        Line line2 = KnowledgeGetter.GetLine((Point)pred[1], (Point)pred[2], (Point)pred[4]);
        //        Line line3 = KnowledgeGetter.GetLine((Point)pred[2], (Point)pred[0], (Point)pred[5]);
        //        Line line4 = KnowledgeGetter.GetLine((Point)pred[3], (Point)pred[4], (Point)pred[5]);
        //        if (line1 == null)
        //        {
        //            line1 = new Line((Point)pred[0], (Point)pred[1], (Point)pred[3]);
        //            line1.AddReason();
        //            line1.AddCondition(pred);
        //            AddProcessor.Add(line1);
        //        }

        //        if (line2 == null)
        //        {
        //            line2 = new Line((Point)pred[1], (Point)pred[2], (Point)pred[4]);
        //            line2.AddReason();
        //            line2.AddCondition(pred);
        //            AddProcessor.Add(line2);
        //        }
        //        if (line3 == null)
        //        {
        //            line3 = new Line((Point)pred[2], (Point)pred[0], (Point)pred[5]);
        //            line3.AddReason();
        //            line3.AddCondition(pred);
        //            AddProcessor.Add(line3);
        //        }
        //        if (eq1.Properties[0].Properties[0].ToString() == "CQ" &&
        //       eq1.Properties[0].Properties[1].ToString() == "AQ")
        //        {
        //            index1 = 1;
        //        }

        //        if (line4 == null)
        //        {
        //            line4 = new Line((Point)pred[3], (Point)pred[4], (Point)pred[5]);
        //            line4.AddReason();
        //            line4.AddCondition(pred);
        //            AddProcessor.Add(line4);
        //        }

        //    }
        //    public void 根据线段比例得出同一直线的另一线段比例(SegmentLengthRatio segmentLengthRatio, PointWithInPoints pointOnSeg)
        //    {
        //        //首先确保pointOnSeg是segmentLengthRatio的

        //        //比较法，例如AB:AR=1/a，R在AB上 AC:CQ = 1/c,Q在AC上
        //        List<Segment> segOfpointOnSeg = new List<Segment>()
        //    {
        //        KnowledgeGetter.GetSegment(pointOnSeg[1][0], pointOnSeg[1][1]),
        //        KnowledgeGetter.GetSegment(pointOnSeg[0], pointOnSeg[1][0]),
        //        KnowledgeGetter.GetSegment(pointOnSeg[0], pointOnSeg[1][1])
        //    };
        //        List<Segment> segOfsegmentLengthRatio = new List<Segment>()
        //    {
        //        KnowledgeGetter.GetSegment(segmentLengthRatio[0][0], segmentLengthRatio[0][1]),
        //        KnowledgeGetter.GetSegment(segmentLengthRatio[1][0], segmentLengthRatio[1][1])
        //};

        //        foreach (Segment line in segOfsegmentLengthRatio)
        //        {
        //            segOfpointOnSeg.Remove(line);
        //        }
        //        if (!(segOfpointOnSeg.Count == 1)) return;
        //        Segment longestseg = KnowledgeGetter.GetSegment(pointOnSeg[1][0], pointOnSeg[1][1]);
        //        var expr1 = segmentLengthRatio.Expr.Clone();
        //        Expr expr4;
        //        //BR：AB = 1-a   AQ:AC= 1-c  
        //        //如果最长的线段在第一位,那就要倒过来
        //        if (segOfsegmentLengthRatio[0] == pointOnSeg[1])
        //        {

        //            expr4 = expr1.Invert();
        //            expr1 = expr4;

        //        }
        //        else if (segOfsegmentLengthRatio[1] == pointOnSeg[1])//最长的线段在第二位，不需要倒过来
        //        {

        //            //Expr expr3 = 1 - expr1;//生成1-a
        //            //Segment longestseg = KnowledgeGetter.GetSegment(pointOnSeg[1][0], pointOnSeg[1][1]);
        //            //SegmentLengthRatio pred = new SegmentLengthRatio(segOfpointOnSeg[0], (Segment)pointOnSeg[1], expr3);
        //            //pred.AddCondition(segmentLengthRatio, pointOnSeg);
        //            //pred.AddReason();
        //            //AddProcessor.AddNew(pred);
        //            //var equation = segOfpointOnSeg[0].Length / longestseg.Length == expr3.Value;
        //            //equation.AddCondition(segmentLengthRatio, pointOnSeg);
        //            //equation.AddReason();
        //            //Cal.AddEquation(equation);
        //        }
        //        else //没有最长的线段，这个要生成2个
        //        {

        //            //找到最长的线段 longestseg
        //            //先取第一条线和最长的比 segOfsegmentLengthRatio
        //            var expr5 = 1 + expr1;
        //            expr4 = expr5.Invert();
        //            SegmentLengthRatio pred2 = new SegmentLengthRatio(segOfsegmentLengthRatio[1], longestseg, expr4);
        //            pred2.AddCondition(segmentLengthRatio, pointOnSeg);
        //            pred2.AddReason();
        //            AddProcessor.Add(pred2);
        //            //var equation1 = new GeoEquation(segOfsegmentLengthRatio[1].Length / longestseg.Length, expr4);
        //            //equation1.AddCondition(segmentLengthRatio, pointOnSeg);
        //            //equation1.AddReason();
        //            //AddProcessor.Add(equation1);

        //            //var expr6 = 1 - expr4;
        //            //SegmentLengthRatio pred3 = new SegmentLengthRatio(segOfsegmentLengthRatio[0], longestseg, expr6);
        //            //pred3.AddCondition(segmentLengthRatio, pointOnSeg);
        //            //pred3.AddReason();
        //            //AddProcessor.Add(pred3);
        //            //var equation2 = new GeoEquation(segOfsegmentLengthRatio[0].Length / longestseg.Length, expr6);
        //            //equation2.AddCondition(segmentLengthRatio, pointOnSeg);
        //            //equation2.AddReason();
        //            //AddProcessor.Add(equation2);
        //            return;
        //        }
        //        Expr expr3 = 1 - expr1;//生成1-a

        //        SegmentLengthRatio pred = new SegmentLengthRatio(segOfpointOnSeg[0], (Segment)pointOnSeg[1], expr3);
        //        pred.AddCondition(segmentLengthRatio, pointOnSeg);
        //        pred.AddReason();
        //        AddProcessor.Add(pred);
        //        //var equation = new GeoEquation(segOfpointOnSeg[0].Length / longestseg.Length, expr3);
        //        //equation.AddCondition(segmentLengthRatio, pointOnSeg);
        //        //equation.AddReason();
        //        //AddProcessor.Add(equation);
        //    }
        //}



        //[Combination]
        //public void 判断完全四边形新(LineIntersection lineIntersectionPoint1, LineIntersection lineIntersectionPoint2)
        //{


        //    //首先判断两条相交知识所包含的四点是不是一致的，拿点集出来
        //    HashSet<Point> intersectionpointList1 = new HashSet<Point> {
        //    (Point)lineIntersectionPoint1[1][0],
        //   (Point)lineIntersectionPoint1[1][1],
        //   (Point)lineIntersectionPoint1[2][0],
        //   (Point)lineIntersectionPoint1[2][1],
        //};

        //    HashSet<Point> intersectionpointList2 = new HashSet<Point> {
        //  (Point)lineIntersectionPoint2[1][0],
        //  (Point)lineIntersectionPoint2[1][1],
        // (Point)lineIntersectionPoint2[2][0],
        // (Point)lineIntersectionPoint2[2][1],
        //};

        //    bool isEqual1 = intersectionpointList1.SetEquals(intersectionpointList2);
        //    if (!isEqual1) return;


        //    //接着判断两条相交知识所包含的交点是不是一致的，不一致就继续
        //    if (lineIntersectionPoint1[0] == lineIntersectionPoint2[0]) return;

        //    //判断完就开始赋点，六个点的位置都确定了
        //    Point p1 = (Point)lineIntersectionPoint1[0];
        //    Point p5 = (Point)lineIntersectionPoint2[0];
        //    Point p2 = (Point)lineIntersectionPoint2[1][0];
        //    Point p3 = (Point)lineIntersectionPoint2[1][1];
        //    Point p4 = (Point)lineIntersectionPoint2[2][0];
        //    Point p6 = (Point)lineIntersectionPoint2[2][1];

        //    //判断1 2 4是否共线，不共线就把4和6换位置
        //    if (!(KnowledgeGetter.HasColine(p1, p2, p4)))
        //    {
        //        Point temp = p4;
        //        p4 = p6;
        //        p6 = temp;
        //    }

        //    CompleteQuadriliateral pred = new CompleteQuadriliateral(p1, p2, p3, p4, p5, p6);
        //    pred.AddReason();
        //    pred.AddCondition(lineIntersectionPoint1, lineIntersectionPoint2);
        //    AddProcessor.Add(pred);

        //    CompleteQuadriliateral pred2 = CqNormaliza(p3, p5, p6, p1, p2, p4);

        //    pred2.AddReason();
        //    pred2.AddCondition(lineIntersectionPoint1, lineIntersectionPoint2);
        //    AddProcessor.Add(pred2);

        //    CompleteQuadriliateral pred3 = CqNormaliza(p1, p4, p6, p2, p3, p5);
        //    pred3.AddReason();
        //    pred3.AddCondition(lineIntersectionPoint1, lineIntersectionPoint2);
        //    AddProcessor.Add(pred3);

        //    CompleteQuadriliateral pred4 = CqNormaliza(p2, p4, p5, p3, p1, p6);
        //    pred4.AddReason();
        //    pred4.AddCondition(lineIntersectionPoint1, lineIntersectionPoint2);
        //    AddProcessor.Add(pred4);

        //}



        //[Combination]
        //public void 判断完全四边形(PointWithInPoints pointOnSegment1, PointWithInPoints pointOnSegment2,
        //PointWithInPoints pointOnSegment3, PointWithInPoints pointOnSegment4)
        //{
        //    //先去重复的点
        //    List<Point> intersectionpoints = new List<Point>
        //    {
        //        (Point)pointOnSegment1[0],
        //        (Point)pointOnSegment1[1],
        //        (Point)pointOnSegment1[2],
        //        (Point)pointOnSegment2[0],
        //        (Point)pointOnSegment2[1],
        //        (Point)pointOnSegment2[2],
        //        (Point)pointOnSegment3[0],
        //        (Point)pointOnSegment3[1],
        //        (Point)pointOnSegment3[2],
        //        (Point)pointOnSegment4[0],
        //        (Point)pointOnSegment4[1],
        //        (Point)pointOnSegment4[2],
        //    };
        //    //检查是否六个不重复的交点
        //    HashSet<Point> uniquepoints = new HashSet<Point>(intersectionpoints);
        //    if (!(uniquepoints.Count == 6)) return;
        //    List<Point> point = new List<Point>(uniquepoints);//转换

        //    //获取6点列表所有的4点组合，如果有共线就退出
        //    List<List<Point>> combinations = GetCombinations(point, 4);
        //    static List<List<Point>> GetCombinations<Point>(List<Point> list, int length)
        //    {
        //        List<List<Point>> result = new List<List<Point>>();
        //        int[] indices = new int[length];
        //        for (int i = 0; i < length; i++) indices[i] = i;

        //        while (indices[0] < list.Count - length + 1)
        //        {
        //            List<Point> combination = new List<Point>();
        //            for (int i = 0; i < length; i++)
        //            {
        //                combination.Add(list[indices[i]]);
        //            }
        //            result.Add(combination);

        //            int t = length - 1;
        //            while (t != 0 && indices[t] == list.Count - length + t) t--;
        //            indices[t]++;
        //            for (int i = t + 1; i < length; i++) indices[i] = indices[i - 1] + 1;
        //        }

        //        return result;
        //    }

        //    foreach (List<Point> combination in combinations)
        //    {
        //        if (KnowledgeGetter.HasColine(combination[0], combination[1], combination[2], combination[3]))
        //        {
        //            return;
        //        }
        //    }
        //    //4个在两点之间的点，出现两次的就是cross
        //    List<Point> betweenpoints = new List<Point>
        //    {
        //        (Point)pointOnSegment1[0],
        //        (Point)pointOnSegment2[0],
        //        (Point)pointOnSegment3[0],
        //        (Point)pointOnSegment4[0],

        //    };
        //    // Point cross = betweenpoints.GroupBy(x => x).Where(g => g.Count() == 2).Select(g => g.Key).SingleOrDefault();
        //    List<Point> crosss = betweenpoints.GroupBy(x => x).Where(g => g.Count() == 2).Select(g => g.Key).ToList();
        //    if (crosss.Count != 1) return;
        //    Point cross = crosss[0];



        //    List<PointWithInPoints> Pbs = new()
        //{
        //    pointOnSegment1,pointOnSegment2,pointOnSegment3,pointOnSegment4
        //};
        //    List<PointWithInPoints> Pbs2 = new List<PointWithInPoints>();
        //    foreach (PointWithInPoints pos in Pbs)
        //    {
        //        if (pos[0] == cross)
        //        {
        //            Pbs2.Add(pos);
        //        }
        //    }
        //    foreach (PointWithInPoints pos in Pbs2)
        //    {
        //        Pbs.Remove(pos);
        //    }

        //    if (Pbs.Count != 2) return;

        //    List<Point> topPbs = new List<Point>
        //    {
        //        (Point)Pbs[0][0],
        //        (Point)Pbs[0][1],
        //        (Point)Pbs[0][2],
        //        (Point)Pbs[1][0],
        //        (Point)Pbs[1][1],
        //        (Point)Pbs[1][2],
        //    };
        //    //Point top = topPbs.GroupBy(x => x).Where(g => g.Count() == 2).Select(g => g.Key).SingleOrDefault();
        //    List<Point> tops = topPbs.GroupBy(x => x).Where(g => g.Count() == 2).Select(g => g.Key).ToList();
        //    if (tops.Count != 1) return;
        //    Point top = tops[0];
        //    if (top is null) return;
        //    topPbs.Remove(top);
        //    topPbs.Remove(top);
        //    Point leftarm = topPbs[0];
        //    Point righttarm = topPbs[2];
        //    topPbs.Remove(leftarm);
        //    topPbs.Remove(righttarm);
        //    Point LeftFoot = topPbs[0];
        //    Point rightFoot = topPbs[1];



        //    CompleteQuadriliateral pred = new CompleteQuadriliateral(top, LeftFoot, righttarm, leftarm, cross, rightFoot);
        //    pred.AddReason();
        //    pred.AddCondition(pointOnSegment1, pointOnSegment2, pointOnSegment3, pointOnSegment4);
        //    AddProcessor.Add(pred);

        //}
    }
}
