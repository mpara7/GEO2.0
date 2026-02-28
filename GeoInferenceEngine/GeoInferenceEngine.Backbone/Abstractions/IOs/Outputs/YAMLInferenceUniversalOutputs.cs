namespace GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs
{
    public class YAMLInferenceOutput
    {
        public string MakerName { get; set; }
        public string Type { get; set; }
        public string YAML { get; set; }
    }
    public class YAMLInferenceUniversalOutputs
    {
        public void From(InferenceUniversalOutputs uOutputs)
        {
            WarningInfo = uOutputs.WarningInfo;
            CrackedInfo = uOutputs.CrackedInfo;
            IsCracked = uOutputs.IsCracked;
            HasWarning = uOutputs.HasWarning;
            IsActiveStop = uOutputs.IsActiveStop;
            ActiveStopReasons = uOutputs.ActiveStopReasons;
            if (uOutputs.Outputs != null)
            {
                Outputs = new();
                foreach (var kv in uOutputs.Outputs)
                {
                    Outputs.Add(new YAMLInferenceOutput()
                    {
                        MakerName = kv.Key,
                        Type = kv.Value.GetType().AssemblyQualifiedName,
                        YAML = YAML.Serialize(kv.Value)
                    });
                }
            }
        }
        public InferenceUniversalOutputs To()
        {
            InferenceUniversalOutputs uOutputs = new InferenceUniversalOutputs();
            uOutputs.WarningInfo = WarningInfo;
            uOutputs.CrackedInfo = CrackedInfo;
            uOutputs.IsCracked = IsCracked;
            uOutputs.HasWarning = HasWarning;
            uOutputs.IsActiveStop = IsActiveStop;
            uOutputs.ActiveStopReasons = ActiveStopReasons;
            if (Outputs != null)
            {
                uOutputs.Outputs = new Dictionary<string, AInferenceOutput>();
                foreach (var yamlOutput in Outputs)
                {
                    uOutputs.Outputs.Add(yamlOutput.MakerName, (AInferenceOutput)YAML.Deserialize(yamlOutput.Type, yamlOutput.YAML));
                }
            }
            return uOutputs;

        }
        public bool HasWarning { get; set; } = false;
        public InferenceWarningInfo WarningInfo { get; set; }
        public bool IsActiveStop { get; set; }
        public List<string> ActiveStopReasons { get; set; } = new List<string>();
        public bool IsCracked { get; set; } = false;
        public InferenceCrackedInfo CrackedInfo { get; set; }
        public List<YAMLInferenceOutput> Outputs { get; set; }
    }
}
