using UnityEngine;
using IceStormSurvival.Core;
using System.Collections.Generic;

namespace IceStormSurvival.Systems
{
    /// <summary>
    /// 生存需求评估系统
    /// </summary>
    public class SurvivalNeeds
    {
        private AIAgent agent;
        private List<Need> currentNeeds;
        
        public SurvivalNeeds(AIAgent owner)
        {
            agent = owner;
            currentNeeds = new List<Need>();
        }
        
        public void EvaluateCurrentNeeds()
        {
            currentNeeds.Clear();
            
            // 评估健康需求
            if (agent.Health < 50)
            {
                var healthNeed = new Need
                {
                    needType = NeedType.Health,
                    currentValue = agent.Health,
                    criticalThreshold = 30f,
                    urgencyMultiplier = (100 - agent.Health) / 100f
                };
                currentNeeds.Add(healthNeed);
                
                Debug.Log($"[{agent.AgentName}] 健康需求: 当前值{agent.Health}, 紧急度{healthNeed.GetUrgency()}");
            }
            
            Debug.Log($"[{agent.AgentName}] 评估生存需求，发现 {currentNeeds.Count} 个需求");
        }
        
        public List<Need> GetCurrentNeeds()
        {
            return new List<Need>(currentNeeds);
        }
    }
}