using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Module
{
    [Serializable]
    public class Module
    {
        public List<Service> Services;
        public List<Metric> Metrics;
        public List<PerfCounter> PerformanceCounters;

        [XmlAttribute]
        public string Version;

        [XmlAttribute]
        public string ModuleName;

        public Module()
        {

        }
        public Module(string name)
        {
            ModuleName = name;
        }

        public Module(string name, string version, List<Service> services, List<Metric> metrics, List<PerfCounter> perfCounters)
        {
            ModuleName = name;
            Version = version;
            Services = services;
            Metrics = metrics;
            PerformanceCounters = perfCounters;
        }

        public ServiceStatus? GetServiceStatus(string serviceName)
        {
            if (Services != null)
            {
                return Services.FirstOrDefault(s => s.ServiceName == serviceName).Status;
            }
            return null;
        }
    }
}
