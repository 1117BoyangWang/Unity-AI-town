using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using IceStormSurvival.Core;

namespace IceStormSurvival.Systems
{
    /// <summary>
    /// AI代理记忆系统
    /// 基于斯坦福生成式代理研究，实现记忆存储、检索和反思机制
    /// </summary>
    public class MemorySystem
    {
        private const int MAX_MEMORIES = 1000;
        private const float REFLECTION_THRESHOLD = 15f;
        private const int MAX_RETRIEVED_MEMORIES = 5;
        
        private string agentName;
        private List<Memory> memories;
        private Dictionary<string, float[]> memoryEmbeddings; // 简化的向量嵌入
        private float lastReflectionTime;
        
        // LLM服务接口（可替换为不同的LLM实现）
        private ILLMService llmService;
        
        public MemorySystem(string ownerName)
        {
            agentName = ownerName;
            memories = new List<Memory>();
            memoryEmbeddings = new Dictionary<string, float[]>();
            lastReflectionTime = 0f;
            
            // 初始化LLM服务
            llmService = LLMServiceFactory.CreateService();
        }
        
        #region 记忆管理
        
        /// <summary>
        /// 添加新记忆
        /// </summary>
        public void AddMemory(Memory memory)
        {
            if (memory == null || string.IsNullOrEmpty(memory.content))
                return;
                
            // 生成记忆的向量嵌入
            var embedding = GenerateEmbedding(memory.content);
            string memoryId = $"{agentName}_{memories.Count}_{DateTime.Now.Ticks}";
            memoryEmbeddings[memoryId] = embedding;
            
            memory.lastAccessed = DateTime.Now;
            memories.Add(memory);
            
            // 记忆数量管理
            if (memories.Count > MAX_MEMORIES)
            {
                RemoveOldestLowImportanceMemory();
            }
            
            Debug.Log($"[{agentName}] 新增记忆: {memory.content.Substring(0, Math.Min(50, memory.content.Length))}...");
        }
        
        /// <summary>
        /// 根据查询检索相关记忆
        /// </summary>
        public List<Memory> RetrieveMemories(string query, int maxCount = MAX_RETRIEVED_MEMORIES)
        {
            if (string.IsNullOrEmpty(query) || memories.Count == 0)
                return new List<Memory>();
                
            var queryEmbedding = GenerateEmbedding(query);
            var scoredMemories = new List<(Memory memory, float score)>();
            
            for (int i = 0; i < memories.Count; i++)
            {
                var memory = memories[i];
                string memoryId = $"{agentName}_{i}_{memory.timestamp.Ticks}";
                
                if (memoryEmbeddings.ContainsKey(memoryId))
                {
                    float relevanceScore = CalculateMemoryRelevance(memory, queryEmbedding, memoryEmbeddings[memoryId]);
                    scoredMemories.Add((memory, relevanceScore));
                }
            }
            
            // 按相关性排序并返回前N个
            var relevantMemories = scoredMemories
                .OrderByDescending(x => x.score)
                .Take(maxCount)
                .Select(x => x.memory)
                .ToList();
                
            // 更新访问时间
            foreach (var memory in relevantMemories)
            {
                memory.lastAccessed = DateTime.Now;
                memory.accessCount++;
            }
            
            return relevantMemories;
        }
        
        /// <summary>
        /// 计算记忆相关性
        /// </summary>
        private float CalculateMemoryRelevance(Memory memory, float[] queryEmbedding, float[] memoryEmbedding)
        {
            DateTime currentTime = DateTime.Now;
            
            // 1. 时近性 (Recency)
            var timeDiff = currentTime - memory.lastAccessed;
            float recencyScore = 1f / (1f + (float)timeDiff.TotalHours);
            
            // 2. 重要性 (Importance)
            float importanceScore = memory.importance / 10f;
            
            // 3. 相关性 (Relevance) - 向量相似度
            float relevanceScore = CalculateCosineSimilarity(queryEmbedding, memoryEmbedding);
            
            // 综合评分 (斯坦福论文中的权重)
            return (recencyScore * 0.5f) + (importanceScore * 0.3f) + (relevanceScore * 0.2f);
        }
        
