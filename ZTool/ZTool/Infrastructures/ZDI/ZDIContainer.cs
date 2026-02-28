using System.Collections;
using System.Reflection;
using Throw;
using ZTool.Infrastructures.ZDI.Creators;

namespace ZTool.Infrastructures.ZDI
{
    /// <summary>
    /// 自动数据上下文
    /// 不考虑Scope 需要创建新的ZDIContainer
    /// </summary>
    public class ZDIContainer
    {
        #region 信息
        /// <summary>
        /// 有无注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasSet<T>()
        {
            Type type = typeof(T);
            return HasSet(type);
        }

        /// <summary>
        /// 有无注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasSet(Type type)
        {
            bool result = Getters.Exists(getter => getter.TargetType == type);
            return result;
        }
        public bool CanGet<T>()
        {
            Type type = typeof(T);
            return CanGet(type);
        }

        /// <summary>
        /// 检查类型是否满足DI 不是有无注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool CanGet(Type type)
        {
            List<Type> requiredTypes = new List<Type>();
            //属性
            foreach (var propInfo in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (propInfo.GetCustomAttribute<ZDIAttribute>() is not null)
                {
                    requiredTypes.Add(propInfo.PropertyType);
                }
            }
            //字段
            foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (fieldInfo.GetCustomAttribute<ZDIAttribute>() is not null)
                {
                    requiredTypes.Add(fieldInfo.FieldType);
                }
            }
            bool result = requiredTypes.TrueForAll(
                type => Getters.Exists(getter => getter.TargetType == type)
                );
            return result;
        }
        #endregion




        public List<AGetter> Getters = new List<AGetter>();
        public ZDIContainer()
        {
            Getters.Add(new InstanceSignletonGetter(this) { TargetType = typeof(ZDIContainer) });
        }
        public ZDIContainer(ZDIContainer parent)
        {
            //跳过parent中的ZDIContainer注册信息
            Getters = new(parent.Getters.Skip(1));
            Getters.Add(new InstanceSignletonGetter(this) { TargetType = typeof(ZDIContainer) });
        }
        /// <summary>
        /// 检查必须有无参构造函数
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="InvalidOperationException"></exception>
        void _checkIfHasNoParametersCtor(Type type)
        {
            //检查是否存在无参构造函数
            bool hasNoParametersCtor = false;
            foreach (var ctor in type.GetConstructors())
            {
                if (ctor.GetParameters().Length == 0)
                {
                    hasNoParametersCtor = true;
                    break;
                }
            }
            if (!hasNoParametersCtor)
                throw new InvalidOperationException($"注册类型{type.FullName}失败，需要有“无参的构造函数”");
        }



        #region 注册

        #region 一般注册
        /// <summary>
        /// 注册Target
        /// </summary>
        /// <typeparam name="Target"></typeparam>
        public void Set<Target>() where Target : class, new()
        {
            Set(typeof(Target), typeof(Target));
        }
        /// <summary>
        /// 注册T为Target
        /// </summary>
        /// <typeparam name="Actual"></typeparam>
        /// <typeparam name="Target"></typeparam>
        public void Set<Actual, Target>() where Actual : class, Target, new()
        {
            Set(typeof(Actual), typeof(Target));
        }
        /// <summary>
        /// 注册type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="group"></param>
        public void Set(Type type)
        {
            Set(type, type);
        }
        /// <summary>
        /// 注册type为targetType
        /// </summary>
        /// <param name="actualType"></param>
        /// <param name="targetType"></param>
        public void Set(Type actualType, Type targetType)
        {
            _checkIfHasNoParametersCtor(actualType);
            actualType.Throw(() => new ArgumentException($"类型：{actualType} 不能是抽象类")).IfTrue(actualType.IsAbstract);
            if (actualType != targetType)
                if (targetType.IsInterface)
                    actualType.Throw(() => new ArgumentException($"{actualType}不是{targetType}的实现类型"))
                        .IfFalse(actualType.IsAssignableTo(targetType));
                else
                    actualType.Throw(() => new ArgumentException($"{actualType}不是{targetType}的子类型"))
                        .IfFalse(actualType.IsSubclassOf(targetType));
            Getters.Add(new NormalGetter(actualType) { TargetType = targetType, Container = this });
        }
        #endregion

