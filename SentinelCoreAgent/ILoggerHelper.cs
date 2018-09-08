using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelCoreAgent
{
    /// <summary>
    /// Interface for loggers initialization simulation 
    /// </summary>
    public interface ILoggerHelper
    {
        /// <summary>
        /// The initialization logging.
        /// </summary>
        /// <param name="loggerName">
        /// The logger Name.
        /// </param>
        /// <param name="agentConfigName">
        /// The agent Config Name.
        /// </param>
        /// <returns>
        /// The <see cref="ILog"/>.
        /// </returns>
        ILog InitLogging(string loggerName, string agentConfigName);
    }
}
