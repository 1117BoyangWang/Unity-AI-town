using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using IceStormSurvival.Systems;
using IceStormSurvival.Core;

namespace IceStormSurvival.Services
{
    /// <summary>
    /// 增强版OpenAI服务 - 支持角色定制和上下文记忆
    /// </summary>
    public class EnhancedOpenAIService : ILLMService
    {
        private const string API_URL = "https://api.openai.com/v1/chat/completions";
        private const int MAX_RETRIES = 3;
        private const float RETRY_DELAY = 2f;
        
        private OpenAIConfig config;
        private Dictionary<string, List<Message>> conversationHistory;
        
        public EnhancedOpenAIService()
        {
            config = ConfigurationManager.Instance.GetOpenAIConfig();
            conversationHistory = new Dictionary<string, List<Message>>();
        }
        
        public async Task<string> GenerateReflection(string prompt)
        {
            string agentContext = ExtractAgentContext(prompt);
            string systemPrompt = BuildReflectionSystemPrompt(agentContext);
            
            return await CallOpenAIWithContext("reflection", systemPrompt, prompt);
        }
        
        public async Task<string> GenerateResponse(string prompt)
        {
            string agentContext = ExtractAgentContext(prompt);
            string systemPrompt = BuildResponseSystemPrompt(agentContext);
            
            return await CallOpenAIWithContext("response", systemPrompt, prompt);
        }
        
        public async Task<string> GeneratePlan(string prompt)
        {
            string agentContext = ExtractAgentContext(prompt);
            string systemPrompt = BuildPlanningSystemPrompt(agentContext);
            
            return await CallOpenAIWithContext("planning", systemPrompt, prompt);
        }
        
        private string BuildReflectionSystemPrompt(string agentContext)
        {
            return $@"你是一个在冰雪风暴末日环境中生存的AI代理。{agentContext}

作为这个角色，你需要基于最近的经历进行深度反思。你的反思应该：

1. **个人化**: 体现你独特的职业背景和性格特点
2. **情感化**: 表达真实的恐惧、希望、担忧或决心
3. **实用性**: 从经历中总结有助于生存的洞察
4. **人性化**: 展现在极端环境下的人性思考

格式要求：
- 用第一人称表达
- 不超过100字
- 语言自然流畅
- 体现末日生存的紧迫感

记住：你不是一个完美的理性机器，而是一个有血有肉、有情感的人。";
        }
        
        private string BuildResponseSystemPrompt(string agentContext)
        {
            return $@"你是一个在冰雪风暴末日环境中生存的AI代理。{agentContext}

在这个极端环境中，你需要：

1. **保持角色一致性**: 你的每个回应都应该符合你的职业、年龄和性格
2. **考虑生存现实**: 食物稀缺、寒冷、资源竞争等都会影响你的心态
3. **体现情感变化**: 根据当前的健康、饥饿、士气状态调整语气
4. **展现人际智慧**: 在合作与竞争之间找到平衡

回应风格：
- 简洁明了（20-50字）
- 符合中国人的表达习惯
- 体现当前的情绪状态
- 包含适当的行动倾向";
        }
        
        private string BuildPlanningSystemPrompt(string agentContext)
        {
            return $@"你是一个在冰雪风暴末日环境中的生存专家。{agentContext}

你需要制定实际可行的生存计划。考虑因素：

1. **当前状态**: 健康、饥饿、体温、能量、士气水平
2. **环境条件**: 天气状况、资源可获得性、安全威胁
3. **社交因素**: 与其他幸存者的关系、团队动态
4. **技能优势**: 充分发挥你的职业技能

计划特点：
- 具体可执行的行动步骤
- 考虑资源限制和风险
- 优先级明确（生存>舒适）
- 适应性强，能根据情况调整
- 体现你的专业知识和性格特点

格式：简明扼要的行动描述，重点突出下一步要做什么。";
        }
        
        private string ExtractAgentContext(string prompt)
        {
            // 从提示中提取代理信息，构建角色背景
            // 这里可以根据实际的prompt格式来解析
            return "你是一个经验丰富的幸存者，具有独特的技能和性格。";
        }
        
        private async Task<string> CallOpenAIWithContext(string conversationType, string systemPrompt, string userPrompt)
        {
            if (string.IsNullOrEmpty(config.apiKey))
            {
                Debug.LogWarning("OpenAI API密钥未配置");
                return GetContextualFallbackResponse(conversationType, userPrompt);
            }
            
            for (int attempt = 0; attempt < MAX_RETRIES; attempt++)
            {
                try
                {
                    var messages = BuildMessageHistory(conversationType, systemPrompt, userPrompt);
                    var requestData = CreateEnhancedRequest(messages);
                    var response = await SendRequestWithRetry(requestData);
                    
                    if (!string.IsNullOrEmpty(response))
                    {
                        // 更新对话历史
                        UpdateConversationHistory(conversationType, userPrompt, response);
                        return response;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"OpenAI API调用失败 (尝试 {attempt + 1}/{MAX_RETRIES}): {e.Message}");
                    
                    if (attempt < MAX_RETRIES - 1)
                    {
                        float delay = RETRY_DELAY * Mathf.Pow(2, attempt); // 指数退避
                        await Task.Delay(TimeSpan.FromSeconds(delay));
                    }
                }
            }
            
            Debug.LogError("OpenAI API多次调用失败，使用备用响应");
            return GetContextualFallbackResponse(conversationType, userPrompt);
        }
        
