using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SentinelCoreAgent.SerializationHelper
{
    /// <summary>
    /// The metric table info.
    /// </summary>
    public class MetricTableInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetricTableInfo"/> class.
        /// </summary>
        public MetricTableInfo()
        {
        }

        /// <summary>
        /// Gets or sets the table OID.
        /// </summary>
        [XmlAttribute]
        public string TableOid { get; set; }

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        [XmlAttribute]
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets the metric list.
        /// </summary>
        [XmlElement("Metric")]
        public MetricInfo[] MetricList { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Metric Table Oid: {0} Name {1}", TableOid, TableName);
        }
    }
}
