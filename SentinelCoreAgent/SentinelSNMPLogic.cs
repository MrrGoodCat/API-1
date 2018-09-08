using SentinelCoreAgent.RegistryStuff;
using SentinelSNMPAgentStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SentinelCoreAgent
{
    /// <summary>
    /// The sentinel SNMP logic.
    /// </summary>
    public class SentinelSNMPLogic // : IMDSNMPAgentExt
    {
        /// <summary>
        /// The delay before agent server start.
        /// </summary>
        private const int DELAY_BEFORE_AGENT_SERVER_START = 15000;

        /// <summary>
        /// The timer clock.
        /// </summary>
        private readonly Timer timerClock = new Timer();

        /// <summary>
        /// The _m host.
        /// </summary>
        private ServiceHost mHost;

        /// <summary>
        /// The registry helper.
        /// </summary>
        private RegistryHelper mRegistryHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SentinelSNMPLogic"/> class.
        /// </summary>
        public SentinelSNMPLogic()
        {
            IsInitialized = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is initialized.
        /// </summary>
        private bool IsInitialized { get; set; }

        /// <summary>
        /// Gets the registry helper.
        /// </summary>
        private RegistryHelper RegHelper
        {
            get
            {
                return this.mRegistryHelper ?? (this.mRegistryHelper = new RegistryHelper());
            }
        }

        /// <summary>
        /// The connect to module.
        /// </summary>
        /// <returns>
        /// The <see cref="GenSNMPStatus"/>.
        /// </returns>
        //public GenSNMPStatus ConnectToModule()
        //{
        //    if (!IsInitialized)
        //    {
        //        return GenSNMPStatus.GSNMP_NOT_INITIALIZED;
        //    }

        //    AgentSingleton.Instance.ConnectToModule();
        //    return GenSNMPStatus.GSNMP_OK;
        //}

        /// <summary>
        /// The disconnect from module.
        /// </summary>
        /// <returns>
        /// The <see cref="GenSNMPStatus"/>.
        /// </returns>
        //public GenSNMPStatus DisconnectFromModule()
        //{
        //    if (!IsInitialized)
        //    {
        //        return GenSNMPStatus.GSNMP_NOT_INITIALIZED;
        //    }

        //    AgentSingleton.Instance.DisconnectFromModule();
        //    return GenSNMPStatus.GSNMP_OK;
        //}

        /// <summary>
        /// Finalization of Sentinel SNMP Agent
        /// </summary>
        /// <returns>status of requested finalization</returns>
        //public GenSNMPStatus FinalizeMDSNMPAgent()
        //{
        //    AgentSingleton.Instance.WriteLog("Start SNMP Agent finalization");
        //    if (!IsInitialized)
        //    {
        //        AgentSingleton.Instance.WriteLog("Impossible to finalize SNMP agent, because it was not init before");
        //        return GenSNMPStatus.GSNMP_NOT_INITIALIZED;
        //    }

        //    try
        //    {
        //        timerClock.Enabled = false;
        //        AgentSingleton.Instance.FinilizeModulesTree();
        //        mHost.Close();
        //        RegHelper.StopRegistryWatcher();
        //    }
        //    catch (Exception e)
        //    {
        //        AgentSingleton.Instance.WriteLog("Error during SNMP agent logic finalization. Exception: ", e.ToString());
        //        return GenSNMPStatus.GSNMP_FAILED;
        //    }
        //    finally
        //    {
        //        AgentSingleton.Instance.WriteLog("Finish SNMP Agent finalization");
        //        IsInitialized = false;
        //    }

        //    return GenSNMPStatus.GSNMP_OK;
        //}

        /// <summary>
        /// Initialization of Sentinel SNMP Agent
        /// </summary>
        /// <param name="genericAgent">
        /// reference of SNMP agent
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="GenSNMPStatus"/>.
        /// </returns>
        public APIStatus InitializeAPIAgent() //ref MModulesOidContainer container)
        {
            AgentSingleton.Instance.SetAgentInfra();
            //bool wasOldAgentDetected = UnloadOldVersionOfSnmpAgent();
            
            this.StartAgentServer();

            AgentSingleton.Instance.InitializeModulesTree();
            AgentSingleton.Instance.InitializeMetricCollectors();
            //AgentSingleton.Instance.GetModulesOIDs(ref container);
            IsInitialized = true;
            timerClock.Elapsed += SaveModulesTree;
            timerClock.Interval = 5000;
            timerClock.Enabled = true;

            return APIStatus.API_OK;
        }

        /// <summary>
        /// Queries module
        /// </summary>
        /// <param name="oidList">
        /// List of OID that are in SNMP request
        /// </param>
        /// <param name="mibData">
        /// structure that contains module statuses and metric values
        /// </param>
        /// <returns>
        /// status of operation
        /// </returns>
        //public GenSNMPStatus QueryModule(List<string> oidList, ref MGenericMIBData mibData)
        //{
        //    mibData = AgentSingleton.Instance.QueryModule(oidList);
        //    return GenSNMPStatus.GSNMP_OK;
        //}

        /// <summary>
        /// The start agent server with delay.
        /// </summary>
        public void StartAgentServerWithDelay()
        {
            Thread.Sleep(DELAY_BEFORE_AGENT_SERVER_START);
            //this.StartAgentServer();
        }

        /// <summary>
        /// The write exception.
        /// </summary>
        /// <param name="routineName">
        /// The routine name.
        /// </param>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="GenSNMPStatus"/>.
        /// </returns>
        //public GenSNMPStatus WriteException(string routineName, string msg)
        //{
        //    if (!IsInitialized)
        //    {
        //        return GenSNMPStatus.GSNMP_NOT_INITIALIZED;
        //    }

        //    AgentSingleton.Instance.WriteException(routineName, msg);
        //    return GenSNMPStatus.GSNMP_OK;
        //}

        /// <summary>
        /// The write log.
        /// </summary>
        /// <param name="routineName">
        /// The routine name.
        /// </param>
        /// <param name="msgType">
        /// The message type.
        /// </param>
        /// <param name="msg">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="GenSNMPStatus"/>.
        /// </returns>
        //public GenSNMPStatus WriteLog(string routineName, byte msgType, string msg)
        //{
        //    if (!IsInitialized)
        //    {
        //        return GenSNMPStatus.GSNMP_NOT_INITIALIZED;
        //    }

        //    AgentSingleton.Instance.WriteLog(routineName, msg);
        //    return GenSNMPStatus.GSNMP_OK;
        //}

        /// <summary>
        /// Serialize modules status tree every 5 sec
        /// </summary>
        /// <param name="sender">
        /// Sender object
        /// </param>
        /// <param name="e">
        /// Event itself
        /// </param>
        private void SaveModulesTree(object sender, ElapsedEventArgs e)
        {
            AgentSingleton.Instance.SerializeModulesTree();
        }

        /// <summary>
        /// Starts Sentinel SNMP Agent WCF server
        /// </summary>
        private void StartAgentServer()
        {
            mHost = new ServiceHost(typeof(TrapSendServer), new Uri(AddressResolver.AgentWcfServer));
            var binding = new NetNamedPipeBinding();
            mHost.AddServiceEndpoint(typeof(ISentinelMDSNMPAgent), binding, string.Empty);
            mHost.Open();
        }

        /// <summary>
        /// The prevent old agent to load again.
        /// </summary>
        private void PreventOldAgentToLoadAgain()
        {
            try
            {
                RegHelper.StartRegistryWatcher();
            }
            catch (Exception exception)
            {
                AgentSingleton.Instance.WriteLog("Error occured while trying to start new thread for preventing load of old agent ", exception.ToString());
            }
        }

        /// <summary>
        /// The unload old version of SNMP agent.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool UnloadOldVersionOfSnmpAgent()
        {
            try
            {
                return RegHelper.DeleteOldAgentKeys();
            }
            catch (Exception exception)
            {
                AgentSingleton.Instance.WriteException("Error occured while trying to delete old agent registry keys ", exception.ToString());
                return false;
            }
        }
    }
}
