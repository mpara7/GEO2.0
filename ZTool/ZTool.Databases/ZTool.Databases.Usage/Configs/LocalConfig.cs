using ZTool.Databases.Mongo;

namespace GeoDatabase.Shared.Configs;
public class LocalConfig : ZTool.Databases.Mongo.MongoCollectionConfig
{
    public LocalConfig()
    {
        DatabaseName = "GeoProject";
        CollectionName = "GeoProblem";
    }
}
