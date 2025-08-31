using System;
using System.Collections.Generic;
using UnityEngine;

namespace IceStormSurvival.Core
{
    /// <summary>
    /// 记忆数据结构
    /// </summary>
    [System.Serializable]
    public class Memory
    {
        public string content;              // 记忆内容
        public float importance;            // 重要性分数 (0-10)
        public float emotionalValue;        // 情感价值 (-5 to +5)
        public DateTime timestamp;          // 时间戳
        public DateTime lastAccessed;       // 上次访问时间
        public MemoryType memoryType;       // 记忆类型
        public Vector3 location;            // 记忆发生地点
        public List<string> tags;           // 标签
        public string relatedAgent;         // 相关代理
        public int accessCount;             // 访问次数
        
        public Memory()
        {
            timestamp = DateTime.Now;
            lastAccessed = DateTime.Now;
            tags = new List<string>();
            accessCount = 0;
        }
        
        /// <summary>
        /// 计算记忆的总体相关性分数
        /// </summary>
        public float CalculateRelevanceScore(string query, DateTime currentTime)
        {
            float recencyScore = CalculateRecencyScore(currentTime);
            float importanceScore = importance / 10f;
            float relevanceScore = CalculateContentRelevance(query);
            
            return (recencyScore * 0.3f) + (importanceScore * 0.4f) + (relevanceScore * 0.3f);
        }
        
        private float CalculateRecencyScore(DateTime currentTime)
        {
            var timeDiff = currentTime - lastAccessed;
            var hoursSinceAccess = (float)timeDiff.TotalHours;
            
            // 时间衰减函数，1小时内为1.0，24小时后降到0.5，7天后降到0.1
            return Mathf.Max(0.1f, 1f / (1f + hoursSinceAccess / 24f));
        }
        
        private float CalculateContentRelevance(string query)
        {
            if (string.IsNullOrEmpty(query) || string.IsNullOrEmpty(content))
                return 0f;
                
            var queryWords = query.ToLower().Split(' ');
            var contentWords = content.ToLower().Split(' ');
            
            int matches = 0;
            foreach (var queryWord in queryWords)
            {
                foreach (var contentWord in contentWords)
                {
                    if (contentWord.Contains(queryWord) || queryWord.Contains(contentWord))
                    {
                        matches++;
                        break;
                    }
                }
            }
            
            return (float)matches / queryWords.Length;
        }
    }

    /// <summary>
    /// 生存行动数据结构
    /// </summary>
    [System.Serializable]
    public class SurvivalAction
    {
        public ActionType actionType;       // 行动类型
        public string description;          // 行动描述
        public float duration;              // 持续时间（秒）
        public float importance;            // 重要性
        public float emotionalImpact;       // 情感影响
        public Priority priority;           // 优先级
        public Vector3 targetLocation;      // 目标位置
        public string targetAgent;          // 目标代理
        public Dictionary<ResourceType, int> requiredResources; // 所需资源
        public Dictionary<ResourceType, int> producedResources; // 产生资源
        public List<string> prerequisites;  // 前置条件
        public List<string> effects;        // 效果
        public float successProbability;    // 成功概率
        public float energyCost;           // 体力消耗
        
        public SurvivalAction()
        {
            requiredResources = new Dictionary<ResourceType, int>();
            producedResources = new Dictionary<ResourceType, int>();
            prerequisites = new List<string>();
            effects = new List<string>();
            successProbability = 1f;
        }
    }

    /// <summary>
    /// 天气信息数据结构
    /// </summary>
    [System.Serializable]
    public class WeatherInfo
    {
        public WeatherType weatherType;     // 天气类型
        public float temperature;           // 温度（摄氏度）
        public float windSpeed;            // 风速
        public float visibility;           // 能见度
        public float humidity;             // 湿度
        public string description;         // 天气描述
        public float severity;             // 严重程度 (0-1)
        public float duration;             // 持续时间（小时）
        public DateTime startTime;         // 开始时间
        
        public bool IsHazardous => severity > 0.7f || temperature < -20f || windSpeed > 50f;
    }

    /// <summary>
    /// 关系数据结构
    /// </summary>
    [System.Serializable]
    public class Relationship
    {
        public string targetAgentName;      // 目标代理名称
        public RelationshipType type;       // 关系类型
        public float affinity;             // 好感度 (-10 to +10)
        public float trust;                // 信任度 (0-10)
        public float respect;              // 尊重度 (0-10)
        public List<string> sharedMemories; // 共同记忆
        public DateTime lastInteraction;    // 上次互动时间
        public int interactionCount;        // 互动次数
        public List<string> conflictHistory; // 冲突历史
        public List<string> cooperationHistory; // 合作历史
        
