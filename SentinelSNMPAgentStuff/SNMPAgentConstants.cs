using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelSNMPAgentStuff
{
    /// <summary>
    /// Sentinel SNMP Agent constants
    /// </summary>
    public class SNMPAgentConstants
    {
        /// <summary>
        /// The state for service/process that does not exist.
        /// </summary>
        public const string NOT_EXISTS_STATE = "Not Exists";

        /// <summary>
        /// The state for counters that failed to be collected.
        /// </summary>
        public const string FAILED_TO_COLLECT_STATE = "FailedToCollect";

        /// <summary>
        /// The regex symbol.
        /// </summary>
        public const string REGEX_SYMBOL = "*";

        /// <summary>
        /// The agent enterprise OID.
        /// </summary>
        public const string AGENT_ENTERPRISE_OID = "1.3.6.1.4.1.3167.4";
    }
}
