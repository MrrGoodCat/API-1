using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SentinelSNMPAgentStuff;
using System.Globalization;
using System.Collections;
using SentinelCoreAgent.SerializationHelper;

namespace SentinelCoreAgent
{
    /// <summary>
    /// The agent singleton.
    /// </summary>
    public sealed class AgentSingleton
    {
        /// <summary>
        /// The modules error status.
        /// </summary>
        private const string ModulesErrorStatus = "Error";

        /// <summary>
        /// The modules no error status.
        /// </summary>
        private const string ModulesNoerrorStatus = "NoError";

        /// <summary>
        /// The component up trap id.
        /// </summary>
        private const int ComponentUpTrapId = 5112;

        /// <summary>
        /// The component down trap id.
        /// </summary>
        private const int ComponentDownTrapId = 5111;

        /// <summary>
        /// The _all modules OID.
        /// </summary>
        private readonly List<string> mAllModulesOid = new List<string>();

        /// <summary>
        /// The _synch lock status.
        /// </summary>
        private readonly object mSynchLockStatus = new object();

        /// <summary>
        /// The collectors assemblies.
        /// </summary>
        private readonly Dictionary<MetricType, string> collectorsAssemblies = new Dictionary<MetricType, string>
                                                                          {
                                                                              { MetricType.PerfCounter, "PerformanceCountersCollector.dll" },
                                                                              { MetricType.ServiceStatus, "ServicesStatusesCollector.dll" },
                                                                              { MetricType.ClusterInfo, "ClusterInfoCollector.dll" },
                                                                          };

        /// <summary>
        /// The metric collectors.
        /// </summary>
        private readonly Dictionary<MetricType, IMetricCollector> metricCollectors = new Dictionary<MetricType, IMetricCollector>();

        /// <summary>
        /// The OID request watcher.
        /// </summary>
        private readonly RequestedOidWatcher mOidRequestWatcher = new RequestedOidWatcher();

        /// <summary>
        /// The mLogger.
        /// </summary>
        private ILog mLogger;

        /// <summary>
        /// The _modules info list.
        /// </summary>
        private InfoStorage mModulesInfo;

        /// <summary>
        /// The configuration handler.
        /// </summary>
        private IConfigurationHandler mConfigurationHandler;

        /// <summary>
        /// The m logger helper.
        /// </summary>
        private ILoggerHelper mLoggerHelper;

        /// <summary>
        /// The _m generic agent.
        /// </summary>
        //private IGenericSNMPAgent mGenericAgent;

        #region Singleton implementation

        /// <summary>
        /// Prevents a default instance of the <see cref="AgentSingleton"/> class from being created.
        /// </summary>
        private AgentSingleton()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static AgentSingleton Instance
        {
            get { return Nested.InstanceNested; }
        }

        #endregion

        /// <summary>
        /// Getting reference to Generic SNMP Agent instance
        /// </summary>
        /// <param name="agent">
        /// ref instance of Generic SNMP Agent
        /// </param>
        public void SetAgentInfra()
        {
            mLoggerHelper = new LoggerHelper();
            mLogger = mLoggerHelper.InitLogging("SentinelSNMPLogger", "SentinelCoreSNMPAgent.dll.config");
            mConfigurationHandler = new ConfigurationHandler();
        }

        /// <summary>
        /// The set agent infra. FOR TESTING PURPOSE!
        /// </summary>
        /// <param name="agent">
        /// The agent.
        /// </param>
        /// <param name="mockLoggerHelper">
        /// The mock logger helper.
        /// </param>
        /// <param name="mockConfHandler">
        /// The mock configuration handler.
        /// </param>
        public void SetAgentInfra(ILoggerHelper mockLoggerHelper, IConfigurationHandler mockConfHandler)
        {

            mLoggerHelper = mockLoggerHelper;
            mLogger = mLoggerHelper.InitLogging("SentinelSNMPLogger", "SentinelCoreSNMPAgent.dll.config");
            mConfigurationHandler = mockConfHandler;
        }

