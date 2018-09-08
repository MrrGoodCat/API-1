using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCountersCollector
{
    public interface IPerformanceCounterCategoryWrapper
    {
        /// <summary>
        /// The get categories.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        List<PerformanceCounterCategory> GetCategories();

        /// <summary>
        /// The get counters.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        List<PerformanceCounter> GetCounters(PerformanceCounterCategory category);

        /// <summary>
        /// The get instance names.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        List<string> GetInstanceNames(PerformanceCounterCategory category);

        /// <summary>
        /// The get counters.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        List<PerformanceCounter> GetCounters(PerformanceCounterCategory category, string instance);

        /// <summary>
        /// The get performance counter value.
        /// </summary>
        /// <param name="counterCategory">
        /// The counter category.
        /// </param>
        /// <param name="counterName">
        /// The counter name.
        /// </param>
        /// <param name="instanceName">
        /// The instance name.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <returns>
        /// The <see cref="float?"/>.
        /// </returns>
        float? GetPerfCounterValue(PerformanceCounterCategory counterCategory, string counterName, string instanceName, ILog logger);

        /// <summary>
        /// The get performance counter next sample.
        /// </summary>
        /// <param name="counterCategory">
        /// The counter category.
        /// </param>
        /// <param name="counterName">
        /// The counter name.
        /// </param>
        /// <param name="instanceName">
        /// The instance name.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <returns>
        /// The <see cref="CounterSample?"/>.
        /// </returns>
        CounterSample? GetPerfCounterNextSample(PerformanceCounterCategory counterCategory, string counterName, string instanceName, ILog logger);
        /// <summary>
        /// Get CPU usage based on Win32
        /// </summary>
        /// <returns></returns>
        object GetCpuUsage();
    }
}