        #region 单例-指定类型
        /// <summary>
        /// 注册单例
        /// </summary>
        /// <typeparam name="Target"></typeparam>
        public void SetSingleton<Target>() where Target : class, new()
        {
            SetSingleton(typeof(Target), typeof(Target));
        }
        /// <summary>
        /// 注册单例
        /// </summary>
        /// <typeparam name="Actual"></typeparam>
        public void SetSingleton<Actual, Target>() where Actual : class, Target, new()
        {
            SetSingleton(typeof(Actual), typeof(Target));
        }
        /// <summary>
        /// 注册单例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SetSingleton(Type type)
        {
            SetSingleton(type, type);
        }
        /// <summary>
        /// 注册单例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SetSingleton(Type type, Type targetType)
        {
            if (type == null || targetType == null)
            {
                throw new ArgumentNullException($"({type})-({targetType})对不能存在空值");
            }
            _checkIfHasNoParametersCtor(type);

            type.Throw(() => new ArgumentException($"{type}不能是接口"))
                .IfTrue(type.IsInterface);
            if (type != targetType)
                if (targetType.IsInterface)
                    type.Throw(() => new ArgumentException($"{type}不是{targetType}的子类型"))
                        .IfFalse(type.IsAssignableTo(targetType));
                else
                    type.Throw(() => new ArgumentException($"{type}不是{targetType}的子类型"))
                        .IfFalse(type.IsSubclassOf(targetType));
            Getters.Add(new SignletonGetter(type) { TargetType = targetType, Container = this });
        }
        #endregion

        #region 单例-指定对象
        /// <summary>
        /// 注册对象为单例
        /// </summary>
        /// <typeparam name="Target"></typeparam>
        public void SetSingleton<Target>(Target instance)
        {
            SetSingleton<Target, Target>(instance);
        }
        /// <summary>
        /// 注册对象为单例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SetSingleton<T, Target>(T instance) where T : Target
        {
            SetSingleton(instance, typeof(Target));
        }
        /// <summary>
        /// 注册对象为单例,非泛型版
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void SetSingleton(object instance, Type targetType)
        {
            var type = instance.GetType();
            if (type != targetType)
                if (targetType.IsInterface)
                    type.Throw(() => new ArgumentException($"{type}不是{targetType}的子类型"))
                        .IfFalse(type.IsAssignableTo(targetType));
                else
                    type.Throw(() => new ArgumentException($"{type}不是{targetType}的子类型"))
                        .IfFalse(type.IsSubclassOf(targetType));
            Getters.Add(new InstanceSignletonGetter(instance) { TargetType = targetType, Container = this });
        }
        #endregion
        #region 动态的上下文 可以是lamba，可以是对象的方法（上下文在类成员中）
        /// <summary>
        /// 动态的上下文 可以是lamba，可以是对象的方法（上下文在类成员中）
        /// </summary>
        /// <typeparam name="Target"></typeparam>
        /// <param name="func"></param>
        public void Set<Target>(Func<object> func)
        {
            var generic = func.GetType().GetGenericArguments()[0];
            Getters.Add(new DelegateGetter(func.Invoke) { TargetType = typeof(Target) });
        }
        #endregion

        #endregion

        #region 操作
        /// <summary>
        /// 取消
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Unset(AGetter getter)
        {
            Getters.Throw(() => new ArgumentNullException($"{getter}不在此容器中")).IfNotContains(getter);

            Getters.Remove(getter);
        }

