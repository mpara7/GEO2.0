using System.Collections;
using System.Reflection;

namespace ZTool.Infrastructures.ZDI.Creators
{
    public abstract class AGetter
    {
        public ZDIContainer Container { get; init; }
        public Type ActualType { get; init; }
        public Type TargetType { get; init; }
        public abstract object Get();
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
                            foreach (var propObj in Container.GetAll(types[0]))
                            {
                                list.Add(propObj);
                            }
                        }
                    }
                    else
                    {
                        var propObj = Container.Get(propInfo.PropertyType);
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
                            foreach (var propObj in Container.GetAll(types[0]))
                            {
                                list.Add(propObj);
                            }
                        }
                    }
                    else
                    {
                        var propObj = Container.Get(fieldInfo.FieldType);
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
    }
}
