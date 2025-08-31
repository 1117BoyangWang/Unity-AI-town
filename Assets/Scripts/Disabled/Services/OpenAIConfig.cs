using UnityEngine;

namespace IceStormSurvival.Services
{
    /// <summary>
    /// OpenAI API 配置
    /// </summary>
    [System.Serializable]
    public class OpenAIConfig
    {
        [Header("API设置")]
        public string apiKey = "";
        
        [Header("模型参数")]
        public string model = "gpt-3.5-turbo";
        public float temperature = 0.7f;
        public int maxTokens = 150;
        public string baseUrl = "https://api.openai.com/v1";
        
        [Header("高级设置")]
        public float requestTimeout = 30f;
        public int maxRetries = 3;
        public bool useProxy = false;
        public string proxyUrl = "";
        
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(model);
        }
    }
}
