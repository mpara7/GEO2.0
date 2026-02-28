// See https://aka.ms/new-console-template for more information
using GeoInferenceEngine.Exe.Protos;
using GeoInferenceEngine.Exe.Services;

using GrpcDotNetNamedPipes;

#if DEBUG
string pipeName = "zqx_default_pn";
if (args.Length == 0)
{
}
else if (args.Length == 1)
{
    pipeName = args[0];
}

#else
if (args.Length == 0)
{
    Console.WriteLine("请输入命名管道名称，直接回车则使用默认名称");
    pipeName = Console.ReadLine();
    if (pipeName == "")
    {
        Console.WriteLine("将使用默认名称 zqx_default_pn");
        pipeName = "zqx_default_pn";
    }

}
else if (args.Length == 1)
{
    pipeName = args[0];
}
#endif
Console.WriteLine("Hello!~!!!!!!!++22!");
var server = new NamedPipeServer(pipeName);
Context.NamedPipeServer = server;
GeoInferenceEngineServer.BindService(server.ServiceBinder, new GeoInferenceEngineService());
Console.WriteLine($"推理服务已开启 管道id{pipeName}");
server.Start();

public static class Context
{
    public static NamedPipeServer NamedPipeServer { get; set; }
}