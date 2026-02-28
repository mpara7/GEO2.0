namespace GeoInferenceEngine.PlaneKnowledges.Knowledges;
/// <summary>
/// 平面图形基类
/// </summary>
public abstract class PlaneFigure : Figure
{
    public static ulong ClassIndex { get; set; }
    public void NormalizeForPolygon()
    {
        var cProperties = this.Properties;
        int size = cProperties.Count;
        int flag = 0;
        Point point = cProperties[0] as Point;//假定第一个点索引值最小
        Point[] pointPreds = new Point[size];//保存原有位置
        cProperties.CopyTo(pointPreds);//复制

        for (int i = 1; i < size; i++)//找到最小的下标
        {
            if (cProperties[i].PosIndex < point.PosIndex)
            {
                flag = i;
                point = cProperties[i] as Point;
            }
        }
        for (int i = 0; i < size; i++)//移位，将最小下标移到第一位
        {
            int z = (i - flag + size) % size;
            cProperties[z] = pointPreds[i];
        }
        if (cProperties[1].PosIndex > cProperties[size - 1].PosIndex)//比较第二个元素与最后一个元素
        {

            var temp = cProperties[1];
            cProperties[1] = cProperties[size - 1];
            cProperties[size - 1] = temp;

        }
    }
    public void NormalizeForTrapezoid()
    {
        var cProperties = this.Properties;
        int size = cProperties.Count;
        //存储最小值的坐标
        int flag = 0;
        Point point = (Point)cProperties[0];//假定第一个点索引值最小

        Point[] pointPreds = new Point[size];//保存原有位置
        cProperties.CopyTo(pointPreds);//复制

        for (int i = 1; i < size; i++)//找到最小的下标
        {
            if (cProperties[i].PosIndex < point.PosIndex)
            {
                flag = i;

            }
        }
        //表示最小值的坐标出现在后面两点，此时要把后面两点提前最前面两点
        if (flag > 1)
        {
            cProperties[0] = pointPreds[2];
            cProperties[1] = pointPreds[3];
            cProperties[2] = pointPreds[0];
            cProperties[3] = pointPreds[1];
        }
        //如果第一个点还不是最小的，进行交换
        if (cProperties[0].PosIndex > cProperties[1].PosIndex)
        {
            var temp = cProperties[1];
            cProperties[1] = cProperties[0];
            cProperties[0] = temp;

            //前面两个点交换了，后面两个点也要交换
            temp = cProperties[3];
            cProperties[3] = cProperties[2];
            cProperties[2] = temp;
        }
    }
}