        /// <summary>
        /// 计算余弦相似度
        /// </summary>
        private float CalculateCosineSimilarity(float[] vectorA, float[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
                return 0f;
                
            float dotProduct = 0f;
            float magnitudeA = 0f;
            float magnitudeB = 0f;
            
            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magnitudeA += vectorA[i] * vectorA[i];
                magnitudeB += vectorB[i] * vectorB[i];
            }
            
            float magnitude = Mathf.Sqrt(magnitudeA) * Mathf.Sqrt(magnitudeB);
            return magnitude > 0 ? dotProduct / magnitude : 0f;
        }
        
        /// <summary>
        /// 删除最旧的低重要性记忆
        /// </summary>
        private void RemoveOldestLowImportanceMemory()
        {
            var lowImportanceMemories = memories
                .Where(m => m.importance < 5f && m.memoryType != MemoryType.Core)
                .OrderBy(m => m.lastAccessed)
                .ToList();
                
            if (lowImportanceMemories.Count > 0)
            {
                var memoryToRemove = lowImportanceMemories.First();
                int index = memories.IndexOf(memoryToRemove);
                
                memories.Remove(memoryToRemove);
                
                // 同时删除对应的嵌入向量
                string memoryId = $"{agentName}_{index}_{memoryToRemove.timestamp.Ticks}";
                if (memoryEmbeddings.ContainsKey(memoryId))
                {
                    memoryEmbeddings.Remove(memoryId);
                }
            }
        }
        
        #endregion
        
        #region 重要性评估
        
        /// <summary>
        /// 更新记忆重要性分数
        /// </summary>
        public void UpdateImportanceScores()
        {
            foreach (var memory in memories)
            {
                // 基于访问频率和情感价值调整重要性
                float accessBonus = Mathf.Log(memory.accessCount + 1) * 0.5f;
                float emotionalBonus = Mathf.Abs(memory.emotionalValue) * 0.3f;
                
                memory.importance = Mathf.Clamp(memory.importance + accessBonus + emotionalBonus, 0f, 10f);
            }
        }
        
        /// <summary>
        /// 获取最近重要记忆的总分
        /// </summary>
        public float GetRecentImportanceSum()
        {
            DateTime cutoffTime = DateTime.Now.AddHours(-24); // 最近24小时
            
            return memories
                .Where(m => m.timestamp > cutoffTime)
                .Sum(m => m.importance);
        }
        
        #endregion
        
        #region 反思机制
        
