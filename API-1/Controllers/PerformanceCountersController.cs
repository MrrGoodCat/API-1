using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API_1.Controllers
{
    public class PerformanceCountersController : ApiController
    {
        public IHttpActionResult Get([FromBody]string counterName)
        {
            string counterValue = "3.14";
            var result = 0;

            var performanceCounterCategories = PerformanceCounterCategory.GetCategories().FirstOrDefault(category => category.CategoryName == "Processor Information");
            //var performanceCounters = performanceCounterCategories.GetCounters("_Total");

            //result = performanceCounters.GetValue().ToString();

            if (performanceCounterCategories.CounterExists("% Processor Time"))
            {
                var performanceCounter = new PerformanceCounter(performanceCounterCategories.CategoryName, "% Processor Time", "_Total");
                using (performanceCounter)
                {
                    result = (int)performanceCounter.Increment();
                }
            }

            return Ok(result);
        }
    }
}
