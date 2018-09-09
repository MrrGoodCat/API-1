using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Module;
using System.Reflection;

namespace SentinelAPICore
{
    public class XmlSerializator
    {
        List<Module.Module> Modules;
        List<Service> services;
        List<Metric> metrics;
        List<PerfCounter> perfCounters;
        

        public XmlSerializator()
        {
            Modules = new List<Module.Module>();
            services = new List<Service>();
            metrics = new List<Metric>();
            perfCounters = new List<PerfCounter>();
        }

        /// <summary>
        /// This method used only to create dummy xml.
        /// </summary>
        public void Serialize()
        {
            services.Add(new Service(@"SQL Server \(.*\)"));
            metrics.Add(new Metric("Page data"));
            perfCounters.Add(new PerfCounter("Avg. Disk sec/Transfer", "PhysicalDisk", "_Total", CounterType.Average));
            Modules.Add(new Module.Module("PhysicalDb", "1.1.2", services, metrics, perfCounters));
            XmlSerializer formatter = new XmlSerializer(typeof(List<Module.Module>));

            using (FileStream fs = new FileStream(@"C:\Logs\Module.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, Modules);
            }


        }

        /// <summary>
        /// This method should be used for serialization of newly added module from UI.
        /// </summary>
        /// <param name="modules"></param>
        public void Serialize(List<Module.Module> modules)
        {
            //services.Add(new Service(@"SQL Server \(.*\)"));
            //metrics.Add(new Metric("Page data"));
            //perfCounters.Add(new PerfCounter("Avg. Disk sec/Transfer", "PhysicalDisk", "_Total", CounterType.Average));
            //modules.Add(new Module.Module("PhysicalDb", "1.1.2", services, metrics, perfCounters));
            XmlSerializer formatter = new XmlSerializer(typeof(List<Module.Module>));

            using (FileStream fs = new FileStream(@"C:\Logs\Module.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, modules);
            }


        }

        public void Deserialize(List<Module.Module> modules)
        {
            string configFile = @"D:\inetpub\SentinelAPIAgent";

            XmlSerializer formatter = new XmlSerializer(typeof(List<Module.Module>));

            using (FileStream fs = new FileStream(configFile += @"\ModulesConfig.xml", FileMode.OpenOrCreate))
            {
                List<Module.Module> modulesCollection = (List<Module.Module>)formatter.Deserialize(fs);

                foreach (Module.Module module in modulesCollection)
                {
                    modules.Add(module);
                }
            }
        }
    }
}
