using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace ZTool.InnerTypeTools.BaseTyps;
public static class ObjectTool
{
    public static string PrintProps(object obj)
    {
        StringBuilder stringBuilder = new StringBuilder();
        PropertyInfo[] properties = obj.GetType().GetProperties();

        foreach (PropertyInfo property in properties)
        {
            stringBuilder.AppendLine($"{property.Name}:{property.GetValue(obj)}");
        }
        return stringBuilder.ToString();
    }

    public static T DeepClone<T>(T original)
    {
        if (original == null)
        {
            return default;
        }

        Type type = original.GetType();

        // 如果是值类型或字符串，则直接返回原始值（因为它们在C#中是不可变的）
        if (type.IsValueType || type == typeof(string))
        {
            return original;
        }

        // 如果类型有标记[Serializable]，可以考虑使用序列化的方法进行深拷贝

        // 创建新的对象实例
        T copy = (T)FormatterServices.GetUninitializedObject(type);

        // 获取所有的字段（包括私有和受保护的）
        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            object originalFieldValue = field.GetValue(original);
            object copiedFieldValue = _deepClone((T)originalFieldValue);
            field.SetValue(copy, copiedFieldValue);
        }

        // 对于属性，只复制可读可写的
        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.CanRead && property.CanWrite && property.GetIndexParameters().Length == 0)
            {
                try
                {
                    object originalPropertyValue = property.GetValue(original, null);
                    object copiedPropertyValue = _deepClone((T)originalPropertyValue);
                    property.SetValue(copy, copiedPropertyValue, null);
                }
                catch
                {
                    // 如果属性的get或set方法抛出异常，则忽略该属性
                }
            }
        }

        return copy;
    }

    static T _deepClone<T>(T original)
    {
        if (original == null)
        {
            return default;
        }

        Type type = original.GetType();

        // 如果是值类型或字符串，则直接返回原始值（因为它们在C#中是不可变的）
        if (type.IsValueType || type == typeof(string))
        {
            return original;
        }

        // 如果类型有标记[Serializable]，可以考虑使用序列化的方法进行深拷贝

        // 创建新的对象实例
        T copy = (T)FormatterServices.GetUninitializedObject(type);

        // 获取所有的字段（包括私有和受保护的）
        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            object originalFieldValue = field.GetValue(original);
            object copiedFieldValue = _deepClone((T)originalFieldValue);
            field.SetValue(copy, copiedFieldValue);
        }

        // 对于属性，只复制可读可写的
        foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (property.CanRead && property.CanWrite && property.GetIndexParameters().Length == 0)
            {
                try
                {
                    object originalPropertyValue = property.GetValue(original, null);
                    object copiedPropertyValue = _deepClone((T)originalPropertyValue);
                    property.SetValue(copy, copiedPropertyValue, null);
                }
                catch
                {
                    // 如果属性的get或set方法抛出异常，则忽略该属性
                }
            }
        }

        return copy;
    }
}
