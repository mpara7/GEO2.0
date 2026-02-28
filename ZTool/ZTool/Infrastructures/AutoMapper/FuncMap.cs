namespace ZTool.Infrastructures.AutoMapper
{
    internal class FuncMap : AMap
    {
        public Action<object, object> PumpAction { get; set; }
        public Func<object, object> MapFunc { get; set; }
        public override object Map(object fromObj)
        {
            return MapFunc(fromObj);
        }

        public override void Pump(object fromObj, object toObj)
        {
            PumpAction(fromObj, toObj);
        }
    }
}
