using SentinelSNMPAgentStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SentinelCoreAgent
{
    /// <summary>
    /// The ping worker.
    /// </summary>
    public class Pinger : IDisposable
    {
        #region Members

        /// <summary>
        /// The _factory.
        /// </summary>
        private readonly ChannelFactory<IPingable> factory;

        /// <summary>
        /// The _timer clock.
        /// </summary>
        private readonly Timer timerClock = new Timer();

        /// <summary>
        /// The _m ping service.
        /// </summary>
        private IPingable mPingService;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Pinger"/> class.
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
        public Pinger(string moduleName, int processId, bool needToSetProcessId)
        {
            ModuleName = moduleName;
            ProcessId = processId;
            Uri tcpUri = new Uri(string.Format(AddressResolver.PingWcfServer, processId));
            EndpointAddress address = new EndpointAddress(tcpUri);
            NetNamedPipeBinding binding = new NetNamedPipeBinding();
            factory = new ChannelFactory<IPingable>(binding, address);
            mPingService = factory.CreateChannel();
            timerClock.Elapsed += OnPing;
            timerClock.Interval = 5000;
            if (needToSetProcessId)
            {
                AgentSingleton.Instance.SetProcessId(ModuleName, ProcessId);
            }
        }

        /// <summary>
        /// Gets the module name.
        /// </summary>
        public string ModuleName { get; private set; }

        /// <summary>
        /// Gets the process id.
        /// </summary>
        public int ProcessId { get; private set; }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (mPingService != null)
            {
                mPingService.Dispose();
            }

            if (factory != null)
            {
                factory.Close();
            }
        }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            timerClock.Enabled = true;
            mPingService = factory.CreateChannel();
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            timerClock.Enabled = false;
        }

        /// <summary>
        /// Ping module.
        /// If module doesn't response, Agent thinks that module is down and sets connectivity to Error
        /// </summary>
        /// <param name="sender">
        /// sender object
        /// </param>
        /// <param name="e">
        /// event object
        /// </param>
        private void OnPing(object sender, ElapsedEventArgs e)
        {
            try
            {
                mPingService.Ping(); // ping the module
                AgentSingleton.Instance.SetModuleStatus(ModuleName, ModuleStatus.NoError, ModuleStatusSources.Connectivity, ProcessId);
            }
            catch (Exception)
            {
                // IsRunning = false;
                // _timerClock.Enabled = false;
                AgentSingleton.Instance.SetModuleStatus(ModuleName, ModuleStatus.Error, ModuleStatusSources.Connectivity, ProcessId);
            }
        }
    }
}