        /// <summary>
        /// Method for initializing metric collectors...
        /// </summary>
        public void InitializeMetricCollectors()
        {
            mLogger.Info("Start collectors initialization");
            foreach (KeyValuePair<MetricType, string> collectorsAssembly in collectorsAssemblies)
            {
                mLogger.InfoFormat("Start initialization of {0} with relevant assembly {1}", collectorsAssembly.Key, collectorsAssembly.Value);
                if (!metricCollectors.ContainsKey(collectorsAssembly.Key))
                {
                    IMetricCollector initializedCollector = mConfigurationHandler.InitializeCollector(mLogger, collectorsAssembly.Value);
                    if (initializedCollector == null)
                    {
                        continue;
                    }

                    metricCollectors.Add(collectorsAssembly.Key, initializedCollector);
                }
            }

            // initialize default collector that would return values as is.
            if (!metricCollectors.ContainsKey(MetricType.Default))
            {
                IMetricCollector initializedCollector = new DefaultCollector();
                metricCollectors.Add(MetricType.Default, initializedCollector);
            }
        }

        /// <summary>
        /// Saves all known OID in the list and passes it to the native SNMP Agent.
        /// </summary>
        /// <param name="container">
        /// Reference to container with all known OIDs
        /// </param>
        public void GetModulesOIDs()//ref MModulesOidContainer container)
        {
            foreach (var moduleInfo in mModulesInfo.ModulesInfo)
            {
                // running over defined modules
                if (!string.IsNullOrEmpty(moduleInfo.OID))
                {
                    this.mAllModulesOid.Add(moduleInfo.OID);
                }

                if (!string.IsNullOrEmpty(moduleInfo.StatusOID))
                {
                    this.mAllModulesOid.Add(moduleInfo.StatusOID);
                }

                if (moduleInfo.MetricTables != null)
                {
                    foreach (MetricTableInfo metricTable in moduleInfo.MetricTables.MetricCollection)
                    {
                        if (!string.IsNullOrEmpty(metricTable.TableOid))
                        {
                            mAllModulesOid.Add(metricTable.TableOid);
                        }
                    }
                }
            }

            //container = new MModulesOidContainer
            //{
            //    ModuleOids = mAllModulesOid
            //};
        }

        /// <summary>
        /// Reads configuration files of modules and builds the tree (module info tree and module statuses tree)
        /// </summary>
        public void InitializeModulesTree()
        {
            mModulesInfo = mConfigurationHandler.LoadConfiguration(mLogger);
            if (mModulesInfo == null)
            {
                throw new Exception("Sentinel SNMP Agent failed to initialize modules configuration");
            }
        }

        /// <summary>
        /// Set current modules process Id to appropriate tree
        /// If process id differs from saved in temp - module was restarted (clean all errors)
        /// </summary>
        /// <param name="moduleName">
        /// Module name
        /// </param>
        /// <param name="processId">
        /// Process id
        /// </param>
        public void SetProcessId(string moduleName, int processId)
        {
            lock (mSynchLockStatus)
            {
                var module = mModulesInfo.ModulesInfo.Find(p => p.Name.Equals(moduleName));
                if (module.ProcessId != processId)
                {
                    mLogger.Info("Module was restarted");
                    ReinitializeModuleErrors(moduleName);
                    module.ProcessId = processId;
                }
                else
                {
                    mLogger.Info("Module wasn't restarted");
                }

                mLogger.InfoFormat("Setting process ID {0} to module {1}", processId, moduleName);
            }
        }

        /// <summary>
        /// The finalize modules tree.
        /// </summary>
        public void FinilizeModulesTree()
        {
            mLogger.Info("Start tree Finit");
            mModulesInfo = null;
        }

