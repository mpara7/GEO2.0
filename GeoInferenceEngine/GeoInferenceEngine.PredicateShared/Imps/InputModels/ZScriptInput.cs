namespace GeoInferenceEngine.Knowledges.Imps.IOs.Inputs
{
    [Description("ZScript输入")]
    public class ZScriptInput : AInferenceInput
    {
        public ZScriptInput() { }
        public ZScriptInput(string problem)
        {
            Content = problem;
        }
        public string Content { get; set; } = "Points:A(0,0) B C D";
    }
}
