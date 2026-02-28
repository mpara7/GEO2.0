namespace ZTool.InnerTypeTools.CollectionTypes;
public static class ListTool
{
    public static IEnumerable<(int index, T item)> Indexlize<T>(this IEnumerable<T> enumerable)
    {
        int i = 0;
        foreach (var item in enumerable)
        {
            yield return (i++, item);
        }
    }

    /// <summary>
    /// 获取对象在队列中的前后对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ts"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static (T?, T?) FinePNObj<T>(List<T> values, T obj)
    {
        int index = values.IndexOf(obj);
        T p = default;
        T n = default;
        if (index > 0)
            p = values[index - 1];
        if (index < values.Count - 1)
            n = values[index + 1];
        return (p, n);
    }


    /// <summary>
    /// 组合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<List<T>> GetCombination<T>(List<T> source, int num)
    {
        List<List<T>> result = new List<List<T>>();
        void innerCombination(List<T> list, int nCount, List<T> group = null, int head = 0)
        {
            if (group == null)
            {
                group = new List<T>();
            }
            //数量够的就直接输出
            if (group.Count == nCount)
            {
                result.Add(new List<T>(group));
            }
            else
            {
                for (int i = head; i < list.Count; i++)
                {
                    //数量不够继续递归
                    if (group.Count < nCount)
                    {
                        group.Add(list[i]);
                        innerCombination(list, nCount, group, i + 1);
                        group.RemoveAt(group.Count - 1);
                    }
                }
            }
        }
        innerCombination(source, num);
        return result;
    }
    /// <summary>
    /// 全组合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<List<T>> GetAllCombination<T>(List<T> source)
    {
        List<List<T>> result = new List<List<T>>();
        for (int i = 0; i < source.Count; i++)
        {
            result.AddRange(GetCombination(source, i));
        }
        return result;
    }

    /// <summary>
    /// 排列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<List<T>> GetArrangement<T>(List<T> source, int num)
    {
        List<List<T>> result = new List<List<T>>();

        void innerArrangement(List<T> list, List<T> group = null)
        {
            if (group == null)
            {
                group = new List<T>();
            }
            //数量够的就直接输出
            if (group.Count == num)
            {
                result.Add(new List<T>(group));
            }
            else
            {
                foreach (var item in list)
                {
                    //数量不够继续递归
                    if (group.Count < num)
                    {
                        var sub = new List<T>(list);
                        group.Add(item);
                        sub.Remove(item);
                        innerArrangement(sub, group);
                        group.Remove(item);
                        sub.Add(item);
                    }
                }
            }
        }
        innerArrangement(source);
        return result;
    }
    /// <summary>
    /// 全排列
    /// </summary>
    public static List<List<T>> AllArrangement<T>(List<T> source)
    {
        List<List<T>> result = new List<List<T>>();
        for (int i = 0; i < source.Count; i++)
        {
            result.AddRange(GetArrangement(source, i));
        }
        return result;
    }
    /// <summary>
    /// 笛卡尔积
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<List<object>> CartesianProduct(List<List<object>> sources)
    {
        List<List<object>> result = new List<List<object>>();
        void innerCartesianProduct(List<object> group, int layer)
        {
            //到达最终层
            if (layer == sources.Count)
            {
                result.Add(group);
                return;
            }
            else
            {
                foreach (var item in sources[layer])
                {
                    var groupPlus = new List<object>(group);
                    groupPlus.Add(item);
                    innerCartesianProduct(groupPlus, layer + 1);
                }

            }
        }
        innerCartesianProduct(new List<object>(), 0);

        return result;
    }
    /// <summary>
    /// 笛卡尔幂
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<List<T>> CartesianPower<T>(List<T> source, int power)
    {
        List<List<T>> result = new List<List<T>>();
        void innerCartesianPower(List<T> group, int power)
        {
            if (power == 0)
            {
                result.Add(group);
                return;
            }
            else if (power > 0)
            {
                foreach (var item in source)
                {
                    var groupPlus = new List<T>(group);
                    groupPlus.Add(item);
                    innerCartesianPower(groupPlus, power - 1);
                }
            }
        }
        innerCartesianPower(new List<T>(), power);
        return result;
    }
}
