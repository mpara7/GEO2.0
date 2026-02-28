using ZTool.Databases.Mongo;

namespace GeoProblemDatabases.Databases;
public class ImageDatabase : MongoDatabase
{

    public static ImageDatabase Get()
    {
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
        return new ImageDatabase(config);
    }
    private ImageDatabase(MongoConnectionConfig config) : base(config)
    {
    }
    public override List<string> GetCollectionNames()
    {
        return base.GetCollectionNames().ToList();
    }
}
