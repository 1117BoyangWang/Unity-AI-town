using UnityEngine;
using IceStormSurvival.Core;

namespace IceStormSurvival.Systems
{
    /// <summary>
    /// 决策制定系统
    /// </summary>
    public class DecisionMaker
    {
        private AIAgent agent;
        
        public DecisionMaker(AIAgent owner)
        {
            agent = owner;
        }
        
        public SurvivalAction MakeDecision()
        {
            // 简化的决策逻辑
            Debug.Log($"[{agent.AgentName}] 正在做决策");
            
            // 基于当前状态做决策
            if (agent.Health < 30)
            {
                return new SurvivalAction
                {
                    actionType = ActionType.Rest,
                    description = "因为健康状况不佳，选择休息",
                    importance = 8f,
                    duration = 30f
                };
            }
            
            // 默认决策
            return new SurvivalAction
            {
                actionType = ActionType.FindFood,
                description = "寻找食物",
                importance = 5f,
                duration = 20f
            };
        }
    }
}