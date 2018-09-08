using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SentinelSNMPAgentStuff
{
    /// <summary>
    /// The module configuration failure.
    /// </summary>
    [Serializable]
    [DataContract]
    public class ModuleConfigurationFaiure
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleConfigurationFaiure"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public ModuleConfigurationFaiure(string message)
        {
            Description = message;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [DataMember]
        public string Description { get; set; }
    }
}
