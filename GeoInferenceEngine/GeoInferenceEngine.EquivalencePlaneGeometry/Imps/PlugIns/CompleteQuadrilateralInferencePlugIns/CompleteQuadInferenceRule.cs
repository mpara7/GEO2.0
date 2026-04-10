using System;
using System.Linq;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.PRs.PairMakers;
using GeoInferenceEngine.EquivalencePlaneGeometry.Models.Exprs.ZExprs;
using GeoInferenceEngine.PlaneKnowledges.Knowledges;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using System.Reflection;
using GeoInferenceEngine.EquivalencePlaneGeometry.PRs.Predicates.Relations.GeoPropRelations;
using GeoInferenceEngine.PredicateShared.Models;



namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.PlugIns.CompleteQuadrilateralInferencePlugIns
{
    public class CompleteQuadInferenceRule : PlaneRuleClass
    {
        [ZDI]
        public TargetBase _targetBase { get; set; }
        public CompleteQuadInferenceRule()
        {
            
        }
        // 1. 在类的顶部（或系统的知识库上下文中）定义一个全局哈希墙
        // 用于记录已经生成过的完全四边形的唯一拓扑指纹
        private static HashSet<string> _globalCQHashSet = new HashSet<string>();

        // 2. 编写一个生成唯一指纹的方法（底层代数化简的轻量级替代）
        private string GetCanonicalHash(Point p1, Point p2, Point p3, Point p4, Point p5, Point p6)
        {
            // 将 6 个顶点放入列表
            List<string> pointNames = new List<string> {
        p1.Name, p2.Name, p3.Name, p4.Name, p5.Name, p6.Name
    };

            // 核心算法：字典序重排 (Lexicographical Sorting)
            // 无论是 CEBAFD 还是 BDFCAE，排序后都会变成 A B C D E F
            pointNames.Sort();

            // 拼接成唯一指纹，例如 "A_B_C_D_E_F"
            return string.Join("_", pointNames);
        }
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

            // 【新增防御 1】：确保这 4 条线是 4 条截然不同的线！不能有重合。
            if (line1_1 == line2_1 || line1_1 == line2_2 || line1_2 == line2_1 || line1_2 == line2_2) return;

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

            // 【新增防御 2（核心）】：完全四边形的 6 个顶点必须完全不同！
            // 只要有任何两个点重合，说明必定发生了“三线共点”，立刻抛弃！
            var uniquePoints = new HashSet<Point> { p1, p2, p3, p4, p5, p6 };
            if (uniquePoints.Count != 6) return;

            //判断1 2 4是否共线，不共线就把4和6换位置
            if (!(KnowledgeGetter.HasColine(p1, p2, p4)))
            {
                Point temp = p4;
                p4 = p6;
                p6 = temp;
            }

            CompleteQuadriliateral pred = CqNormaliza(p1, p2, p3, p4, p5, p6);
            pred.AddReason();
            pred.AddCondition(lineIntersectionPoint1, lineIntersectionPoint2);
            AddProcessor.Add(pred);

            //CompleteQuadriliateral pred2 = CqNormaliza(p3, p5, p6, p1, p2, p4);

            //pred2.AddReason();
            //pred2.AddCondition(lineIntersectionPoint1, lineIntersectionPoint2);
            //AddProcessor.Add(pred2);

            //CompleteQuadriliateral pred3 = CqNormaliza(p1, p4, p6, p2, p3, p5);
            //pred3.AddReason();
            //pred3.AddCondition(lineIntersectionPoint1, lineIntersectionPoint2);
            //AddProcessor.Add(pred3);

            //CompleteQuadriliateral pred4 = CqNormaliza(p2, p4, p5, p3, p1, p6);
            //pred4.AddReason();
            //pred4.AddCondition(lineIntersectionPoint1, lineIntersectionPoint2);
            //AddProcessor.Add(pred4);
        }
        [Combination]
        public CompleteQuadriliateral CqNormaliza(Point p1, Point p2, Point p3, Point p4, Point p5, Point p6)
        {
            // === 新增功能：对前三点进行按字典顺序（字母顺序）排序 ===
            // 这里使用 ToString() 获取点名进行比较，如果你的 Point 类是用 .Name 属性表示名字，
            // 请将 p1.ToString() 替换为 p1.Name
            if (string.Compare(p1.ToString(), p2.ToString()) > 0)
            {
                Point temp = p1; p1 = p2; p2 = temp;
            }
            if (string.Compare(p2.ToString(), p3.ToString()) > 0)
            {
                Point temp = p2; p2 = p3; p3 = temp;
            }
            if (string.Compare(p1.ToString(), p2.ToString()) > 0)
            {
                Point temp = p1; p1 = p2; p2 = temp;
            }
            // ========================================================

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



        public void 梅涅劳斯定理(CompleteQuadriliateral cq)
        {
            // 1. 安全校验：如果当前没有 TargetBase 或者没有目标，直接返回
            if (_targetBase == null) return;

            // 2. 遍历所有的目标 (假设你的 TargetBase 里面存放列表的属性叫 Targets 或类似名称，请根据实际属性名微调)
            // 通常在原系统中可能是 _targetBase.Targets 或者 _targetBase.GetTargets()
            // 这里以一个通用的获取列表方式为例
            foreach (var target in _targetBase.ToSolves)
            {
                // 3. 检查该目标是否已经证明成功了，如果成功了就跳过，节约算力
                if (target.IsSuccess) continue;

                // 4. 【核心防御：动态反射探针】
                // 获取当前 target 在运行时的真实子类类型，并探测它有没有叫 "Target" 的属性
                PropertyInfo propInfo = target.GetType().GetProperty("Target");

                // 如果这个目标对象没有 Target 属性（说明它不是我们要的类型），直接跳过
                if (propInfo == null) continue;
                // 动态提取出 Target 属性的值
                object toSolveProp = propInfo.GetValue(target);

                // 5. 运行时类型断言 (RTTI)：判断待证结论是否为共线线段比 (SLR)
                if (toSolveProp is SLR targetSlr)
                {
                    // 提取待求线段比的三个点 (例如结论是 AF/FD)
                    Point pA = (Point)targetSlr.point1;
                    Point pF = (Point)targetSlr.point2; // 截点
                    Point pD = (Point)targetSlr.point3;

                    // 防御校验：判断这三个点是否全部在当前完全四边形的 6 个顶点中
                    HashSet<Point> cqPoints = new HashSet<Point> {
                    (Point)cq[0], (Point)cq[1], (Point)cq[2],
                    (Point)cq[3], (Point)cq[4], (Point)cq[5]
                     };
                    if (!cqPoints.Contains(pA) || !cqPoints.Contains(pF) || !cqPoints.Contains(pD)) return;

                    // 提取完全四边形的 4 条线
                    List<HashSet<Point>> lines = new List<HashSet<Point>>
                    {
                        new HashSet<Point> { (Point)cq[0], (Point)cq[3], (Point)cq[1] },
                        new HashSet<Point> { (Point)cq[1], (Point)cq[4], (Point)cq[2] },
                        new HashSet<Point> { (Point)cq[2], (Point)cq[5], (Point)cq[0] },
                        new HashSet<Point> { (Point)cq[3], (Point)cq[4], (Point)cq[5] }
    };
                    // --- 严格落实你的算法步骤 ---

                    // (1) 确定目标线 (A,F,D 所在的线) 和 截线 (过 F，但不过 A、D 的线)
                    HashSet<Point> baseLine = lines.FirstOrDefault(l => l.Contains(pA) && l.Contains(pF) && l.Contains(pD));
                    HashSet<Point> transversalLine = lines.FirstOrDefault(l => l.Contains(pF) && !l.Contains(pA) && !l.Contains(pD));

                    if (baseLine == null || transversalLine == null) return;

                    // (2) 确定哪个三角形被哪条线所截：寻找第 3 个顶点 C
                    // 逻辑：在 6 个点中，剔除基线AD上的点，剔除截线BEF上的点，剩下的那个孤立点绝对是 C
                    Point pC = cqPoints.First(p => !baseLine.Contains(p) && !transversalLine.Contains(p));

                    // 找到截线上的另外两个截点 (B 和 E)
                    var otherTransversalPoints = transversalLine.Where(p => p != pF).ToList();
                    Point pCut1 = otherTransversalPoints[0];
                    Point pCut2 = otherTransversalPoints[1];

                    // 将截点正确匹配到边 DC 和 CA 上
                    Point cutOnDC = lines.First(l => l.Contains(pD) && l.Contains(pC)).First(p => p == pCut1 || p == pCut2);
                    Point cutOnCA = lines.First(l => l.Contains(pA) && l.Contains(pC)).First(p => p == pCut1 || p == pCut2);

                    // 组装最终唯一的等式： (A->F->D) * (D->CutOnDC->C) * (C->CutOnCA->A) = 1
                    SLR ratioDC = KnowledgeGetter.GetSegmentLengthRatio1(pD, cutOnDC, pC);
                    SLR ratioCA = KnowledgeGetter.GetSegmentLengthRatio1(pC, cutOnCA, pA);
                    Expr o = 1;
                    SREE preciseEquation = new SREE(o, targetSlr, ratioDC, ratioCA); // 极度精准，只生成 1 个！
                    preciseEquation.AddReason();
                    preciseEquation.AddCondition(cq);
                    AddProcessor.Add(preciseEquation);

                    return;
                }
            }
        
                    // 提取完全四边形的 6 个交点
                    // 根据原有逻辑定义，这 6 个点构成的 4 条共线直线分别为：
                    // L1: p0, p3, p1
                    // L2: p1, p4, p2
                    // L3: p2, p5, p0
                    // L4: p3, p4, p5
                    Point p0 = (Point)cq[0];
            Point p1 = (Point)cq[1];
            Point p2 = (Point)cq[2];
            Point p3 = (Point)cq[3];
            Point p4 = (Point)cq[4];
            Point p5 = (Point)cq[5];

            Expr one = 1;

            // =========================================================
            // 第 1 组：以 L4 (p3, p4, p5) 为截线，截三角形 (p0, p1, p2)
            // =========================================================
            SLR eq1_r1 = KnowledgeGetter.GetSegmentLengthRatio1(p0, p3, p1);
            SLR eq1_r2 = KnowledgeGetter.GetSegmentLengthRatio1(p1, p4, p2);
            SLR eq1_r3 = KnowledgeGetter.GetSegmentLengthRatio1(p2, p5, p0);

            SREE equation1 = new SREE(one, eq1_r1, eq1_r2, eq1_r3);
            //equation1.AddReason();
            //equation1.AddCondition(cq);
            //AddProcessor.Add(equation1);

            GeoEquation geq = equation1.ToGeoEquation();
            geq.AddReason();
            AddProcessor.Add(geq);

            // =========================================================
            // 第 2 组：以 L1 (p0, p3, p1) 为截线，截三角形 (p2, p5, p4)
            // =========================================================
            SLR eq2_r1 = KnowledgeGetter.GetSegmentLengthRatio1(p2, p0, p5);
            SLR eq2_r2 = KnowledgeGetter.GetSegmentLengthRatio1(p5, p3, p4);
            SLR eq2_r3 = KnowledgeGetter.GetSegmentLengthRatio1(p4, p1, p2);

            SREE equation2 = new SREE(one, eq2_r1, eq2_r2, eq2_r3);
            //equation2.AddReason();
            //equation2.AddCondition(cq);
            //AddProcessor.Add(equation2);

            GeoEquation geq2 = equation2.ToGeoEquation();
            geq.AddReason();
            AddProcessor.Add(geq2);


            // =========================================================
            // 第 3 组：以 L2 (p1, p4, p2) 为截线，截三角形 (p0, p1, p3) 
            // 严格拓扑闭环应为三角形 (p0, p3, p5)
            // =========================================================
            SLR eq3_r1 = KnowledgeGetter.GetSegmentLengthRatio1(p0, p1, p3);
            SLR eq3_r2 = KnowledgeGetter.GetSegmentLengthRatio1(p3, p4, p5);
            SLR eq3_r3 = KnowledgeGetter.GetSegmentLengthRatio1(p5, p2, p0);

            SREE equation3 = new SREE(one, eq3_r1, eq3_r2, eq3_r3);
            //equation3.AddReason();
            //equation3.AddCondition(cq);
            //AddProcessor.Add(equation3);

            GeoEquation geq3 = equation3.ToGeoEquation();
            geq.AddReason();
            AddProcessor.Add(geq3);

            // =========================================================
            // 第 4 组：以 L3 (p2, p5, p0) 为截线，截三角形 (p1, p4, p3)
            // =========================================================
            SLR eq4_r1 = KnowledgeGetter.GetSegmentLengthRatio1(p1, p2, p4);
            SLR eq4_r2 = KnowledgeGetter.GetSegmentLengthRatio1(p4, p5, p3);
            SLR eq4_r3 = KnowledgeGetter.GetSegmentLengthRatio1(p3, p0, p1);

            SREE equation4 = new SREE(one, eq4_r1, eq4_r2, eq4_r3);
            //equation4.AddReason();
            //equation4.AddCondition(cq);
            //AddProcessor.Add(equation4);

            GeoEquation geq4 = equation4.ToGeoEquation();
            geq.AddReason();
            AddProcessor.Add(geq4);
            //SLR ABBC = KnowledgeGetter.GetSegmentLengthRatio1((Point)completeQuadriliateral[0], (Point)completeQuadriliateral[3], (Point)completeQuadriliateral[1]);
            //SLR CDDF = KnowledgeGetter.GetSegmentLengthRatio1((Point)completeQuadriliateral[1], (Point)completeQuadriliateral[4], (Point)completeQuadriliateral[2]);
            //SLR FEEA = KnowledgeGetter.GetSegmentLengthRatio1((Point)completeQuadriliateral[2], (Point)completeQuadriliateral[5], (Point)completeQuadriliateral[0]);

            ////创建等式，创建一个新知识
            //Expr i = 1;
            //SREE equation = new SREE(i, ABBC, CDDF, FEEA);
            //equation.AddReason();
            //equation.AddCondition(completeQuadriliateral);
            //AddProcessor.Add(equation);


        }

        public void 梅涅劳斯逆定理(SREE eq1)
        {

            //（1）要先判断这个等式的形式好吧
            if (eq1.count != 3) return;

            //（2）符合条件后，判断一下是不是只有六个点
            List<Point> points = new List<Point>
            {
                eq1.SegLR1.point1,
                eq1.SegLR1.point2,
                eq1.SegLR1.point3,

                eq1.SegLR2.point1,
                eq1.SegLR2.point2,
                eq1.SegLR2.point3,

                eq1.SegLR3.point1,
                eq1.SegLR3.point2,
                eq1.SegLR3.point3,


            };
            List<Point> uniquePoints = points.Distinct().ToList();
            if (uniquePoints.Count != 6) return;

            //（3）每条等式是不是都有重复的一个点，拿出来


            uniquePoints.Remove(eq1.SegLR1.point2);
            uniquePoints.Remove(eq1.SegLR2.point2);
            uniquePoints.Remove(eq1.SegLR3.point2);
            //（4）判断这个完全四边形是不是生成过的，没生成就生成一个new的
            Point p1 = uniquePoints[0];
            Point p2 = uniquePoints[1];
            Point p3 = uniquePoints[2];


            Point p4 = eq1.SegLR1.point2;
            Point p5 = eq1.SegLR2.point2;
            Point p6 = eq1.SegLR3.point2;

            string hashFingerprint = GetCanonicalHash(p1, p2, p3, p4, p5, p6);

            // 【防爆熔断机制】如果哈希池中已经存在该指纹，说明是同构冗余，直接丢弃！
            if (_globalCQHashSet.Contains(hashFingerprint))
            {
                return; // 状态空间坍缩，终止发散
            }

            // 如果是首次发现，将指纹打入哈希表
            _globalCQHashSet.Add(hashFingerprint);
            CompleteQuadriliateral pred = CqNormaliza(p1, p2, p3, p4, p5, p6);
            pred.AddReason();
            pred.AddCondition(eq1);
            AddProcessor.Add(pred);

            //getter里面取
            Line line1 = KnowledgeGetter.GetLine((Point)pred[0], (Point)pred[1], (Point)pred[3]);
            Line line2 = KnowledgeGetter.GetLine((Point)pred[1], (Point)pred[2], (Point)pred[4]);
            Line line3 = KnowledgeGetter.GetLine((Point)pred[2], (Point)pred[0], (Point)pred[5]);
            Line line4 = KnowledgeGetter.GetLine((Point)pred[3], (Point)pred[4], (Point)pred[5]);
            if (line1 == null)
            {
                line1 = new Line((Point)pred[0], (Point)pred[1], (Point)pred[3]);
                line1.AddReason();
                line1.AddCondition(pred);
                AddProcessor.Add(line1);
            }

            if (line2 == null)
            {
                line2 = new Line((Point)pred[1], (Point)pred[2], (Point)pred[4]);
                line2.AddReason();
                line2.AddCondition(pred);
                AddProcessor.Add(line2);
            }
            if (line3 == null)
            {
                line3 = new Line((Point)pred[2], (Point)pred[0], (Point)pred[5]);
                line3.AddReason();
                line3.AddCondition(pred);
                AddProcessor.Add(line3);
            }


            if (line4 == null)
            {
                line4 = new Line((Point)pred[3], (Point)pred[4], (Point)pred[5]);
                line4.AddReason();
                line4.AddCondition(pred);
                AddProcessor.Add(line4);
            }

        }
        public void 比例式化简(SREE equation1, SLR segmentLengthRatio1)
        {



            if (equation1.count == 4)
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
                SLR segmentLengthRatio = KnowledgeGetter.GetSegmentLengthRatio1((Point)pequtaion[0].Properties[0], (Point)pequtaion[0].Properties[1], (Point)pequtaion[0].Properties[2]);
                if (segmentLengthRatio.Expr == 99 || segmentLengthRatio.Expr == expr)
                {
                    segmentLengthRatio.Expr = value;
                    segmentLengthRatio.AddReason();
                    segmentLengthRatio.AddCondition(equation1, segmentLengthRatio1);
                    AddProcessor.Add(segmentLengthRatio);
                }


            }


        }

        public void 圆幂定理(Circle circle, LineIntersectionPoint cross)
        {
            Point p = (Point)cross[0];
            Line line1 = (Line)cross[1];
            Line line2 = (Line)cross[2];

            if (line1 == line2) return;

            if (!TryGetSecantEnds(circle, line1, p, out Point a, out Point b, out bool inner1)) return;
            if (!TryGetSecantEnds(circle, line2, p, out Point c, out Point d, out bool inner2)) return;

            Segment pa = KnowledgeGetter.GetSegment(p, a);
            Segment pb = KnowledgeGetter.GetSegment(p, b);
            Segment pc = KnowledgeGetter.GetSegment(p, c);
            Segment pd = KnowledgeGetter.GetSegment(p, d);

            if (pa is null || pb is null || pc is null || pd is null) return;

            // 统一计算格式：左边乘除式 = 1
            GeoEquation pred = new GeoEquation(
                pa.Length.Mul(pb.Length).Div(pc.Length).Div(pd.Length).Simplify(),
                Expr.One
            );
            pred.AddReason();
            pred.AddCondition(circle, cross);
            AddProcessor.Add(pred);

            // 只有内部点情形，才额外补一条 2 因子 SREE
            if (inner1 && inner2)
            {
                SLR r1 = KnowledgeGetter.GetSegmentLengthRatio1(a, p, b);
                SLR r2 = KnowledgeGetter.GetSegmentLengthRatio1(d, p, c);

                SREE sree = new SREE(Expr.One, r1, r2);
                sree.AddReason();
                sree.AddCondition(circle, cross);
                AddProcessor.Add(sree);

                GeoEquation geq = sree.ToGeoEquation();
                geq.AddReason();
                AddProcessor.Add(geq);
            }
        }
        private bool TryGetSecantEnds(
                Circle circle, Line line, Point p,
                out Point end1, out Point end2,
                out bool isInnerPoint)
        {
            end1 = null;
            end2 = null;
            isInnerPoint = false;

            HashSet<ulong> circlePointHashCodes = circle.Properties
                .Skip(1)
                .OfType<Point>()
                .Select(x => x.HashCode)
                .ToHashSet();

            List<Point> secantPoints = line.Properties
                .OfType<Point>()
                .Where(x => x.HashCode != p.HashCode && circlePointHashCodes.Contains(x.HashCode))
                .GroupBy(x => x.HashCode)
                .Select(g => g.First())
                .ToList();

            if (secantPoints.Count != 2) return false;

            int indexP = IndexOfPoint(line, p);
            int index1 = IndexOfPoint(line, secantPoints[0]);
            int index2 = IndexOfPoint(line, secantPoints[1]);

            if (indexP < 0 || index1 < 0 || index2 < 0) return false;

            isInnerPoint = Math.Min(index1, index2) < indexP && indexP < Math.Max(index1, index2);

            end1 = secantPoints[0];
            end2 = secantPoints[1];
            return true;
        }

        //public void 相交弦定理(Circle circle, LineIntersectionPoint cross)
        //{
        //    Point p = (Point)cross[0];
        //    Line line1 = (Line)cross[1];
        //    Line line2 = (Line)cross[2];

        //    if (line1 == line2) return;

        //    // line1 上找该圆的两个弦端点，并判断 p 是否在内部
        //    if (!TryGetChordEnds(circle, line1, p, out Point a, out Point b)) return;

        //    // line2 上找该圆的两个弦端点，并判断 p 是否在内部
        //    if (!TryGetChordEnds(circle, line2, p, out Point c, out Point d)) return;

        //    // 五个点必须互异，排掉退化情况
        //    if (new HashSet<ulong> { a.HashCode, b.HashCode, c.HashCode, d.HashCode, p.HashCode }.Count != 5)
        //        return;

        //    Segment ap = KnowledgeGetter.GetSegment(a, p);
        //    Segment pb = KnowledgeGetter.GetSegment(p, b);
        //    Segment cp = KnowledgeGetter.GetSegment(c, p);
        //    Segment pd = KnowledgeGetter.GetSegment(p, d);

        //    if (ap is null || pb is null || cp is null || pd is null) return;

        //    GeoEquation pred = new GeoEquation(
        //       ap.Length.Mul(pb.Length).Div(cp.Length).Div(pd.Length).Simplify(), Expr.One
        //    );

        //    pred.AddCondition("相交弦定理", circle, cross);
        //    AddProcessor.Add(pred);
        //}
        /// <summary>
        /// 在给定圆和直线中，找出该直线与圆对应的两个圆上点，
        /// 并且要求 crossPoint 在线上夹在这两个点之间
        /// </summary>
        //private bool TryGetChordEnds(Circle circle, Line line, Point crossPoint, out Point end1, out Point end2)
        //{
        //    end1 = null;
        //    end2 = null;

        //    // circle.Properties[0] 默认是圆心，后面才是圆上点
        //    HashSet<ulong> circlePointHashCodes = circle.Properties
        //        .Skip(1)
        //        .OfType<Point>()
        //        .Select(x => x.HashCode)
        //        .ToHashSet();

        //    // 取这条线上、又在该圆上的点，去掉交点本身
        //    List<Point> chordPoints = line.Properties
        //        .OfType<Point>()
        //        .Where(x => x.HashCode != crossPoint.HashCode && circlePointHashCodes.Contains(x.HashCode))
        //        .GroupBy(x => x.HashCode)
        //        .Select(g => g.First())
        //        .ToList();

        //    // 相交弦情形下一条弦对应这个圆应当正好有两个端点
        //    if (chordPoints.Count != 2) return false;

        //    int indexP = IndexOfPoint(line, crossPoint);
        //    int index1 = IndexOfPoint(line, chordPoints[0]);
        //    int index2 = IndexOfPoint(line, chordPoints[1]);

        //    if (indexP < 0 || index1 < 0 || index2 < 0) return false;

        //    // 不用 PointWithInPoints，直接靠 Line 中的次序判断“内部”
        //    if (!(Math.Min(index1, index2) < indexP && indexP < Math.Max(index1, index2)))
        //        return false;

        //    end1 = chordPoints[0];
        //    end2 = chordPoints[1];
        //    return true;
        //}

        private int IndexOfPoint(Line line, Point point)
        {
            for (int i = 0; i < line.Properties.Count; i++)
            {
                if (line.Properties[i] is Point p && p.HashCode == point.HashCode)
                    return i;
            }
            return -1;
        }

        #region 特殊处理

        public void GeoEquation识别梅涅劳斯逆定理条件(GeoEquationInfo equationInfo)
        {
            if (equationInfo is null) return;
            GeoEquation识别梅涅劳斯逆定理条件(equationInfo.GeoEquation);
        }

        public void GeoEquation识别梅涅劳斯逆定理条件(GeoEquation eq)
        {
            if (!TryConvertGeoEquationToSREE(eq, out SREE sree)) return;

            sree.AddReason();
            sree.AddCondition(eq);
            AddProcessor.Add(sree);
        }

        private bool TryConvertGeoEquationToSREE(GeoEquation eq, out SREE sree)
        {
            sree = null;
            if (eq is null) return false;
            if (!IsExprOne(eq.RightPart)) return false;

            Expr simplifiedLeft = eq.LeftPart?.Clone()?.Simplify();
            ProductNode product = simplifiedLeft as ProductNode;
            if (product is null) return false;

            if (!product.IsPositive) return false;
            if (!IsExprOne(product.Constant)) return false;
            if (product.Multipliers.Count != 3 || product.Divisors.Count != 3) return false;

            List<Expr> divisors = product.Divisors
                .Select(d => d.Clone().Simplify())
                .ToList();

            List<SLR> slrs = new List<SLR>();

            foreach (var multiplier in product.Multipliers.Select(m => m.Clone().Simplify()))
            {
                bool matched = false;

                for (int i = 0; i < divisors.Count; i++)
                {
                    if (TryBuildSlr(multiplier, divisors[i], out SLR slr))
                    {
                        slrs.Add(slr);
                        divisors.RemoveAt(i);
                        matched = true;
                        break;
                    }
                }

                if (!matched) return false;
            }

            if (slrs.Count != 3) return false;

            sree = new SREE(Expr.One, slrs.ToArray());
            return true;
        }

        private bool TryBuildSlr(Expr numeratorExpr, Expr denominatorExpr, out SLR slr)
        {
            slr = null;

            if (!TryExtractSegment(numeratorExpr, out Segment numeratorSegment)) return false;
            if (!TryExtractSegment(denominatorExpr, out Segment denominatorSegment)) return false;

            Point n1 = (Point)numeratorSegment[0];
            Point n2 = (Point)numeratorSegment[1];
            Point d1 = (Point)denominatorSegment[0];
            Point d2 = (Point)denominatorSegment[1];

            Point commonPoint = null;

            if (n1.HashCode == d1.HashCode || n1.HashCode == d2.HashCode)
                commonPoint = n1;
            else if (n2.HashCode == d1.HashCode || n2.HashCode == d2.HashCode)
                commonPoint = n2;

            if (commonPoint == null) return false;

            Point p1 = n1.HashCode == commonPoint.HashCode ? n2 : n1;
            Point p3 = d1.HashCode == commonPoint.HashCode ? d2 : d1;

            if (p1.HashCode == p3.HashCode) return false;
            if (!KnowledgeGetter.HasColine(p1, commonPoint, p3)) return false;

            slr = KnowledgeGetter.GetSegmentLengthRatio1(p1, commonPoint, p3);
            return slr is not null;
        }

        private bool TryExtractSegment(Expr expr, out Segment segment)
        {
            segment = null;
            if (expr == null) return false;

            Expr simplified = expr.Clone().Simplify();

            if (simplified is MutNode mutNode &&
                mutNode.Mut is GeoProp geoProp &&
                geoProp.Knowledge is Segment seg)
            {
                segment = seg;
                return true;
            }

            return false;
        }

        private bool IsExprOne(Expr expr)
        {
            if (expr == null) return false;
            return expr.Clone().Simplify().ToString() == Expr.One.ToString();
        }




        #endregion


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
