using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SentinelSNMPAgentStuff
{
    /// <summary>
    /// The interface.
    /// </summary>
    [ServiceContract]
    public interface IPingable : IDisposable
    {
        /// <summary>
        /// The ping.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        [OperationContract]
        string Ping();
    }

    /// <summary>
    /// The SentinelMDSNMPAgent interface.
    /// </summary>
    [ServiceContract]
    public interface ISentinelMDSNMPAgent : IDisposable
    {
        /// <summary>
        /// The send trap.
        /// </summary>
        /// <param name="trap">
        /// The trap.
        /// </param>
        /// <param name="trapToClear">
        /// The trap to clear.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(TrapConfigurationFailure))]
        Guid SendTrap(SentinelTrap trap, Guid trapToClear);

        /// <summary>
        /// The connect.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        /// <param name="processId">
        /// The process id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(ModuleConfigurationFaiure))]
        bool Connect(string module, int processId);

        /// <summary>
        /// The set module status.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="trapToClear">
        /// The trap to clear.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [OperationContract]
        bool SetModuleStatus(ModuleStatus status, Guid trapToClear);

        /// <summary>
        /// The get module status.
        /// </summary>
        /// <returns>
        /// The <see cref="ModuleStatus"/>.
        /// </returns>
        [OperationContract]
        ModuleStatus GetModuleStatus();

        /// <summary>
        /// The disconnect.
        /// </summary>
        /// <param name="processId">
        /// The process Id.
        /// </param>
        [OperationContract]
        void DisconnectExt(int processId);

        /// <summary>
        /// The disconnect.
        /// </summary>
        [OperationContract]
        void Disconnect();

        /// <summary>
        /// The check availability.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [OperationContract]
        bool CheckAvailability();
    }

    /// <summary>
    /// The sentinel trap.
    /// </summary>
    [DataContract]
    public class SentinelTrap
    {
        [DataMember]
        public int TrapID { get; private set; }

        [DataMember]
        public string ModuleName { get; private set; }

        [DataMember]
        public Hashtable Parameters { get; private set; }

        /// <summary>
        /// Gets the ordered parameters.
        /// </summary>
        [DataMember]
        public Dictionary<string, string> OrderedParameters { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SentinelTrap"/> class.
        /// </summary>
        /// <param name="trapID">
        /// The trap id.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public SentinelTrap(int trapID, Hashtable parameters)
        {
            this.TrapID = trapID;
            this.Parameters = new Hashtable();
            this.OrderedParameters = new Dictionary<string, string>();
            this.Parameters = parameters;
            foreach (DictionaryEntry entry in parameters)
            {
                this.OrderedParameters.Add(entry.Key.ToString(), entry.Value.ToString());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SentinelTrap"/> class.
        /// </summary>
        /// <param name="trapID">
        /// The trap id.
        /// </param>
        public SentinelTrap(int trapID)
        {
            this.TrapID = trapID;
            this.Parameters = new Hashtable();
            this.OrderedParameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SentinelTrap"/> class.
        /// </summary>
        public SentinelTrap()
        {
            this.Parameters = new Hashtable();
            this.OrderedParameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// The add parameter.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AddParameter(string key, object value)
        {
            if (!this.Parameters.ContainsKey(key))
            {
                if (value != null)
                {
                    this.Parameters.Add(key, value);
                    this.OrderedParameters.Add(key, value.ToString());
                }
            }
            else
            {
                if (value != null)
                {
                    this.Parameters[key] = value;
                    this.OrderedParameters[key] = value.ToString();
                }
            }
        }

        /// <summary>
        /// The add parameter.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void AddParameter(string key, string value)
        {
            if (!this.Parameters.ContainsKey(key))
            {
                this.Parameters.Add(key, value);
                this.OrderedParameters.Add(key, value);
            }
            else
            {
                this.Parameters[key] = value;
                this.OrderedParameters[key] = value;
            }
        }

        /// <summary>
        /// The set trap id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        public void SetTrapId(int id)
        {
            this.TrapID = id;
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            string parametersString = string.Empty;
            if (this.OrderedParameters != null)
            {
                foreach (KeyValuePair<string, string> entry in this.OrderedParameters)
                {
                    parametersString += "[" + entry.Key + "] = " + entry.Value + "; ";
                }
            }
            else
            {
                foreach (DictionaryEntry entry in this.Parameters)
                {
                    parametersString += "[" + entry.Key + "] = " + entry.Value + "; ";
                }
            }

            return string.Format("Sentinel Trap ID = {0} with parameters {1}", this.TrapID, parametersString);
        }
    }
}
