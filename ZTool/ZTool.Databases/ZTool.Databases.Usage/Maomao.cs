using MongoDB.Bson;

namespace ZTool.Databases.Usage;
public class Maomao : ADatabaseObject
{
    public int? Age;
    public double? Weight;
    public XiaoMaomao XiaoMaomao;
    public override string ToString()
    {
        return $"Age:{Age},Weight{Weight}";
    }
}
public class XiaoMaomao : ADatabaseObject
{
    public List<int> Ints= new List<int>() {4};
    public int? Age;
    public override string ToString()
    {
        return $"Age:{Age}";
    }
}