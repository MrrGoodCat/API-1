using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCountersCollector
{
    using log4net;
    using SentinelCoreAgent;
    using SentinelCoreAgent.SerializationHelper;
    using SentinelSNMPAgentStuff;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The performance counters collector.
    /// </summary>
    public class PerformanceCountersCollector //: IMetricCollector
    {
        /// <summary>
        /// The categories cache timeout.
        /// </summary>
        private const int CATEGORIES_CACHE_TIMEOUT = 10;

        /// <summary>
        /// The average values.
        /// </summary>
        private readonly Dictionary<string, string> averageValues = new Dictionary<string, string>();

        /// <summary>
        /// The counter samples.
        /// </summary>
        private readonly Dictionary<string, PerfCounterSampleHolder> counterSamples =
            new Dictionary<string, PerfCounterSampleHolder>();

        /// <summary>
        /// The performance counter wrapper.
        /// </summary>
        private IPerformanceCounterCategoryWrapper performanceCounterWrapper;

        /// <summary>
        /// The all categories.
        /// </summary>
        private List<PerformanceCounterCategory> allCategories;

        /// <summary>
        /// The last categories sample time.
        /// </summary>
        private DateTime lastCategoriesSampleTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceCountersCollector"/> class.
        /// </summary>
        public PerformanceCountersCollector()
        {
            this.performanceCounterWrapper = new PerformanceCounterCategoryWrapper();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceCountersCollector"/> class.
        /// </summary>
        /// <param name="performanceCounterWrapper">
        /// The performance Counter Wrapper.
        /// </param>
        public PerformanceCountersCollector(IPerformanceCounterCategoryWrapper performanceCounterWrapper)
        {
            this.performanceCounterWrapper = performanceCounterWrapper;
        }

        /// <summary>
        /// Gets performance counters on a local machine
        /// </summary>
        /// <param name="moduleName">
        /// The name of module to be queried
        /// </param>
        /// <param name="countersList">
        /// a list of performance counters that should be collected
        /// </param>
        /// <param name="logger">
        /// Logger object
        /// </param>
        /// <returns>
        /// CollectorResult object - contains results and exceptions after collection
        /// </returns>
        // ReSharper disable once FunctionComplexityOverflow
        public CollectorResult CollectMetrics(string moduleName, List<MetricInfo> countersList, ILog logger)
        {
            bool isPerfCounterTableRebootRequired = false;
            bool wasCategoriesResampled = false;
            var resultedMetricTable = new Dictionary<string, string>();
            bool failedToGetCategories = false;

            try
            {
                if (this.lastCategoriesSampleTime.AddMinutes(CATEGORIES_CACHE_TIMEOUT) < DateTime.Now)
                {
                    this.allCategories = new List<PerformanceCounterCategory>();
                    try
                    {
                        // Resample all categories on the local machine once per timeout in order to reduce performance load
                        this.allCategories = this.performanceCounterWrapper.GetCategories();
                        this.lastCategoriesSampleTime = DateTime.Now;
                        wasCategoriesResampled = true;
                    }
                    catch (Exception ex)
                    {
                        failedToGetCategories = true;
                        logger.Error("Error occured while getting performance counters categories", ex);
                    }
                }

                logger.Debug("Got all performance counters categories on the server");
                foreach (MetricInfo arg in countersList)
                {
                    List<PerformanceCounterCategory> matchedCategories;
                    bool isCategoryDynamic = false;
                    bool isInstanceDynamic = false;

                    // first it is required to check, whether we have real name or regex template.
                    // As indication will be symbol "*"
                    if (arg.CategoryName.Contains(SNMPAgentConstants.REGEX_SYMBOL))
                    {
                        matchedCategories = this.allCategories.FindAll(c => Regex.Match(c.CategoryName, arg.CategoryName).Success);
                        isCategoryDynamic = true;
                    }
                    else
                    {
                        matchedCategories = this.allCategories.FindAll(c => c.CategoryName.Equals(arg.CategoryName));
                    }

                    if (!matchedCategories.Any())
                    {
                        logger.WarnFormat("The counter category {0} is missing", arg.CategoryName);
                        isPerfCounterTableRebootRequired = true;
                        resultedMetricTable.Add(arg.CounterName, failedToGetCategories ? SNMPAgentConstants.FAILED_TO_COLLECT_STATE : SNMPAgentConstants.NOT_EXISTS_STATE); // adding the value to resulted table
                    }

                    foreach (PerformanceCounterCategory category in matchedCategories)
                    {
                        List<string> instances;
                        if (arg.CounterInstanceName.Contains(SNMPAgentConstants.REGEX_SYMBOL))
                        {
                            isInstanceDynamic = true;
                            instances = this.performanceCounterWrapper.GetInstanceNames(category).FindAll(i => Regex.Match(i, arg.CounterInstanceName).Success);
                        }
                        else
                        {
                            instances = new List<string> { arg.CounterInstanceName };
                        }

                        foreach (string instance in instances)
                        {
                            string counterName = isInstanceDynamic ? instance : arg.CounterName;
                            string metricName = isCategoryDynamic
                                ? string.Format("{0}-{1}", category.CategoryName, counterName)
                                : counterName;

                            this.GetThePerfCounterValue(
                                moduleName,
                                metricName,
                                arg,
                                instance,
                                category,
                                resultedMetricTable,
                                logger);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.Error("Error occured while collecting performance counters", exception);
            }

            if (isPerfCounterTableRebootRequired && wasCategoriesResampled)
            {
                // be carefull this method can make MEMORY LEAKS!
                // releasing all perf counters and updating internal hash tables
                PerformanceCounter.CloseSharedResources();
            }
            
            return new CollectorResult(resultedMetricTable);
        }

        /// <summary>
        /// The get the performance counter value.
        /// </summary>
        /// <param name="moduleName">
        /// Module Name
        /// </param>
        /// <param name="metricName">
        /// The metric name.
        /// </param>
        /// <param name="metric">
        /// The metric.
        /// </param>
        /// <param name="instanceName">
        /// The instance Name.
        /// </param>
        /// <param name="category">
        /// Category name
        /// </param>
        /// <param name="metricTable">
        /// The metric table.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        private void GetThePerfCounterValue(string moduleName, string metricName, MetricInfo metric, string instanceName, PerformanceCounterCategory category, Dictionary<string, string> metricTable, ILog logger)
        {
            try
            {
                if (metric.CounterType.Equals("Average"))
                {
                    // checking if it is perfcounter of average type
                    logger.DebugFormat("Performace counter {0} - {1} is average", category.CategoryName, metric.CounterName);
                    var val = this.performanceCounterWrapper.GetPerfCounterNextSample(category, metric.CounterName, instanceName, logger);
                    if (val != null)
                    {
                        // creating sample holder for perf counter
                        var holder = new PerfCounterSampleHolder((CounterSample)val, metric.CollectingInterval, metric.Delta);

                        // calculating the value of avarage perf counter
                        this.UpdateCounterSamples(metricName, metricTable, moduleName, metric.CounterName, holder, logger);
                    }
                }
                else
                {
                    // it is simple perfcounter
                    logger.DebugFormat("Performace counter {0} - {1} is simple. Getting it's value", category.CategoryName, metric.CounterName);
                    var val = this.performanceCounterWrapper.GetPerfCounterValue(category, metric.CounterName, instanceName, logger);

                    if (val != null)
                    {
                        if (!metricTable.ContainsKey(metricName))
                        {
                            metricTable.Add(metricName, ((float)val).ToString(CultureInfo.InvariantCulture)); // adding the value to resulted table
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.ErrorFormat("Error occured while trying to performance counter {0} status", metricName);
                logger.Error(exception);
            }

            if (!metricTable.ContainsKey(metricName))
            {
                logger.ErrorFormat("Error occured while trying to collect metric {0} state", metricName);
                //metricTable.Add(metricName, SNMPAgentConstants.FAILED_TO_COLLECT_STATE); // adding the value to resulted table
            }
        }

        /// <summary>
        /// Calculates average performance counters.
        /// </summary>
        /// <param name="metricName">
        /// Metric name it will be passed to Sentinel
        /// </param>
        /// <param name="table">
        /// metric table
        /// </param>
        /// <param name="moduleName">
        /// The key part 1. Module name + the name of metric that is collected
        /// </param>
        /// <param name="counterName">
        /// The key part 2. Module name + the name of metric that is collected
        /// </param>
        /// <param name="collectedSample">
        /// performance counter sample
        /// </param>
        /// <param name="logger">
        /// Logger object
        /// </param>
        private void UpdateCounterSamples(string metricName, Dictionary<string, string> table, string moduleName, string counterName, PerfCounterSampleHolder collectedSample, ILog logger)
        {
            // As a key for CounterSamples dictionary combination of module name and metric name will be used
            // (for cases different modules - the same metric name)
            if (!this.counterSamples.ContainsKey(moduleName + "_" + metricName))
            {
                // checking whether it is first sample
                logger.DebugFormat("Adding first sample of {0}", moduleName + "_" + counterName);
                this.counterSamples.Add(moduleName + "_" + metricName, collectedSample); // holds last sample of relevant perf counter
            }
            else
            {
                logger.DebugFormat("We already have sample for {0}", moduleName + "_" + metricName);
                PerfCounterSampleHolder currentSample = this.counterSamples[moduleName + "_" + metricName]; // the smaple was collected and previous polling
                DateTime checkTime = currentSample.CollectingTime.AddSeconds(currentSample.Interval); // adding collecting period
                if (checkTime < collectedSample.CollectingTime)
                {
                    // checking if it is time to culculate the value of average perf counter
                    logger.Debug("It is time to calculate the new perf. counter value.");
                    if (currentSample.Sample.TimeStamp100nSec == collectedSample.Sample.TimeStamp100nSec)
                    {
                        // checking if not the same sample
                        logger.Debug("The sample is the same");
                        currentSample.TryToWaitDelta(); // waiting delta
                        this.UpdateMetricTableWithExistingValueOrNA(metricName, table, logger);
                        return;
                    }

                    if (!table.ContainsKey(metricName))
                    {
                        if (!this.averageValues.ContainsKey(metricName))
                        {
                            this.averageValues.Add(metricName, CounterSample.Calculate(currentSample.Sample, collectedSample.Sample).ToString(CultureInfo.InvariantCulture)); // add value
                        }
                        else
                        {
                            this.averageValues[metricName] =
                                CounterSample.Calculate(currentSample.Sample, collectedSample.Sample).ToString(CultureInfo.InvariantCulture); // update value
                        }

                        logger.DebugFormat("Updating value of counter {0} with new one: {1}", metricName, this.averageValues[metricName]);
                        table.Add(metricName, this.averageValues[metricName]); // add value to resulted table
                        collectedSample.CollectingTime = checkTime; // update time
                        logger.Debug("Updating sample");
                        this.counterSamples[moduleName + "_" + metricName] = collectedSample; // update sample
                    }
                }
            }

            this.UpdateMetricTableWithExistingValueOrNA(metricName, table, logger);
        }

        /// <summary>
        /// The update metric table with existing value or NA.
        /// </summary>
        /// <param name="metricName">
        /// The metric name.
        /// </param>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        private void UpdateMetricTableWithExistingValueOrNA(string metricName, Dictionary<string, string> table, ILog logger)
        {
            if (!table.ContainsKey(metricName))
            {
                if (this.averageValues.ContainsKey(metricName))
                {
                    logger.Debug("Returning previos value of counter");
                    table.Add(metricName, this.averageValues[metricName]); // if it is not the time to calculate - returning previous value
                }
                else
                {
                    logger.DebugFormat("Returning NA state for the metric {0}", metricName);
                    table.Add(metricName, "NA"); // if it is not the time to calculate - returning previous value
                }
            }
        }
    }
}
