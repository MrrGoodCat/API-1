using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceCountersCollector
{
    public enum ModuleStatus
    {
        /// <summary>
        /// The no error.
        /// </summary>
        NoError = 0,

        /// <summary>
        /// The error.
        /// </summary>
        Error = 1,

        /// <summary>
        /// The unknown.
        /// </summary>
        Unknown = 2,

        /// <summary>
        /// The disconnected.
        /// </summary>
        Disconnected = 3
    }
}
