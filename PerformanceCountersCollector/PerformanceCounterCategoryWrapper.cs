using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCountersCollector
{
    /// <summary>
    /// Wraps main methods of Performance counters categories classes
    /// </summary>
    public class PerformanceCounterCategoryWrapper : IPerformanceCounterCategoryWrapper
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
        public List<PerformanceCounterCategory> GetCategories()
        {
            return PerformanceCounterCategory.GetCategories().ToList();
        }

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
        public List<PerformanceCounter> GetCounters(PerformanceCounterCategory category)
        {
            return category.GetCounters().ToList();
        }

        /// <summary>
        /// The get instance names.
        /// </summary>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        public List<string> GetInstanceNames(PerformanceCounterCategory category)
        {
            return category.GetInstanceNames().ToList();
        }

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
        public List<PerformanceCounter> GetCounters(PerformanceCounterCategory category, string instance)
        {
            return category.GetCounters(instance).ToList();
        }

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
        public float? GetPerfCounterValue(PerformanceCounterCategory counterCategory, string counterName, string instanceName, ILog logger)
        {
            try
            {
                if (counterCategory == null)
                {
                    logger.ErrorFormat("Performance counter category: '{0}' is not exist.", counterCategory);
                    return null;
                }
                if (counterCategory.CounterExists(counterName))
                {
                    var performanceCounter = new PerformanceCounter(counterCategory.CategoryName, counterName, instanceName);
                    using (performanceCounter)
                    {
                        return performanceCounter.NextValue();
                    }
                }
                else
                {
                    logger.ErrorFormat("Performance counter {0} {1} is missing on server", counterCategory.CategoryName, counterName);
                }
            }
            catch (Exception exception)
            {
                logger.ErrorFormat("Error occurred while getting counter {0} for category {1}. {2}", counterName, counterCategory.CategoryName, exception);
            }

            return null;
        }

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
        public CounterSample? GetPerfCounterNextSample(PerformanceCounterCategory counterCategory, string counterName, string instanceName, ILog logger)
        {
            try
            {
                if (counterCategory == null)
                {
                    logger.ErrorFormat("Performance counter category: '{0}' is not exist.", counterCategory);
                    return null;
                }
                if (counterCategory.CounterExists(counterName))
                {
                    var performanceCounter = new PerformanceCounter(counterCategory.CategoryName, counterName, instanceName);
                    using (performanceCounter)
                    {
                        return performanceCounter.NextSample();
                    }
                }
                else
                {
                    logger.ErrorFormat("Performance counter {0} {1} is missing on server", counterCategory.CategoryName, counterName);
                }
            }
            catch (Exception exception)
            {
                logger.ErrorFormat("Error occurred while getting counter {0} for category {1}. {2}", counterName, counterCategory.CategoryName, exception);
            }

            return null;
        }

        /// <summary>
        /// Get CPU usage based on Win32
        /// </summary>
        /// <returns></returns>
        public object GetCpuUsage()
        {
            //Get CPU usage values using a WMI query
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor");
            var cpuTimes = searcher.Get().Cast<ManagementObject>().Select(mo => new
            {
                Name = mo["Name"],
                Usage = mo["PercentProcessorTime"]
            }
            )
            .ToList();

            //The '_Total' value represents the average usage across all cores,
            //and is the best representation of overall CPU usage
            var query = cpuTimes.Where(x => x.Name.ToString() == "_Total").Select(x => x.Usage);
            var cpuUsage = query.SingleOrDefault();

            return cpuUsage;
        }
    }
}
