using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCountersCollector
{
    public class PerfCounterSampleHolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerfCounterSampleHolder"/> class.
        /// </summary>
        /// <param name="sample">
        /// The sample.
        /// </param>
        /// <param name="interval">
        /// The interval.
        /// </param>
        /// <param name="delta">
        /// The delta.
        /// </param>
        public PerfCounterSampleHolder(CounterSample sample, int interval, int delta)
        {
            Sample = sample;
            Interval = interval;
            CollectingTime = DateTime.Now;
            Delta = delta;
        }

        /// <summary>
        /// Gets or sets the sample.
        /// </summary>
        public CounterSample Sample { get; set; }

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Gets or sets the delta.
        /// </summary>
        public int Delta { get; set; }

        /// <summary>
        /// Gets or sets the collecting time.
        /// </summary>
        public DateTime CollectingTime { get; set; }

        /// <summary>
        /// The try to wait delta.
        /// </summary>
        public void TryToWaitDelta()
        {
            CollectingTime = CollectingTime.AddSeconds(Delta);
        }
    }
}
