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
using SentinelAPICore;

namespace API_1.Controllers
{
    public class PerformanceCountersController : ApiController
    {
        SentinelAPICoreSingletone SentinelAPICore = SentinelAPICoreSingletone.Instance;

        [Route("api/CPU")]
        public IHttpActionResult GetCpuUsage()
        {
            Logger.InitLogger();
            ILog log = Logger.Log;
            SentinelAPICore.Deserialize();
            object result;
            try
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
                result = query.SingleOrDefault();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.NotFound, e.ToString());
            }
            return Ok(result + " " + SentinelAPICore.Modules.Last().PerformanceCounters.First().CounterName);
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
            float result;
            PerformanceCounter performanceCounter = new PerformanceCounter("LogicalDisk", "% Free Space", label + ":");
            try
            {
                result = performanceCounter.NextValue();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.NotFound, $"Disk {label} is not found on the system /n" + e.ToString());
            }
            return Ok(100 - result);
        }

        [Route("api/PerfCounters/{counterCategory}/{counterName}/{counterInstance}")]
        public IHttpActionResult GetPerformaneCounter([FromUri] string counterName, string counterCategory, string counterInstance)
        {
            PerformanceCounter performanceCounter = new PerformanceCounter(counterCategory, counterName, counterInstance);
            float result;
            try
            {
                result = performanceCounter.NextValue();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.NotFound,  e.ToString());
            }
            return Ok(result);
        }
    }

}
