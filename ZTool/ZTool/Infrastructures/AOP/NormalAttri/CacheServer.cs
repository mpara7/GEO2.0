namespace ZTool.Infrastructures.AOP.NormalAttri;
/// <summary>
/// 当一个函数的计算量比较大时 且 结果只与参数相关 时可以使用
/// CacheServer将记录调用的参数并在下一次遇到同样的参数时直接返回上一次的结果
/// PS 部分参数可能需要重写Equals方法
/// 使用方法
/// 使用 要缓存的计算函数 构造 CacheServer对象
/// 调用 .Call并传入参数 当作正常参数使用
/// </summary>
public abstract class CacheServerBase
{
    public int MaxCacheNum { get; set; }
}

public class CacheServer<T, R> : CacheServerBase
{
    Dictionary<T, R> HistoryResults = new Dictionary<T, R>();
    public R Get(T a1, out bool found)
    {
        if (HistoryResults.ContainsKey(a1))
        {
            found = true;
            return HistoryResults[a1];
        }
        found = false;
        return default;
    }
    public void Set(T a1, R result)
    {
        if (HistoryResults.ContainsKey(a1))
        {
            HistoryResults[a1] = result;
        }
        else
        {
            HistoryResults.Add(a1, result);
        }
    }
}
/// <summary>
/// 基础类型需要最好写成int?
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
/// <typeparam name="R"></typeparam>
public class CacheServer<T1, T2, R> : CacheServerBase
{
    struct T<T1, T2>
    {
        public T1 A1 { get; set; }
        public T2 A2 { get; set; }
    }
    Dictionary<T<T1, T2>, R> HistoryResults = new();
    public R Get(T1 a1, T2 a2, out bool found)
    {
        var key = new T<T1, T2>() { A1 = a1, A2 = a2 };
        if (HistoryResults.ContainsKey(key))
        {
            found = true;
            return HistoryResults[key];
        }
        found = false;
        return default;
    }
    public void Set(T1 a1, T2 a2, R result)
    {
        var key = new T<T1, T2>() { A1 = a1, A2 = a2 };
        if (HistoryResults.ContainsKey(key))
        {
            HistoryResults[key] = result;
        }
        else
        {
            HistoryResults.Add(key, result);
        }
    }
}
public class CacheServer<T1, T2, T3, R> : CacheServerBase
{
    struct T<T1, T2, T3>
    {
        public T1 A1 { get; set; }
        public T2 A2 { get; set; }
        public T3 A3 { get; set; }
    }
    Dictionary<T<T1, T2, T3>, R> HistoryResults = new();
    public R Get(T1 a1, T2 a2, T3 a3, out bool found)
    {
        var key = new T<T1, T2, T3>() { A1 = a1, A2 = a2, A3 = a3 };
        if (HistoryResults.ContainsKey(key))
        {
            found = true;
            return HistoryResults[key];
        }
        found = false;
        return default;
    }
    public void Set(T1 a1, T2 a2, T3 a3, R result)
    {
        var key = new T<T1, T2, T3>() { A1 = a1, A2 = a2, A3 = a3 };
        if (HistoryResults.ContainsKey(key))
        {
            HistoryResults[key] = result;
        }
        else
        {
            HistoryResults.Add(key, result);
        }
    }
}