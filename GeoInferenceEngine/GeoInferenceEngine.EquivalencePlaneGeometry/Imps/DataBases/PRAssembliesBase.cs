
using System.Reflection;

namespace GeoInferenceEngine.EquivalencePlaneGeometry.Imps.DataBases
{
    [Description("谓词逻辑程序集")]
    public class PRAssembliesBase : IInferenceDatabase
    {
        public Assembly[] KnowledgeAssemblies => new Assembly[] { typeof(PRAssembliesBase).Assembly };

        public Assembly[] RuleAssemblies => new Assembly[] { typeof(PRAssembliesBase).Assembly };
    }
}
