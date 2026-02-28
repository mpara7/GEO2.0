global using GeoProblemDatabases.Collections;
global using GeoProblemDatabases.Databases;
global using GeoProblemDatabases.Models;

using ZTool.Databases.Mongo;

namespace GeoProblemDatabases;
public enum DatabaseSource
{
    /// <summary>
    /// 远程服务器 带账号密码
    /// </summary>
    Remote,
    /// <summary>
    /// 本地服务器(调试) 不带账号密码
    /// </summary>
    Local,
    /// <summary>
    /// 在远程服务器用本地链接 带账号密码
    /// </summary>
    ServerLocal
}
public class GlobalDatabaseSetting
{
    public static DatabaseSource DatabaseSource { get; set; } = DatabaseSource.Local;
    public static MongoConnectionConfig RemoteDBConfig
    {
        get => new MongoConnectionConfig()
        {
            //MongoUrl = "81.71.50.245",
            MongoUrl = "159.75.249.157",
            UserName = "admin",
            Password = "__QK1001207890",
        };
    }
    public static MongoConnectionConfig LocalDBConfig
    {
        get => new MongoConnectionConfig()
        {
        };
    }
    public static MongoConnectionConfig ServerLocalDBConfig
    {
        get => new MongoConnectionConfig()
        {
            UserName = "admin",
            Password = "__QK1001207890",
        };
    }
}