        private List<Message> BuildMessageHistory(string conversationType, string systemPrompt, string userPrompt)
        {
            var messages = new List<Message>
            {
                new Message { role = "system", content = systemPrompt }
            };
            
            // 添加相关的历史对话（限制数量以控制token使用）
            if (conversationHistory.ContainsKey(conversationType))
            {
                var history = conversationHistory[conversationType];
                int maxHistoryItems = 4; // 最多保留最近4轮对话
                int startIndex = Mathf.Max(0, history.Count - maxHistoryItems);
                
                for (int i = startIndex; i < history.Count; i++)
                {
                    messages.Add(history[i]);
                }
            }
            
            messages.Add(new Message { role = "user", content = userPrompt });
            return messages;
        }
        
        private EnhancedOpenAIRequest CreateEnhancedRequest(List<Message> messages)
        {
            return new EnhancedOpenAIRequest
            {
                model = config.model,
                messages = messages,
                temperature = config.temperature,
                max_tokens = config.maxTokens,
                top_p = 0.9f,
                frequency_penalty = 0.1f,
                presence_penalty = 0.1f,
                stop = new[] { "\n\n", "---" } // 停止序列
            };
        }
        
        private async Task<string> SendRequestWithRetry(EnhancedOpenAIRequest requestData)
        {
            string jsonData = JsonUtility.ToJson(requestData);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            
            using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.timeout = (int)config.requestTimeout;
                
                // 设置请求头
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {config.apiKey}");
                request.SetRequestHeader("User-Agent", "IceStormSurvivalTown/1.0");
                
                var operation = request.SendWebRequest();
                
                while (!operation.isDone)
                {
                    await Task.Yield();
                }
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseText = request.downloadHandler.text;
                    var response = JsonUtility.FromJson<OpenAIResponse>(responseText);
                    
                    if (response.choices != null && response.choices.Length > 0)
                    {
                        string content = response.choices[0].message.content.Trim();
                        
                        // 记录token使用情况
                        if (response.usage != null)
                        {
                            Debug.Log($"Token使用: {response.usage.total_tokens} " +
                                    $"(输入: {response.usage.prompt_tokens}, 输出: {response.usage.completion_tokens})");
                        }
                        
                        return content;
                    }
                }
                else
                {
                    string errorMsg = $"HTTP {request.responseCode}: {request.error}";
                    if (!string.IsNullOrEmpty(request.downloadHandler.text))
                    {
                        errorMsg += $"\n响应内容: {request.downloadHandler.text}";
                    }
                    
                    Debug.LogError($"OpenAI API错误: {errorMsg}");
                    throw new Exception(errorMsg);
                }
            }
            
            return null;
        }
        
        private void UpdateConversationHistory(string conversationType, string userPrompt, string response)
        {
            if (!conversationHistory.ContainsKey(conversationType))
            {
                conversationHistory[conversationType] = new List<Message>();
            }
            
            var history = conversationHistory[conversationType];
            
            // 添加用户消息和AI响应
            history.Add(new Message { role = "user", content = userPrompt });
            history.Add(new Message { role = "assistant", content = response });
            
            // 限制历史记录长度（避免内存和token过多使用）
            int maxHistoryLength = 20;
            if (history.Count > maxHistoryLength)
            {
                history.RemoveRange(0, history.Count - maxHistoryLength);
            }
        }
        
        private string GetContextualFallbackResponse(string conversationType, string prompt)
        {
            switch (conversationType)
            {
                case "reflection":
                    return GetFallbackReflection(prompt);
                case "response":
                    return GetFallbackResponse(prompt);
                case "planning":
                    return GetFallbackPlan(prompt);
                default:
                    return "我需要时间思考当前的情况...";
            }
        }
        
        private string GetFallbackReflection(string prompt)
        {
            var reflections = new[]
            {
                "在这个严酷的环境中，我深刻体会到了团队合作的重要性。每个人的技能都是宝贵的资源。",
                "面对死亡的威胁，我意识到什么才是真正重要的 - 不是物质财富，而是人与人之间的信任。",
                "饥饿和寒冷教会了我珍惜每一口食物、每一丝温暖。以前觉得理所当然的东西现在都变得珍贵。",
                "我发现在绝境中，人性的光辉和黑暗都会被放大。我选择相信希望，哪怕只有一线生机。",
                "这些经历让我成长了很多。我学会了在恐惧中保持冷静，在绝望中寻找出路。"
            };
            
            int index = Mathf.Abs(prompt.GetHashCode()) % reflections.Length;
            return reflections[index];
        }
        
        private string GetFallbackResponse(string prompt)
        {
            var responses = new[]
            {
                "我明白了，让我们先确保安全。",
                "这确实是个挑战，但我们能克服。",
                "我需要先评估一下风险。",
                "团结就是力量，我们一起面对。",
                "保持冷静，我们会找到解决办法的。"
            };
            
            int index = Mathf.Abs(prompt.GetHashCode()) % responses.Length;
            return responses[index];
        }
        
        private string GetFallbackPlan(string prompt)
        {
            var plans = new[]
            {
                "优先确保基本生存需求：寻找食物和保暖材料。",
                "检查周围环境的安全性，然后规划下一步行动。",
                "与其他幸存者协调，分工合作提高效率。",
                "根据天气情况调整活动计划，避免不必要的风险。",
                "保存体力，专注于最重要的生存任务。"
            };
            
            int index = Mathf.Abs(prompt.GetHashCode()) % plans.Length;
            return plans[index];
        }
    }
    
    [System.Serializable]
    public class EnhancedOpenAIRequest : OpenAIRequest
    {
        public string[] stop; // 停止序列
    }
}