        /// <summary>
        /// 生成反思记忆
        /// </summary>
        public async Task GenerateReflection()
        {
            if (Time.time - lastReflectionTime < 300f) // 5分钟冷却
                return;
                
            try
            {
                // 获取高重要性的最近记忆
                var importantMemories = GetRecentImportantMemories();
                
                if (importantMemories.Count < 3)
                    return;
                    
                // 构建反思提示
                string reflectionPrompt = BuildReflectionPrompt(importantMemories);
                
                // 调用LLM生成反思
                string reflectionContent = await llmService.GenerateReflection(reflectionPrompt);
                
                if (!string.IsNullOrEmpty(reflectionContent))
                {
                    var reflectionMemory = new Memory
                    {
                        content = reflectionContent,
                        importance = 8f, // 反思记忆具有高重要性
                        emotionalValue = CalculateReflectionEmotion(reflectionContent),
                        timestamp = DateTime.Now,
                        memoryType = MemoryType.Reflection
                    };
                    
                    AddMemory(reflectionMemory);
                    lastReflectionTime = Time.time;
                    
                    Debug.Log($"[{agentName}] 生成反思: {reflectionContent}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"反思生成错误: {e.Message}");
            }
        }
        
        /// <summary>
        /// 获取最近的重要记忆
        /// </summary>
        private List<Memory> GetRecentImportantMemories()
        {
            DateTime cutoffTime = DateTime.Now.AddHours(-24);
            
            return memories
                .Where(m => m.timestamp > cutoffTime && m.importance > 5f)
                .OrderByDescending(m => m.importance)
                .Take(5)
                .ToList();
        }
        
        /// <summary>
        /// 构建反思提示
        /// </summary>
        private string BuildReflectionPrompt(List<Memory> memories)
        {
            var memoryTexts = memories.Select(m => $"- {m.content}").ToList();
            
            return $@"作为{agentName}，基于以下最近的重要经历，请进行深度反思：

{string.Join("\n", memoryTexts)}

请从这些经历中总结出一个深层的洞察或教训，用第一人称表达，不超过100字：";
        }
        
        /// <summary>
        /// 计算反思的情感价值
        /// </summary>
        private float CalculateReflectionEmotion(string reflectionContent)
        {
            string lowerContent = reflectionContent.ToLower();
            
            // 简单的情感分析
            float emotionalValue = 0f;
            
            // 正面情感关键词
            string[] positiveWords = { "学会", "成长", "希望", "团结", "感激", "成功", "帮助", "友谊" };
            foreach (var word in positiveWords)
            {
                if (lowerContent.Contains(word))
                    emotionalValue += 0.5f;
            }
            
            // 负面情感关键词
            string[] negativeWords = { "困难", "恐惧", "失败", "孤独", "绝望", "危险", "死亡", "饥饿" };
            foreach (var word in negativeWords)
            {
                if (lowerContent.Contains(word))
                    emotionalValue -= 0.5f;
            }
            
            return Mathf.Clamp(emotionalValue, -3f, 3f);
        }
        
        #endregion
        
        #region 向量嵌入
        
        /// <summary>
        /// 生成文本的向量嵌入（简化版本）
        /// 在实际项目中应该使用真正的嵌入模型
        /// </summary>
        private float[] GenerateEmbedding(string text)
        {
            const int embeddingSize = 128;
            float[] embedding = new float[embeddingSize];
            
            // 简化的嵌入生成：基于字符和单词的哈希
            string lowerText = text.ToLower();
            var words = lowerText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 0; i < embeddingSize; i++)
            {
                float value = 0f;
                
                // 基于字符的特征
                if (i < lowerText.Length)
                {
                    value += (float)lowerText[i] / 255f;
                }
                
                // 基于单词的特征
                int wordIndex = i % words.Length;
                if (wordIndex < words.Length)
                {
                    value += (float)words[wordIndex].GetHashCode() / int.MaxValue;
                }
                
                // 基于语义关键词的特征
                value += GetSemanticFeature(lowerText, i);
                
                embedding[i] = value;
            }
            
            // 归一化
            float magnitude = Mathf.Sqrt(embedding.Sum(x => x * x));
            if (magnitude > 0)
            {
                for (int i = 0; i < embeddingSize; i++)
                {
                    embedding[i] /= magnitude;
                }
            }
            
            return embedding;
        }
        
        /// <summary>
        /// 基于语义关键词生成特征
        /// </summary>
        private float GetSemanticFeature(string text, int featureIndex)
        {
            // 定义不同类别的关键词
            string[][] categories = {
                new string[] { "食物", "饥饿", "吃", "饭", "面包", "水" }, // 食物类
                new string[] { "温暖", "冷", "火", "毯子", "避难所" },     // 温度类
                new string[] { "朋友", "帮助", "合作", "团队", "一起" },   // 社交类
                new string[] { "危险", "害怕", "死亡", "受伤", "威胁" },   // 危险类
                new string[] { "建造", "修理", "工具", "材料", "房子" },   // 建设类
            };
            
            int categoryIndex = featureIndex % categories.Length;
            var keywords = categories[categoryIndex];
            
            float feature = 0f;
            foreach (var keyword in keywords)
            {
                if (text.Contains(keyword))
                {
                    feature += 1f / keywords.Length;
                }
            }
            
            return feature;
        }
        
        #endregion
        
        #region 公共接口
        
        /// <summary>
        /// 获取所有记忆
        /// </summary>
        public List<Memory> GetAllMemories()
        {
            return new List<Memory>(memories);
        }
        
        /// <summary>
        /// 获取特定类型的记忆
        /// </summary>
        public List<Memory> GetMemoriesByType(MemoryType type)
        {
            return memories.Where(m => m.memoryType == type).ToList();
        }
        
        /// <summary>
        /// 获取记忆统计信息
        /// </summary>
        public MemoryStats GetMemoryStats()
        {
            return new MemoryStats
            {
                totalMemories = memories.Count,
                averageImportance = memories.Count > 0 ? memories.Average(m => m.importance) : 0f,
                oldestMemory = memories.Count > 0 ? memories.Min(m => m.timestamp) : DateTime.Now,
                newestMemory = memories.Count > 0 ? memories.Max(m => m.timestamp) : DateTime.Now,
                reflectionCount = memories.Count(m => m.memoryType == MemoryType.Reflection)
            };
        }
        
        /// <summary>
        /// 清理过期记忆
        /// </summary>
        public void CleanupExpiredMemories()
        {
            DateTime expirationDate = DateTime.Now.AddDays(-30); // 30天过期
            
            var expiredMemories = memories
                .Where(m => m.timestamp < expirationDate && m.memoryType != MemoryType.Core)
                .ToList();
                
            foreach (var memory in expiredMemories)
            {
                memories.Remove(memory);
            }
            
            if (expiredMemories.Count > 0)
            {
                Debug.Log($"[{agentName}] 清理了 {expiredMemories.Count} 个过期记忆");
            }
        }
        
        #endregion
    }
    
