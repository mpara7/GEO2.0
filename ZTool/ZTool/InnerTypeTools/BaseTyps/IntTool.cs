namespace ZTool.InnerTypeTools.BaseTyps;
public static class IntTool
{
    public static IEnumerator<int> GetEnumerator(this int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return i;
        }
    }
}
