using System.Reflection;

namespace ZTool.UsefulTypes;

/// <summary>
/// 描述
/// 使用:[Description("描述1","描述2",...)]
/// </summary>
public class DescriptionAttribute : Attribute
{
    public string Description { get => Descriptions[0]; }
    public string[] Descriptions { get; set; }
    public DescriptionAttribute(params string[] descriptions)
    {
        Descriptions = descriptions;
    }
    public static string GetDescription<T>()
    {
        var attri = typeof(T).GetCustomAttribute<DescriptionAttribute>(false);
        if (attri != null)
        {
            return attri.Description;
        }
        return typeof(T).Name;
    }
    public static string GetDescription(Type selectedType)
    {
        var attri = selectedType.GetCustomAttribute<DescriptionAttribute>(false);
        if (attri != null)
        {
            return attri.Description;
        }
        return selectedType.Name;
    }
    public static string[] GetDescriptions(Type selectedType)
    {
        var attri = selectedType.GetCustomAttribute<DescriptionAttribute>(false);
        if (attri != null)
        {
            return attri.Descriptions;
        }
        return new string[] { };
    }
}
public static class DescriptionAttributeTool
{
    /// <summary>
    /// 从程序集中搜索所有类,如果其带有中文名标签则添加到词典中
    /// </summary>
    /// <param name="assemblies">待搜索的程序集</param>
    /// <param name="filter">过滤条件</param>
    /// <returns></returns>
    public static Dictionary<string, string[]> GetClassDescriptions(IEnumerable<Assembly> assemblies, Func<Type, bool>? filter = null)
    {
        if (filter is null)
            filter = type => true;
        var result = assemblies
            .SelectMany(assemble => assemble.GetTypes())
            .Where(type => type.GetCustomAttribute(typeof(DescriptionAttribute)) is not null)
            .Select(member => (member.Name, ((DescriptionAttribute)member.GetCustomAttribute(typeof(DescriptionAttribute))).Descriptions))//keySelector: (a, b) => a,elementSelector:(a, b) => b
            .ToDictionary(x => x.Name, x => x.Descriptions);
        return result;
    }

    /// <summary>
    /// 从程序集中搜索所有类,使用mapper指定搜索的成员信息并判断时候满足条件,如果其带有中文名标签则添加到词典中
    /// </summary>
    /// <param name="assemblies">待搜索的程序集</param>
    /// <param name="mapper">由用户指定从类型到所需成员的映射</param>
    /// <param name="filter">过滤条件</param>
    /// <returns></returns>
    public static Dictionary<string, string[]> GetClassDescriptions(IEnumerable<Assembly> assemblies, Func<Type, List<MemberInfo>> mapper, Func<MemberInfo, bool>? filter = null)
    {
        if (filter is null)
            filter = member => true;
        Dictionary<string, List<string>> ChineseNames = new Dictionary<string, List<string>>();
        var result = assemblies
            .SelectMany(assemble => assemble.GetTypes())
            .SelectMany(type => mapper(type))
            .Where(member => member.GetCustomAttribute(typeof(DescriptionAttribute)) is not null)
            .Where(member => filter(member))
            .Select(member => (member.Name, ((DescriptionAttribute)member.GetCustomAttribute(typeof(DescriptionAttribute))).Descriptions))
            .ToDictionary(x => x.Name, x => x.Descriptions);
        return result;
    }
}