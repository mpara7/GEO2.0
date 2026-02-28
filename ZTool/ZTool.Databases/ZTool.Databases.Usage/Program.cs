// See https://aka.ms/new-console-template for more information
using System.Linq.Expressions;

using ZTool.Databases;
using ZTool.Databases.Mongo;
using ZTool.Databases.Tools;
using ZTool.Databases.Usage;

//UpdateStruct.Parper();
UpdateStruct.Run2();

//Console.WriteLine("Hello, World!");
//var mao = new XiaoMaomao() { Age = 10};
//var newMao=CloneTool.CloneT(mao);
//newMao.Ints.Add(20);
//newMao.Id = "0 0";
//var list=FieldChangeTool.GetFieldChanges(mao,newMao);
//Console.WriteLine();


//ADataCollection<Maomao> database = new MongoDatabase<Maomao>(new LocalDatabaseConfig());
////清空数据库-删
//database.DeleteWhere(m=>true);
////插入新数据-增
//database.Insert(new Maomao() { Id = MongoTool.NewID(), Age = 20, Weight = 60, XiaoMaomao = new XiaoMaomao() { Age = 2 } });
//database.Insert(new Maomao() { Id = MongoTool.NewID(), Age = 23, Weight = 59 });
//database.Insert(new Maomao() { Id = MongoTool.NewID(), Age = 24, Weight = 61 });
////过滤器-查
//var gg = database.FindAll(m => m.Weight);
//var gg2 = database.FindAll(m => new { m.Weight, m.Age, m.Id,m.XiaoMaomao });
////部分映射-查
//var gg3 = database.GetCount(m => m.Weight == 60);
//var gg4 = database.GetCount(m => m.Weight <= 61&&m.Age>22);
////改
//var id=gg2[0].Id;
//database.Update(id,new List<FieldChange>() { new FieldChange("XiaoMaomao.Age", 3) });
//database.Update(id, new List<FieldChange>() { new FieldChange("XiaoMaomao.Ints", new List<int>() { 9000}) });
//var gg5=database.Find(id);
//Console.WriteLine();
