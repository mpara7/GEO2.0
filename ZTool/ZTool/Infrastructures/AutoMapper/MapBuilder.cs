using System.Linq.Expressions;
using System.Reflection;

namespace ZTool.Infrastructures.AutoMapper
{
    public class MapBuilder<From, To>
    {
        public ClassMap Map { get; set; }
        public MapBuilder(ZMapper zMapper)
        {
            Map = new ClassMap() { FromType = typeof(From), ToType = typeof(To), ZMapper = zMapper };
        }
        public MapBuilder<From, To> NotMapProp(Expression<Func<To, object>> toExpr)
        {
            var (toMember, _) = ParseExpr(toExpr);
            Map.NotConfigPropMaps.Add(toMember);
            return this;
        }
        /// <summary>
        /// 指定不同名的映射关系
        /// </summary>
        /// <param name="fromExpr"></param>
        /// <param name="toExpr"></param>
        /// <returns></returns>
        public MapBuilder<From, To> MapProp(Expression<Func<From, object>> fromExpr, Expression<Func<To, object>> toExpr)
        {
            var (fromMember, _) = ParseExpr(fromExpr);
            var (toMember, _) = ParseExpr(toExpr);
            Map.ConfigPropMaps.Add((fromMember, toMember));
            return this;
        }
        /// <summary>
        /// 最终处理
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public MapBuilder<From, To> Final(Action<From, To> action)
        {
            Map.Final = (from, to) => action((From)from, (To)to);
            return this;
        }
        static (PropertyInfo, Type) ParseExpr<F, T>(Expression<Func<F, T>> toExpr)
        {
            if (toExpr.Body is MemberExpression m)
            {
                if (m.Member is PropertyInfo p)
                {
                    return (p, p.PropertyType);
                }
                else if (m.Member is FieldInfo f)
                {
                    throw new InvalidOperationException($"{toExpr}不是属性表达式");
                }
            }
            else if (toExpr.Body is UnaryExpression u)
            {
                if (u.Operand is MemberExpression m1)
                {
                    if (m1.Member is PropertyInfo p1)
                    {
                        return (p1, p1.PropertyType);
                    }
                    else if (m1.Member is FieldInfo f1)
                    {
                        throw new InvalidOperationException($"{toExpr}不是属性表达式");
                    }
                }
            }
            throw new InvalidOperationException($"{toExpr}不是合法的成员表达式");
        }
    }
}
