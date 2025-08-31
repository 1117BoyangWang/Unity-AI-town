using UnityEngine;
using IceStormSurvival.Core;
using IceStormSurvival.Services;
using System.Threading.Tasks;

namespace IceStormSurvival.Systems
{
    /// <summary>
    /// 社交行为系统
    /// </summary>
    public class SocialBehavior
    {
        private AIAgent agent;
        private ILLMService llmService;
        
        public SocialBehavior(AIAgent owner)
        {
            agent = owner;
            llmService = LLMServiceFactory.CreateService();
        }
        
        public async Task InteractWith(AIAgent otherAgent)
        {
            if (otherAgent == null) return;
            
            try
            {
                string interactionPrompt = $"作为{agent.AgentName}与{otherAgent.AgentName}进行对话";
                string response = await llmService.GenerateResponse(interactionPrompt);
                
                Debug.Log($"[{agent.AgentName}] 与 {otherAgent.AgentName} 交流: {response}");
                
                // 更新关系
                float currentRelationship = agent.GetRelationshipWith(otherAgent);
                agent.AddRelationship(otherAgent, currentRelationship + 0.1f);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"社交互动失败: {e.Message}");
            }
        }
    }
}