using AgentsCenter.Information;
using AgentsCenter.Information.AgentsMonitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SentinelAPICore
{
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public class AgentsCenterAccessor : System.ServiceModel.ClientBase<IAgentsMonitoringStatusService>, IAgentsCenterAccessor
    {

        public AgentsCenterAccessor(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        #region IAgentsMonitoringStatusService Members

        public AgentsCenter.Information.AgentsStatistics GetAgentsStatistics()
        {
            return base.Channel.GetAgentsStatistics();
        }

        public List<AgentsCenter.Information.AgentCurrentStatus> GetAgentsStatus(string identifier)
        {
            return base.Channel.GetAgentsStatus(identifier);
        }

        public List<AgentsCenter.Information.AgentCurrentStatus> GetNextAgentsStatus(string identifier)
        {
            return base.Channel.GetNextAgentsStatus(identifier);
        }

        #endregion

        public static string ToString(AgentState agentState)
        {
            switch (agentState)
            {
                case AgentState.AGENT_STATE_DOWN:
                    return "Down";
                case AgentState.AGENT_STATE_ERROR:
                    return "Error";
                case AgentState.AGENT_STATE_IDLE:
                    return "Online";
                default:
                    return agentState.ToString();
            }
        }

        public static string ToString(AgentType agentType)
        {
            switch (agentType)
            {
                case AgentType.AGENT_TYPE_DESKTOP_ANALYTICS:
                    return "Desktop Analytics";
                case AgentType.AGENT_TYPE_SCREEN_AGENT:
                    return "ScreenAgent";
                case AgentType.AGENT_TYPE_VRA:
                    return "VRA";
                case AgentType.AGENT_TYPE_PO:
                    return "PO Client";
                default:
                    return agentType.ToString();
            }
        }
    }
}
