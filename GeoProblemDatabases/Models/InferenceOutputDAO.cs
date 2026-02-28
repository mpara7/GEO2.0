using ZTool.Databases;

namespace GeoProblemDatabases.Models
{
    public class InferenceOutputInfo
    {
        public InferenceOutputInfo()
        {
        }
        public InferenceOutputInfo(string exInfo, string typeName, string yaml)
        {
            ExInfo = exInfo;
            TypeName = typeName;
            YAML = yaml;
        }
        public string ExInfo { get; set; }
        public string TypeName { get; set; }
        public string YAML { get; set; }
    }
    public class InferenceOutputDAO : ADatabaseObject
    {
        public string ProblemID { get; set; }
        /// <summary>
        /// list type.qname-yaml
        /// 因为可能会有多个输出获取器 输出同一种输出（比较任务）
        /// </summary>
        public List<List<InferenceOutputInfo>> Outputs { get; set; } = new();
    }
}
