// See https://aka.ms/new-console-template for more information
using GeoProblemDatabaseDLL;
using GeoProblemDatabases;
GlobalDatabaseSetting.DatabaseSource = DatabaseSource.Remote;
var c=GeoProblemDatabaseQApp.QApp["几何模型"]["1.1.1"][0];
Console.WriteLine(c);