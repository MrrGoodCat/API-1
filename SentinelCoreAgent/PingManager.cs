using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelCoreAgent
{
    /// <summary>
    /// The ping manager.
    /// </summary>
    public class PingManager : IPingManager
    {
        /// <summary>
        /// The ping collection.
        /// </summary>
        private static readonly Dictionary<string, Pinger> PingerCollection = new Dictionary<string, Pinger>();

        /// <summary>
        /// The _synch object.
        /// </summary>
        private static readonly object SynchObject = new object();

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
        public void CreateNewPinger(string moduleName, int processId, bool needToSetProcessId)
        {
            lock (SynchObject)
            {
                if (!PingerCollection.ContainsKey(moduleName))
                {
                    // create new pinger
                    Pinger pinger = new Pinger(moduleName, processId, needToSetProcessId);
                    pinger.Start();
                    PingerCollection.Add(moduleName, pinger);
                }
                else
                {
                    // start existing pinger
                    Pinger pinger = PingerCollection[moduleName];
                    if (pinger.ProcessId != processId)
                    {
                        // recreate pinger
                        pinger.Stop();
                        pinger = new Pinger(moduleName, processId, needToSetProcessId);
                        pinger.Start();
                        PingerCollection[moduleName] = pinger;
                    }
                    else
                    {
                        pinger.Start();
                    }
                }
            }
        }

        /// <summary>
        /// The dispose old ping worker.
        /// </summary>
        /// <param name="moduleName">
        /// The module name.
        /// </param>
        public void DisposeOldPinger(string moduleName)
        {
            lock (SynchObject)
            {
                if (PingerCollection.ContainsKey(moduleName))
                {
                    Pinger pinger = PingerCollection[moduleName];
                    pinger.Stop();
                }
            }
        }
    }
}
