using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PerformanceCountersCollector;
using log4net;
using System.Threading.Tasks;
using System.Management;
using Module;

namespace API_1.Controllers
{
    public class PerformanceCountersController : ApiController
    {
        PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        IPerformanceCounterCategoryWrapper performanceCounterCategoryWrapper = new PerformanceCounterCategoryWrapper();

        [Route("api/CPU")]
        public IHttpActionResult GetCpuUsage()
        {
            Logger.InitLogger();
            ILog log = Logger.Log;
            XmlSerializator xml = new XmlSerializator();
            xml.Serialize();
            var result = performanceCounterCategoryWrapper.GetCpuUsage();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Route ("api/RAM")]
        public IHttpActionResult GetRamUsage()
        {
            PerformanceCounter performanceCounterRAM = new PerformanceCounter();

            performanceCounterRAM.CounterName = "% Committed Bytes In Use";
            performanceCounterRAM.CategoryName = "Memory";

            return Ok(performanceCounterRAM.NextValue());
        }

        [Route("api/LogicalDisk/{label}")]
        public IHttpActionResult GetDisksUtilization(string label)
        {
            PerformanceCounter performanceCounter = new PerformanceCounter("LogicalDisk", "% Free Space", label + ":");
            return Ok(100 - performanceCounter.NextValue());
        }
    }

}
