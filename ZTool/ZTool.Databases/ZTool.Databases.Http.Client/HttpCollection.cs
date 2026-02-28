using Newtonsoft.Json;

using System.Linq.Expressions;

using ZTool.Databases.Tools;

namespace ZTool.Databases.Http.Client;
/// <summary>
/// 需要路由为对象名称
/// </summary>
/// <typeparam name="T"></typeparam>
public class HttpCollection<T> : ADataCollectionAsync<T> where T : ADatabaseObject
{
    string dataObjectTypeStr;
    HttpClient httpClient;
    public HttpCollection(HttpClient httpClient)
    {
        dataObjectTypeStr = typeof(T).Name.Replace("DAO","");
        this.httpClient = httpClient;

    }
    public HttpCollection(HttpCollectionConfig config)
    {
        dataObjectTypeStr = typeof(T).Name;
        this.httpClient = new HttpClient() { BaseAddress = new Uri(config.BaseAddress) };

    }
    public override async Task Delete(string id)
    {
        await httpClient.DeleteAsync($"{dataObjectTypeStr}/{id}");
    }
    
    [Obsolete("未实现", error: true)]
    public override Task DeleteWhere(Expression<Predicate<T>> predicate)
    {
        throw new NotImplementedException();
    }

    public override async Task<T> Find(string id, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true)
    {
        return await httpClient.GetFromNewtonJsonAsync<T>($"{dataObjectTypeStr}/{id}");
    }

    public override async Task<List<T>> FindAll(Expression<Func<T, object>> fields = null, bool include_OR_exculde = true)
    {
        return await httpClient.GetFromNewtonJsonAsync<List<T>>($"{dataObjectTypeStr}");
    }
    [Obsolete("未实现", error: true)]
    public override Task<List<string>> FindAllID()
    {
        throw new NotImplementedException();
    }
    [Obsolete("未实现", error: true)]
    public override Task<T> FindOneWhere(Expression<Predicate<T>> predicate, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true)
    {
        throw new NotImplementedException();
    }
    [Obsolete("未实现",error: true)]
    public override Task<List<T>> FindPage(int pageSize, int pageIndex, Expression<Predicate<T>> predicate, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true)
    {
        throw new NotImplementedException();
    }
    [Obsolete("未实现", error: true)]
    public override Task<List<T>> FindWhere(Expression<Predicate<T>> predicate, Expression<Func<T, object>> fields = null, bool include_OR_exculde = true)
    {
        throw new NotImplementedException();
    }
    public override async Task<long> GetCount(Expression<Predicate<T>> predicate = null)
    {
        return await httpClient.GetFromNewtonJsonAsync<long>($"{dataObjectTypeStr}/{HttpDatabaseKeyWords.CountStr}");
    }
    /// <summary>
    /// 返回id
    /// </summary>
    /// <param name="dataObject"></param>
    /// <returns></returns>
    public override async Task<string> Insert(T dataObject)
    {
        
        var json = JsonConvert.SerializeObject(dataObject);
        
        var response = await httpClient.PostAsNewtonJsonAsync($"{dataObjectTypeStr}", json);
        dataObject.Id = await response.Content.ReadAsStringAsync();
        return dataObject.Id;
    }

    public override async Task Update(string id, List<FieldChange> changes)
    {
        await httpClient.PutAsNewtonJsonAsync($"{dataObjectTypeStr}/{id}", changes.Select(c => TransferableFieldChange.FromFieldChange(c)));
    }

    public override async Task Update(string id, T obj)
    {
        var changes = FieldChangeTool.MakeAllFieldChanges(obj);
        await Update(id, changes);
    }

    public override async Task Update(T obj)
    {
        var changes = FieldChangeTool.MakeAllFieldChanges(obj);
        await Update(obj.Id, changes);
    }
    [Obsolete("未实现", error: true)]
    public override Task UpdateSturcture<New>(Func<T, New> mapper)
    {
        throw new NotImplementedException();
    }
}
