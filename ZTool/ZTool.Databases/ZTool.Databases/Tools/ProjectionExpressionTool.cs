using System.Linq.Expressions;
using System.Reflection;

namespace ZTool.Databases.Tools;
public static class ProjectionExpressionTool
{
    /// <summary>
    /// 返回空则表示表达式不是成员表达式
    /// 由于表达式返回值可以自动转换返回值
    /// 需要从 UnaryExpression 获取 MemberExpression
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expression"></param>
    /// <returns>若可能不符合表达式返回空</returns>
    public static MemberInfo? GetMember(Expression expression)
    {
        UnaryExpression unaryExpression = expression as UnaryExpression;
        MemberExpression memberExpression;
        if (unaryExpression != null)
        {
            memberExpression = unaryExpression.Operand as MemberExpression;
        }
        else
        {
            memberExpression = expression as MemberExpression;
        }
        if (memberExpression == null) return null;
        return memberExpression.Member;
    }
    public static List<Expression> Parse<T>(Expression<Func<T, object>> fields)
    {
        List<Expression> fieldList = new();
        if (fields.Body is NewExpression ne)
        {
            fieldList = ne.Arguments.Select(ne => ne).ToList();
        }
        else
        {
            fieldList.Add(fields.Body);
        }
        return fieldList;
    }
}
