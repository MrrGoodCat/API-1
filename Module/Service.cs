using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
