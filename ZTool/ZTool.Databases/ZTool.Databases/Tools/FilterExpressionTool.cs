using System.Linq.Expressions;
using System.Reflection;

namespace ZTool.Databases.Tools;
public static class FilterExpressionTool
{
    public static List<(ExpressionType type, object left, object right)> Parse<T>(Expression<Predicate<T>> predicate)
    {
        List<(ExpressionType type, object left, object right)> values = new List<(ExpressionType type, object left, object right)>();
        if (predicate is null) return values;
        if (predicate.Body is BinaryExpression b1 && b1.NodeType == ExpressionType.AndAlso)
        {
            values = CrackAndAlsoExpression(b1);
        }
        else if (predicate.Body is BinaryExpression b2)
        {
            var value = CrackBinaryExpression(b2);
            values.Add(value);
        }
        values.Reverse();
        return values;
    }
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
    public static (ExpressionType type, object left, object right) CrackBinaryExpression(BinaryExpression b)
    {
        var actrul = Expression.Lambda(b.Right);
        var value = actrul.Compile().DynamicInvoke();
        var m = GetMember(b.Left);
        return (b.NodeType, b.Left, value);
    }
    public static void RecursionAndAlsoExpression(List<BinaryExpression> expressions, BinaryExpression b)
    {
        if (b.NodeType == ExpressionType.AndAlso)
        {
            if (b.Right is BinaryExpression rightB)
                expressions.Add(rightB);
            if (b.Left is BinaryExpression bb)
            {
                if (bb.NodeType == ExpressionType.AndAlso)
                {
                    RecursionAndAlsoExpression(expressions, bb);
                }
                else
                {
                    expressions.Add(bb);
                }
            }
        }
    }
    public static List<(ExpressionType type, object left, object right)> CrackAndAlsoExpression(BinaryExpression b)
    {
        List<BinaryExpression> expressions = new List<BinaryExpression>();
        RecursionAndAlsoExpression(expressions, b);
        return expressions.Select(e => CrackBinaryExpression(e)).ToList();
    }
}
