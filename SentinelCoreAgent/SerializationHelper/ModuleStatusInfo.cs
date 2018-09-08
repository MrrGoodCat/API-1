using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SentinelCoreAgent.SerializationHelper
{
    /// <summary>
    /// The module status info.
    /// </summary>
    public class ModuleStatusInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleStatusInfo"/> class.
        /// </summary>
        public ModuleStatusInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleStatusInfo"/> class.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        public ModuleStatusInfo(string status)
        {
            Status = status;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the process id.
        /// </summary>
        [XmlAttribute]
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [XmlAttribute]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it is required to send auto fixing trap.
        /// </summary>
        [XmlAttribute]
        public bool SendAutoFixing { get; set; }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Satus: {0}", Status);
        }
    }
}