        /// <summary>
        /// Set module status
        /// </summary>
        /// <param name="moduleName">
        /// Module name
        /// </param>
        /// <param name="status">
        /// New status to change
        /// </param>
        /// <param name="sources">
        /// API, Connectivity, Traps
        /// </param>
        /// <param name="processId">
        /// process id
        /// </param>
        public void SetModuleStatus(string moduleName, ModuleStatus status, ModuleStatusSources sources, int? processId)
        {
            var module = mModulesInfo.ModulesInfo.Find(p => p.Name.Equals(moduleName));
            switch (sources)
            {
                case ModuleStatusSources.API:
                    lock (mSynchLockStatus)
                    {
                        module.Status = status.ToString();
                    }

                    mLogger.DebugFormat("Status via API was changed to {0}", status.ToString());
                    break;
                case ModuleStatusSources.Connectivity:
                    if (processId == module.ProcessId)
                    {
                        try
                        {
                            if (module.Status.Equals(Enum.GetName(typeof(ModuleStatus), ModuleStatus.Error)) && status == ModuleStatus.NoError)
                            {
                                mLogger.InfoFormat("Module {0} is UP! Sending Generic trap", moduleName);
                                RaiseTrap(InitializeDownUpTrap(ComponentUpTrapId, moduleName), moduleName, Guid.Empty);

                                // save state to temp file
                                SerializeModulesTree();
                            }

                            if (module.Status.Equals(Enum.GetName(typeof(ModuleStatus), ModuleStatus.NoError)) && status == ModuleStatus.Error)
                            {
                                mLogger.InfoFormat("Module {0} is DOWN! Sending Generic trap", moduleName);
                                RaiseTrap(InitializeDownUpTrap(ComponentDownTrapId, moduleName), moduleName, Guid.Empty);

                                // save state to temp file
                                SerializeModulesTree();
                            }
                        }
                        catch (Exception exception)
                        {
                            mLogger.Error("Error occured while trying to send generic trap");
                            mLogger.Error(exception);
                        }

                        lock (mSynchLockStatus)
                        {
                            module.Status = status.ToString();
                        }

                        mLogger.DebugFormat("Module: {0} Connectivity Status: {1}", moduleName, status.ToString());
                    }

                    break;
            }
        }

        /// <summary>
        /// If client was restarted - all errors should be cleared.
        /// </summary>
        /// <param name="moduleName">
        /// Module name
        /// </param>
        public void ReinitializeModuleErrors(string moduleName)
        {
            mLogger.InfoFormat("Start module error reinit for module {0}", moduleName);
            var module = mModulesInfo.ModulesInfo.Find(p => p.Name.Equals(moduleName));
            if (moduleName.Equals("TextCapture") && module.SendAutoFixing)
            {
                SendAutoFixTrapForTextCapture(moduleName);
            }
        }

        /// <summary>
        /// Gets specific modules status
        /// </summary>
        /// <param name="moduleName">
        /// Module name
        /// </param>
        /// <returns>
        /// Modules status
        /// </returns>
        public ModuleStatus GetModuleStatus(string moduleName)
        {
            var module = mModulesInfo.ModulesInfo.Find(p => p.Name.Equals(moduleName));
            return GetStatus(module.Status);
        }

