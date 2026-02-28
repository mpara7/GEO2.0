using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.Knowledges;

[Description("CompleteQuadriliateral ")]
/// <summary>
/// 完全四边形
/// </summary>
public class CompleteQuadriliateral : PlaneFigure
{


    /// <summary>
    /// 完全四边形
    /// </summary>
    public CompleteQuadriliateral(Point p1, Point p2, Point p3, Point p4, Point p5, Point p6)
    {
        Add(p1, p2, p3, p4, p5, p6);
        //Normalize();
        //SetHashCode();
        SortAndSetHashCode();
    }
    public override string ToString() => $"{Properties[0]}{Properties[1]}{Properties[2]}{Properties[3]}{Properties[4]}{Properties[5]} 构成完全四边形 ";

    public override void Normalize()
    {
    }

    public virtual void SortAndSetHashCode()
    {
        unchecked // 允许整数溢出（哈希码允许重复）
        {
            int hash = 17;
            foreach (var point in Properties)
            {
                hash = hash * 31 + point.GetHashCode();
            }
            this.HashCode = (ulong)hash;
        }
    }
    //public virtual void SortAndSetHashCode()
    //{
    //    // 1. 创建一个新的变量保存排序后的 Properties
    //    var sortedProperties = new List<Knowledge>(Properties);
    //    sortedProperties.Sort(new Comparison<Knowledge>((pred1, pred2) =>
    //    {
    //        if (pred1.PosIndex == pred2.PosIndex)
    //            return 0;
    //        else if (pred1.PosIndex > pred2.PosIndex)
    //            return 1;
    //        else
    //            return -1;
    //    }));

    //    // 2. 生成 HashCode
    //    if (!ClassIndexDict.ContainsKey(this.GetType().Name))
    //    {
    //        ClassIndexDict.Add(this.GetType().Name, (ulong)ClassIndexDict.Count);
    //    }
    //    HashCode = ClassIndexDict[this.GetType().Name] << 54;

    //    for (int k = 0; k < sortedProperties.Count && k < 6; ++k)
    //    {
    //        if (k == 0)
    //            HashCode |= sortedProperties[k].PosIndex;
    //        else
    //            HashCode |= (ulong)sortedProperties[k].PosIndex << k * 9;
    //    }
    //}
}