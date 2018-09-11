using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SentinelAPICore;

namespace API_1.Controllers
{
    public class ServicesController : ApiController
    {
        SentinelAPICoreSingletone SentinelAPICore = SentinelAPICoreSingletone.Instance;

        [Route ("api/services/{moduleName}/{serviceName}")]
        public IHttpActionResult GetServiceStatus(string moduleName, string serviceName)
        {
            
            return Ok(SentinelAPICore.GetServiceStatus(moduleName, serviceName).ToString());
        }
    }
}
