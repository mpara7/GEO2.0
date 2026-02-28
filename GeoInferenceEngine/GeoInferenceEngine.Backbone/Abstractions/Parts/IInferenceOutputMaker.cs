namespace GeoInferenceEngine.Backbone.Abstractions.Parts;
public interface IInferenceOutputMaker<out T> where T : AInferenceOutput
{
    public abstract string Name { get; set; }
    public abstract T Make();
}
