namespace ZTool.Databases.Mongo;

public class MongoConnectionConfig
{
    public string MongoUrl { get; set; } = "127.0.0.1";
    public string DatabaseName { get; set; }
    public string CollectionName { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}