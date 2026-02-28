using Newtonsoft.Json;

namespace ZTool.Databases;
public class TransferableFieldChange
{
    public string Field { get; set; }
    public string Type { get; set; }
    public string Json { get; set; }
    public FieldChange ToFieldChange()
    {
        Type t = System.Type.GetType(Type);
        return new FieldChange(Field, JsonConvert.DeserializeObject(Json, t));
    }
    public static TransferableFieldChange FromFieldChange(FieldChange fc)
    {
        return new TransferableFieldChange()
        {
            Field = fc.Field,
            Type = ((Type)fc.NewValue.GetType()).AssemblyQualifiedName,
            Json = JsonConvert.SerializeObject(fc.NewValue),
        };
    }
}
