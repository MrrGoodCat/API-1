using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Module;

namespace SentinelAPICore
{
    public class SentinelAPICoreSingletone
    {
        public List<Module.Module> Modules;
        private XmlSerializator xml = new XmlSerializator();

        private SentinelAPICoreSingletone()
        {
            Modules = new List<Module.Module>();
        }

        public static SentinelAPICoreSingletone Instance
        {
            get { return Nested.InstanceNested; }
        }

        public void Deserialize()
        {
            xml.Deserialize(Modules);
        }

        public string GetServiceStatus(string serviceName)
        {
            if (DoesServiceExist(serviceName))
            {
                ServiceController sc = new ServiceController(serviceName);
                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        return "Running";
                    case ServiceControllerStatus.Stopped:
                        return "Stopped";
                    case ServiceControllerStatus.Paused:
                        return "Paused";
                    case ServiceControllerStatus.StopPending:
                        return "Stopping";
                    case ServiceControllerStatus.StartPending:
                        return "Starting";
                    default:
                        return "Status Changing";
                }
            }
            else
            {
                return $"Servise {serviceName} is not exist on the system";
            }
            
        }

        private bool DoesServiceExist(string serviceName)
        {
            bool isExist = ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals(serviceName));

            if (isExist)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private class Nested
        {
            /// <summary>
            /// The instance.
            /// </summary>
            internal static readonly SentinelAPICoreSingletone InstanceNested = new SentinelAPICoreSingletone();

            /// <summary>
            /// Prevents a default instance of the <see cref="Nested"/> class from being created.
            /// </summary>
            private Nested()
            {
            }
        }
    }
}
