using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SentinelCoreAgent.RegistryStuff
{
    public class RegistryHelper
    {
        /// <summary>
        /// The scope path.
        /// </summary>
        private const string SCOPE = @"root\default";

        /// <summary>
        /// The path to SNMP agent win 2003.
        /// </summary>
        private const string PATH_TO_SNMP_AGENTS_WIN2003 = @"SYSTEM\\CurrentControlSet\\services\\SNMP\\Parameters\\ExtensionAgents";

        /// <summary>
        /// The path to SNMP agent win 2008.
        /// </summary>
        private const string PATH_TO_SNMP_AGENTS_WIN2008 = @"SYSTEM\\CurrentControlSet\\Control\\SNMP\\Parameters\\ExtensionAgents";

        /// <summary>
        /// The registry watch thread.
        /// </summary>
        private Thread mRegWatchThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryHelper"/> class.
        /// </summary>
        public RegistryHelper()
        {
            IsTimeToStop = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is time to stop.
        /// </summary>
        private bool IsTimeToStop { get; set; }

        /// <summary>
        /// The delete old agent key.
        /// </summary>
        public bool DeleteOldAgentKeys()
        {
            bool wasSmthDeleted = false;
            wasSmthDeleted |= this.DeleteRegKey(PATH_TO_SNMP_AGENTS_WIN2003, "SentinelSNMPAgent");
            wasSmthDeleted |= this.DeleteRegKey(PATH_TO_SNMP_AGENTS_WIN2008, "SentinelSNMPAgent");
            return wasSmthDeleted;
        }

        /// <summary>
        /// The start registry watcher.
        /// </summary>
        public void StartRegistryWatcher()
        {
            mRegWatchThread = new Thread(this.WatchTheOldAgentRegistry);
            mRegWatchThread.Start();
        }

        /// <summary>
        /// The stop registry watcher.
        /// </summary>
        public void StopRegistryWatcher()
        {
            IsTimeToStop = true;
        }

        /// <summary>
        /// The watch the old agent registry.
        /// </summary>
        private void WatchTheOldAgentRegistry()
        {
            try
            {
                ManagementScope managementScope = new ManagementScope(SCOPE);
                WqlEventQuery query = new WqlEventQuery(this.GetQuery());
                ManagementEventWatcher watcher = new ManagementEventWatcher(managementScope, query);
                watcher.EventArrived += HandleEvent;
                watcher.Start();
                while (!IsTimeToStop)
                {
                    Thread.Sleep(1000);
                }

                watcher.Stop();
            }
            catch (ManagementException managementException)
            {
                AgentSingleton.Instance.WriteException("An error occurred while watching the registry: ", managementException.ToString());
            }
        }

        /// <summary>
        /// The get the query.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetQuery()
        {
            string query = "SELECT * FROM RegistryKeyChangeEvent WHERE Hive = 'HKEY_LOCAL_MACHINE' AND ";
            string keyPathes = string.Empty;
            RegistryKey servicesKey = Registry.LocalMachine.OpenSubKey(PATH_TO_SNMP_AGENTS_WIN2003);
            RegistryKey controlKey = Registry.LocalMachine.OpenSubKey(PATH_TO_SNMP_AGENTS_WIN2008);
            if (servicesKey != null)
            {
                keyPathes = string.Format(@"KeyPath = '{0}'", PATH_TO_SNMP_AGENTS_WIN2003);
            }

            if (controlKey != null)
            {
                if (!string.IsNullOrEmpty(keyPathes))
                {
                    keyPathes = string.Format("({0} OR KeyPath = '{1}')", keyPathes, PATH_TO_SNMP_AGENTS_WIN2008);
                }
                else
                {
                    keyPathes = string.Format(@"KeyPath = '{0}'", PATH_TO_SNMP_AGENTS_WIN2008);
                }
            }

            AgentSingleton.Instance.WriteLog(query + keyPathes);
            return query + keyPathes;
        }

        /// <summary>
        /// The handle event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HandleEvent(object sender, EventArrivedEventArgs e)
        {
            try
            {
                AgentSingleton.Instance.WriteLog("Registry key was changed. Delete if old agent entries");
                this.DeleteOldAgentKeys();
            }
            catch (Exception exception)
            {
                AgentSingleton.Instance.WriteException(
                    "Error occurred while deleting old agent errors after event", exception.ToString());
            }
        }

        /// <summary>
        /// The delete registry key.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool DeleteRegKey(string path, string value)
        {
            bool wasSmthDeleted = false;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(path, true);
            if (key != null)
            {
                try
                {
                    key.DeleteValue(value);
                    wasSmthDeleted = true;
                }
                catch
                {
                }
            }

            return wasSmthDeleted;
        }
    }
}
