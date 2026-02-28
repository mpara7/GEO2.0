namespace ZTool.Databases.Tools;
public static class FieldChangeTool
{
    public static bool IsEnumerable(this Type type)
    {
        if (type.IsArray)
        {
            return true;
        }
        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
        {
            return true;
        }
        foreach (var it in type.GetInterfaces())
            if (it.IsGenericType && typeof(IEnumerable<>) == it.GetGenericTypeDefinition())
                return true;
        return false;
    }

    public static List<FieldChange> GetFieldChanges<T>(T old, T newObj)
    {
        List<FieldChange> fieldChanges = new List<FieldChange>();
        foreach (var field in typeof(T).GetFields())
        {
            //字符串一般处理吧
            if (field.FieldType.IsEnumerable() && field.FieldType != typeof(string))
            {
                bool isEqual = true;
                var enumable = (System.Collections.IEnumerable)field.GetValue(old);
                var oldEnumerator = enumable.GetEnumerator();
                var enumable2 = (System.Collections.IEnumerable)field.GetValue(newObj);
                var newEnumerator = enumable2.GetEnumerator();
                //不同步时为true
                var a = oldEnumerator.MoveNext();
                var b = newEnumerator.MoveNext();
                bool nsync = a ^ b;
                //一开始就不一样
                if (nsync)
                {
                    fieldChanges.Add(new FieldChange(field.Name, enumable2));
                }
                else
                {
                    //一样且不为空
                    while (!nsync && a && isEqual)
                    {
                        if (!oldEnumerator.Current.Equals(newEnumerator.Current))
                        {
                            isEqual = false;
                        }
                        a = oldEnumerator.MoveNext();
                        b = newEnumerator.MoveNext();
                        nsync = a ^ b;
                        //不一致时不等
                        if (nsync)
                        {
                            isEqual = false;
                        }
                    }
                }
                if (!isEqual)
                {
                    fieldChanges.Add(new FieldChange(field.Name, enumable2));
                }

            }
            //基本类型
            else
            {
                var oldValue = field.GetValue(old);
                var newValue = field.GetValue(newObj);
                if (oldValue is null)
                {
                    if (newValue is not null)
                    {
                        fieldChanges.Add(new FieldChange(field.Name, newValue));
                    }
                }
                else
                {
                    if (!oldValue.Equals(newValue))
                    {
                        fieldChanges.Add(new FieldChange(field.Name, newValue));
                    }
                }

            }
        }
        foreach (var field in typeof(T).GetProperties())
        {
            //字符串一般处理吧
            if (field.PropertyType.IsEnumerable() && field.PropertyType != typeof(string))
            {
                bool isEqual = true;
                var enumable = (System.Collections.IEnumerable)field.GetValue(old);
                var oldEnumerator = enumable.GetEnumerator();
                var enumable2 = (System.Collections.IEnumerable)field.GetValue(newObj);
                var newEnumerator = enumable2.GetEnumerator();
                //不同步时为true
                var a = oldEnumerator.MoveNext();
                var b = newEnumerator.MoveNext();
                bool nsync = a ^ b;
                //一开始就不一样
                if (nsync)
                {
                    fieldChanges.Add(new FieldChange(field.Name, enumable2));
                }
                else
                {
                    //一样且不为空
                    while (!nsync && a && isEqual)
                    {
                        if (!oldEnumerator.Current.Equals(newEnumerator.Current))
                        {
                            isEqual = false;
                        }
                        a = oldEnumerator.MoveNext();
                        b = newEnumerator.MoveNext();
                        nsync = a ^ b;
                        //不一致时不等
                        if (nsync)
                        {
                            isEqual = false;
                        }
                    }
                }

                if (!isEqual)
                {
                    fieldChanges.Add(new FieldChange(field.Name, enumable2));
                }

            }
            //基本类型
            else
            {
                var oldValue = field.GetValue(old);
                var newValue = field.GetValue(newObj);
                if (oldValue is null)
                {
                    if (newValue is not null)
                    {
                        fieldChanges.Add(new FieldChange(field.Name, newValue));
                    }
                }
                else
                {
                    if (!oldValue.Equals(newValue))
                    {
                        fieldChanges.Add(new FieldChange(field.Name, newValue));
                    }
                }

            }
        }
        return fieldChanges;
    }
    /// <summary>
    /// 用于查找需要更新的字段
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static List<FieldChange> MakeAllFieldChanges<T>(T obj)
    {
        List<FieldChange> fieldChanges = new List<FieldChange>();
        foreach (var item in typeof(T).GetFields())
        {
            if (item.Name == "Id") continue;
            fieldChanges.Add(new FieldChange(item.Name, item.GetValue(obj)));
        }
        foreach (var item in typeof(T).GetProperties())
        {
            fieldChanges.Add(new FieldChange(item.Name, item.GetValue(obj)));
        }
        return fieldChanges;
    }
}
