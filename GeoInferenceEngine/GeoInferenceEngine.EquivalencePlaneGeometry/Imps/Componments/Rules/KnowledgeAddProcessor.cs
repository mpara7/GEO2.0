//KnowledgeBaseTransfer
using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.Cal;
using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases;
using GeoInferenceEngine.Knowledges;
using GeoInferenceEngine.PredicateShared.Models;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments.PRs;
[Description("知识添加处理")]
public class KnowledgeAddProcessor : IInferenceComponent
{
    public Action<Knowledge> OnAddNew;
    [ZDI]
    protected Logger logger;
    [ZDI]
    protected AppInfo AppInfo;
    [ZDI]
    protected EngineInfo ERBase;
    [ZDI]
    protected KnowledgeBase knowledgeBase;
    public Knowledge Add(Knowledge knowledge)
    {
        //判断是否是新知识
        if (knowledgeBase.Knowledges.ContainsKey(knowledge.HashCode))
        {
#if DEBUG
            Knowledge preOldKnowledge = knowledgeBase.Knowledges[knowledge.HashCode];
            var old = UlongTool.UlongToStr(preOldKnowledge.HashCode);
            var nnew = UlongTool.UlongToStr(knowledge.HashCode);
#endif
            Knowledge oldKnowledge = knowledgeBase.Knowledges[knowledge.HashCode];

            if (knowledge.Expr is not null)
            {
                if (knowledge.Expr.CompareTo(oldKnowledge.Expr) == ExprCompareResult.Equal)
                {
                }
                else
                {
                    AppInfo.IsActivedStop = true;
                    AppInfo.ActivedStopReasons.Add($"新知识：{knowledge}与旧知识{oldKnowledge}矛盾");
                }
            }
            return oldKnowledge;
        }
        else
        {
            //插入
            OnAddNew?.Invoke(knowledge);

            ERBase.NewKnowledges.Add(knowledge);
            ERBase.HasNewKnowledge = true;
            ERBase.IsOutOfPair = false;
            logger.Debug($"已添加新知识{knowledge}");
            knowledgeBase.Knowledges.Add(knowledge.HashCode, knowledge);

            var categoryType = knowledge.GetType();
            if (!knowledgeBase.Categories.ContainsKey(knowledge.GetType()))
            {
                var categoryInfo = new List<Knowledge>() { };
                knowledgeBase.Categories.Add(categoryType, categoryInfo);
            }

            knowledge.PosIndex = (uint)knowledgeBase.Categories[categoryType].Count;
            knowledgeBase.Categories[categoryType].Add(knowledge);
            return knowledge;
        }

    }

