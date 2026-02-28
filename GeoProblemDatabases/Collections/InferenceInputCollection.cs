using ZTool.Databases.Mongo;

namespace GeoProblemDatabases.Collections
{

    public class InferenceInputCollection : MongoCollection<InferenceInputDAO>
    {
        public static Dictionary<string, InferenceInputCollection> Cached = new();
        public static InferenceInputCollection Get(string collectionName)
        {
            if (Cached.ContainsKey(collectionName))
                return Cached[collectionName];
            InferenceInputCollection collection = null;
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
            config.DatabaseName = "InferenceInput";
            config.CollectionName = collectionName;
            collection = new InferenceInputCollection(config);
            Cached.Add(collectionName, collection);
            return collection;
        }
        private InferenceInputCollection(MongoConnectionConfig config) : base(config)
        {
        }

    }
}
