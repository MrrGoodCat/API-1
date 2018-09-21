using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace SentinelAPICore
{
    /// <summary>
    /// Used to get AgentsCenterAccessor in normal mode and AgentsCenter simulator in testing mode
    /// </summary>
    public class AgentsCenterHelper
    {
        public static IAgentsCenterAccessor GetAgentCenterAccessor(Binding wsBinding, EndpointAddress endpointAdd)
        {
            return new AgentsCenterAccessor(wsBinding, endpointAdd);
        }

    }
}
