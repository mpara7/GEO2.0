using ZTool.Databases;

namespace GeoProblemDatabases.Models
{
    public class InferenceInputDAO : ADatabaseObject
    {
        public string ProblemID { get; set; }
        /// <summary>
        /// Key=数据的类型 Value=YAML序列化
        /// </summary>
        public List<Dictionary<string, string>> Inputs { get; set; } = new();
    }
}
