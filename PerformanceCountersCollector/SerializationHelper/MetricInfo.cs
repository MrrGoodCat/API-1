using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PerformanceCountersCollector.SerializationHelper
{
    /// <summary>
    /// The metric info.
    /// </summary>
    public class MetricInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetricInfo"/> class.
        /// </summary>
        public MetricInfo()
        {
        }

        /// <summary>
        /// Gets or sets the default name.
        /// </summary>
        [XmlAttribute]
        public string ClusterPart { get; set; }

        /// <summary>
        /// Gets or sets the default name.
        /// </summary>
        [XmlAttribute]
        public string DefaultName { get; set; }

        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        [XmlAttribute]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the process name.
        /// </summary>
        [XmlAttribute]
        public string ProcessName { get; set; }

        /// <summary>
        /// Gets or sets the service name.
        /// </summary>
        [XmlAttribute]
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the metric type.
        /// </summary>
        [XmlAttribute]
        public string MetricType { get; set; }

        /// <summary>
        /// Gets the parsed metric type.
        /// </summary>
        [XmlIgnore]
        public MetricType ParsedMetricType
        {
            get
            {
                return (MetricType)Enum.Parse(typeof(MetricType), MetricType);
            }
        }

        /// <summary>
        /// Gets or sets the category name.
        /// </summary>
        [XmlAttribute]
        public string CategoryName { get; set; }

        /// <summary>
        /// Gets or sets the counter name.
        /// </summary>
        [XmlAttribute]
        public string CounterName { get; set; }

        /// <summary>
        /// Gets or sets the counter instance name.
        /// </summary>
        [XmlAttribute]
        public string CounterInstanceName { get; set; }

        /// <summary>
        /// Gets or sets the counter type.
        /// </summary>
        [XmlAttribute]
        public string CounterType { get; set; }

        /// <summary>
        /// Gets or sets the collecting interval.
        /// </summary>
        [XmlAttribute]
        public int CollectingInterval { get; set; }

        /// <summary>
        /// Gets or sets the delta.
        /// </summary>
        [XmlAttribute]
        public int Delta { get; set; }
    }
}
