using UnityEngine;
using IceStormSurvival.Core;

namespace IceStormSurvival.Systems
{
    public class DecisionMaker
    {
        private AIAgent agent;
        
        public DecisionMaker(AIAgent owner)
        {
            agent = owner;
        }
        
        public void MakeDecision()
        {
            Debug.Log($"[{agent.AgentName}] 做出决策");
        }
    }
}