    public Knowledge Add(Angle newAngle)
    {
        logger.Debug($"新角{newAngle}");
        var Angles = knowledgeBase.Categories[typeof(Angle)];
        List<Point> newAngle_edge1 = newAngle.Edge1;
        List<Point> newAngle_edge2 = newAngle.Edge2;
        Angle oldAngle = null;
        foreach (Angle angle in Angles)
        {
            List<Point> angle_edge1 = angle.Edge1;
            List<Point> angle_edge2 = angle.Edge2;
            bool areEqual = angle_edge1.SequenceEqual(newAngle_edge1) && angle_edge2.SequenceEqual(newAngle_edge2);
            bool areEqual1 = angle_edge1.SequenceEqual(newAngle_edge2) && angle_edge2.SequenceEqual(newAngle_edge1);
            if (angle[1] == newAngle[1] && areEqual  || angle[1] == newAngle[1] && areEqual1)
            {
                oldAngle = angle;
            }
        }
        if(oldAngle is not null)
        {
            return oldAngle;
        }
        if(1 + newAngle_edge2.Count + newAngle_edge1.Count > 3)
        {
            
            List<Angle> needToUpdates = new();
            foreach (Angle angle in Angles)
            {
                List<Point> angle_edge1 = angle.Edge1;
                List<Point> angle_edge2 = angle.Edge2;
                bool containsAll = angle_edge1.All(item => newAngle_edge1.Contains(item)) && angle_edge2.All(item => newAngle_edge2.Contains(item));
                bool containsAll1 = angle_edge1.All(item => newAngle_edge2.Contains(item)) && angle_edge2.All(item => newAngle_edge1.Contains(item));
                if ((angle.IsAvailable == true && angle.Vertex == newAngle.Vertex && containsAll) || (angle.IsAvailable == true && angle.Vertex == newAngle.Vertex && containsAll1)){
                    needToUpdates.Add(angle);
                }
            }
            //如果无需更新
            if (needToUpdates.Count == 0)
            {
                return this.Add((Knowledge)newAngle);
            }
            //如果需要更新
            else
            {
                    needToUpdates.ForEach(A => A.IsAvailable = false);
                    //替换那些包含了待更新直线的知识
                    List<Knowledge> updatedKnowledges = new();
                    foreach (var knowledge in knowledgeBase.Knowledges.Values)
                    {
                        Knowledge clone = null;
                        for (int i = 0; i < knowledge.Properties.Count; i++)
                        {
                            var part = knowledge.Properties[i];

                            if (needToUpdates.Contains(part))
                            {
                                if (clone is null)
                                    clone = knowledge.Clone();
                                clone.Properties[i] = newAngle;
                            }

                        }
                        //如果需要更新
                        if (clone is not null)
                        {
                            if (clone.GetType() == typeof(PointOnAngularBisector))
                            {
                                  PointOnAngularBisector other = (PointOnAngularBisector)clone;
                                  other.PointOnBisector = new List<Point>(((PointOnAngularBisector)knowledge).PointOnBisector);
                                  knowledge.IsAvailable = false;
                                  other.IsAvailable = true;
                                  other.AddReason();
                                  other.Conditions.Clear();
                                  other.AddCondition(knowledge,newAngle);
                                  updatedKnowledges.Add(other);
                            }else{
                                knowledge.IsAvailable = false;
                                clone.IsAvailable = true;
                                clone.AddReason();
                                clone.Conditions.Clear();
                                clone.AddCondition(knowledge, newAngle);
                                updatedKnowledges.Add(clone);}
                        }
                    }
                    this.Add((Knowledge)newAngle);
                    //更新
                    foreach (Knowledge updatedKnowledge in updatedKnowledges)
                    {
                        updatedKnowledge.SetHashCode();
                        this.Add(updatedKnowledge);
                    }
                    return newAngle;
            }
        }
        else
        {
            return (Angle)this.Add((Knowledge)newAngle);

        }

    }

