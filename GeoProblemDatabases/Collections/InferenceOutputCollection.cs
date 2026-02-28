using ZTool.Databases.Mongo;

namespace GeoProblemDatabases.Collections;
public class InferenceOutputCollection : MongoCollection<InferenceOutputDAO>
{
    public static Dictionary<string, InferenceOutputCollection> Cached = new();
    public static InferenceOutputCollection Get(string collectionName)
    {
        if (Cached.ContainsKey(collectionName))
            return Cached[collectionName];
        InferenceOutputCollection collection = null;
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
        config.DatabaseName = "InferenceOutput";
        config.CollectionName = collectionName;
        collection = new InferenceOutputCollection(config);
        Cached.Add(collectionName, collection);
        return collection;
    }
    private InferenceOutputCollection(MongoConnectionConfig config) : base(config)
    {
    }
}
