using ZTool.Databases.Mongo;

namespace GeoProblemDatabases.Collections;
public class ImageCollection : MongoCollection<ImageDAO>
{
    public static Dictionary<string, ImageCollection> Cached = new();
    public static ImageCollection Get(string collectionName)
    {
        if (Cached.ContainsKey(collectionName))
            return Cached[collectionName];
        ImageCollection collection = null;
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
        config.DatabaseName = "Image";
        config.CollectionName = collectionName;
        collection = new ImageCollection(config);
        Cached.Add(collectionName, collection);
        return collection;

    }
    public ImageCollection(MongoConnectionConfig config) : base(config)
    {
    }
}
