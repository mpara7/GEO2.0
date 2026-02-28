namespace ZTool.UsefulInterfaces;

/// <summary>
/// 原型模式接口
/// 使用Clone从对象复制另一个对象
/// 泛型只能是它自身
/// 如果需要多种数据结构间转换可以尝试异构 ObjectTool
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICloneable<T> where T : ICloneable<T>
{
    public T Clone();
}
