using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OneOf.Types;

namespace GeoInferenceEngine.Backbone.Abstractions.IOs.Outputs
{
    public static class GlobalTimer
{
        
        private static readonly Stopwatch _stopwatch = new Stopwatch();

    public static void Start()
    {
        _stopwatch.Reset();
        _stopwatch.Start();
    }

    public static void Stop()
    {
        _stopwatch.Stop();
    }

    public static TimeSpan Elapsed => _stopwatch.Elapsed;
}
}
