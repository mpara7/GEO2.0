using System.Reflection;
using YamlDotNet.Serialization;

namespace ZTool.Tools;
public class YAML
{
    public static void Show(object obj)
    {
        Console.WriteLine(YAML.Serialize(obj));
    }
    public static string Serialize<T>(T obj)
    {
        var serializer = new SerializerBuilder().Build();
        return serializer.Serialize(obj);
    }
    public static T Deserialize<T>(string yaml, bool isIgnoreUnmatchedProperties = true)
    {
        IDeserializer deserializer;
        if (isIgnoreUnmatchedProperties)
            deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        else
            deserializer = new DeserializerBuilder().Build();
        return deserializer.Deserialize<T>(yaml);
    }
    public static object? Deserialize(Type outputType, string yaml, bool isIgnoreUnmatchedProperties = true)
    {
        IDeserializer deserializer;
        if (isIgnoreUnmatchedProperties)
            deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        else
            deserializer = new DeserializerBuilder().Build();
        return deserializer.Deserialize(yaml, outputType);
    }
    public static object? Deserialize(string typeStr, string yaml, bool isIgnoreUnmatchedProperties = true)
    {
        Type outputType = Type.GetType(typeStr);
        IDeserializer deserializer;
        if (isIgnoreUnmatchedProperties)
            deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        else
            deserializer = new DeserializerBuilder().Build();
        return deserializer.Deserialize(yaml, outputType);
    }
}