        public Relationship()
        {
            sharedMemories = new List<string>();
            conflictHistory = new List<string>();
            cooperationHistory = new List<string>();
            lastInteraction = DateTime.Now;
        }
        
        public float GetOverallRelationshipScore()
        {
            float baseScore = affinity;
            float trustBonus = trust * 0.5f;
            float respectBonus = respect * 0.3f;
            
            return Mathf.Clamp(baseScore + trustBonus + respectBonus, -15f, 15f);
        }
    }

    /// <summary>
    /// 需求数据结构
    /// </summary>
    [System.Serializable]
    public class Need
    {
        public NeedType needType;           // 需求类型
        public float currentValue;          // 当前值 (0-100)
        public float criticalThreshold;     // 临界阈值
        public float urgencyMultiplier;     // 紧急度乘数
        public DateTime lastSatisfied;      // 上次满足时间
        public float decayRate;            // 衰减速率
        public List<ActionType> satisfyingActions; // 满足该需求的行动
        
        public Need()
        {
            satisfyingActions = new List<ActionType>();
            lastSatisfied = DateTime.Now;
        }
        
        public float GetUrgency()
        {
            float urgency = (100f - currentValue) / 100f;
            
            if (currentValue < criticalThreshold)
            {
                urgency *= urgencyMultiplier;
            }
            
            return Mathf.Clamp01(urgency);
        }
        
        public bool IsCritical => currentValue < criticalThreshold;
    }

    /// <summary>
    /// 情感状态数据结构
    /// </summary>
    [System.Serializable]
    public class EmotionalState
    {
        public Dictionary<EmotionType, float> emotions; // 各种情感的强度
        public EmotionType dominantEmotion;    // 主导情感
        public float overallMood;              // 整体情绪 (-5 to +5)
        public float stability;                // 情绪稳定性 (0-1)
        public DateTime lastUpdate;            // 上次更新时间
        
        public EmotionalState()
        {
            emotions = new Dictionary<EmotionType, float>();
            foreach (EmotionType emotion in Enum.GetValues(typeof(EmotionType)))
            {
                emotions[emotion] = 0f;
            }
            lastUpdate = DateTime.Now;
        }
        
        public void UpdateEmotion(EmotionType emotion, float intensity)
        {
            emotions[emotion] = Mathf.Clamp(intensity, 0f, 10f);
            UpdateDominantEmotion();
            UpdateOverallMood();
            lastUpdate = DateTime.Now;
        }
        
        private void UpdateDominantEmotion()
        {
            float maxIntensity = 0f;
            EmotionType dominant = EmotionType.Calm;
            
            foreach (var kvp in emotions)
            {
                if (kvp.Value > maxIntensity)
                {
                    maxIntensity = kvp.Value;
                    dominant = kvp.Key;
                }
            }
            
            dominantEmotion = dominant;
        }
        
        private void UpdateOverallMood()
        {
            float positiveSum = emotions[EmotionType.Happy] + emotions[EmotionType.Hope] + 
                               emotions[EmotionType.Gratitude] + emotions[EmotionType.Love] + 
                               emotions[EmotionType.Pride] + emotions[EmotionType.Excited];
                               
            float negativeSum = emotions[EmotionType.Sad] + emotions[EmotionType.Angry] + 
                               emotions[EmotionType.Fear] + emotions[EmotionType.Despair] + 
                               emotions[EmotionType.Guilt] + emotions[EmotionType.Shame];
            
            overallMood = Mathf.Clamp((positiveSum - negativeSum) / 2f, -5f, 5f);
        }
    }

    /// <summary>
    /// 技能数据结构
    /// </summary>
    [System.Serializable]
    public class Skill
    {
        public SkillType skillType;         // 技能类型
        public float level;                 // 技能等级 (0-10)
        public float experience;            // 经验值
        public DateTime lastUsed;           // 上次使用时间
        public int useCount;               // 使用次数
        public List<string> specializations; // 专门化分支
        public float learningRate;          // 学习速率
        
        public Skill()
        {
            specializations = new List<string>();
            lastUsed = DateTime.Now;
            learningRate = 1f;
        }
        
        public void GainExperience(float amount)
        {
            experience += amount * learningRate;
            
            // 根据经验值更新技能等级
            float newLevel = Mathf.Sqrt(experience / 100f);
            level = Mathf.Min(newLevel, 10f);
            
            lastUsed = DateTime.Now;
            useCount++;
        }
        
