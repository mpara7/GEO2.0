using System;
using System.Collections.Generic;

using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Inputs;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs;
using Google.Protobuf.WellKnownTypes;

using Grpc.Core;
using ZTool.Tools;


namespace GeoInferenceEngine.Exe.Services;
public class GeoInferenceEngineService:GeoInferenceEngine.Exe.Protos.GeoInferenceEngineServer.GeoInferenceEngineServerBase
{


    GeoInferenceApp app = new GeoInferenceApp();
    InferenceUniversalInputs input;
    private static T UnpackString<T>(StringValue yaml)
    {
        YamlDotNet.Serialization.Deserializer deserializer = new();
        return deserializer.Deserialize<T>(yaml.Value);
    }
    private static Task<StringValue> PackObj<T>(T obj)
    {
        YamlDotNet.Serialization.Serializer serializer = new YamlDotNet.Serialization.Serializer();
        return Task.FromResult(new StringValue() { Value = serializer.Serialize(obj) });
    }
    private static Task<Empty> Empty()
    {
        return Task.FromResult(new Empty());
    }
    #region 推理设置

    public override Task<Empty> SetInferenceInfo(StringValue request, ServerCallContext context)
    {
        input = UnpackString<YAMLInferenceUniversalInputs>(request).To();
        return Empty();
    }

    #endregion
    #region 推理机命令
    public override Task<Empty> Stop(Empty request, ServerCallContext context)
    {
        app.Stop();
        return Empty();
    }
    public override Task<Empty> Continue(Empty request, ServerCallContext context)
    {
        app.Continue();
        return Empty();
    }

    public override Task<Empty> Break(Empty request, ServerCallContext context)
    {
        app.Pause();
        return Empty();
    }
    public override Task<Empty> Run(Empty request, ServerCallContext context)
    {
        new Thread(() =>
        {
            try
            {
                app.SetInput(input);
                app.Prepare();
                app.Start();
            }
            catch(Exception e)
            {
            }
        }).Start();
        return Empty();
    }
    #endregion

    #region 获取信息
    public override Task<StringValue> GetEngineStatu(Empty request, ServerCallContext context)
    {
        return PackObj(app.AppInfo.AppStatu);
    }
    public override Task<StringValue> GetProcessingInfo(Empty request, ServerCallContext context)
    {
        InferenceUniversalOutputs output = new InferenceUniversalOutputs();
        output.Outputs = app.GetProcessingInfos();
        var yaml= new YAMLInferenceUniversalOutputs();
        yaml.From(output);
        return PackObj(yaml);
    }
    public override Task<StringValue> GetResults(Empty request, ServerCallContext context)
    {
        InferenceUniversalOutputs output = new InferenceUniversalOutputs();

        output = app.GetResults();
        var yaml = new YAMLInferenceUniversalOutputs();
        yaml.From(output);
        return PackObj(yaml);
    }
    public override Task<StringValue> GetCrackedInfo(Empty request, ServerCallContext context)
    {
        return PackObj(app.AppInfo.CreckedInfo);
    }
    #endregion

    public override Task<Empty> Dispose(Empty request, ServerCallContext context)
    {
        Task.Run(() => { 
            Task.Delay(100);
            Environment.Exit(0); 
        });
        Console.WriteLine("100ms后退出");
        return Empty();
    }
}
