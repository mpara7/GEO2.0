namespace ZTool.Infrastructures.AutoMapper
{
    public abstract class AMap
    {
        public Type FromType { get; init; }
        public Type ToType { get; init; }
        public ZMapper ZMapper { get; init; }
        public abstract object Map(object fromObj);
        public abstract void Pump(object fromObj, object toObj);
    }
}