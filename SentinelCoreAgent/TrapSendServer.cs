using log4net;
using SentinelSNMPAgentStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SentinelCoreAgent
{
    /// <summary>
    /// The trap send server.
    /// </summary>
    public class TrapSendServer : ISentinelMDSNMPAgent
    {
        /// <summary>
        /// The ping manager.
        /// </summary>
        private readonly IPingManager pingManager;

        /// <summary>
        /// The mLogger.
        /// </summary>
        private ILog mLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrapSendServer"/> class.
        /// </summary>
        public TrapSendServer()
        {
            pingManager = new PingManager();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrapSendServer"/> class. 
        /// FOR TESTING PURPOSE!
        /// </summary>
        /// <param name="mockPingManager">
        /// The mock Ping Manager.
        /// </param>
        public TrapSendServer(IPingManager mockPingManager)
        {
            pingManager = mockPingManager;
        }

        /// <summary>
        /// Gets the module name.
        /// </summary>
        public string ModuleName { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether is initialized.
        /// </summary>
        private bool IsInitialized { get; set; }

        /// <summary>
        /// Send trap
        /// </summary>
        /// <param name="trap">
        /// Trap information
        /// </param>
        /// <param name="trapToClear">
        /// Guide of trap to clear
        /// </param>
        /// <returns>
        /// returns Guide of a sent trap
        /// </returns>
        public Guid SendTrap(SentinelTrap trap, Guid trapToClear)
        {
            mLogger.InfoFormat("Sending trap {0}", trap);
            if (IsInitialized)
            {
                AgentSingleton.Instance.RaiseTrap(trap, ModuleName, trapToClear);
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Send trap
        /// </summary>
        /// <param name="moduleName">
        /// The module Name.
        /// </param>
        /// <param name="trap">
        /// Trap information
        /// </param>
        /// <param name="trapToClear">
        /// Guide of trap to clear
        /// </param>
        /// <returns>
        /// returns Guide of a sent trap
        /// </returns>
        public Guid SendTrap(string moduleName, SentinelTrap trap, Guid trapToClear)
        {
            mLogger.InfoFormat("Sending trap {0}", trap);
            if (IsInitialized)
            {
                AgentSingleton.Instance.RaiseTrap(trap, moduleName, trapToClear);
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Connect to MD SNMP Agent
        /// </summary>;
        /// <param name="module">
        /// Module name
        /// </param>
        /// <param name="processId">
        /// process id
        /// </param>
        /// <returns>
        /// boolean result
        /// </returns>
        public bool Connect(string module, int processId)
        {
            ModuleName = module;
            if (!AgentSingleton.Instance.CheckModuleConfiguration(ModuleName))
            {
                throw new FaultException<ModuleConfigurationFaiure>(new ModuleConfigurationFaiure("Configuration file for module is missing"), new FaultReason("Missing config file"));
            }

            ModuleName = AgentSingleton.Instance.GetModuleName(ModuleName);
            IsInitialized = true;
            AgentSingleton.Instance.GetLogger(ref mLogger);
            pingManager.CreateNewPinger(module, processId, true);
            mLogger.InfoFormat("Connection is opened from module: {0}", ModuleName);
            return true;
        }

        /// <summary>
        /// The set module status.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="trapToClear">
        /// The trap to clear.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool SetModuleStatus(ModuleStatus status, Guid trapToClear)
        {
            if (IsInitialized)
            {
                if (status == ModuleStatus.NoError)
                {
                    AgentSingleton.Instance.ReinitializeModuleErrors(ModuleName); // ClearErrorTrap(ModuleName, trapToClear);
                }

                AgentSingleton.Instance.SetModuleStatus(ModuleName, status, ModuleStatusSources.API, null);
                return true;
            }

            mLogger.Info("Module is not initialized");
            return false;
        }

        /// <summary>
        /// The get module status.
        /// </summary>
        /// <returns>
        /// The <see cref="ModuleStatus"/>.
        /// </returns>
        public ModuleStatus GetModuleStatus()
        {
            if (IsInitialized)
            {
                return AgentSingleton.Instance.GetModuleStatus(ModuleName);
            }

            mLogger.Info("Module is not initialized");
            return ModuleStatus.Unknown;
        }

        /// <summary>
        /// The get module status. FOR TESTING PURPOSES!!!
        /// </summary>
        /// <param name="moduleName">
        /// The module Name.
        /// </param>
        /// <returns>
        /// The <see cref="ModuleStatus"/>.
        /// </returns>
        public ModuleStatus GetModuleStatus(string moduleName)
        {
            if (IsInitialized)
            {
                return AgentSingleton.Instance.GetModuleStatus(moduleName);
            }

            mLogger.Info("Module is not initialized");
            return ModuleStatus.Unknown;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (IsInitialized)
            {
                mLogger.InfoFormat("Module {0} was disposed.", ModuleName);
                IsInitialized = false;
            }
        }

        /// <summary>
        /// The disconnect.
        /// </summary>
        public void Disconnect()
        {
            mLogger.InfoFormat("Module {0} was disconected", ModuleName);
            IsInitialized = false;
            pingManager.DisposeOldPinger(ModuleName);
            AgentSingleton.Instance.SetModuleStatus(ModuleName, ModuleStatus.Disconnected, ModuleStatusSources.Connectivity, null);
        }

        /// <summary>
        /// The disconnect.
        /// </summary>
        /// <param name="processId">
        /// The process Id.
        /// </param>
        public void DisconnectExt(int processId)
        {
            mLogger.InfoFormat("Module {0} process id {1} was disconected", ModuleName, processId);
            IsInitialized = false;
            pingManager.DisposeOldPinger(ModuleName);
            AgentSingleton.Instance.SetModuleStatus(ModuleName, ModuleStatus.Disconnected, ModuleStatusSources.Connectivity, processId);
        }

        /// <summary>
        /// The check availability.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckAvailability()
        {
            return true;
        }
    }
}
