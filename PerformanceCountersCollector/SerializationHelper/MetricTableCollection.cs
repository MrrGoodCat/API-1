using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PerformanceCountersCollector.SerializationHelper
{
    /// <summary>
    /// The metric table collection.
    /// </summary>
    public class MetricTableCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetricTableCollection"/> class.
        /// </summary>
        public MetricTableCollection()
        {
        }

        /// <summary>
        /// Gets or sets the metric collection.
        /// </summary>
        [XmlElement("MetricTable")]
        public MetricTableInfo[] MetricCollection { get; set; }
    }
}
