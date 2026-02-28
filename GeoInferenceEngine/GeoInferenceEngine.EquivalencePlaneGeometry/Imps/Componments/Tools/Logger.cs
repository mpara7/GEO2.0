using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments
{
    public class LoggerConfig : AInferenceSetting
    {
        public bool IsToConsole { get; set; } = false;
    }
    [Description("日志")]
    public class Logger : IInferenceComponent
    {
        [ZDI]
        LoggerConfig config;
        public List<string> LogContents { get; set; } = new();
        public Dictionary<string, Dictionary<string, List<string>>> CateLogContents { get; set; } = new();
        protected (string, string, string) logExInfo([CallerMemberName] string callerMember = "")
        {
            StackTrace stackTrace = new StackTrace();
            MethodBase method = stackTrace.GetFrame(2).GetMethod();
            Type callingType = method.DeclaringType;
            var des = DescriptionAttribute.GetDescription(callingType);
            return (des, method.Name, callerMember);
        }
        public void Release()
        {
            //LogContents = new();
            //CateLogContents = new();
            //GC.Collect();
        }
        public void Debug(string content)
        {
            var (type, method, mode) = logExInfo();
            string info = $"{type} {method} [{mode}] {content} {DateTime.Now}";
            if (config.IsToConsole)
                Console.WriteLine(info);
            LogContents.Add(info);

            if (!CateLogContents.ContainsKey(type))
            {
                CateLogContents.Add(type, new Dictionary<string, List<string>>());
            }
            var c = CateLogContents[type];
            if (!c.ContainsKey(method))
            {
                c.Add(method, new List<string>());
            }
            c[method].Add(content);
        }
        public void Info(string content)
        {
            var (type, method, mode) = logExInfo();
            string info = $"{type} {method} {mode} {content} {DateTime.Now}";
            if (config.IsToConsole)
                Console.WriteLine(info);
            LogContents.Add(info);

            if (!CateLogContents.ContainsKey(type))
            {
                CateLogContents.Add(type, new Dictionary<string, List<string>>());
            }
            var c = CateLogContents[type];
            if (!c.ContainsKey(method))
            {
                c.Add(method, new List<string>());
            }
            c[method].Add(content);
        }
        public void Warn(string content)
        {
            var (type, method, mode) = logExInfo();
            string info = $"{type} {method} {mode} {content} {DateTime.Now}";
            if (config.IsToConsole)
                Console.WriteLine(info);
            LogContents.Add(info);

            if (!CateLogContents.ContainsKey(type))
            {
                CateLogContents.Add(type, new Dictionary<string, List<string>>());
            }
            var c = CateLogContents[type];
            if (!c.ContainsKey(method))
            {
                c.Add(method, new List<string>());
            }
            c[method].Add(content);
        }
        public void Error(string content)
        {
            var (type, method, mode) = logExInfo();
            string info = $"{type} {method} {mode} {content} {DateTime.Now}";
            if (config.IsToConsole)
                Console.WriteLine(info);
            LogContents.Add(info);

            if (!CateLogContents.ContainsKey(type))
            {
                CateLogContents.Add(type, new Dictionary<string, List<string>>());
            }
            var c = CateLogContents[type];
            if (!c.ContainsKey(method))
            {
                c.Add(method, new List<string>());
            }
            c[method].Add(content);
        }
        public void Fatal(string content)
        {
            var (type, method, mode) = logExInfo();
            string info = $"{type} {method} {mode} {content} {DateTime.Now}";
            if (config.IsToConsole)
                Console.WriteLine(info);
            LogContents.Add(info);

            if (!CateLogContents.ContainsKey(type))
            {
                CateLogContents.Add(type, new Dictionary<string, List<string>>());
            }
            var c = CateLogContents[type];
            if (!c.ContainsKey(method))
            {
                c.Add(method, new List<string>());
            }
            c[method].Add(content);
        }
    }
}
