using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Module
{
    [Serializable]
    public class Metric
    {
        [XmlAttribute]
        public string MetricName;

        [NonSerialized]
        public object MetricValue;
        [XmlAttribute]
        public MetricType MetricType;

        public Metric()
        {

        }

        public Metric(string name)
        {
            MetricName = name;
            MetricType = MetricType.Default;
        }
    }
}
