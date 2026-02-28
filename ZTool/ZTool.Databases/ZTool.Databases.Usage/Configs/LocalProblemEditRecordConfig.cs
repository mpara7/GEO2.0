using ZTool.Databases.Mongo;

namespace GeoDatabase.Shared.Configs;
public class LocalProblemEditRecordConfig : MongoCollectionConfig
{
    public LocalProblemEditRecordConfig()
    {
        DatabaseName = "GeoProject";
        CollectionName = "EditRecord";
    }
}
