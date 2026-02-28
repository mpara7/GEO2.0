using System.Linq.Expressions;
using System.Reflection;

namespace ZTool.InnerTypeTools.BaseTyps;
//Expression https://docs.microsoft.com/zh-cn/dotnet/api/system.linq.expressions.expression?view=net-6.0
//单元操作 Operand https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.operandtype?view=net-6.0
public static class ExpressionTool
{
    /// <summary>
    /// 返回空则表示表达式不是成员表达式
    /// 由于表达式返回值可以自动转换返回值
    /// 需要从 UnaryExpression 获取 MemberExpression
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expression"></param>
    /// <returns>若可能不符合表达式返回空</returns>
    public static MemberInfo? GetMember<T>(Expression<Func<T, object>> expression)
    {
        UnaryExpression unaryExpression = expression.Body as UnaryExpression;
        MemberExpression memberExpression;
        if (unaryExpression != null)
        {
            memberExpression = unaryExpression.Operand as MemberExpression;
        }
        else
        {
            memberExpression = expression.Body as MemberExpression;
        }
        if (memberExpression == null) return null;
        return memberExpression.Member;
    }

    /// <summary>
    /// 返回空则表示表达式不是成员表达式
    /// 似乎不支持表达式中的泛型类型推断
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    /// <param name="expression"></param>
    /// <returns>若可能不符合表达式返回空</returns>
    public static MemberInfo GetMember<I, P>(Expression<Func<I, P>> expression)
    {
        MemberExpression memberExpression = expression.Body as MemberExpression;
        if (memberExpression == null) return null;
        return memberExpression.Member;
    }

    /// <summary>
    /// 返回空则表示表达式不是成员表达式
    /// 似乎不支持表达式中的泛型类型推断
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    /// <param name="expression"></param>
    /// <returns>若可能不符合表达式返回空</returns>
    public static MemberInfo GetOper<I>(Expression<Func<I, bool>> expression)
    {
        Console.WriteLine(expression.Parameters);
        BinaryExpression binaryExpression = expression.Body as BinaryExpression;
        var gg = binaryExpression.Right as MemberExpression;
        var value = Expression.Lambda(gg).Compile().DynamicInvoke();
        if (binaryExpression == null) return null;

        MemberExpression memberExpression = expression.Body as MemberExpression;

        return memberExpression.Member;
    }
}
