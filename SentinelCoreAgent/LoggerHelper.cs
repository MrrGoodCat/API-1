using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SentinelCoreAgent
{
    /// <summary>
    /// The logger helper.
    /// </summary>
    public class LoggerHelper : ILoggerHelper
    {
        /// <summary>
        /// The default log path.
        /// </summary>
        public const string DEFAULT_LOG_PATH = @"C:\";

        /// <summary>
        /// The agent log file name.
        /// </summary>
        public const string AGENT_LOGFILE_NAME = "SentinelSNMPAgentExt.log.txt";

        /// <summary>
        /// Initialization of a logger for Sentinel SNMP Agent (all parameters in config file)
        /// </summary>
        /// <param name="loggerName">
        /// The logger Name.
        /// </param>
        /// <param name="configName">
        /// The agent Config Name.
        /// </param>
        /// <returns>
        /// The <see cref="ILog"/>.
        /// </returns>
        public ILog InitLogging(string loggerName, string configName)
        {
            ILog logger = null;
            try
            {
                string logPath = string.Format("{0}{1}", DEFAULT_LOG_PATH, AGENT_LOGFILE_NAME);
                string niceLogPath = this.GetLogLocation();
                if (!string.IsNullOrEmpty(niceLogPath))
                {
                    string logLocation = string.Format(@"{0}\Sentinel SNMP Agent\", niceLogPath);
                    if (!Directory.Exists(logLocation))
                    {
                        Directory.CreateDirectory(logLocation);
                    }

                    logPath = string.Format("{0}{1}", logLocation, AGENT_LOGFILE_NAME);
                }

                GlobalContext.Properties["SentinelSNMPAgentLogPath"] = logPath;
                string pathToConfig = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + configName;
                XmlConfigurator.Configure(new FileInfo(pathToConfig));
                logger = LogManager.GetLogger(loggerName);
            }
            catch
            {
            }

            return logger;
        }

        /// <summary>
        /// The get log location.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetLogLocation()
        {
            string logLocation = Environment.GetEnvironmentVariable("NiceLogLocation", EnvironmentVariableTarget.Machine);
            if (string.IsNullOrEmpty(logLocation))
            {
                logLocation = Environment.GetEnvironmentVariable("NiceLogLocation", EnvironmentVariableTarget.User);
            }

            if (string.IsNullOrEmpty(logLocation))
            {
                logLocation = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            }

            return logLocation;
        }
    }
}
