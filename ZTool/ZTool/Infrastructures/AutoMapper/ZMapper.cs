namespace ZTool.Infrastructures.AutoMapper
{
    public static class QMapper
    {
        public static ZMapper Mapper = new();
        public static T QMap<T>(this object obj) where T : new()
        {
            var toObj = Mapper.Map(obj, obj.GetType(), typeof(T));
            return (T)toObj;
        }
        public static MapBuilder<From, To> CreateMap<From, To>() where From : new()
        {
            return Mapper.CreateMap<From, To>();
        }
    }

    /// <summary>
    /// 形式转换
    /// 自动找对应的规则
    /// 同名 或 小写化去下划线后相同
    /// 列表自动对应
    /// 指定字段属性
    /// 
    /// 映射
    /// 类型相同则直接赋值 深克隆
    /// 除了内置的基本 不同类型需要提前定义 
    /// 提供兜底处理
    /// </summary>
    public class ZMapper
    {
        #region 快速的静态方法

        #endregion
        public ZMapper()
        {
            Mappers.Add(new FuncMap() { FromType = typeof(int), ToType = typeof(string), MapFunc = i => i.ToString() });
        }
        public Func<Type, object> CreatNewFunc { get; set; } = Activator.CreateInstance;
        List<AMap> Mappers { get; set; } = new();
        public void AddMapFunc<From, To>(Func<From, To> func)
        {
            Mappers.Add(new FuncMap() { FromType = typeof(From), ToType = typeof(To), MapFunc = from => func((From)from) });
        }
        public MapBuilder<From, To> CreateMap<From, To>() where From : new()
        {
            MapBuilder<From, To> config = new MapBuilder<From, To>(this);
            Mappers.Add(config.Map);
            return config;
        }
        public To Map<To>(object from)
        {
            //后来先用
            var usingMappers = Mappers.ToList();
            usingMappers.Reverse();
            var map = usingMappers.First(m => m.FromType == from.GetType() && m.ToType == typeof(To));
            var result = (To)map.Map(from);
            return result;
        }
        public To Map<From, To>(From from)
        {
            //后来先用
            var usingMappers = Mappers.ToList();
            usingMappers.Reverse();
            var map = usingMappers.First(m => m.FromType == typeof(From) && m.ToType == typeof(To));
            var result = (To)map.Map(from);
            return result;
        }

        public object Map(object from, Type fromType, Type toType)
        {
            //后来先用
            var usingMappers = Mappers.ToList();
            usingMappers.Reverse();
            var map = usingMappers.First(m => m.FromType == fromType && m.ToType == toType);
            var result = map.Map(from);
            return result;
        }
    }
}
