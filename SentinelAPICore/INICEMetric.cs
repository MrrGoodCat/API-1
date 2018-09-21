using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelAPICore
{
    public interface INICEMetric
    {
        MetricColorIDs ColorID { get; }
        string Group { get; }
        bool IsAggregationEnable { get; }
        string MeasurementUnit { get; }
        string Name { get; }
        string ToString();
        NICEMetricType Type { get; }
    }
}
