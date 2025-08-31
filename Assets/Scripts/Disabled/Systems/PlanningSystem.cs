using UnityEngine;
using IceStormSurvival.Core;
using IceStormSurvival.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IceStormSurvival.Systems
{
    /// <summary>
    /// 行动规划系统
    /// </summary>
    public class PlanningSystem
    {
        private AIAgent agent;
        private ILLMService llmService;
        
        public PlanningSystem(AIAgent owner)
        {
            agent = owner;
            llmService = LLMServiceFactory.CreateService();
        }
        
        public async Task<List<SurvivalAction>> GenerateNewPlan()
        {
            var plan = new List<SurvivalAction>();
            
            // 简化的计划生成逻辑
            string planPrompt = $"作为{agent.AgentName}，在冰雪风暴环境中制定生存计划";
            
            try
            {
                string planText = await llmService.GeneratePlan(planPrompt);
                Debug.Log($"[{agent.AgentName}] 生成新计划: {planText}");
                
                // 简化：直接添加一些基础行动
                plan.Add(new SurvivalAction
                {
                    actionType = ActionType.FindFood,
                    description = "寻找食物",
                    importance = 7f,
                    duration = 30f
                });
                
                plan.Add(new SurvivalAction
                {
                    actionType = ActionType.Rest,
                    description = "休息恢复体力",
                    importance = 5f,
                    duration = 20f
                });
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"计划生成失败，使用默认计划: {e.Message}");
                
                // 默认计划
                plan.Add(new SurvivalAction
                {
                    actionType = ActionType.Rest,
                    description = "休息",
                    importance = 3f,
                    duration = 10f
                });
            }
            
            return plan;
        }
    }
}