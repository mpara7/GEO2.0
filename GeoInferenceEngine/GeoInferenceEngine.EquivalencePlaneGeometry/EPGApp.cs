global using GeoInferenceEngine.Knowledges.Models;
global using ZTool.InnerTypeTools.BaseTyps;
global using ZTool.UsefulTypes;
global using GeoInferenceEngine.Knowledges;
global using ZTool.InnerTypeTools.CollectionTypes;
global using GeoInferenceEngine.EquivalencePlaneGeometry.Models;
using GeoInferenceEngine.Backbone;

namespace GeoInferenceEngine.EquivalencePlaneGeometry
{
    public static class EPGApp
    {
        public static void RunByZScript(string zscript, EngineConfig engineConfig)
        {
            var InferenceApp = new GeoInferenceApp();
            InferenceUniversalInputs uInput = new InferenceUniversalInputs()
            {
                Inputs = new List<AInferenceInput>() { new ZScriptInput() { Content = zscript } },
                EngineConfig = engineConfig,
            };
            InferenceApp.SetInput(uInput);
            InferenceApp.IsThrowExection = false;
            InferenceApp.Prepare();
            InferenceApp.Start();
            InferenceApp.ToConsole();
        }
        public static InferenceUniversalOutputs RunByZScriptWithResult(string zscript, EngineConfig engineConfig)
        {
            var InferenceApp = new GeoInferenceApp();
            InferenceUniversalInputs uInput = new InferenceUniversalInputs()
            {
                Inputs = new List<AInferenceInput>() { new ZScriptInput() { Content = zscript } },
                EngineConfig = engineConfig,
            };
            InferenceApp.SetInput(uInput);
            InferenceApp.IsThrowExection = true;
            InferenceApp.Prepare();
            InferenceApp.Start();
            return InferenceApp.GetResults();
        }

        public static GeoInferenceApp GetApp(string zscriptInputs, EngineConfig engineConfig, bool isThrowExection = true, bool isRunByAsync = false)
        {
            InferenceUniversalInputs inferenceGenericInputs = new InferenceUniversalInputs();
            inferenceGenericInputs.Inputs.Add(new ZScriptInput() { Content = zscriptInputs });
            inferenceGenericInputs.EngineConfig = engineConfig;
            return GetApp(inferenceGenericInputs, isThrowExection, isRunByAsync);
        }

        public static GeoInferenceApp GetApp(InferenceUniversalInputs inferenceGenericInputs, bool isThrowExection = true, bool isRunByAsync = false)
        {
            GeoInferenceApp geoInferenceApp = new GeoInferenceApp();
            geoInferenceApp.IsThrowExection = isThrowExection;
            geoInferenceApp.IsRunByAsync = isRunByAsync;
            geoInferenceApp.SetInput(inferenceGenericInputs);
            geoInferenceApp.Prepare();
            return geoInferenceApp;
        }

        public static void AnswerToConsole(this GeoInferenceApp app)
        {
            if (app.AppInfo.WarningInfo.Warnings.Count > 0)
            {
                app.AppInfo.WarningInfo.Warnings.ForEach(Console.WriteLine);
            }
            if (app.AppInfo.IsActivedStop)
            {
                foreach (var result in app.AppInfo.ActivedStopReasons)
                {
                    Console.WriteLine(result);
                }
                return;
            }
            var output = app.GetResult<HumanLikeAnswerOutput>();
            if (output is not null)
                Console.WriteLine(output);
        }

        public static void ToConsole(this GeoInferenceApp app)
        {
            if (app.AppInfo.WarningInfo.Warnings.Count > 0)
            {
                app.AppInfo.WarningInfo.Warnings.ForEach(Console.WriteLine);
            }
            if (app.AppInfo.IsActivedStop)
            {
                foreach (var result in app.AppInfo.ActivedStopReasons)
                {
                    Console.WriteLine(result);
                }
            }
            else if (app.AppInfo.IsCracked)
            {
                Console.WriteLine("已崩溃");
                Console.WriteLine(app.AppInfo.CreckedInfo);
            }
            var results = app.GetResults();
        }
    } 

}