    public Knowledge Add(Line newLine)
    {
        logger.Debug($"新直线{newLine}");
        var lines = knowledgeBase.Categories[typeof(Line)];
        var oldLine = lines.FirstOrDefault(l => newLine.Points.TrueForAll(p => l.Properties.Contains(p)));
        if (oldLine is not null)
        {
            ERBase.NewKnowledges.Add(oldLine);
            return oldLine;
        }
            
        Line returnLine = newLine;
        if (newLine.Points.Count > 2)
        {
            //搜集待更新直线信息
            List<Line> needToUpdates = new();
            foreach (var line in knowledgeBase.Categories[typeof(Line)])
            {
                if (line.Properties.Where(p => newLine.Points.Contains(p)).Count() >= 2)
                {
                    needToUpdates.Add((Line)line);
                }
            }
            //如果无需更新
            if (needToUpdates.Count == 0)
            {
                this.Add((Knowledge)returnLine);
            }
            //如果需要更新
            else
            {
                needToUpdates.ForEach(l => l.IsAvailable = false);

                //组合所有点 确定最终直线
                Point[] points = null;
                points = newLine.Points.Union(needToUpdates.SelectMany(l => l.Points)).Distinct().ToArray();
                //生成新组合线
                if (points.Count() > newLine.Points.Count())
                {
                    newLine.IsAvailable = false;
                    returnLine = new Line(points);
                    returnLine.AddReason();
                    returnLine.AddCondition(newLine);
                    returnLine.AddCondition(needToUpdates.ToArray());
                }
                this.Add((Knowledge)returnLine);

                //替换那些包含了待更新直线的知识中
                List<Knowledge> updatedKnowledges = new();
                foreach (var knowledge in knowledgeBase.Knowledges.Values)
                {
                    Knowledge clone = null;
                    for (int i = 0; i < knowledge.Properties.Count; i++)
                    {
                        var part = knowledge.Properties[i];
                        if (needToUpdates.Contains(part))
                        {
                            //考虑到可能同时更新多个属性（如：直线平行）
                            if (clone is null)
                                clone = knowledge.Clone();
                            clone.Properties[i] = returnLine;
                            clone.SetHashCode();

                        }
                    }
                    //如果需要更新
                    if (clone is not null)
                    {
                        knowledge.IsAvailable = false;
                        clone.IsAvailable = true;
                        clone.AddReason();
                        clone.Conditions.Clear();
                        clone.AddCondition(knowledge, returnLine);
                        clone.SetHashCode();
                        updatedKnowledges.Add(clone);
                    }
                }

                //更新
                foreach (var updatedKnowledge in updatedKnowledges)
                {
                    this.Add((Knowledge)updatedKnowledge);
                }
            }
        }
        returnLine = (Line)Add((Knowledge)returnLine);
        return returnLine;
    }
    [ZDI]
    CalAddProcessor calAddProcessor;
    public Knowledge Add(GeoEquation equation)
    {
        calAddProcessor.Add(equation);
        ERBase.NewKnowledges.Add(equation);
        return equation;
    }


    public Knowledge Add(AngleSize pred)
    {
        var re = Add((Knowledge)pred);
        if (re == pred)
        {
            calAddProcessor.UpdateRatioInfo(pred.Angle.Size, pred.Expr.Clone(),
                 pred, GeoEquationUnits.Angle);
        }
        return re;
    }
    public Knowledge Add(AngleSizeEqual pred)
    {
        var re = Add((Knowledge)pred);
        if (re == pred)
        {
            calAddProcessor.UpdateRatioInfo(pred.Angle1.Size, pred.Angle2.Size,
                Expr.One, pred, GeoEquationUnits.Angle);
        }
        return re;
    }
    public Knowledge Add(AngleSizeRatio pred)
    {
        var re = Add((Knowledge)pred);
        if (re == pred)
        {
            calAddProcessor.UpdateRatioInfo(pred.Angle1.Size, pred.Angle2.Size,
                pred.Expr.Clone(), pred, GeoEquationUnits.Angle);
        }
        return re;
    }

    public Knowledge Add(SegmentLength segmentLength)
    {
        var re = Add((Knowledge)segmentLength);
        if (re == segmentLength)
        {
            calAddProcessor.UpdateRatioInfo(segmentLength.Segment.Length, segmentLength.Expr.Clone(),
                 segmentLength);
        }
        return re;
    }
    public Knowledge Add(SegmentLengthEqual pred)
    {
        var re = Add((Knowledge)pred);
        if (re == pred)
        {
            calAddProcessor.UpdateRatioInfo(pred.Seg1.Length, pred.Seg2.Length,
                Expr.One, pred);
        }
        return re;
    }
    public Knowledge Add(SegmentLengthRatio pred)
    {
        var re = Add((Knowledge)pred);
        if (re == pred)
        {
            calAddProcessor.UpdateRatioInfo(pred.Seg1.Length, pred.Seg2.Length,
               pred.Expr.Clone(), pred);
        }
        return re;
    }

    public void removeKnowledge(Knowledge knowledge)
    {
        bool removed = knowledgeBase.Knowledges.Remove(knowledge.HashCode);
        List<Knowledge>  knowledges = knowledgeBase.Categories[knowledge.GetType()];
        for (int i = 0; i < knowledges.Count; i++)
        {
            if (knowledges[i].HashCode == knowledge.HashCode)
            {
                knowledges.RemoveAt(i);
            }
            
        }

    }


}
