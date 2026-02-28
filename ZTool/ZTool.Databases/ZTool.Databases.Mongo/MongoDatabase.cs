using MongoDB.Bson;
using MongoDB.Driver;
using System.Timers;

namespace ZTool.Databases.Mongo;
public class MongoDatabase : ADatabase
{
    public IMongoClient Client { get; set; }
    public IMongoDatabase Database { get; set; }


    private static System.Timers.Timer timer;
    private static bool isConnected = false;

    public MongoDatabase(MongoConnectionConfig config)
    {
        MongoClientGetter clientGetter = new MongoClientGetter();
        clientGetter.Host = config.MongoUrl;
        clientGetter.UserName = config?.UserName;
        clientGetter.Password = config?.Password;

        int timeoutInMilliseconds = 10000;//5秒延迟
        timer = new System.Timers.Timer(timeoutInMilliseconds);
        timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
        timer.AutoReset = false; // 只触发一次
        timer.Enabled = true;

        Client = clientGetter.GetMongoClient();
        Database = Client.GetDatabase(config.DatabaseName);
        //加一个线程计时器
        timer.Start();

        //Thread.Sleep(10000);

        Database.ListCollectionNames();
        isConnected = true;
        timer.Stop();
    }

    private static void TimerElapsed(object sender, ElapsedEventArgs e)
    {
        // 连接超时处理
        if (!isConnected)
        {
            try
            {
                throw new TimeoutException("操作已超时");
            }
            catch (Exception)
            {


            }

            // 在这里执行连接超时的错误处理逻辑
        }
    }


    public override bool IsCollectionExists(string collectionName)
    {
        var filter = new BsonDocument("name", collectionName);
        var options = new ListCollectionNamesOptions { Filter = filter };
        return Database.ListCollectionNames(options).Any();
    }
    public override List<string> GetCollectionNames()
    {
        return Database.ListCollectionNames().ToList();
    }
    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return Database.GetCollection<T>(name);
    }
    public override void Create(string collectionName)
    {
        Database.CreateCollection(collectionName);
    }
    public override void Drop(string collectionName)
    {
        Database.DropCollection(collectionName);
    }
    public override void Clear(string collectionName)
    {
        Drop(collectionName);
        Create(collectionName);
    }


}
