using MongoDB.Driver;

using System.Linq.Expressions;

namespace ZTool.Databases;
/// <summary>
/// 抽象数据库
/// 自动创建数据库
/// 默认一个ADatabase对于一张表
/// </summary>
/// <typeparam name="T">存数据库的对象模型每个字段（属性）能一一对应</typeparam>
public abstract class ADataCollection<T> where T : ADatabaseObject
{
    /// <summary>
    /// 增加
    /// </summary>
    /// <param name="dataObject"></param>
    public abstract string Insert(T dataObject);
    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id"></param>
    public abstract void Delete(string id);
    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id"></param>
    public abstract void Delete(T obj);
    /// <summary>
    /// 条件删除
    /// </summary>
    /// <param name="id"></param>
    public abstract void DeleteWhere(Expression<Predicate<T>> predicate);
    /// <summary>
    /// 部分修改
    /// </summary>
    /// <param name="id"></param>
    public abstract void Update(string id, List<FieldChange> changes);
    /// <summary>
    /// 部分修改
    /// </summary>
    /// <param name="id"></param>
    public abstract void Update(string id, T obj);
    /// <summary>
    /// 部分修改
    /// </summary>
    /// <param name="id"></param>
    public abstract void Update(T obj);
    public abstract long GetCount(Expression<Predicate<T>> predicate = null);
    /// <summary>
    /// 查询，若不存在返回空
    /// </summary>
    /// <param name="id"></param>
    /// <param name="fields">默认为选择全部字段，规范：多值映射：m => new { m.Weight, m.Id } 单值映射：m =>m.Weight</param>
    /// <param name="include_OR_exculde">默认为包含模式</param>
    /// <returns></returns>
    public abstract T Find(string id, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate">默认为任意，规范：单条件：m => m.Weight == 20 多条件：m => m.Weight == 20 && m.Age >= 3</param>
    /// <param name="fields">默认为选择全部字段，规范：多值映射：m => new { m.Weight, m.Id } 单值映射：m =>m.Weight</param>
    /// <param name="include_OR_exculde">默认为包含模式</param>
    /// <returns></returns>
    public abstract T FindOneWhere(Expression<Predicate<T>> predicate, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true);
    public abstract List<T> FindWhere(FilterDefinition<T> filter = null, int? pageSize = null, int? pageIndex = null);
    /// <summary>
    /// 条件查询
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public abstract List<T> FindWhere(Expression<Predicate<T>> predicate, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true);
    public abstract List<T> FindPage(int pageSize, int pageIndex, Expression<Predicate<T>> predicate, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true);
    /// <summary>
    /// 查询所有ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public abstract List<string> FindAllID();
    /// <summary>
    /// 查询所有
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public abstract List<T> FindAll(Expression<Func<T, object>> fields = null, bool include_OR_exculde = true);
    /// <summary>
    /// 根据类型更新表的结构
    /// </summary>
    public abstract void UpdateSturcture<New>(Func<T, New> mapper) where New : ADatabaseObject;
}
