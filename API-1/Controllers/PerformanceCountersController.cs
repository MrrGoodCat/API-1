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
            performanceCounterRAM.InstanceName = null;

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

        [Route("api/PerfCounters/{counterInformation}")]
        public IHttpActionResult GetPerformaneCounter([FromUri] string counterInformation)
        {
            PerformanceCounter performanceCounter = new PerformanceCounter();
            string[] tempCounterInfo = SentinelAPICore.BaseSixFourDecode(counterInformation);
            if (tempCounterInfo != null)
            {
                switch (tempCounterInfo.Length)
                {
                    case 3:
                        performanceCounter.CategoryName = tempCounterInfo[0];
                        performanceCounter.CounterName = tempCounterInfo[1];
                        performanceCounter.InstanceName = tempCounterInfo[2];
                        break;
                    case 2:
                        performanceCounter.CategoryName = tempCounterInfo[0];
                        performanceCounter.CounterName = tempCounterInfo[1];
                        performanceCounter.InstanceName = null;
                        break;
                    case 1:
                        performanceCounter.CategoryName = tempCounterInfo[0];
                        performanceCounter.CounterName = null;
                        performanceCounter.InstanceName = null;
                        break;
                    default:
                        performanceCounter = null;
                        break;
                }
            }
            float result;
            try
            {
                result = performanceCounter.NextValue();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.NotFound,  $"Category '{performanceCounter.CategoryName}' - Counter '{performanceCounter.CounterName}' - Instance '{performanceCounter.InstanceName}' " + e.ToString());
            }
            return Ok(result);
        }
    }

}
