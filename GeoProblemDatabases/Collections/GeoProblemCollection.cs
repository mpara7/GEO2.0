

using ZTool.Databases.Mongo;

namespace GeoProblemDatabases.Collections;
public class GeoProblemCollection : MongoCollection<GeoProblemDAO>
{
    public static Dictionary<string, GeoProblemCollection> Cached = new();
    public static GeoProblemCollection Get(string collectionName)
    {
        if (Cached.ContainsKey(collectionName))
            return Cached[collectionName];
        GeoProblemCollection collection = null;
        MongoConnectionConfig config = null;
        switch (GlobalDatabaseSetting.DatabaseSource)
        {
            case DatabaseSource.Remote:
                config = GlobalDatabaseSetting.RemoteDBConfig;
                break;
            case DatabaseSource.Local:
                config = GlobalDatabaseSetting.LocalDBConfig;
                break;
            case DatabaseSource.ServerLocal:
                config = GlobalDatabaseSetting.ServerLocalDBConfig;
                break;
            default:
                throw new Exception();
        }
        config.DatabaseName = "GeoProblem";
        config.CollectionName = collectionName;
        collection = new GeoProblemCollection(config);
        Cached.Add(collectionName, collection);
        return collection;
    }
    private GeoProblemCollection(MongoConnectionConfig config) : base(config)
    {
    }
    public GeoProblemDAO GetProblemInfo(string id)
    {
        return Find(id, p => new { p.Id, p.Labels, p.LastUpdateTime });
    }
    public List<GeoProblemDAO> GetAllProblemUpdateTime()
    {
        return FindAll(p => new { p.Id, p.LastUpdateTime });
    }
    public List<GeoProblemDAO> GetAllProblemIDWithTitle()
    {
        return FindAll(p => new { p.Id, p.Title });
    }
    public List<GeoProblemDAO> GetAllProblemInfo()
    {
        return FindAll(p => new { p.Id, p.Labels });
    }
    public GeoProblemDAO GeoProblemShort(string id)
    {
        return Find(id, p => new { p.Title, p.LastUpdateTime, p.Labels, p.Id });
    }
}
