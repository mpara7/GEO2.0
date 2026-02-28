using System.Reflection;
namespace GeoInferenceEngine.Backbone.Imps.Modules
{
    [Description("输出模块")]
    public class EngineOutputGetter : IEngineOutputGetter
    {
        public List<IInferenceOutputMaker<AInferenceOutput>> ProcessingInfoMakers { get; internal set; } = new List<IInferenceOutputMaker<AInferenceOutput>>();
        public List<IInferenceOutputMaker<AInferenceOutput>> ResultMakers { get; internal set; } = new List<IInferenceOutputMaker<AInferenceOutput>>();

        public void AddProcessingInfoMaker(IInferenceOutputMaker<AInferenceOutput> getter)
        {
            ProcessingInfoMakers.Add(getter);
        }

        public void AddResultMaker(IInferenceOutputMaker<AInferenceOutput> getter)
        {
            ResultMakers.Add(getter);
        }



        public Dictionary<string, AInferenceOutput> GetProcessingInfos()
        {
            Dictionary<string, int> displayTimes = new Dictionary<string, int>();
            Dictionary<string, AInferenceOutput> result = new();
            foreach (var getters in ProcessingInfoMakers)
            {
                DescriptionAttribute des = getters.GetType().GetCustomAttribute<DescriptionAttribute>();
                string title = "";
                if (des is not null)
                    title = des.Description;
                else
                    title = getters.GetType().Name;

                if (result.ContainsKey(title))
                {
                    if (!displayTimes.ContainsKey(title))
                        displayTimes.Add(title, 1);
                    result.Add(des.Description + displayTimes[des.Description], getters.Make());
                    displayTimes[des.Description]++;
                }
                else
                {
                    result.Add(title, getters.Make());
                }
            }
            return result;
        }

        int i = 0;

        public Dictionary<string, AInferenceOutput> GetResults()
        {
            Dictionary<string, int> displayTimes = new Dictionary<string, int>();
            Dictionary<string, AInferenceOutput> result = new();
            foreach (var getters in ResultMakers)
            {

                DescriptionAttribute des = getters.GetType().GetCustomAttribute<DescriptionAttribute>();
                string title = "";
                if (des is not null)
                    title = des.Description;
                else
                    title = getters.GetType().Name;

                if (result.ContainsKey(title))
                {
                    if (!displayTimes.ContainsKey(title))
                        displayTimes.Add(title, 1);
                    if (displayTimes.ContainsKey(title))
                        displayTimes.Add(title + (i++).ToString(), 1);
                    else
                        displayTimes.Add(title, 1);
                    result.Add(des.Description + displayTimes[des.Description], getters.Make());
                    displayTimes[des.Description]++;
                }
                else
                {
                    result.Add(title, getters.Make());
                }
            }
            return result;
        }
        public T GetProcessingInfo<T>(string name = null) where T : AInferenceOutput
        {
            if (name != null)
            {
                var maker = ProcessingInfoMakers.FirstOrDefault(g => g.Name == name);
                if (maker != null)
                {
                    return (T)maker.Make();
                }
            }
            foreach (var resultMaker in ProcessingInfoMakers)
            {
                var GTypes = resultMaker.GetType().GetGenericArguments();
                if (GTypes[0] is T)
                {
                    return (T)resultMaker.Make();
                }
            }
            return default(T);
        }
        public T GetResult<T>(string name = null) where T : AInferenceOutput
        {
            if (name != null)
            {
                var maker = ResultMakers.FirstOrDefault(g => g.Name == name);
                if (maker != null)
                {
                    return (T)maker.Make();
                }
            }
            foreach (var resultMaker in ResultMakers)
            {
                var GTypes = resultMaker.GetType().GetGenericArguments();
                if (GTypes[0] is T)
                {
                    return (T)resultMaker.Make();
                }
            }
            return default(T);
        }
    }
}