        public Target Get<Target>()
        {
            return (Target)Get(typeof(Target));
        }
        /// <summary>
        /// 适用于需要在运行时才能确定的类型
        /// </summary>
        /// <typeparam name="T">用于最后强制转换，不用担心影响容器内部</typeparam>
        /// <param name="targetType"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object Get(Type targetType)
        {
            var usingGetters = Getters.ToList();
            usingGetters.Reverse();

            AGetter creator;
            creator = usingGetters.FirstOrDefault(g => g.TargetType == targetType);

            creator.ThrowIfNull(() => throw new InvalidOperationException($"目标类型：{targetType} 没有记录"));
            var obj = creator.Get();
            return obj;
        }
        /// <summary>
        /// 适用于需要在运行时才能确定的类型
        /// </summary>
        /// <typeparam name="T">用于最后强制转换，不用担心影响容器内部</typeparam>
        /// <param name="targetType"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T QGet<T>()
        {
            return  (T)QGet(typeof(T));
        }
        /// <summary>
        /// 适用于需要在运行时才能确定的类型
        /// </summary>
        /// <typeparam name="T">用于最后强制转换，不用担心影响容器内部</typeparam>
        /// <param name="targetType"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object QGet(Type targetType)
        {
            Set(targetType);
            return Get(targetType);
        }
        /// <summary>
        /// 适用于需要在运行时才能确定的类型
        /// </summary>
        /// <typeparam name="T">用于最后强制转换，不用担心影响容器内部</typeparam>
        /// <param name="targetType"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object GetWithoutDI(Type targetType)
        {
            var usingGetters = Getters.ToList();
            usingGetters.Reverse();

            AGetter creator;
            creator = usingGetters.FirstOrDefault(g => g.TargetType == targetType);

            creator.ThrowIfNull(() => throw new InvalidOperationException($"目标类型：{targetType} 没有记录"));
            var obj = creator.Get();

            return obj;
        }
        public void InjectDI(object obj)
        {
            var ActualType = obj.GetType();
            //属性依赖注入-------------------------------------------------------------------
            var propInfos = ActualType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var propInfo in propInfos)
            {
                if (propInfo.GetCustomAttribute<ZDIAttribute>() is not null)
                {
                    //列表特殊处理
                    if (propInfo.PropertyType.IsGenericType && propInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Type[] types = propInfo.PropertyType.GenericTypeArguments;
                        if (types.Length == 1)
                        {
                            IList list = (IList)Activator.CreateInstance(propInfo.PropertyType);
                            propInfo.SetValue(obj, list);
                            foreach (var propObj in GetAll(types[0]))
                            {
                                list.Add(propObj);
                            }
                        }
                    }
                    else
                    {
                        var propObj = Get(propInfo.PropertyType);
                        propInfo.SetValue(obj, propObj);
                    }
                }
            }
            //字段依赖注入-------------------------------------------------------------------
            var fieldInfos = ActualType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.GetCustomAttribute<ZDIAttribute>() is not null)
                {
                    //列表特殊处理
                    if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Type[] types = fieldInfo.FieldType.GenericTypeArguments;
                        if (types.Length == 1)
                        {
                            IList list = (IList)Activator.CreateInstance(fieldInfo.FieldType);
                            fieldInfo.SetValue(obj, list);
                            foreach (var propObj in GetAll(types[0]))
                            {
                                list.Add(propObj);
                            }
                        }
                    }
                    else
                    {
                        var propObj = Get(fieldInfo.FieldType);
                        fieldInfo.SetValue(obj, propObj);
                    }
                }
            }
        }

        public void DoZCtor(object obj)
        {
            if (obj is IZCtor zCtor)
            {
                zCtor.ZCtor();
            }
        }
        /// <summary>
        /// 获取一个目标类型下的所有注册
        /// 返回列表为空代表没有对应记录
        /// </summary>
        /// <typeparam name="Target"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public List<Target> GetAll<Target>()
        {
            List<Target> objectList = new List<Target>();
            var usingGetters = Getters.ToList();
            //找到注册的所有目标类型
            IEnumerable<AGetter> getters = usingGetters.Where(g => g.TargetType == typeof(Target));
            foreach (var getter in getters)
            {
                objectList.Add((Target)getter.Get());
            }
            return objectList;
        }
        /// <summary>
        /// 获取一个目标类型下的所有注册
        /// 返回列表为空代表没有对应记录
        /// </summary>
        /// <typeparam name="Target"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public List<object> GetAll(Type type)
        {
            List<object> objectList = new List<object>();
            //找到注册的所有目标类型
            IEnumerable<AGetter> getters = Getters.Where(g => g.TargetType == type);
            foreach (var getter in getters)
            {
                objectList.Add(getter.Get());
            }
            return objectList;
        }
        #endregion
    }
}
