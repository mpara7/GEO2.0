namespace ZTool.InnerTypeTools.BaseTyps;

public static class CharTool
{
    public static List<int> GetCharPositions(char c, string source)
    {
        var result = new List<int>();
        int index = 0;
        foreach (var itr in source)
        {
            if (itr == c)
            {
                result.Add(index);
            }
            index++;
        }
        return result;
    }
    public static bool IsChinese(char c)
    {
        if (c >= 0x4e00 && c <= 0x9FFF)
            return true;
        else
            return false;
    }

    public static bool IsCapital(char c)
    {
        if (c >= 'A' && c <= 'Z')
            return true;
        else
            return false;
    }
    public static bool IsDigit(char c)
    {
        if (c >= '0' && c <= '9')
            return true;
        else
            return false;
    }

}
