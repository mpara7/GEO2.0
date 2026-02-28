namespace ZTool.InnerTypeTools.BaseTyps;

public static class StringTool
{
    public static string SafeReplace(string toReplace, Dictionary<string, string> replaceDict)
    {
        Dictionary<string, string> atob = new Dictionary<string, string>();
        Dictionary<string, string> btoc = new Dictionary<string, string>();

        int i = 0;
        foreach (var item in replaceDict)
        {
            atob.Add(item.Key, $"@{i}");
            btoc.Add($"@{i}", item.Value);
            i++;
        }
        foreach (var item in atob)
        { toReplace = toReplace.Replace(item.Key, item.Value); }
        foreach (var item in btoc)
        { toReplace = toReplace.Replace(item.Key, item.Value); }
        return toReplace;
    }

    /// <summary>
    /// StringList:[a,b,c,d],Separator:(" ")->"a b c d"
    /// </summary>
    public static string ComposeList<T>(IEnumerable<T> list, string separator = ",", Func<T, string> toString = null)
    {
        var result = "";
        bool first = true;
        foreach (var item in list)
        {
            if (first)
            {
                if (toString is not null)
                {
                    result += toString(item);
                }
                else
                {
                    result += item;
                }

                first = false;
            }
            else
            {
                if (toString is not null)
                {
                    result += $"{separator}{toString(item)}";
                }
                else
                {
                    result += $"{separator}{item}";
                }
            }

        }
        return result;
    }
    public static bool HasChinese(string str)
    {
        foreach (var c in str)
        {
            if (CharTool.IsChinese(c))
                return true;
        }
        return false;
    }
    public static string ConvertToBase64(this string str)
    {
        byte[] b = System.Text.Encoding.Default.GetBytes(str);
        return Convert.ToBase64String(b);
    }
    public static string ConvertFromBase64(this string str64)
    {
        byte[] b = Convert.FromBase64String(str64);
        return System.Text.Encoding.Default.GetString(b);
    }

    public static List<string> GetAToZList()
    {
        List<string> charList = new List<string>();
        for (char c = 'A'; c <= 'Z'; c++)
        {
            charList.Add(c.ToString());
        }
        return charList;
    }
    public static List<string> GetaTozList()
    {
        List<string> charList = new List<string>();
        for (char c = 'a'; c <= 'z'; c++)
        {
            charList.Add(c.ToString());
        }
        return charList;
    }
}
