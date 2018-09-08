using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SentinelCoreAgent.SerializationHelper;
using SentinelSNMPAgentStuff;
using System.Xml.Serialization;
using System.Xml;

namespace SentinelCoreAgent
{
    /// <summary>
    /// All information regarding component OID, name and status
    /// </summary>
    public class ConfigurationHandler : IConfigurationHandler
    {
        /// <summary>
        /// The config files extension.
        /// </summary>
        private const string CONFIG_FILE_EXTENSION = "*.xml";

        /// <summary>
        /// The modules config file name.
        /// </summary>
        private const string MODULES_TEMP_FILE_NAME = "ComponentsTempStatus.xml";

        /// <summary>
        /// The modules tree directory.
        /// </summary>
        private const string TEMP_DIR = "Temp";

        /// <summary>
        /// The config directory.
        /// </summary>
        private const string CONFIG_DIR = "Modules";

        /// <summary>
        /// The ping manager.
        /// </summary>
        private readonly IPingManager pingManager = new PingManager();

        /// <summary>
        /// Gets or sets the location current.
        /// </summary>
        private string LocationCurrent { get; set; }

        /// <summary>
        /// The load configuration.
        /// </summary>
        /// <param name="mLogger">
        /// The m Logger.
        /// </param>
        /// <returns>
        /// The <see cref="InfoStorage"/>.
        /// </returns>
        public InfoStorage LoadConfiguration(ILog mLogger)
        {
            mLogger.Info("Starting loading components configuration");
            LocationCurrent = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string pathToConfigFolder = string.Format(@"{0}\{1}\", LocationCurrent, CONFIG_DIR);
            string fullPathToTemp = string.Format(@"{0}\{1}\{2}\{3}", LocationCurrent, CONFIG_DIR, TEMP_DIR, MODULES_TEMP_FILE_NAME);

            InfoStorage result = null;
            if (Directory.Exists(pathToConfigFolder))
            {
                try
                {
                    TempModuleState temp = null;
                    if (File.Exists(fullPathToTemp))
                    {
                        mLogger.InfoFormat("Temp file {0} exists. Loading it.", MODULES_TEMP_FILE_NAME);
                        try
                        {
                            temp = InfoStorage.FromFile(new TempModuleState(), fullPathToTemp);
                        }
                        catch (Exception exception)
                        {
                            mLogger.ErrorFormat("Temp file {0} is corrupted!", MODULES_TEMP_FILE_NAME);
                            mLogger.Error("Exception: " + exception);
                        }
                    }
                    else
                    {
                        mLogger.Info("There is no temp file. Starting from the scratch");
                    }

                    string[] moduleConfigFiles = Directory.GetFiles(pathToConfigFolder, CONFIG_FILE_EXTENSION);
                    if (moduleConfigFiles.Length > 0)
                    {
                        result = new InfoStorage
                        {
                            ModulesInfo = new List<ModuleInfo>()
                        };

                        foreach (string moduleConfigFile in moduleConfigFiles)
                        {
                            var configFile = new FileInfo(moduleConfigFile);
                            mLogger.DebugFormat("Found config file: {0}", configFile.FullName);
                            try
                            {
                                ModuleInfo module = InfoStorage.FromFile(new ModuleInfo(), configFile.FullName);
                                if (module != null)
                                {
                                    if (temp != null)
                                    {
                                        var tmpModule = temp.ModuleStatus.Find(t => t.Name.Equals(module.Name));
                                        if (tmpModule != null)
                                        {
                                            if (tmpModule.ProcessId != 0 && tmpModule.Status != ModuleStatus.Disconnected.ToString())
                                            {
                                                pingManager.CreateNewPinger(tmpModule.Name, tmpModule.ProcessId, false);
                                            }

                                            module.ProcessId = tmpModule.ProcessId;
                                            module.Status = tmpModule.Status;
                                            module.SendAutoFixing = tmpModule.SendAutoFixing;
                                        }
                                        else
                                        {
                                            mLogger.WarnFormat("Temp data for module {0} was not found", module.Name);
                                        }
                                    }

                                    result.ModulesInfo.Add(module);
                                }
                                else
                                {
                                    mLogger.ErrorFormat("File {0} is corrupted!", moduleConfigFile);
                                }
                            }
                            catch (Exception exception)
                            {
                                mLogger.Error("Exception: " + exception);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    mLogger.Error("Exception: " + exception);
                }
            }
            else
            {
                mLogger.ErrorFormat("Modules configuration folder was not found. Path {0} does not exist!", pathToConfigFolder);
            }

            return result;
        }

        /// <summary>
        /// Serialize modules statuses tree
        /// </summary>
        /// <param name="mLogger">
        /// The m Logger.
        /// </param>
        /// <param name="modules">
        /// The modules.
        /// </param>
        public void SaveConfiguration(ILog mLogger, InfoStorage modules)
        {
            string tempLocation = string.Format(@"{0}\{1}\{2}", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), CONFIG_DIR, TEMP_DIR);
            if (!Directory.Exists(tempLocation))
            {
                Directory.CreateDirectory(tempLocation);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(TempModuleState));
            XmlWriterSettings writerSettings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true
            };
            var tmpSave = new TempModuleState
            {
                ModuleStatus = new List<ModuleStatusInfo>()
            };

            foreach (var component in modules.ModulesInfo)
            {
                tmpSave.ModuleStatus.Add(new ModuleStatusInfo
                {
                    Name = component.Name,
                    ProcessId = component.ProcessId,
                    Status = component.Status,
                    SendAutoFixing = component.SendAutoFixing
                });
            }

            using (XmlWriter xmlWriter = XmlWriter.Create(string.Format(@"{0}\{1}", tempLocation, MODULES_TEMP_FILE_NAME), writerSettings))
            {
                serializer.Serialize(xmlWriter, tmpSave);
            }
        }

        /// <summary>
        /// The initialize collector.
        /// </summary>
        /// <param name="mLogger">
        /// The m Logger.
        /// </param>
        /// <param name="collectorName">
        /// The collector name.
        /// </param>
        /// <returns>
        /// The <see cref="IMetricCollector"/>.
        /// </returns>
        public IMetricCollector InitializeCollector(ILog mLogger, string collectorName)
        {
            try
            {
                string currentLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!string.IsNullOrEmpty(currentLocation))
                {
                    string[] collectors = Directory.GetFiles(currentLocation, collectorName);
                    mLogger.InfoFormat("Loading {0}", collectorName);
                    foreach (string collector in collectors)
                    {
                        Assembly assembly = Assembly.LoadFrom(collector);
                        Type[] typesInAssembly = assembly.GetTypes();
                        foreach (Type type in typesInAssembly)
                        {
                            if (null != type.GetInterface(typeof(IMetricCollector).FullName))
                            {
                                mLogger.DebugFormat("Found appropriate type {0}. Creating instance", type);
                                return (IMetricCollector)Activator.CreateInstance(type);
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception exception)
            {
                mLogger.Error(exception);
                return null;
            }
        }
    }
}
