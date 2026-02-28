using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZTool.Databases.Mongo;

namespace ZTool.Databases.Usage;
internal class LocalDatabaseConfig: MongoCollectionConfig
{
    public LocalDatabaseConfig()
    {
        DatabaseName = "TestDB";
        CollectionName = "Test4";
    }
}
