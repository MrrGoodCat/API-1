using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using AgentsCenter.Information;

namespace SentinelAPICore
{
    public class MetricsCollector
    {
        /// <summary>
        /// The online screen agents.
        /// </summary>
        private const string ONLINE_SCREEN_AGENTS = "Online ScreenAgents";

        /// <summary>
        /// The erroneous screen agents.
        /// </summary>
        private const string ERRONEOUS_SCREEN_AGENTS = "Erroneous ScreenAgents";

        /// <summary>
        /// The online PO clients.
        /// </summary>
        private const string ONLINE_PO_CLIENTS = "Online PO Clients";

        /// <summary>
        /// The erroneous PO clients.
        /// </summary>
        private const string ERRONEOUS_PO_CLIENTS = "Erroneous PO Clients";

        string ip = "10.128.32.241";
        List<AgentCurrentStatus> Agents = new List<AgentCurrentStatus>();

        void GetData()
        {
            try
            {
                NiceComponent component = NodeServer.Components.Find(item => (item.Type == ComponentType.AgentsCenter));
                if (component == null)
                {
                    string erMsg = string.Format(SentinelConstants.ISP_MISSING_COMPONENT_TEMPLATE, ComponentType.AgentsCenter, NodeServer.HostName);
                    LogTrace.WriteErrorFormatted(erMsg);
                    this.AddAllMetricsToFailed(mMetricList, erMsg, FailedMetricReason.ISP_MissingComponent);

                    //Lets make poller yellow
                    throw new Exception(string.Format("AgentsCenter is not configured for {0}", NodeServer.IPAddr));
                }

                if (!component.IsActiveInCluster)
                {
                    LogTrace.WriteTrace("Not collecting metrics since not active in cluster.");
                    return true;
                }
                string endpoint = "http://" + ip + ":62124/AgentsCenter.Information.AgentsMonitoring.IAgentsMonitoringStatusService";

            Binding wsBinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            EndpointAddress endpointAdd = new EndpointAddress(endpoint);

            using (IAgentsCenterAccessor client = AgentsCenterHelper.GetAgentCenterAccessor(wsBinding, endpointAdd))
            {
                client.Open();

                //get sentinel identifier
                string sentinelIdentifier = Dns.GetHostName();

                agents.Clear();

                //get a list that containes the info of each agent
                List<AgentCurrentStatus> tempAgentslist = client.GetAgentsStatus(sentinelIdentifier);

                while (tempAgentslist != null && tempAgentslist.Count != 0)
                {
                    LogTrace.WriteTraceFormatted("GetNextAgentsStatus returned {0} more agents", tempAgentslist.Count);
                    agents.AddRange(tempAgentslist);
                    tempAgentslist = client.GetNextAgentsStatus(sentinelIdentifier);
                }

                LogTrace.WriteTraceFormatted("Total amount of agents is {0}", agents.Count);

                //loop though each metric and publish it (or not)
                foreach (NICEMetric metric in mMetricList)
                {
                    double stateCount = 0;              //number of agents in certain state
                    double totalAgentsCount = 0;        //number of agents of certain type

                    switch (metric.Name)
                    {
                        case ONLINE_SCREEN_AGENTS:
                            GetAgentsCount(ref totalAgentsCount, ref stateCount, AgentType.AGENT_TYPE_SCREEN_AGENT, AgentState.AGENT_STATE_IDLE);
                            break;
                        case ERRONEOUS_SCREEN_AGENTS:
                            GetAgentsCount(ref totalAgentsCount, ref stateCount, AgentType.AGENT_TYPE_SCREEN_AGENT, AgentState.AGENT_STATE_ERROR);
                            break;
                        case ONLINE_PO_CLIENTS:
                            GetAgentsCount(ref totalAgentsCount, ref stateCount, AgentType.AGENT_TYPE_PO, AgentState.AGENT_STATE_IDLE);
                            break;
                        case ERRONEOUS_PO_CLIENTS:
                            GetAgentsCount(ref totalAgentsCount, ref stateCount, AgentType.AGENT_TYPE_PO, AgentState.AGENT_STATE_ERROR);
                            break;
                        default:
                            LogTrace.WriteErrorFormatted("Unknown metric has been found in the metric list: \"{0}\"", metric.Name);
                            continue;
                    }

                    if (totalAgentsCount > 0)  //if there are any agents of specified type, publish the metric
                    {
                        double metricValue = stateCount / totalAgentsCount * 100;       //100 = 100%
                        metricValue = Math.Round(metricValue, 2);
                        LogTrace.WriteTraceFormatted("Metric \"{0}\" is being published with value {1}%.", metric.Name, metricValue);
                        publishedMetricsList.Add(new PublishedMetricValue(metric, metricValue, NodeServer, component));
                    }
                    else
                    {
                        LogTrace.WriteTraceFormatted("Not publishing metrics since there are no agents for \"{0}\".", metric.Name);
                    }
                }

                client.Close();
            }

            success = true;
        }
            catch (Exception e)
            {
                LogTrace.WriteErrorFormatted("An exception occured: {0}", e);
                res = NpsUtility.GetTheExceptionReason(e);
                success = false;
            }

        }

/// <summary>
/// The get agents count.
/// return list of agents of specified type and their count in specified state
/// </summary>
/// <param name="totalAgentsCount">
/// The total agents count.
/// </param>
/// <param name="stateCount">
/// The state count.
/// </param>
/// <param name="agentType">
/// The agent type.
/// </param>
/// <param name="agentState">
/// The agent state.
/// </param>
private void GetAgentsCount(ref double totalAgentsCount, ref double stateCount, AgentType agentType, AgentState agentState)
{
    LogTrace.WriteTraceFormatted("Attempt to get information about {0} agents int {1} state for {2}", agentType, agentState, NodeServer.IPAddr);
    List<AgentCurrentStatus> totalAgents = agents.FindAll(agent => agent.AgentId.AgentType == agentType);
    totalAgentsCount = totalAgents.Count;
    stateCount = totalAgents.FindAll(agent => agent.AgentState == agentState).Count;
    LogTrace.WriteTraceFormatted(
        "{0} agents found of type {1} on server {2}. {3} of them are in state {4}",
        totalAgents.Count,
        agentType,
        NodeServer.IPAddr,
        stateCount,
        agentState);
}
    }
}
