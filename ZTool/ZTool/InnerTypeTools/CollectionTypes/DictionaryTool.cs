namespace ZTool.InnerTypeTools.CollectionTypes;
public static class DictionaryTool
{
    /// <summary>
    /// 计数列表中的元素与出现次数
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static void Categorize<K, V>(Dictionary<K, List<V>> dict, K key, V value)
    {
        if (dict.ContainsKey(key))
        {
            dict[key].Add(value);
        }
        else
        {
            dict.Add(key, new List<V>());
        }
    }

    /// <summary>
    /// 计数列表中的元素与出现次数
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static Dictionary<K, int> CountItemNum<K>(IEnumerable<K> list)
    {
        Dictionary<K, int> dict = new Dictionary<K, int>();
        foreach (var item in list)
        {
            if (dict.ContainsKey(item))
            {
                dict[item]++;
            }
            else
            {
                dict.Add(item, 1);
            }
        }
        return dict;
    }
}
