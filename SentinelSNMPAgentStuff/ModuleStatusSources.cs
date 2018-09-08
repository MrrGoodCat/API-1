using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelSNMPAgentStuff
{
    public enum ModuleStatusSources
    {
        /// <summary>
        /// The API.
        /// </summary>
        API = 0,

        /// <summary>
        /// The connectivity.
        /// </summary>
        Connectivity = 1,

        /// <summary>
        /// The traps.
        /// </summary>
        Traps = 2
    }
}
