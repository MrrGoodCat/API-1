using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SentinelCoreAgent.SerializationHelper
{
    /// <summary>
    /// The module status entry.
    /// </summary>
    [XmlRoot("TempModuleState", IsNullable = false)]
    public class TempModuleState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TempModuleState"/> class.
        /// </summary>
        public TempModuleState()
        {
        }

        /// <summary>
        /// Gets or sets the module status.
        /// </summary>
        [XmlArray("ModulesStatuses")]
        [XmlArrayItem("ModuleStatus")]
        public List<ModuleStatusInfo> ModuleStatus { get; set; }
    }
}
