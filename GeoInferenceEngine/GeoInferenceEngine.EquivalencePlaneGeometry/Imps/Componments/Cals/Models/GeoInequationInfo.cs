using GeoInferenceEngine.Knowledges.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoInferenceEngine.PredicateShared.Models.Exprs
{
    public abstract class GeoInequationInfo
    {
        public GeoInequation GeoInequation { get; set; }
        public GeoInequationInfo(GeoInequation geoInequation)
        {
            GeoInequation = geoInequation;
        }
    }
}
