using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using IceStormSurvival.Systems;

namespace IceStormSurvival.Services
{
    /// <summary>
    /// OpenAI API 服务实现
    /// 提供真实的GPT模型调用功能
    /// </summary>
    public class OpenAIService : ILLMService
    {
        private const string API_URL = "https://api.openai.com/v1/chat/completions";
        private const int MAX_RETRIES = 3;
        private const float RETRY_DELAY = 1f;
        
        private string apiKey;
        private string model;
        private float temperature;
        private int maxTokens;
        
        public OpenAIService()
        {
            LoadConfiguration();
        }
        
        private void LoadConfiguration()
        {
            // 从配置文件或环境变量读取设置
            var config = ConfigurationManager.Instance.GetOpenAIConfig();
            
            apiKey = config.apiKey;
            model = config.model;
            temperature = config.temperature;
            maxTokens = config.maxTokens;
            
            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogError("OpenAI API密钥未配置！请在配置文件中设置API_KEY。");
            }
        }
        
        public async Task<string> GenerateReflection(string prompt)
        {
            string systemPrompt = @"你是一个在冰雪风暴末日环境中生存的AI代理。基于你的经历和记忆，进行深度反思。
要求：
1. 用第一人称表达
2. 体现人性和情感
3. 不超过100字
4. 结合末日生存背景";
            
            return await CallOpenAI(systemPrompt, prompt);
        }
        
        public async Task<string> GenerateResponse(string prompt)
        {
            string systemPrompt = @"你是一个在冰雪风暴末日环境中生存的AI代理。请基于当前情况做出符合你性格的回应。
要求：
1. 保持角色一致性
2. 考虑生存环境的影响
3. 回应简洁明了
4. 体现真实的人性反应";
            
            return await CallOpenAI(systemPrompt, prompt);
        }
        
        public async Task<string> GeneratePlan(string prompt)
        {
            string systemPrompt = @"你是一个在冰雪风暴末日环境中的生存专家。基于当前情况制定实际可行的生存计划。
要求：
1. 计划要具体可执行
2. 考虑资源限制
3. 优先考虑安全和生存
4. 计划要有逻辑性和连贯性";
            
            return await CallOpenAI(systemPrompt, prompt);
        }
        
        private async Task<string> CallOpenAI(string systemPrompt, string userPrompt)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogWarning("API密钥未配置，返回默认响应");
                return GetFallbackResponse(userPrompt);
            }
            
            for (int attempt = 0; attempt < MAX_RETRIES; attempt++)
            {
                try
                {
                    var requestData = CreateRequestData(systemPrompt, userPrompt);
                    var response = await SendRequest(requestData);
                    
                    if (!string.IsNullOrEmpty(response))
                    {
                        return response;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"OpenAI API调用失败 (尝试 {attempt + 1}/{MAX_RETRIES}): {e.Message}");
                    
                    if (attempt < MAX_RETRIES - 1)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(RETRY_DELAY * (attempt + 1)));
                    }
                }
            }
            
            Debug.LogError("OpenAI API调用失败，使用备用响应");
            return GetFallbackResponse(userPrompt);
        }
        
        private OpenAIRequest CreateRequestData(string systemPrompt, string userPrompt)
        {
            return new OpenAIRequest
            {
                model = model,
                messages = new List<Message>
                {
                    new Message { role = "system", content = systemPrompt },
                    new Message { role = "user", content = userPrompt }
                },
                temperature = temperature,
                max_tokens = maxTokens,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
            };
        }
        
        private async Task<string> SendRequest(OpenAIRequest requestData)
        {
            string jsonData = JsonUtility.ToJson(requestData);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            
            using (UnityWebRequest request = new UnityWebRequest(API_URL, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                
                // 设置请求头
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
                
                // 发送请求
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
                        return response.choices[0].message.content.Trim();
                    }
                }
                else
                {
                    Debug.LogError($"OpenAI API错误: {request.error}\n响应: {request.downloadHandler.text}");
                    throw new Exception($"API调用失败: {request.error}");
                }
            }
            
            return null;
        }
        
        private string GetFallbackResponse(string prompt)
        {
            // 备用响应，当API不可用时使用
            var fallbackResponses = new string[]
            {
                "在这种严酷的环境下，我需要保持冷静，优先确保基本的生存需求。",
                "团队合作是我们在末日中生存的关键，我应该与其他人协调行动。",
                "资源有限，我必须谨慎计划每一个行动，避免不必要的风险。",
                "虽然环境恶劣，但我不能失去希望，这是支撑我继续前进的动力。",
                "我需要根据当前情况调整策略，灵活应对各种挑战。"
            };
            
            int index = Mathf.Abs(prompt.GetHashCode()) % fallbackResponses.Length;
            return fallbackResponses[index];
        }
    }
    
    // OpenAI API 数据结构
    [System.Serializable]
    public class OpenAIRequest
    {
        public string model;
        public List<Message> messages;
        public float temperature;
        public int max_tokens;
        public float top_p;
        public float frequency_penalty;
        public float presence_penalty;
    }
    
    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }
    
    [System.Serializable]
    public class OpenAIResponse
    {
        public Choice[] choices;
        public Usage usage;
    }
    
    [System.Serializable]
    public class Choice
    {
        public Message message;
        public string finish_reason;
    }
    
    [System.Serializable]
    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }
}
