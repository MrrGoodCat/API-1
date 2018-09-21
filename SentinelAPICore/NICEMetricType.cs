using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelAPICore
{
    public enum NICEMetricType
    {
        [Description("Gauge")]
        Gauge,  //Can grow up and down.

        [Description("Counter")]
        Counter //from 0 to any number. Can only grow up.
    }

    public enum MetricColorIDs
    {
        SmallerBetter = 1,
        GreaterBetter = 2,
        Empty = 3
    }
}
