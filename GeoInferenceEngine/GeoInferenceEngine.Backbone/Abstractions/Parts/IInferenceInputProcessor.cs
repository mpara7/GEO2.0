namespace GeoInferenceEngine.Backbone.Abstractions.Parts;
/// <summary>
/// 输入处理
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IInferenceInputProcessor<in T> where T : AInferenceInput
{
    public abstract void Process(T input);
}

