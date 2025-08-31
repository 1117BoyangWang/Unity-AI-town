using IceStormSurvival.Services;

namespace IceStormSurvival.Systems
{
    /// <summary>
    /// LLM服务工厂
    /// 根据配置创建适当的LLM服务实例
    /// </summary>
    public static class LLMServiceFactory
    {
        /// <summary>
        /// 创建LLM服务实例
        /// </summary>
        /// <returns>LLM服务实例</returns>
        public static ILLMService CreateService()
        {
            var config = ConfigurationManager.Instance;
            
            if (config.IsConfigured())
            {
                // 如果配置了OpenAI API，使用增强版OpenAI服务
                return new EnhancedOpenAIService();
            }
            else
            {
                // 否则使用Mock服务
                return new MockLLMService();
            }
        }
    }
}
