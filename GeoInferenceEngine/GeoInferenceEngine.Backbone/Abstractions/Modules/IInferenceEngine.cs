namespace GeoInferenceEngine.Backbone.Abstractions.Modules
{
    public interface IInferenceEngine
    {
        public void Init();
        public void StepForward();
        public void Release();
    }
}