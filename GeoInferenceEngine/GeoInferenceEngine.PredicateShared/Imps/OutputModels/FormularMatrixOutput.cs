using GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.PredicateShared.Imps.OutputModels
{
    public class FormularMatrixOutput:AInferenceOutput
    {
        public List<string> MutNames { get; set; } = new();
        public List<List<string>> Values { get; set; } = new ();
    }
}
