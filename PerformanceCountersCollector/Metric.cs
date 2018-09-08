using SentinelCoreAgent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCountersCollector
{
    public class Metric
    {
        public MetricType MetricType { set; get; }
        public string CategoryName { set; get; }
        public string CounterName { set; get; }
        public string CounterInstanceName { set; get; }
        public CounterType CounterType { set; get; }
        public int CollectingInterval { set; get; }
        public int Delta { set; get; }
    }
}
