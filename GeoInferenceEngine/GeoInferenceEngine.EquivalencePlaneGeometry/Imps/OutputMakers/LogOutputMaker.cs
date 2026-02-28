using GeoInferenceEngine.EquivalencePlaneGeometry.Imps.Componments;

using System.Text;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.IO.Outputs
{

    [Description("日志输出生成器")]
    public class LogOutputMaker : IInferenceOutputMaker<LogOutput>
    {
        [ZDI]
        Logger fileLogger;
        string name;
        public string Name { get => name; set => name = value; }
        public LogOutput Make()
        {
            lock (fileLogger.LogContents)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < fileLogger.LogContents.Count; i++)
                {
                    sb.AppendLine(fileLogger.LogContents[i]);
                }
                return new LogOutput() { Content = sb.ToString() };
            }
        }
    }
}
