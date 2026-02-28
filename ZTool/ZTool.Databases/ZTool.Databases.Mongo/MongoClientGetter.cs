using MongoDB.Driver;

namespace ZTool.Databases.Mongo;

/// <summary>
/// 自动配置用户验证
/// </summary>
public class MongoClientGetter
{
    public string Host { get; set; } = "127.0.0.1";
    public string UserSource { get; set; } = "admin";
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string DbAuthMechanism { get; set; } = "SCRAM-SHA-1";

    public MongoClient GetMongoClient()
    {
        MongoClientSettings settings = new MongoClientSettings();
        //用户验证配置
        if (UserName is not null && Password is not null)
        {
            MongoInternalIdentity internalIdentity =
                  new MongoInternalIdentity(UserSource, UserName);
            PasswordEvidence passwordEvidence = new PasswordEvidence(Password);
            MongoCredential mongoCredential =
                 new MongoCredential(DbAuthMechanism,
                         internalIdentity, passwordEvidence);
            List<MongoCredential> credentials =
                       new List<MongoCredential>() { mongoCredential };
            settings.Credentials = credentials;
        }
        MongoServerAddress address = new MongoServerAddress(Host);
        settings.Server = address;

        MongoClient client = new MongoClient(settings);
        return client;
    }

}
