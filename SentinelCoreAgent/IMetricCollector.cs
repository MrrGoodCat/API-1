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
    /// Interface for metric collectors
    /// </summary>
    public interface IMetricCollector
    {
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
        CollectorResult CollectMetrics(string moduleName, List<MetricInfo> metricList, ILog logger);
    }

    /// <summary>
    /// Metric collector result
    /// </summary>
    public class CollectorResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectorResult"/> class.
        /// </summary>
        /// <param name="metricTable">
        /// The metric table.
        /// </param>
        public CollectorResult(List<List<string>> metricTable)
        {
            MetricTableList = metricTable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectorResult"/> class.
        /// </summary>
        /// <param name="metricTable">
        /// The metric table.
        /// </param>
        public CollectorResult(Dictionary<string, string> metricTable)
        {
            MetricTableDic = metricTable;
        }

        /// <summary>
        /// Gets the metric table.
        /// </summary>
        public List<List<string>> MetricTable
        {
            get
            {
                if (MetricTableList != null)
                {
                    return MetricTableList;
                }

                if (MetricTableDic != null)
                {
                    return this.MetricTableDic.Select(keyValuePair => new List<string> { keyValuePair.Key, keyValuePair.Value }).ToList();
                }

                return new List<List<string>>();
            }
        }

        /// <summary>
        /// Gets the metric table.
        /// </summary>
        public Dictionary<string, string> MetricTableDic { get; private set; }

        /// <summary>
        /// Gets or sets the metric table.
        /// </summary>
        private List<List<string>> MetricTableList { get; set; }
    }
}
