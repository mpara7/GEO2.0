using GeoInferenceEngine.Knowledges;

namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
[Description("全等三角形", "三角形全等")]
public class TriangleCongruence : Knowledge
{
    public TriangleCongruence(Point point1, Point point2, Point point3, Point point4, Point point5, Point point6)
    {
        Add(point1, point2, point3, point4, point5, point6);
        Normalize();
        SetHashCode();

    }



    public override string ToString() => $"三角形{Properties[0]}{Properties[1]}{Properties[2]}与三角形{Properties[3]}{Properties[4]}{Properties[5]}全等".Replace("点", string.Empty);

    public override void Normalize()
    {
        this.NormalizeForTriangleCongruence();

    }
    void NormalizeForTriangleCongruence()
    {
        int size = Properties.Count;
        int num = size / 2;
        int flag = 0;
        Point point = (Point)Properties[0];
        Point[] pointPreds = new Point[size];
        Properties.CopyTo(pointPreds);//复制
        for (int i = 1; i < num; i++)//找到最小的下标
        {
            if (Properties[i].PosIndex < point.PosIndex)
            {
                flag = i;
                point = (Point)Properties[i];
            }
        }
        for (int i = 0; i < num; i++)
        {
            int z = (i - flag + num) % num;
            Properties[z] = pointPreds[i];
            Properties[z + num] = pointPreds[i + num];
        }
        if (Properties[1].PosIndex > Properties[num - 1].PosIndex)
        {
            for (int i = 1; i <= num / 2; i++)
            {
                Point temp = (Point)Properties[i];
                Properties[i] = Properties[num - i];
                Properties[num - i] = temp;
                Point temp1 = (Point)Properties[i + num];
                Properties[i + num] = Properties[2 * num - i];
                Properties[2 * num - i] = temp1;
            }
        }
    }



}