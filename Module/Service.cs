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
    public class Service
    {
        [XmlAttribute]
        public string ServiceName;

        [XmlAttribute]
        [NonSerialized]
        public ServiceStatus Status;

        public Service()
        {

        }

        public Service(string name)
        {
            ServiceName = name;
        }

        public ServiceStatus GetServiceStatus()
        {
            ServiceController sc = new ServiceController(ServiceName);

            switch (sc.Status)
            {
                case ServiceControllerStatus.Running:
                    return ServiceStatus.Running;
                case ServiceControllerStatus.Stopped:
                    return ServiceStatus.Stopped;
                case ServiceControllerStatus.Paused:
                    return ServiceStatus.Paused;
                case ServiceControllerStatus.StopPending:
                    return ServiceStatus.Stopping;
                case ServiceControllerStatus.StartPending:
                    return ServiceStatus.Starting;
                default:
                    return ServiceStatus.Unknown;
            }
        }
    }
}