        /// <summary>
        /// Initiate SNMP trap sending
        /// </summary>
        /// <param name="trap">
        /// Contains information about trap, like trap ID and variables with values.
        /// </param>
        /// <param name="moduleName">
        /// Contains information about module name
        /// </param>
        /// <param name="trapToClear">
        /// Guide of a trap to clear
        /// </param>
        public void RaiseTrap(SentinelTrap trap, string moduleName, Guid trapToClear)
        {
            try
            {
                mLogger.InfoFormat("RaiseTrap: Trap raising for trap ID: {0}", trap.TrapID.ToString(CultureInfo.InvariantCulture));
                var module = mModulesInfo.ModulesInfo.Find(p => p.Name.Equals(moduleName));
                if (moduleName.Equals("TextCapture") && trap.TrapID == 21002)
                {
                    mLogger.DebugFormat("{0} trap is TC autocleared trap.", trap.TrapID);
                    module.SendAutoFixing = true;
                }

                var newTrap = 0; //new MGenericTrapData
                //{
                //    EnterpriseIndex = GetOidIndex(module.OID),
                //    TrapID = trap.TrapID,
                //    IsSticky = true,
                //    TrapName = module.Name,
                //    VarBindList = new List<VarBindEntry>()
                //};
                int index = 0;
                if (trap.OrderedParameters != null)
                {
                    foreach (KeyValuePair<string, string> item in trap.OrderedParameters)
                    {
                        index++;
                        mLogger.DebugFormat("RaiseTrap: Varbind item type: {0}, value: {1}", item.Key, item.Value);
                        //newTrap.VarBindList.Add(new VarBindEntry
                        //{
                        //    TypeOID = string.Format("{0}.0.{1}", module.OID, index),
                        //    Value = item.Value
                        //});
                    }
                }
                else
                {
                    //foreach (DictionaryEntry item in trap.Parameters)
                    //{
                    //    index++;
                    //    mLogger.DebugFormat("RaiseTrap: Varbind item type: {0}, value: {1}", item.Key, item.Value);
                    //    newTrap.VarBindList.Add(new VarBindEntry
                    //    {
                    //        TypeOID = string.Format("{0}.0.{1}", module.OID, index),
                    //        Value = item.Value
                    //    });
                    //}
                }

                //DispatchTrap(newTrap); // Paasing trap to Agent in order to send it.
                mLogger.DebugFormat("RaiseTrap: Trap raising completed");
            }
            catch (Exception exception)
            {
                mLogger.Error("Error occured while sending a trap. ", exception);
            }
        }

        /// <summary>
        /// The serialize modules tree.
        /// </summary>
        public void SerializeModulesTree()
        {
            mConfigurationHandler.SaveConfiguration(mLogger, mModulesInfo);
        }

