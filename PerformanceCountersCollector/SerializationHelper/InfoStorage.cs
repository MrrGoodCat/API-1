using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PerformanceCountersCollector.SerializationHelper
{
    /// <summary>
    /// The info storage.
    /// </summary>
    public class InfoStorage
    {
        /// <summary>
        /// Gets or sets the modules info.
        /// </summary>
        public List<ModuleInfo> ModulesInfo { get; set; }

        /// <summary>
        /// Generates instance from the XML
        /// </summary>
        /// <typeparam name="T">
        /// Type of object
        /// </typeparam>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T FromXml<T>(T obj, string xml)
        {
            var ser = new XmlSerializer(obj.GetType());
            var stringReader = new StringReader(xml);
            var xmlReader = new XmlTextReader(stringReader);
            var newMibInfoStorage = (T)ser.Deserialize(xmlReader);
            xmlReader.Close();
            stringReader.Close();
            return newMibInfoStorage;
        }

        /// <summary>
        /// Generates instance from the XML file
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <param name="fileName">
        /// The file Name.
        /// </param>
        /// <typeparam name="T">
        /// Type of object
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T FromFile<T>(T obj, string fileName)
        {
            using (var file = new StreamReader(fileName))
            {
                return FromXml(obj, file.ReadToEnd());
            }
        }
    }
}
