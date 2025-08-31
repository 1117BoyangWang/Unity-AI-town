using System.Threading.Tasks;
using UnityEngine;

namespace IceStormSurvival.Services
{
    /// <summary>
    /// 模拟LLM服务
    /// 用于没有配置真实API时的测试
    /// </summary>
    public class MockLLMService : ILLMService
    {
        private readonly string[] reflectionTemplates = {
            "在这个冰雪世界中，我意识到{0}对我来说很重要。",
            "经历了{0}，我觉得应该更加谨慎。",
            "通过{0}的经历，我学到了团结的重要性。",
            "面对{0}，我感到自己变得更加坚强。"
        };
        
        private readonly string[] responseTemplates = {
            "我觉得{0}是个不错的想法。",
            "在这种情况下，{0}可能是最好的选择。",
            "考虑到我们的处境，{0}值得尝试。",
            "我同意{0}，但需要小心执行。"
        };
        
        private readonly string[] planTemplates = {
            "首先我应该{0}，然后{1}，最后{2}。",
            "为了{0}，我计划{1}，接着{2}。",
            "考虑到当前情况，我会{0}，同时{1}。"
        };

        public async Task<string> GenerateReflection(string prompt)
        {
            // 模拟API延迟
            await Task.Delay(Random.Range(500, 1500));
            
            var template = reflectionTemplates[Random.Range(0, reflectionTemplates.Length)];
            return string.Format(template, ExtractKeyword(prompt));
        }

        public async Task<string> GenerateResponse(string prompt)
        {
            await Task.Delay(Random.Range(300, 1000));
            
            var template = responseTemplates[Random.Range(0, responseTemplates.Length)];
            return string.Format(template, ExtractKeyword(prompt));
        }

        public async Task<string> GeneratePlan(string prompt)
        {
            await Task.Delay(Random.Range(800, 2000));
            
            var actions = new string[] {
                "寻找食物", "生火取暖", "强化避难所", "照顾伤者", 
                "收集资源", "巡逻周围", "制作工具", "休息恢复"
            };
            
            var action1 = actions[Random.Range(0, actions.Length)];
            var action2 = actions[Random.Range(0, actions.Length)];
            var action3 = actions[Random.Range(0, actions.Length)];
            
            var template = planTemplates[Random.Range(0, planTemplates.Length)];
            return string.Format(template, action1, action2, action3);
        }
        
        private string ExtractKeyword(string prompt)
        {
            // 简单的关键词提取
            if (prompt.Contains("食物")) return "食物获取";
            if (prompt.Contains("温暖")) return "保暖";
            if (prompt.Contains("安全")) return "安全防护";
            if (prompt.Contains("社交")) return "人际关系";
            if (prompt.Contains("生存")) return "生存策略";
            return "当前情况";
        }
    }
}
