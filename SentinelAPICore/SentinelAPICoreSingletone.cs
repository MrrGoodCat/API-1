using System;
using System.Collections.Generic;
using System.Linq;
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

        public ServiceStatus GetServiceStatus(string moduleName, string serviceName)
        {
            var module = Modules.FirstOrDefault(m => m.ModuleName == moduleName);
            var setvice = module.Services.FirstOrDefault(s => s.ServiceName == serviceName);

            return setvice.Status;
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
