using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelCoreAgent
{
    /// <summary>
    /// Interface for ping component simulation
    /// </summary>
    public interface IPingManager
    {
        /// <summary>
        /// The create new ping.
        /// </summary>
        /// <param name="moduleName">
        /// The module name.
        /// </param>
        /// <param name="processId">
        /// The process id.
        /// </param>
        /// <param name="needToSetProcessId">
        /// The need to set process id.
        /// </param>
        void CreateNewPinger(string moduleName, int processId, bool needToSetProcessId);

        /// <summary>
        /// The dispose old ping worker.
        /// </summary>
        /// <param name="moduleName">
        /// The module name.
        /// </param>
        void DisposeOldPinger(string moduleName);
    }
}
