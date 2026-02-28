

namespace GeoInferenceEngine.Backbone.Abstractions.Modules
{
    public interface IEngineOutputGetter
    {
        List<IInferenceOutputMaker<AInferenceOutput>> ProcessingInfoMakers { get; }
        List<IInferenceOutputMaker<AInferenceOutput>> ResultMakers { get; }

        void AddProcessingInfoMaker(IInferenceOutputMaker<AInferenceOutput> getter);
        void AddResultMaker(IInferenceOutputMaker<AInferenceOutput> getter);

        T GetProcessingInfo<T>(string name = null) where T : AInferenceOutput;
        T GetResult<T>(string name = null) where T : AInferenceOutput;
        /// <summary>
        /// 生成器名字+输出
        /// </summary>
        /// <returns></returns>
        Dictionary<string, AInferenceOutput> GetProcessingInfos();
        /// <summary>
        /// 生成器名字+输出
        /// </summary>
        /// <returns></returns>
        Dictionary<string, AInferenceOutput> GetResults();
    }
}