using System.Threading.Tasks;

namespace IceStormSurvival.Services
{
    /// <summary>
    /// LLM服务接口
    /// 定义AI语言模型的标准接口
    /// </summary>
    public interface ILLMService
    {
        /// <summary>
        /// 生成反思内容
        /// </summary>
        /// <param name="prompt">反思提示</param>
        /// <returns>反思结果</returns>
        Task<string> GenerateReflection(string prompt);
        
        /// <summary>
        /// 生成回应
        /// </summary>
        /// <param name="prompt">对话提示</param>
        /// <returns>回应内容</returns>
        Task<string> GenerateResponse(string prompt);
        
        /// <summary>
        /// 生成行动计划
        /// </summary>
        /// <param name="prompt">计划请求</param>
        /// <returns>行动计划</returns>
        Task<string> GeneratePlan(string prompt);
    }
}
