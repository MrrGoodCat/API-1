using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Module
{
    public class XmlSerializator
    {
        List<Module> Modules;
        List<Service> services;
        List<Metric> metrics;
        List<PerfCounter> perfCounters;

        public XmlSerializator()
        {
            Modules = new List<Module>();
            services = new List<Service>();
            metrics = new List<Metric>();
            perfCounters = new List<PerfCounter>();
        }

        public void Serialize()
        {
            services.Add(new Service(@"SQL Server \(.*\)"));
            metrics.Add(new Metric("Page data"));
            perfCounters.Add(new PerfCounter("Avg. Disk sec/Transfer", "PhysicalDisk", "_Total", CounterType.Average));
            Modules.Add(new Module("PhysicalDb", "1.1.2", services, metrics, perfCounters));
            XmlSerializer formatter = new XmlSerializer(typeof(List<Module>));

            using (FileStream fs = new FileStream(@"C:\Logs\Module.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, Modules);
            }

            //using (FileStream fs = new FileStream("people.xml", FileMode.OpenOrCreate))
            //{
            //    Module newModule = (Module[])formatter.Deserialize(fs);

            //    foreach (Person p in newpeople)
            //    {
            //        Console.WriteLine("Имя: {0} --- Возраст: {1}", p.Name, p.Age);
            //    }
            //}
        }
    }
}
