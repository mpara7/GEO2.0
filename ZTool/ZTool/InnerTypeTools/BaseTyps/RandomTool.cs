namespace ZTool.InnerTypeTools.BaseTyps;
public class RandomTool
{
    public static double Normal()
    {
        Random rand = new Random();

        double u1 = 1.0 - rand.NextDouble(); // uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();

        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2); // random normal(0,1)
        return randStdNormal;
        // 如果需要生成特定均值和标准差的正态分布随机数，可以进行如下转换
        double mean = 10.0;
        double stdDev = 2.0;
        double randNormal = mean + stdDev * randStdNormal; // random normal(mean,stdDev^2)
        return randNormal;
    }
}
