using MongoDB.Bson;
using MongoDB.Driver;

using System.Linq.Expressions;
using System.Reflection;
using ZTool.Databases.Tools;

namespace ZTool.Databases.Mongo;

/// <summary>
/// 由于只分析二元表达式，过滤谓词需要(bool)==false 不能直接!(bool)
/// </summary>
/// <typeparam name="T"></typeparam>
public class MongoCollection<T> : ADataCollection<T> where T : ADatabaseObject
{
    public MongoConnectionConfig Config { get; init; }
    protected MongoClient Client { get; set; }
    protected IMongoDatabase Database { get; set; }
    protected IMongoCollection<T> Collection { get; set; }
    string collectionName;

    /// <summary>
    /// 需要通过get方法获取
    /// </summary>
    public MongoCollection(MongoConnectionConfig config)
    {
        collectionName = config.CollectionName;

        MongoClientGetter clientGetter = new MongoClientGetter();
        clientGetter.Host = config.MongoUrl;
        clientGetter.UserName = config?.UserName;
        clientGetter.Password = config?.Password;

        Config = config;
        Client = clientGetter.GetMongoClient();
        Database = Client.GetDatabase(config.DatabaseName);
        Collection = Database.GetCollection<T>(config.CollectionName);
    }
    public void UnsetFields<New>()
    {
        Dictionary<string, Type> oldProps = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance).ToDictionary(f => f.Name, f => f.FieldType);
        Dictionary<string, Type> newProps = typeof(New).GetFields(BindingFlags.Public | BindingFlags.Instance).ToDictionary(f => f.Name, f => f.FieldType);
        foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            oldProps.Add(propertyInfo.Name, propertyInfo.PropertyType);
        }
        foreach (var propertyInfo in typeof(New).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            newProps.Add(propertyInfo.Name, propertyInfo.PropertyType);
        }

        List<UpdateDefinition<T>> updates = new List<UpdateDefinition<T>>();
        foreach (var old in oldProps)
        {
            //旧字段不包含 类型不符合
            if (!newProps.ContainsKey(old.Key) || newProps[old.Key] != old.Value)
            {
                var update = Builders<T>.Update.Unset(old.Key);
                updates.Add(update);
            }
        }
        var Collection = Database.GetCollection<T>(collectionName);
        foreach (var id in FindAllID())
        {
            foreach (var update in updates)
            {
                Collection.FindOneAndUpdate<T>(MongoTool<T>.IDFilter(id), update);
            }
        }
    }
    public override void UpdateSturcture<New>(Func<T, New> mapper)
    {

        var Collection = Database.GetCollection<New>(collectionName);
        foreach (var id in FindAllID())
        {
            var t = Find(id);
            var newObj = mapper.Invoke(t);
            Delete(id);
            newObj.Id = id;
            Collection.InsertOne(newObj);
        }
    }
    /// <summary>
    /// 如果是id空白的则会分配一个
    /// </summary>
    /// <param name="dataObject"></param>
    /// <returns></returns>
    public override string Insert(T dataObject)
    {
        if (dataObject.Id is null)
            dataObject.Id = ObjectId.GenerateNewId().ToString();
        Collection.InsertOne(dataObject);
        return dataObject.Id;
    }
    public override void Delete(string id)
    {
        Collection.DeleteOne(MongoTool<T>.IDFilter(id));
    }
    public override void Delete(T obj)
    {
        Collection.DeleteOne(MongoTool<T>.IDFilter(obj.Id));
    }
    public override void DeleteWhere(Expression<Predicate<T>> predicate = null)
    {
        Collection.DeleteMany(MongoTool<T>.MakeFilter(predicate));
    }

    public void Update(string id, T oldObj, T newObj)
    {
        Update(id, FieldChangeTool.GetFieldChanges(oldObj, newObj));
    }
    public override void Update(string id, List<FieldChange> changes)
    {
        var fieldUpdates = MongoTool<T>.GetFieldUpdates(changes);
        foreach (var update in fieldUpdates)
        {
            Collection.FindOneAndUpdate<T>(MongoTool<T>.IDFilter(id), update);
        }
    }
    public override void Update(string id, T obj)
    {
        var changes = FieldChangeTool.MakeAllFieldChanges(obj);
        Update(id, changes);
    }
    public override void Update(T obj)
    {
        Update(obj.Id, obj);
    }

    public override long GetCount(Expression<Predicate<T>> predicate = null)
    {
        return Collection.CountDocuments(MongoTool<T>.MakeFilter(predicate));
    }

    public override T Find(string id, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true)
    {
        return _anyfind(MongoTool<T>.IDFilter(id), MongoTool<T>.MakeProjection(fields, include_OR_exculde)).FirstOrDefault();
    }
    public override T FindOneWhere(Expression<Predicate<T>> predicate, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true)
    {
        return FindWhere(predicate, fields, include_OR_exculde).FirstOrDefault();
    }
    public override List<T> FindWhere(FilterDefinition<T> filter = null, int? pageSize = null, int? pageIndex = null)
    {
        List<T> result;
        var fluent = Collection.Find(filter ?? FilterDefinition<T>.Empty);
        if (pageSize is not null && pageIndex is not null)
        {
            fluent = fluent.Skip(pageSize * pageIndex)
                .Limit(pageSize);
        }
        return fluent.ToList();
    }
    public override List<T> FindWhere(Expression<Predicate<T>> predicate, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true)
    {
        return anyfind(predicate, null, null, fields, include_OR_exculde);
    }
    public override List<T> FindPage(int pageSize, int pageIndex, Expression<Predicate<T>> predicate, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true)
    {
        return anyfind(predicate, pageSize, pageIndex, fields, include_OR_exculde);
    }
    public override List<string> FindAllID()
    {
        return FindAll(t => t.Id).Select(i => i.Id).ToList();
    }
    public override List<T> FindAll(Expression<Func<T, object>> fields = null, bool include_OR_exculde = true)
    {
        return anyfind(null, null, null, fields, include_OR_exculde);
    }

    protected virtual List<T> anyfind(Expression<Predicate<T>> predicate = null, int? pageSize = null, int? pageIndex = null, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true)
    {
        ProjectionDefinition<T> projection = MongoTool<T>.MakeProjection(fields, include_OR_exculde);
        FilterDefinition<T> conditionFilter = null;
        if (predicate is not null)
        {
            conditionFilter = MongoTool<T>.MakeFilter(predicate);
        }
        return _anyfind(conditionFilter, projection, pageSize, pageIndex);
    }
    protected virtual List<T> _anyfind(FilterDefinition<T> filter = null, ProjectionDefinition<T> projection = null, int? pageSize = null, int? pageIndex = null)
    {
        List<T> result;
        var fluent = Collection.Find(filter ?? FilterDefinition<T>.Empty);
        if (pageSize is not null && pageIndex is not null)
        {
            fluent = fluent.Skip(pageSize * pageIndex)
                .Limit(pageSize);
        }
        if (projection is not null)
            fluent = fluent.Project<T>(projection);
        return fluent.ToList();
    }


}
