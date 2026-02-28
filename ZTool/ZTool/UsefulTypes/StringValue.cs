namespace ZTool.UsefulTypes;
public class StringValue
{
    public StringValue()
    {

    }
    public StringValue(string value = "")
    {
        Value = value;
    }
    public string Value { get; set; }

    public static implicit operator StringValue(string value)
    {
        return new StringValue(value);
    }
    public static implicit operator string(StringValue value)
    {
        return value.Value;
    }
}