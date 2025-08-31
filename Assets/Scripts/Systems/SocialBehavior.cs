using System.Threading.Tasks;
using UnityEngine;
using IceStormSurvival.Core;

namespace IceStormSurvival.Systems
{
    public class SocialBehavior
    {
        private AIAgent agent;
        
        public SocialBehavior(AIAgent owner)
        {
            agent = owner;
        }
        
        public async Task InteractWith(AIAgent other)
        {
            await Task.Delay(50);
            Debug.Log($"[{agent.AgentName}] 与 {other.AgentName} 进行社交互动");
        }
    }
}
