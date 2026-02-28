namespace ZTool.Infrastructures.ZDI.Creators
{
    public class SignletonGetter : AGetter
    {
        object instance;
        public Type ActualType { get; set; }
        public SignletonGetter(Type actualType)
        {
            this.ActualType = actualType;
        }
        public override string ToString()
        {
            return $"SignletonGetter:{this.ActualType}->{this.TargetType}";
        }
        public override object Get()
        {
            if (instance is not null)
            {
                return instance;
            }
            else
            {
                var obj = create();
                instance = obj;
                InjectDI(obj);
                DoZCtor(obj);
                return obj;
            }

        }
        public object create()
        {
            //搜索无参构造函数
            var ctors = ActualType.GetConstructors();
            var ctor = ctors.First(ctor => ctor.GetParameters().Length == 0);
            //调用构造函数
            List<object> @params = new List<object>();
            object obj = ctor.Invoke(@params.ToArray());
            return obj;
        }




    }
}