        public float GetEffectiveness()
        {
            // 考虑技能等级和最近使用情况
            float baseEffectiveness = level / 10f;
            
            var timeSinceUse = DateTime.Now - lastUsed;
            float rustPenalty = (float)timeSinceUse.TotalDays * 0.01f;
            
            return Mathf.Max(0.1f, baseEffectiveness - rustPenalty);
        }
    }

    /// <summary>
    /// 资源数据结构
    /// </summary>
    [System.Serializable]
    public class Resource
    {
        public ResourceType resourceType;   // 资源类型
        public int quantity;               // 数量
        public float quality;              // 质量 (0-1)
        public DateTime acquiredDate;       // 获得日期
        public DateTime expiryDate;        // 过期日期
        public Vector3 location;           // 存储位置
        public string owner;               // 拥有者
        public bool isShared;              // 是否共享
        public List<string> usageHistory;  // 使用历史
        
        public Resource()
        {
            acquiredDate = DateTime.Now;
            usageHistory = new List<string>();
        }
        
        public bool IsExpired => DateTime.Now > expiryDate;
        public bool IsUsable => quantity > 0 && !IsExpired && quality > 0.1f;
        
        public void Consume(int amount, string user)
        {
            quantity = Mathf.Max(0, quantity - amount);
            usageHistory.Add($"{user} used {amount} on {DateTime.Now}");
        }
    }

    /// <summary>
    /// 计划数据结构
    /// </summary>
    [System.Serializable]
    public class Plan
    {
        public string planName;             // 计划名称
        public string description;          // 计划描述
        public List<SurvivalAction> actions; // 行动列表
        public DateTime createdTime;        // 创建时间
        public DateTime targetCompletionTime; // 目标完成时间
        public float estimatedDuration;     // 预计持续时间
        public Priority priority;           // 优先级
        public float successProbability;    // 成功概率
        public List<string> dependencies;   // 依赖条件
        public Dictionary<ResourceType, int> resourceRequirements; // 资源需求
        public string createdBy;            // 创建者
        public bool isActive;              // 是否激活
        public int currentStep;            // 当前步骤
        
        public Plan()
        {
            actions = new List<SurvivalAction>();
            dependencies = new List<string>();
            resourceRequirements = new Dictionary<ResourceType, int>();
            createdTime = DateTime.Now;
            isActive = false;
            currentStep = 0;
        }
        
        public bool IsCompleted => currentStep >= actions.Count;
        public bool IsOverdue => DateTime.Now > targetCompletionTime;
        
        public SurvivalAction GetCurrentAction()
        {
            if (currentStep < actions.Count)
                return actions[currentStep];
            return null;
        }
        
        public void AdvanceToNextStep()
        {
            if (currentStep < actions.Count)
                currentStep++;
        }
    }

    /// <summary>
    /// 事件数据结构
    /// </summary>
    [System.Serializable]
    public class GameEvent
    {
        public EventType eventType;         // 事件类型
        public string description;          // 事件描述
        public DateTime timestamp;          // 时间戳
        public Vector3 location;           // 发生地点
        public List<string> involvedAgents; // 涉及的代理
        public Dictionary<string, object> parameters; // 事件参数
        public float impact;               // 影响程度
        public bool isGlobal;              // 是否全局事件
        public string triggerReason;       // 触发原因
        
        public GameEvent()
        {
            timestamp = DateTime.Now;
            involvedAgents = new List<string>();
            parameters = new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// 位置数据结构
    /// </summary>
    [System.Serializable]
    public class LocationInfo
    {
        public string locationName;         // 地点名称
        public Vector3 position;           // 位置坐标
        public string description;         // 描述
        public List<ResourceType> availableResources; // 可用资源
        public List<BuildingType> buildings; // 建筑物
        public float safety;               // 安全度
        public float comfort;              // 舒适度
        public WeatherInfo localWeather;   // 局部天气
        public List<string> connectedLocations; // 连接的地点
        public Dictionary<string, float> agentPresence; // 代理在场情况
        
        public LocationInfo()
        {
            availableResources = new List<ResourceType>();
            buildings = new List<BuildingType>();
            connectedLocations = new List<string>();
            agentPresence = new Dictionary<string, float>();
        }
        
        public bool IsSafe => safety > 0.7f;
        public bool IsComfortable => comfort > 0.6f;
        public bool HasShelter => buildings.Contains(BuildingType.Shelter);
    }
}
