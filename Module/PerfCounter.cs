using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Module
{
    [Serializable]
    public class PerfCounter
    {
        [XmlAttribute]
        public string CounterName;
        [XmlAttribute]
        public string CounterCategoryName;
        [XmlAttribute]
        public string CounterInstanceName;
        [XmlAttribute]
        public CounterType CounterType;

        public PerfCounter()
        {

        }

        public PerfCounter(string name, string category, string instance, CounterType type)
        {
            CounterName = name;
            CounterCategoryName = category;
            CounterInstanceName = instance;
            CounterType = type;
        }
    }
}
