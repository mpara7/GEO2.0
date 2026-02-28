// See https://aka.ms/new-console-template for more information
using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Inputs;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs;
using GeoInferenceEngine.Exe.Client;
using GeoInferenceEngine.Knowledges.Imps.IOs.Inputs;
using ZTool.Tools;

var exePath = @"..\..\..\..\GeoInferenceExe\bin\Debug\net8.0\GeoInferenceExe.exe";
var process = GeoInferenceExeClient.StartExeProcess(exePath);

var client = GeoInferenceExeClient.Get();
var yaml = File.ReadAllText("默认解题.txt");
var zscript = @"Points:A(1,0) B(1,0) C(1,0) D(1,0) E(1,0) F(1,0) G(1,0) H(1,0)
直线平行 AB,CD
直线平行 CD,EF
直线平行 EF,GH
Prove:直线平行 AB,GH";

InferenceUniversalInputs geoEngineCommonInput = new()
{
    Inputs = new List<AInferenceInput>() { new ZScriptInput(zscript) },
    EngineConfig = YAML.Deserialize<EngineConfig>(yaml)
};
YAMLInferenceUniversalInputs input = new YAMLInferenceUniversalInputs();
input.From(geoEngineCommonInput);
client.SetInferenceInfo(input);

var b = client.Run();
Dictionary<string, AInferenceOutput> outputs = new();
bool isCracked = false;
while (true)
{
    Thread.Sleep(1000);
    AppStatus engineStatu = client.GetEngineStatu();
    if (engineStatu == AppStatus.Finished)
    {
        var output = client.GetResults().To();
        if (output.IsActiveStop)
        {
            Console.WriteLine("已主动停机");
            Console.WriteLine(output.ActiveStopReasons[0]);
            break;
        }
        else
        {
            outputs = client.GetResults().To().Outputs;
            break;
        }

    }
    else if (engineStatu == AppStatus.Cracked)
    {
        isCracked = true;
        break;
    }
}
client.Dispose();
foreach (var item in outputs)
{
    Console.WriteLine(item.ToString());
}
if (isCracked)
{
    Console.WriteLine("推理机已崩溃");
    process.Kill();
}