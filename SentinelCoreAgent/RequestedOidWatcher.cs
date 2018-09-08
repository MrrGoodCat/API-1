using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelCoreAgent
{
    /// <summary>
    /// The requested OID watcher.
    /// </summary>
    internal class RequestedOidWatcher
    {
        /// <summary>
        /// The OID watch.
        /// </summary>
        private readonly Dictionary<string, OidEntry> oidsWatch = new Dictionary<string, OidEntry>();

        /// <summary>
        /// The add OID.
        /// </summary>
        /// <param name="oidFromAgent">
        /// The OID from agent.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool AddOid(string oidFromAgent)
        {
            if (oidsWatch.ContainsKey(oidFromAgent))
            {
                if (oidsWatch[oidFromAgent].IsValid)
                {
                    return false;
                }

                oidsWatch[oidFromAgent] = new OidEntry();
                return true;
            }

            oidsWatch.Add(oidFromAgent, new OidEntry());
            return true;
        }
    }
}
