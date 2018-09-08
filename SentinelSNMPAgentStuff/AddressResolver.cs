using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelSNMPAgentStuff
{
    /// <summary>
    /// The address resolver.
    /// </summary>
    public class AddressResolver
    {
        /// <summary>
        /// The agent WCF server.
        /// </summary>
        public const string AgentWcfServer = "net.pipe://localhost/AgentWCFServer";

        /// <summary>
        /// The ping WCF server.
        /// </summary>
        public const string PingWcfServer = "net.pipe://localhost/PingModule{0}";
    }
}