    /// <summary>
    /// 记忆统计信息
    /// </summary>
    public class MemoryStats
    {
        public int totalMemories;
        public float averageImportance;
        public DateTime oldestMemory;
        public DateTime newestMemory;
        public int reflectionCount;
    }
    
    /// <summary>
    /// LLM服务接口
    /// </summary>
    public interface ILLMService
    {
        Task<string> GenerateReflection(string prompt);
        Task<string> GenerateResponse(string prompt);
        Task<string> GeneratePlan(string prompt);
    }
    
    /// <summary>
    /// LLM服务工厂
    /// </summary>
    public static class LLMServiceFactory
    {
        public static ILLMService CreateService()
        {
            // 根据配置返回不同的LLM实现
            var config = Services.ConfigurationManager.Instance;
            
            if (config.IsConfigured())
            {
                Debug.Log("使用增强版OpenAI服务");
                return new Services.EnhancedOpenAIService();
            }
            else
            {
                Debug.LogWarning("OpenAI未配置，使用模拟服务");
                Debug.LogWarning("要启用真实AI对话，请参考OPENAI_SETUP.md配置API密钥");
                return new MockLLMService();
            }
        }
    }
    
    /// <summary>
    /// 模拟LLM服务（用于测试）
    /// </summary>
    public class MockLLMService : ILLMService
    {
        private readonly string[] reflectionTemplates = {
            "通过这些经历，我意识到团队合作的重要性，只有相互帮助才能在困境中生存。",
            "我学会了在资源匮乏时要更加珍惜每一样物品，合理分配使用。",
            "这些困难让我明白，保持希望和积极的心态对团队士气至关重要。",
            "我发现在危机时刻，冷静思考比匆忙行动更能找到解决方案。",
            "通过观察和交流，我认识到每个人都有独特的技能，应该发挥所长。"
        };
        
        public async Task<string> GenerateReflection(string prompt)
        {
            await Task.Delay(100); // 模拟网络延迟
            return reflectionTemplates[UnityEngine.Random.Range(0, reflectionTemplates.Length)];
        }
        
        public async Task<string> GenerateResponse(string prompt)
        {
            await Task.Delay(100);
            return "这是一个模拟的LLM响应。";
        }
        
        public async Task<string> GeneratePlan(string prompt)
        {
            await Task.Delay(100);
            return "这是一个模拟的计划生成。";
        }
    }
}
