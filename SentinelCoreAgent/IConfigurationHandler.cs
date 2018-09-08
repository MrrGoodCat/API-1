using log4net;
using SentinelCoreAgent.SerializationHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentinelCoreAgent
{
    /// <summary>
    /// Interface for simulating operations with Agent configuration
    /// </summary>
    public interface IConfigurationHandler
    {
        /// <summary>
        /// The load configuration.
        /// </summary>
        /// <param name="mLogger">
        /// The m logger.
        /// </param>
        /// <returns>
        /// The <see cref="InfoStorage"/>.
        /// </returns>
        InfoStorage LoadConfiguration(ILog mLogger);

        /// <summary>
        /// The save configuration.
        /// </summary>
        /// <param name="mLogger">
        /// The m logger.
        /// </param>
        /// <param name="modules">
        /// The modules.
        /// </param>
        void SaveConfiguration(ILog mLogger, InfoStorage modules);

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
        IMetricCollector InitializeCollector(ILog mLogger, string collectorName);
    }
}
