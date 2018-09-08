using log4net;
using SentinelCoreAgent.SerializationHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelCoreAgent
{
    /// <summary>
    /// Default collector for returning static values
    /// </summary>
    public class DefaultCollector : IMetricCollector
    {
        /// <summary>
        /// Contains pairs 'service/process name' - 'service/process status'
        /// </summary>
        private Dictionary<string, string> resultedDefaultsTable = new Dictionary<string, string>();

        /// <summary>
        /// The collect metrics.
        /// </summary>
        /// <param name="moduleName">
        /// The module name.
        /// </param>
        /// <param name="metricList">
        /// The metric list.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <returns>
        /// The <see cref="CollectorResult"/>.
        /// </returns>
        public CollectorResult CollectMetrics(string moduleName, List<MetricInfo> metricList, ILog logger)
        {
            resultedDefaultsTable.Clear();
            foreach (MetricInfo metricInfo in metricList)
            {
                if (!resultedDefaultsTable.ContainsKey(metricInfo.DefaultName))
                {
                    resultedDefaultsTable.Add(metricInfo.DefaultName, metricInfo.DefaultValue);
                }
            }

            return new CollectorResult(resultedDefaultsTable);
        }
    }
}
