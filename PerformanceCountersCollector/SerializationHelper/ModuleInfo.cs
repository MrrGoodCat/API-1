using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PerformanceCountersCollector.SerializationHelper
{
    /// <summary>
    /// The module info.
    /// </summary>
    [XmlRoot("Module", IsNullable = false)]
    public class ModuleInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleInfo"/> class. 
        /// Only for serialization
        /// </summary>
        public ModuleInfo()
        {
            ProcessId = 0;
            Status = ModuleStatus.Unknown.ToString();
        }

        /// <summary>
        /// Gets or sets the OID.
        /// </summary>
        [XmlAttribute]
        public string OID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the module status OID.
        /// </summary>
        [XmlAttribute]
        public string StatusOID { get; set; }

        /// <summary>
        /// Gets or sets the process id.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is required to send auto fixing trap.
        /// </summary>
        public bool SendAutoFixing { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether module is integrated or not.
        /// </summary>
        [XmlAttribute]
        public bool NotIntegrated { get; set; }

        /// <summary>
        /// Gets or sets the metric tables.
        /// </summary>
        [XmlElement("MetricTables")]
        public MetricTableCollection MetricTables { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", OID, Name);
        }
    }
}
