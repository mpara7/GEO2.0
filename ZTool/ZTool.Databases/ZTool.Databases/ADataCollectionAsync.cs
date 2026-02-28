using System.Linq.Expressions;

namespace ZTool.Databases;
/// <summary>
/// 异步版本的数据集合
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ADataCollectionAsync<T> where T : ADatabaseObject
{
    /// <summary>
    /// 插入
    /// </summary>
    /// <param name="dataObject"></param>
    /// <returns>id</returns>
    public abstract Task<string> Insert(T dataObject);
    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id"></param>
    public abstract Task Delete(string id);
    /// <summary>
    /// 条件删除
    /// </summary>
    /// <param name="id"></param>
    public abstract Task DeleteWhere(Expression<Predicate<T>> predicate);
    /// <summary>
    /// 部分修改
    /// </summary>
    /// <param name="id"></param>
    public abstract Task Update(string id, List<FieldChange> changes);
    /// <summary>
    /// 部分修改
    /// </summary>
    /// <param name="id"></param>
    public abstract Task Update(string id, T obj);
    /// <summary>
    /// 部分修改
    /// </summary>
    /// <param name="id"></param>
    public abstract Task Update(T obj);
    /// <summary>
    /// 总数
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public abstract Task<long> GetCount(Expression<Predicate<T>> predicate = null);
    /// <summary>
    /// 查询，若不存在返回空
    /// </summary>
    /// <param name="id"></param>
    /// <param name="fields">默认为选择全部字段，规范：多值映射：m => new { m.Weight, m.Id } 单值映射：m =>m.Weight</param>
    /// <param name="include_OR_exculde">默认为包含模式</param>
    /// <returns></returns>
    public abstract Task<T> Find(string id, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true);
    /// <summary>
    /// 带条件查询
    /// </summary>
    /// <param name="predicate">默认为任意，规范：单条件：m => m.Weight == 20 多条件：m => m.Weight == 20 && m.Age >= 3</param>
    /// <param name="fields">默认为选择全部字段，规范：多值映射：m => new { m.Weight, m.Id } 单值映射：m =>m.Weight</param>
    /// <param name="include_OR_exculde">默认为包含模式</param>
    /// <returns></returns>
    public abstract Task<T> FindOneWhere(Expression<Predicate<T>> predicate, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true);
    /// <summary>
    /// 条件查询
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public abstract Task<List<T>> FindWhere(Expression<Predicate<T>> predicate, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true);
    public abstract Task<List<T>> FindPage(int pageSize, int pageIndex, Expression<Predicate<T>> predicate, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true);
    /// <summary>
    /// 查询所有ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public abstract Task<List<string>> FindAllID();
    /// <summary>
    /// 查询所有
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public abstract Task<List<T>> FindAll(Expression<Func<T, object>> fields = null, bool include_OR_exculde = true);
    /// <summary>
    /// 根据类型更新表的结构
    /// </summary>
    public abstract Task UpdateSturcture<New>(Func<T, New> mapper) where New : ADatabaseObject;
}
