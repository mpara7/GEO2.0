namespace GeoInferenceEngine.Backbone.AppConfiger;
public static class InferenceComponentGroupAttribute
{
    public static List<Type> GetTIfExist(Type target)
    {

        List<Type> types = new List<Type>();
        var attriType = typeof(InferenceComponentGroupAttribute<>);
        object[] attris = target.GetCustomAttributes(false);
        foreach (var attri in attris)
        {
            if (attri.GetType().Name.Contains("InferenceComponentGroupAttribute"))
            {
                var t = attri.GetType().GetGenericArguments()[0];
                types.Add(t);
            }
        }
        return types;
    }
}
/// <summary>
/// 泛型标记了同一组的 Component
/// </summary>
public class InferenceComponentGroupAttribute<T> : Attribute
{

}