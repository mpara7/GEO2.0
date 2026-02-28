
using System.Threading.Channels;
using ZTool.Tools;
using ZTool.Usage.ZDI;
using ZTool.UsefulTypes;
using var _ = new Defer(()=>Console.WriteLine("a"));
Console.WriteLine("g");
Thread.Sleep(1000);
//ZDIUsage.Usage();
//ZTaskQueneUsage.Usage();
//ZEventUsage.Usage();
//ZMapperUsage.Usage();
//ZAOPUsage.Usage();