        /// <summary>
        /// Query module
        /// </summary>
        /// <param name="oidList">
        /// The OID List.
        /// </param>
        /// <returns>
        /// MGenericMIBData object with module name, it's status and metric table
        /// </returns>
        public List<string> QueryModule(List<string> oidList)
        {
            bool skipPublishValues = true;
            var metricEntry = new List<string>();
           // var statusEntry = new List<MStatusEntry>();
            var oidsToClearList = new List<int>();

            try
            {
                foreach (string oid in oidList)
                {
                    string requestedOid = oid;
                    mLogger.DebugFormat("Requested Oid is {0}", requestedOid);

                    // getting all modules to query
                    var modulesToQueryList = mModulesInfo.ModulesInfo.FindAll(p => ShouldOidBeProcessed(requestedOid, p.OID));
                    foreach (ModuleInfo moduleInfo in modulesToQueryList)
                    {
                        if (ShouldOidBeProcessed(requestedOid, moduleInfo.StatusOID))
                        {
                            if (!mOidRequestWatcher.AddOid(moduleInfo.StatusOID))
                            {
                                mLogger.DebugFormat("No need to republish module status {0}", moduleInfo.Name);
                                continue; // if data is still valid - skip query
                            }

                            if (!oidsToClearList.Contains(GetOidIndex(moduleInfo.StatusOID)))
                            {
                                oidsToClearList.Add(GetOidIndex(moduleInfo.StatusOID));
                            }

                            //if (moduleInfo.NotIntegrated)
                            //{
                            //    statusEntry.Add(new MStatusEntry(moduleInfo.Name, MIBSeverity.MIB_SEV_NOT_INTEGRATED, GetOidIndex(moduleInfo.StatusOID)));
                            //}
                            //else
                            //{
                            //    statusEntry.Add(new MStatusEntry(moduleInfo.Name, ConvertSeverity(GetStatus(moduleInfo.Status)), GetOidIndex(moduleInfo.StatusOID)));
                            //}

                            skipPublishValues = false;
                            mLogger.DebugFormat("Agent will update {0} status", moduleInfo.Name);
                        }

                        if (moduleInfo.MetricTables != null && moduleInfo.MetricTables.MetricCollection.Length > 0)
                        {
                            var queryMetricTables = from m in moduleInfo.MetricTables.MetricCollection.ToList()
                                                    where ShouldOidBeProcessed(requestedOid, m.TableOid)
                                                    select m;
                            foreach (MetricTableInfo metricTable in queryMetricTables)
                            {
                                mLogger.Debug("Query " + metricTable);
                                if (!mOidRequestWatcher.AddOid(metricTable.TableOid))
                                {
                                    mLogger.Debug("Metric table is UP TO DATE. Skipping it.");
                                    continue; // if data is still valid - skip query
                                }

                                if (!oidsToClearList.Contains(GetOidIndex(metricTable.TableOid)))
                                {
                                    oidsToClearList.Add(GetOidIndex(metricTable.TableOid));
                                }

                                mLogger.Debug("Asking collectors.");
                                List<List<string>> metricValues = GetMetricTable(metricTable.TableName, metricTable.MetricList.ToList());
                                if (metricValues.Count == 0)
                                {
                                    mLogger.DebugFormat("None of metrics were collected for {0} table", metricTable.TableName);
                                    continue;
                                }

                                skipPublishValues = false; // force Sentinel SNMP Agent to publish newly collected data
                                mLogger.DebugFormat("Agent will update {0} metric table", metricTable.TableName);
                                metricEntry.AddRange(metricValues.FirstOrDefault());
                                {
                                    
                                }; //MetricValuesList = metric
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                mLogger.Error(exception);
            }

            if (skipPublishValues)
            {
                return null;
                // should Sentinel SNMP Agent publish newly collected data or not.
                //return new MGenericMIBData
                //{
                //    SkipPublishing = true,
                //};
            }

            return metricEntry;
            //return new MGenericMIBData
            //{
            //    SkipPublishing = false,
            //    MetricList = metricEntry,
            //    StatusList = statusEntry,
            //    OidClearIndexList = oidsToClearList
            //};
        }

        /// <summary>
        /// Checks if tree contains relevant module information
        /// </summary>
        /// <param name="moduleName">
        /// Module name
        /// </param>
        /// <returns>
        /// boolean result of check
        /// </returns>
        public bool CheckModuleConfiguration(string moduleName)
        {
            var module = mModulesInfo.ModulesInfo.Find(t => (t.Name.Equals(moduleName) || t.OID.Equals(moduleName)));
            return module != null;
        }

        /// <summary>
        /// Get module name. Support for connecting to Agent via OID
        /// </summary>
        /// <param name="module">
        /// Module name
        /// </param>
        /// <returns>
        /// module name
        /// </returns>
        public string GetModuleName(string module)
        {
            var moduleInfo = mModulesInfo.ModulesInfo.Find(t => (t.Name.Equals(module) || t.OID.Equals(module)));
            return moduleInfo.Name;
        }

        /// <summary>
        /// Initialization logging for MD SNMP Agent
        /// </summary>
        /// <param name="log">
        /// The logger of request keeper class
        /// </param>
        public void GetLogger(ref ILog log)
        {
            log = mLogger;
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
        public void WriteException(string routineName, string msg)
        {
            mLogger.ErrorFormat("{0}: {1}", routineName, msg);
        }

        /// <summary>
        /// The write log.
        /// </summary>
        /// <param name="routineName">
        /// The routine name.
        /// </param>
        /// <param name="msg">
        /// The message.
        /// </param>
        public void WriteLog(string routineName, string msg)
        {
            mLogger.DebugFormat("{0}: {1}", routineName, msg);
        }

        /// <summary>
        /// The write log.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        public void WriteLog(string msg)
        {
            mLogger.Debug(msg);
        }

        /// <summary>
        /// The disconnect from module.
        /// </summary>
        public void DisconnectFromModule()
        {
            mLogger.InfoFormat("MDSNMPAgentMocker.DisconnectFromModule Disconnecting from module");
            ClientDisconnected();
        }

        /// <summary>
        /// The connect to module.
        /// </summary>
        public void ConnectToModule()
        {
            mLogger.InfoFormat("MDSNMPAgentMocker.ConnectToModule Trying connection to module");
            ClientConnected();
        }

        /// <summary>
        /// The should OID be processed.
        /// </summary>
        /// <param name="requestedOid">
        /// The requested OID.
        /// </param>
        /// <param name="componentOid">
        /// The component OID.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ShouldOidBeProcessed(string requestedOid, string componentOid)
        {
            return (requestedOid.Contains(componentOid + ".") || componentOid.Contains(requestedOid + ".") || componentOid.Equals(requestedOid)) && requestedOid.Contains(SNMPAgentConstants.AGENT_ENTERPRISE_OID);
        }

        /// <summary>
        /// Getting table with values for defined list of metrics. 
        /// But first checking Cache.
        /// </summary>
        /// <param name="moduleName">
        /// Module name
        /// </param>
        /// <param name="metricList">
        /// Metric list to collect
        /// </param>
        /// <returns>
        /// table with metric values
        /// </returns>
        private List<List<string>> GetMetricTable(string moduleName, List<MetricInfo> metricList)
        {
            mLogger.DebugFormat("Getting metric table for {0} module", moduleName);
            List<MetricType> metricTypes =
                (from m in metricList group m by m.ParsedMetricType into mg select mg.Key).ToList();
            List<List<string>> metricTable = new List<List<string>>();
            CollectorResult result = null;
            foreach (var type in metricTypes)
            {
                if (metricCollectors.ContainsKey(type))
                {
                    mLogger.DebugFormat("Collecting metrics of {0} type", type);
                    var metricsToCollect = metricList.FindAll(m => m.ParsedMetricType == type);
                    result = metricCollectors[type].CollectMetrics(moduleName, metricsToCollect, mLogger);
                }

                if (result != null)
                {
                    foreach (var values in result.MetricTable)
                    {
                        mLogger.DebugFormat("Collected Metric values: {0} Module: {1}.", string.Join(",", values.ToArray()), moduleName);
                        metricTable.Add(values);
                    }
                }
                else
                {
                    mLogger.ErrorFormat("For type {0} result of collecting metric is null. Module: {1}", type, moduleName);
                }
            }

            return metricTable;
        }

        /// <summary>
        /// Creating generic traps UP, DOWN component
        /// </summary>
        /// <param name="id">
        /// trap id
        /// </param>
        /// <param name="moduleName">
        /// module name
        /// </param>
        /// <returns>
        /// The <see cref="SentinelTrap"/>.
        /// </returns>
        private SentinelTrap InitializeDownUpTrap(int id, string moduleName)
        {
            SentinelTrap trap = new SentinelTrap(id);
            trap.AddParameter("ModuleName", moduleName);
            return trap;
        }

        /// <summary>
        /// The client disconnected.
        /// </summary>
        private void ClientDisconnected()
        {
            mLogger.InfoFormat("Client disconnected! Notifying GenericSNMPAgent");
            //if (mGenericAgent.ClientDisconnected() != GenSNMPStatus.GSNMP_OK)
            //{
            //    mLogger.ErrorFormat("Failed to notify GenericSNMPAgent on disconnection from module");
            //}
            //else
            //{
            //    mLogger.InfoFormat("Successfully notified GenericSNMPAgent on disconnection from module");
            //}
        }

        /// <summary>
        /// Converts from string to enumerator Module status
        /// </summary>
        /// <param name="status">
        /// Status to analyze
        /// </param>
        /// <returns>
        /// The <see cref="ModuleStatus"/>.
        /// </returns>
        private ModuleStatus GetStatus(string status)
        {
            switch (status)
            {
                case ModulesNoerrorStatus:
                    return ModuleStatus.NoError;
                case ModulesErrorStatus:
                    return ModuleStatus.Error;
                default:
                    return ModuleStatus.Unknown;
            }
        }

        /// <summary>
        /// Method sends auto fixing trap for Text Capture component.
        /// This is the only component that has such trap. That's why it is hardcoded.
        /// </summary>
        /// <param name="moduleName">
        /// The module Name.
        /// </param>
        private void SendAutoFixTrapForTextCapture(string moduleName)
        {
            try
            {
                SentinelTrap fixTrapToSend = new SentinelTrap(21502);
                fixTrapToSend.AddParameter("ErrorCause", "AutoFixingTrap");
                RaiseTrap(fixTrapToSend, moduleName, Guid.Empty);
            }
            catch (Exception exception)
            {
                mLogger.Error(exception);
            }
        }

        /// <summary>
        /// Gets the index of appropriate OID.
        /// </summary>
        /// <param name="oidToFind">
        /// OID the index of which should be searched
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int GetOidIndex(string oidToFind)
        {
            int index = mAllModulesOid.IndexOf(oidToFind);
            if (index == -1)
            {
                throw new Exception(string.Format("The index of OID {0} was not found! Please check the configuration file.", oidToFind));
            }

            return index;
        }

        /// <summary>
        /// The client connected.
        /// </summary>
        private void ClientConnected()
        {
            mLogger.InfoFormat("Connection to module succeeded! Notifying GenericSNMPAgent");
            //if (mGenericAgent.ClientConnected() != GenSNMPStatus.GSNMP_OK)
            //{
            //    mLogger.ErrorFormat("Failed to notify GenericSNMPAgent on connection to module");
            //}
            //else
            //{
            //    mLogger.InfoFormat("Successfully notified GenericSNMPAgent on connection to module");
            //}
        }

        /// <summary>
        /// Clear error traps:
        ///             all that don't have fix trap - if "trapToClear" is Empty
        ///             with specific guide - if "trapToClear" is not Empty
        /// </summary>
        /// <summary>
        /// Pass trap to SNMP Agent
        /// </summary>
        /// <param name="trapData">
        /// Trap to be send
        /// </param>
        //private void DispatchTrap(MGenericTrapData trapData)
        //{
        //    mLogger.InfoFormat("Dispatching a trap to GenericSNMPAgent");
        //    GenSNMPStatus status = mGenericAgent.HandleTrap(trapData);
        //    if (status != GenSNMPStatus.GSNMP_OK)
        //    {
        //        mLogger.ErrorFormat("GenericSNMPAgent failed handling a trap");
        //    }
        //}

        /// <summary>
        /// The convert severity.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <returns>
        /// The <see cref="MIBSeverity"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// In case unknown module status
        /// </exception>
        //private MIBSeverity ConvertSeverity(ModuleStatus status)
        //{
        //    switch (status)
        //    {
        //        case ModuleStatus.NoError:
        //            return MIBSeverity.MIB_SEV_NO_ERROR;
        //        case ModuleStatus.Error:
        //            return MIBSeverity.MIB_SEV_ERROR;
        //        case ModuleStatus.Unknown:
        //            return MIBSeverity.MIB_SEV_UNKNOWN;
        //        default:
        //            throw new ArgumentException("Unknown module status" + (int)status);
        //    }
        //}

        /// <summary>
        /// The nested.
        /// </summary>
        private class Nested
        {
            /// <summary>
            /// The instance.
            /// </summary>
            internal static readonly AgentSingleton InstanceNested = new AgentSingleton();

            /// <summary>
            /// Prevents a default instance of the <see cref="Nested"/> class from being created.
            /// </summary>
            private Nested()
            {
            }
        }
    }
}
