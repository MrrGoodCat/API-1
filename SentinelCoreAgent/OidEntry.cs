using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace SentinelCoreAgent
{
    /// <summary>
    /// The OID entry.
    /// </summary>
    internal class OidEntry
    {
        /// <summary>
        /// The _timer clock.
        /// </summary>
        private readonly Timer mTimerClock = new Timer();

        /// <summary>
        /// Initializes a new instance of the <see cref="OidEntry"/> class.
        /// </summary>
        public OidEntry()
        {
            mTimerClock.Elapsed += ClearTheList;
            mTimerClock.Interval = 15000; // 15 sec
            mTimerClock.Enabled = true;
            IsValid = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// The clear the list.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void ClearTheList(object sender, EventArgs args)
        {
            IsValid = false;
            mTimerClock.Enabled = false;
        }
    }
}
