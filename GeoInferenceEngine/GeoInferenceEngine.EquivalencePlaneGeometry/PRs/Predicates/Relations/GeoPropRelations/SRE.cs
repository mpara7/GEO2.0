using System.Text;
using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;

[Description("Expr")]
/// <summary>
/// class SegmentLengthRatio: 线段长度比值
/// </summary>
public class SRE : Knowledge
{
    public SegmentLengthRatio SegLR1 { get => (SegmentLengthRatio)Properties[0]; }
    public SegmentLengthRatio SegLR2 { get => (SegmentLengthRatio)Properties[1]; }
    public SegmentLengthRatio SegLR3 { get => (SegmentLengthRatio)Properties[2]; }
    public int count { get => (int)Properties.Count; }
    // <summary>
    /// 构造函数
    /// </summary>
    public SRE(params SegmentLengthRatio[] Slr )
    {
        Add(Slr);
        //Debug.Assert(!ToString().Contains("CD与AD的比"));
        //Normalize();
        SetHashCode();

    }


    public override string ToString()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < Properties.Count; i++)
        {
            var segmentLengthRatio = (SegmentLengthRatio)Properties[i];
            sb.Append($"({segmentLengthRatio.Properties[0]}/{segmentLengthRatio.Properties[1]})");

            // 如果不是最后一个，添加乘号
            if (i < Properties.Count - 1)
            {
                sb.Append("*");
            }
        }

        sb.Append(" = 1");

        return sb.ToString();
    }

    public override void Normalize()
    {
        // 遍历所有的 SegmentLengthRatio 对象
        for (int i = 0; i < Properties.Count - 1; i++)
        {
            var current = (SegmentLengthRatio)Properties[i];
            var next = (SegmentLengthRatio)Properties[i + 1];

            // 获取两个线段比值的第一个端点
            string currentFirstPoint = current.Properties[0].ToString();
            string nextFirstPoint = next.Properties[0].ToString();

            // 比较字母表顺序
            if (string.Compare(currentFirstPoint, nextFirstPoint) > 0)
            {
                // 如果当前的端点比下一个端点字母表顺序靠后，则交换顺序
                Properties[i] = next;
                Properties[i + 1] = current;

                // 交换时可能需要反转符号
                
            }
        }
    }


    public override void SetHashCode()
    {
        // 初始化哈希值
        int hash = 17;

        // 遍历所有的 SegmentLengthRatio
        foreach (var segmentLengthRatio in Properties)
        {
            // 计算每个 SegmentLengthRatio 的哈希值，并根据位置加权
            hash = hash * 31 + segmentLengthRatio.GetHashCode();
        }

        // 设置 SRE 类的哈希值
        HashCode = (ulong)hash;
    }


}