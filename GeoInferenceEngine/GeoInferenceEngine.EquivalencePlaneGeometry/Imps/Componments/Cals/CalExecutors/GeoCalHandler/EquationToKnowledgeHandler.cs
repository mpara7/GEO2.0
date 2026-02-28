public class EquationToKnowledgeHandler : IInferenceComponent
{
    [ZDI]
    private FormularBase _formularBase;

    //加入知识库
    //public void AddToKnowledgeBase(GeoEquationInfo equationInfo)
    //{
    //    if (equationInfo.equation.Conditions.Count != 0) return;
    //    var equation = equationInfo.equation;
    //    var equationCondition = equationInfo.equationCondition;

    //    AddProcessor.AddNew(equation);

    //    if (equationCondition.knowledgeCondition.Count != 0)
    //    {
    //        equation.AddReason(equationCondition.conditionName);
    //        foreach (var item in equationCondition.knowledgeCondition)
    //        {
    //            equation.AddCondition(item.equation);
    //            AddToKnowledgeBase(item);
    //        }
    //    }
    //}

    public void EquationToKnowledge()
    {
        //①知识：**的长度（大小等）
        //②知识：**相等
        //③线段上一点 or 夹角

        //GenerateCUAD(item);
    }

    //private void GenerateCUAD(GeoEquationInfo equationInfo)
    //{
    //    if (equationInfo.equation.ToString() == "(线段BP的长度*线段CQ的长度*线段AR的长度/线段CP的长度/线段AQ的长度/线段BR的长度)=1")

    //    {
    //    }

    //    List<Segment> m = new List<Segment>();
    //    List<Segment> d = new List<Segment>();
    //    List<Point> CQ0 = new List<Point>();
    //    List<Point> cq2 = new List<Point>();
    //    var M = equationInfo.equation.LeftPart;
    //    if (equationInfo.equation.NodeNumber() != 7) return;

    //    if (M is ProductNode product)

    //    {
    //        if (product.Multipliers.Count != 3 ||
    //            product.Divisors.Count != 3)
    //            return;

    //        foreach (var item in product.Multipliers)

    //        {
    //            if (item is GeoPropNode proNode)

    //            {
    //                if (proNode.Figure is Segment segment)

    //                {
    //                    m.Add(segment);

    //                }
    //                else return;
    //            }
    //            else return;
    //        }
    //        foreach (var item in product.Divisors)
    //        {
    //            if (item is GeoPropNode proNode)
    //            {
    //                if (proNode.Figure is Segment segment)

    //                {
    //                    d.Add(segment);

    //                }
    //                else return;
    //            }
    //            else return;

    //        }
    //        var mpoint = m.SelectMany(segment => segment.Properties);
    //        var dpoint = d.SelectMany(segment => segment.Properties);
    //        mpoint = mpoint.Distinct();
    //        dpoint = dpoint.Distinct();
    //        if (mpoint.Count() != 6 || dpoint.Count() != 6) return;

    //        var start = m[0][0];
    //        CQ0.Add((Point)start);
    //        var travel = m[0][1];
    //        cq2.Add((Point)travel);

    //        var d1 = d.FirstOrDefault(segment => segment.Properties.Contains(travel));
    //        if (d1 == null) { return; }
    //        travel = d1.Properties.FirstOrDefault(Point => Point != travel);
    //        CQ0.Add((Point)travel);

    //        var m1 = m.FirstOrDefault(segment => segment.Properties.Contains(travel));
    //        if (m1 == null) { return; }
    //        travel = m1.Properties.FirstOrDefault(Point => Point != travel);
    //        cq2.Add((Point)travel);

    //        var d2 = d.FirstOrDefault(segment => segment.Properties.Contains(travel));
    //        if (d2 == null) { return; }
    //        travel = d2.Properties.FirstOrDefault(Point => Point != travel);
    //        CQ0.Add((Point)travel);

    //        var m2 = m.FirstOrDefault(segment => segment.Properties.Contains(travel));
    //        if (m2 == null) { return; }
    //        travel = m2.Properties.FirstOrDefault(Point => Point != travel);
    //        cq2.Add((Point)travel);

    //        var d3 = d.FirstOrDefault(segment => segment.Properties.Contains(travel));
    //        if (d3 == null) { return; }
    //        travel = d3.Properties.FirstOrDefault(Point => Point != travel);

    //        if (travel != start) return;

    //        if (knowledgeGetter.HasCollinear(CQ0[0], CQ0[1], CQ0[2]))
    //        {
    //            var temp = CQ0;
    //            CQ0 = cq2;
    //            cq2 = temp;

    //        }
    //        CQ0.Sort((a, b) => a.PosIndex.CompareTo(b.PosIndex));
    //        CQ0.AddRange(cq2);
    //        if (!(knowledgeGetter.HasCollinear(CQ0[0], CQ0[1], CQ0[3]))) // 如果0 1 3不共线，那么判断0 1 4，如果014还不共线，那么015一定共线，交换3和5
    //        {
    //            if (!(knowledgeGetter.HasCollinear(CQ0[0], CQ0[1], CQ0[4])))
    //            {
    //                Point SP = CQ0[5];
    //                CQ0[5] = CQ0[3];
    //                CQ0[3] = SP;

    //            }
    //            else

    //            {
    //                Point SP = CQ0[4];
    //                CQ0[4] = CQ0[3];
    //                CQ0[3] = SP;

    //            }
    //            if (!(knowledgeGetter.HasCollinear(CQ0[1], CQ0[2], CQ0[4])))

    //            {
    //                Point SP = CQ0[5];
    //                CQ0[5] = CQ0[4];
    //                CQ0[4] = SP;

    //            }
    //        }
    //        else

    //        {
    //            if (!(knowledgeGetter.HasCollinear(CQ0[1], CQ0[2], CQ0[4])))
    //            {
    //                Point SP = CQ0[5];
    //                CQ0[5] = CQ0[4];
    //                CQ0[4] = SP;
    //            }
    //        }

    //        //if (!(knowledgeGetter.HasCollinear(CQ1[1], CQ1[2], CQ1[5]))) return;

    //        CompleteQuadriliateral pred0 = new CompleteQuadriliateral(CQ0[0], CQ0[1], CQ0[2], CQ0[3], CQ0[4], CQ0[5]);

    //        pred0.AddReason("由等式生成完全四边形");
    //        AddToKnowledgeBase(equationInfo);
    //        pred0.AddCondition(equationInfo.equation);

    //        if (pred0.ToString().Contains("R"))
    //            Console.WriteLine();
    //        AddProcessor.AddNew(pred0);

    //    }
    //}
}