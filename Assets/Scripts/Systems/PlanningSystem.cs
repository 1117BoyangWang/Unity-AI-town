using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using IceStormSurvival.Core;

namespace IceStormSurvival.Systems
{
    public class PlanningSystem
    {
        private AIAgent agent;
        
        public PlanningSystem(AIAgent owner)
        {
            agent = owner;
        }
        
        public async Task<List<SurvivalAction>> GenerateNewPlan()
        {
            await Task.Delay(100); // 模拟LLM思考时间
            
            var plan = new List<SurvivalAction>();
            
            // 简化的计划生成逻辑
            plan.Add(new SurvivalAction
            {
                actionType = ActionType.FindFood,
                description = "寻找食物",
                duration = 300f,
                importance = 7f,
                emotionalImpact = 1f
            });
            
            Debug.Log($"[{agent.AgentName}] 生成新计划");
            return plan;
        }
    }
}
