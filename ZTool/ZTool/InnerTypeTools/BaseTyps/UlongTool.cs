namespace ZTool.InnerTypeTools.BaseTyps;

public static class UlongTool
{
    public static string UlongToStr(ulong value)
    {
        string strResult = "";
        var bytes = BitConverter.GetBytes(value);
        foreach (var b in bytes)
        {
            var partial = Convert.ToString(b, 2).PadLeft(8, '0');
            strResult = partial + strResult;
        }
        return strResult;
    }
}
