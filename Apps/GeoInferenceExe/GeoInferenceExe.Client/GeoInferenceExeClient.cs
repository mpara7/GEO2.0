using GeoInferenceEngine.Backbone;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Inputs;
using GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs;
using GeoInferenceEngine.Exe.Protos;
using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using GrpcDotNetNamedPipes;

using System.Diagnostics;


namespace GeoInferenceEngine.Exe.Client;
public class GeoInferenceExeClient : GeoInferenceEngineServer.GeoInferenceEngineServerClient
{
    static T UnpackString<T>(StringValue yaml)
    {
        YamlDotNet.Serialization.Deserializer deserializer = new();
        return deserializer.Deserialize<T>(yaml.Value);
    }
    static Task<StringValue> PackObj<T>(T obj)
    {
        YamlDotNet.Serialization.Serializer serializer = new YamlDotNet.Serialization.Serializer();
        return Task.FromResult(new StringValue() { Value = serializer.Serialize(obj) });
    }
    public static Empty Empty { get; } = new Empty();

    public static GeoInferenceExeClient Get(string name = "zqx_default_pn")
    {
        var options = new NamedPipeChannelOptions();
        options.ConnectionTimeout = 5000;

        var channel = new NamedPipeChannel(".", name, options);
        var client = new GeoInferenceExeClient(channel);
        return client;
    }
    public static Process StartExeProcess(string exePath, string pipeName = "zqx_default_pn")
    {
        Process process = new Process();
        process.StartInfo.FileName = exePath;
        process.StartInfo.Arguments = pipeName;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        return process;
    }

    public GeoInferenceExeClient(CallInvoker channel) : base(channel)
    {
    }
    #region MyRegion
    public void SetInferenceInfo(YAMLInferenceUniversalInputs input)
    {
        base.SetInferenceInfo(PackObj(input).Result);
    }

    public AppStatus GetEngineStatu()
    {
        return UnpackString<AppStatus>(base.GetEngineStatu(Empty));
    }

    public YAMLInferenceUniversalOutputs GetProcessingInfo()
    {
        return UnpackString<YAMLInferenceUniversalOutputs>(base.GetProcessingInfo(Empty));
    }
    public YAMLInferenceUniversalOutputs GetResults()
    {
        return UnpackString<YAMLInferenceUniversalOutputs>(base.GetResults(Empty));
    }
    public InferenceUniversalOutputs GetCrackedInfo()
    {
        return UnpackString<InferenceUniversalOutputs>(base.GetCrackedInfo(Empty));
    }
    #endregion

    #region 命令
    /// <summary>
    /// 重写为异步方法
    /// </summary>
    /// <param name="headers"></param>
    /// <param name="deadline"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public AsyncUnaryCall<Empty> Run(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
    {
        return base.RunAsync(Empty, headers, deadline, cancellationToken);
    }

    public Empty Pause(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
    {
        return base.Break(Empty, headers, deadline, cancellationToken);
    }
    public Empty Continue(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
    {
        return base.Continue(Empty, headers, deadline, cancellationToken);
    }
    public Empty Stop(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default)
    {
        return base.Stop(Empty, headers, deadline, cancellationToken);
    }
    #endregion

    public Empty Dispose()
    {
        return base.Dispose(Empty);
    }

}
