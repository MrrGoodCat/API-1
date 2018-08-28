using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCountersCollector
{
    public enum CounterType
    {
        [Description("Average")]
        Average,

        [Description("_Total")]
        Total,
    }
}
