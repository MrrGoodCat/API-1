using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelCoreAgent
{
    /// <summary>
    /// The metric type.
    /// </summary>
    public enum MetricType
    {
        /// <summary>
        /// The default.
        /// </summary>
        Default = 0,

        /// <summary>
        /// The performance counter metric.
        /// </summary>
        PerfCounter = 1,

        /// <summary>
        /// The service status metric
        /// </summary>
        ServiceStatus = 2,

        /// <summary>
        /// The cluster info.
        /// </summary>
        ClusterInfo = 3
    }
}
