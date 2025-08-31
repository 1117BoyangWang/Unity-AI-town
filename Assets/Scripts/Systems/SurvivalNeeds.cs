using UnityEngine;
using IceStormSurvival.Core;

namespace IceStormSurvival.Systems
{
    public class SurvivalNeeds
    {
        private AIAgent agent;
        
        public SurvivalNeeds(AIAgent owner)
        {
            agent = owner;
        }
        
        public void EvaluateCurrentNeeds()
        {
            // 简化的需求评估逻辑
            Debug.Log($"[{agent.AgentName}] 评估生存需求");
        }
    }
}
