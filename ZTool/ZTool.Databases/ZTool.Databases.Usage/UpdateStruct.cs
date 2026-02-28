using GeoDatabase.Shared.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZTool.Databases.Mongo;

namespace ZTool.Databases.Usage;
public class Maomao2:ADatabaseObject
{
    public int weight { get; set; }
}
public class DaMaomao : ADatabaseObject
{
    public string weight { get; set; } = "";
}
class G: MongoCollection<GeoProblemDAO>
{
    public G(LocalDatabaseConfig local):base(local)
    {

    }
}
internal class UpdateStruct
{

    public static void Parper()
    {
        ADataCollection<Maomao2> database = new MongoCollection<Maomao2>(new MongoCollectionConfig() { DatabaseName="CatManage",CollectionName="Maomao"});
        for (int i = 0; i < 10; i++)
        {
            Maomao2 maomao2 = new Maomao2();
            maomao2.weight = i;
            database.Insert(maomao2);
        }
    }
    public static void Run()
    {
        MongoCollection<Maomao2> database = new MongoCollection<Maomao2>(new MongoCollectionConfig() { DatabaseName = "CatManage", CollectionName = "Maomao" });
        database.UnsetFields<DaMaomao>();
    }
    public static void Run2()
    {
        ADataCollection<GeoProblemDAO> aDataCollection =new G(new LocalDatabaseConfig());
        MongoCollection<DaMaomao> database = new MongoCollection<DaMaomao>(new MongoCollectionConfig() { DatabaseName = "CatManage", CollectionName = "Maomao" });
        var a= database.FindAll();
        foreach (var item in a)
        {
            Console.WriteLine(item);
        }
    }
}
