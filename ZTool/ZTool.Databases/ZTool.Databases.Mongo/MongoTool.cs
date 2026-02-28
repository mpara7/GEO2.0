using MongoDB.Bson;
using MongoDB.Driver;

using System.Linq.Expressions;

using ZTool.Databases.Tools;

namespace ZTool.Databases.Mongo;
public class MongoTool
{
    public static string NewID()
    {
        return ObjectId.GenerateNewId().ToString();
    }
}
internal class MongoTool<T> where T : ADatabaseObject
{

    #region Filter
    public static FilterDefinition<T> IDFilter(string id)
    {
        return Builders<T>.Filter.Eq("Id", new ObjectId(id));
    }

    public static FilterDefinition<T> EmptyFilter = Builders<T>.Filter.Empty;

    /// <summary>
    /// 用于查找需要更新的字段
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static FilterDefinition<T> MakeFilter(Expression<Predicate<T>> predicate)
    {
        var filteList = FilterExpressionTool.Parse(predicate);
        FilterDefinition<T> filter;
        FilterDefinition<T> buildFilter((ExpressionType type, object left, object right) item)
        {
            var m = item.left as MemberExpression;
            if (m is null)
                throw new ArgumentException();
            switch (item.type)
            {
                case ExpressionType.Equal:
                    return Builders<T>.Filter.Eq(m.Member.Name, item.right);
                case ExpressionType.GreaterThan:
                    return Builders<T>.Filter.Gt(m.Member.Name, item.right);
                case ExpressionType.GreaterThanOrEqual:
                    return Builders<T>.Filter.Gte(m.Member.Name, item.right);
                case ExpressionType.LessThan:
                    return Builders<T>.Filter.Lt(m.Member.Name, item.right);
                case ExpressionType.LessThanOrEqual:
                    return Builders<T>.Filter.Lte(m.Member.Name, item.right);
                case ExpressionType.NotEqual:
                    return Builders<T>.Filter.Not(Builders<T>.Filter.Eq(m.Member.Name, item.right));
                default:
                    throw new ArgumentException($"不支持的表达式类型 {item.type}");
            }
        }
        if (filteList.Count == 0)
        {
            filter = Builders<T>.Filter.Empty;
        }
        else if (filteList.Count == 1)
        {
            filter = buildFilter(filteList[0]);
        }
        else
        {
            filter = Builders<T>.Filter.And(filteList.Select(i => buildFilter(i)));
        }
        return filter;
    }
    #endregion

    /// <summary>
    /// 用于查找需要更新的字段
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static ProjectionDefinition<T> MakeProjection(Expression<Func<T, object>> fieldsExpression, bool include_OR_exculde = true)
    {
        if (fieldsExpression is null) return null;

        ProjectionDefinition<T> projection = null;
        var fields = ProjectionExpressionTool.Parse(fieldsExpression);
        if (include_OR_exculde)
        {
            var builder = Builders<T>.Projection;
            var firstField = true;
            foreach (var field in fields)
            {
                var fieldName = string.Empty;
                if (field is UnaryExpression u)
                {
                    if (u.NodeType == ExpressionType.Convert)
                    {
                        if (u.Operand is MemberExpression m)
                        {
                            fieldName = m.Member.Name;
                        }
                    }
                }
                else if (field is MemberExpression m)
                {
                    fieldName = m.Member.Name;
                }
                var m2 = field as MemberExpression;
                if (firstField)
                {
                    projection = builder.Include(fieldName);
                    firstField = false;
                }
                else
                {
                    projection = projection.Include(fieldName);
                }
            }
        }
        else
        {
            var builder = Builders<T>.Projection;
            var firstField = true;
            foreach (var field in fields)
            {
                var fieldName = string.Empty;
                if (field is UnaryExpression u)
                {
                    if (u.NodeType == ExpressionType.Convert)
                    {
                        if (u.Operand is MemberExpression m)
                        {
                            fieldName = m.Member.Name;
                        }
                    }
                }
                else if (field is MemberExpression m)
                {
                    fieldName = m.Member.Name;
                }
                var m2 = field as MemberExpression;
                if (firstField)
                {
                    projection = builder.Exclude(fieldName);
                    firstField = false;
                }
                else
                {
                    projection = projection.Exclude(fieldName);
                }
            }
        }
        return projection;
    }

    /// <summary>
    /// 用于查找需要更新的字段
    /// 如果对象为空可能会由于无法推断数据类型而报错
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static List<UpdateDefinition<T>> GetFieldUpdates(IEnumerable<FieldChange> fieldChanges)
    {
        List<UpdateDefinition<T>> updateDefinitions = new();
        var fieldName = "";
        foreach (var fieldChange in fieldChanges)
        {
            fieldName = fieldChange.Field;
            var update = Builders<T>.Update.Set(fieldChange.Field, fieldChange.NewValue);
            updateDefinitions.Add(update);
        }

        try
        {

        }
        catch (Exception ex)
        {
            throw new Exception(innerException: ex, message: fieldName) { };
        }


        return updateDefinitions;
    }


}
