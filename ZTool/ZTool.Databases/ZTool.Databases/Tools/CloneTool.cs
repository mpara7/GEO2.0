namespace ZTool.Databases.Tools;
public static class CloneTool
{
    public static T CloneT<T>(T obj)
    {
        YamlDotNet.Serialization.Serializer serializer = new YamlDotNet.Serialization.Serializer();
        var yaml = serializer.Serialize(obj);
        YamlDotNet.Serialization.Deserializer deserializer = new YamlDotNet.Serialization.Deserializer();
        return deserializer.Deserialize<T>(yaml);
    }
}
