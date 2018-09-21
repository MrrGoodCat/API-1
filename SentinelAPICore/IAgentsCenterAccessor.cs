using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using AgentsCenter.Information.AgentsMonitoring;

namespace SentinelAPICore
{
    public interface IAgentsCenterAccessor : IAgentsMonitoringStatusService, IDisposable
    {
        new AgentsCenter.Information.AgentsStatistics GetAgentsStatistics();
        new List<AgentsCenter.Information.AgentCurrentStatus> GetAgentsStatus(string identifier);
        new List<AgentsCenter.Information.AgentCurrentStatus> GetNextAgentsStatus(string identifier);
        void Open();
        void Abort();
        void Close();
        void DisplayInitializationUI();
        ChannelFactory<IAgentsMonitoringStatusService> ChannelFactory { get; }
        ClientCredentials ClientCredentials { get; }
        CommunicationState State { get; }
        IClientChannel InnerChannel { get; }
        ServiceEndpoint Endpoint { get; }
    }
}
