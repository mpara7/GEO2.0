using ZTool.Databases.Mongo;
using ZTool.Databases.Mongo;
namespace GeoDatabase.Shared.Configs;
public class LocalUserConfig : MongoCollectionConfig
{
    public LocalUserConfig()
    {
        DatabaseName = "GeoProject";
        CollectionName = "User";
    }
}
