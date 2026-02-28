namespace ZTool.Infrastructures.ZDI.Creators
{
    public class NormalGetter : AGetter
    {
        public NormalGetter(Type actualType)
        {
            this.ActualType = actualType;
        }
        public override string ToString()
        {
            return $"一般注册 实际类型{ActualType} 目标类型{TargetType}";
        }

        public override object Get()
        {
            //搜索无参构造函数
            var ctors = ActualType.GetConstructors();
            var ctor = ctors.First(ctor => ctor.GetParameters().Length == 0);
            //调用构造函数
            List<object> @params = new List<object>();
            object obj = ctor.Invoke(@params.ToArray());
            InjectDI(obj);
            DoZCtor(obj);
            return obj;
        }

    }
}
