using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Diagnostics.Metrics;

namespace ZTool.Databases.Http.Server;
/// <summary>
/// 路由要与ADatabaseObject的名称对应
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ADatabaseCollectionController<T,DC> : ControllerBase where  T : ADatabaseObject where DC : ADataCollection<T>
{
    protected DC collection { get; set; }
    [HttpPut("{id}")]
    public virtual void Update(string id,  List<TransferableFieldChange> tFieldChanges)
    {
        List<FieldChange> fieldChanges = tFieldChanges.Select(fc => fc.ToFieldChange()).ToList();
        collection.Update(id, fieldChanges);
    }
    [HttpPost]
    public virtual string Insert([FromBody] string dataObjectStr)
    {
        var dataObject = JsonConvert.DeserializeObject<T>(dataObjectStr);
        collection.Insert(dataObject);
        return dataObject.Id;
    }
    [HttpDelete("{id}")]
    public virtual void Delete(string id)
    {
        collection.Delete(id);
    }
    [HttpGet("{id}")]
    public virtual T Find(string id)
    {
        var dataObject = collection.Find(id);
        return dataObject;
    }
    [HttpGet]
    public virtual List<T> FindAll()
    {
        var GeoProblemDAO = collection.FindAll();
        return GeoProblemDAO;
    }
    [HttpGet($"{HttpDatabaseKeyWords.CountStr}")]
    public virtual long GetCount()
    {
        return collection.GetCount();
    }
}
