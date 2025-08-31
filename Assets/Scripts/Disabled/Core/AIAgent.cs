using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using IceStormSurvival.Systems;

namespace IceStormSurvival.Core
{
    /// <summary>
    /// 冰雪风暴末日生存AI代理
    /// 基于斯坦福生成式代理研究，专为末日生存场景优化
    /// </summary>
    public class AIAgent : MonoBehaviour
    {
        [Header("基础信息")]
        [SerializeField] private string agentName;
        [SerializeField] private string profession; // 职业：医生、工程师、教师等
        [SerializeField] private int age;
        [SerializeField] private string personality; // 性格描述
        
        [Header("生存状态")]
        [SerializeField] private float health = 100f;
        [SerializeField] private float hunger = 100f;
        [SerializeField] private float warmth = 100f;
        [SerializeField] private float energy = 100f;
        [SerializeField] private float morale = 100f; // 士气/希望值
        
        [Header("技能属性")]
        [SerializeField] private float medicalSkill = 0f;
        [SerializeField] private float constructionSkill = 0f;
        [SerializeField] private float huntingSkill = 0f;
        [SerializeField] private float cookingSkill = 0f;
        [SerializeField] private float leadershipSkill = 0f;
        
        [Header("社交网络")]
        [SerializeField] private Dictionary<AIAgent, float> relationships = new Dictionary<AIAgent, float>();
        [SerializeField] private List<AIAgent> trustedAllies = new List<AIAgent>();
        
        // 核心系统组件
        private MemorySystem memorySystem;
        private PlanningSystem planningSystem;
        private SurvivalNeeds survivalNeeds;
        private SocialBehavior socialBehavior;
        private DecisionMaker decisionMaker;
        
        // 当前状态
        private AgentState currentState;
        private List<SurvivalAction> currentPlan;
        private int currentPlanIndex = 0;
        
        // 环境感知
        private List<GameObject> nearbyObjects = new List<GameObject>();
        private List<AIAgent> nearbyAgents = new List<AIAgent>();
        private WeatherSystem currentWeather;
        
        public string AgentName => agentName;
        public string Profession => profession;
        public AgentState CurrentState => currentState;
        public float Health => health;
        public float Morale => morale;
        
        #region Unity生命周期
        
        private void Awake()
        {
            InitializeAgent();
        }
        
        private void Start()
        {
            StartCoroutine(AgentLifeCycle());
        }
        
        private void Update()
        {
            UpdatePerception();
            UpdateSurvivalNeeds();
        }
        
        #endregion
        
        #region 初始化
        
        private void InitializeAgent()
        {
            // 初始化记忆系统
            memorySystem = new MemorySystem(agentName);
            
            // 初始化规划系统
            planningSystem = new PlanningSystem(this);
            
            // 初始化生存需求系统
            survivalNeeds = new SurvivalNeeds(this);
            
            // 初始化社交行为系统
            socialBehavior = new SocialBehavior(this);
            
            // 初始化决策系统
            decisionMaker = new DecisionMaker(this);
            
            // 设置初始状态
            currentState = AgentState.Idle;
            
            // 添加初始记忆
            AddInitialMemories();
        }
        
        private void AddInitialMemories()
        {
            string initialMemory = $"我是{agentName}，职业是{profession}。" +
                                 $"我{age}岁，性格{personality}。" +
                                 $"冰雪风暴席卷了世界，我必须在这个小镇中生存下去。";
            
            memorySystem.AddMemory(new Memory
            {
                content = initialMemory,
                importance = 9f,
                emotionalValue = -2f, // 负面情绪，因为是灾难背景
                timestamp = DateTime.Now,
                memoryType = MemoryType.Core
            });
            
            // 根据职业添加技能记忆
            AddProfessionMemories();
        }
        
        private void AddProfessionMemories()
        {
            string skillMemory = "";
            switch (profession.ToLower())
            {
                case "医生":
                    medicalSkill = 8f;
                    skillMemory = "我有丰富的医疗知识，可以治疗伤病，在生存中这是珍贵的技能。";
                    break;
                case "工程师":
                    constructionSkill = 8f;
                    skillMemory = "我擅长建造和修理，可以帮助加固避难所，维修重要设备。";
                    break;
                case "教师":
                    leadershipSkill = 7f;
                    skillMemory = "我善于组织和教导他人，可以帮助团队保持团结和希望。";
                    break;
                case "猎人":
                    huntingSkill = 8f;
                    skillMemory = "我有野外生存和狩猎经验，可以为团队提供食物。";
                    break;
                case "厨师":
                    cookingSkill = 8f;
                    skillMemory = "我会做饭，能将有限的食材做得更有营养和美味，提升大家的士气。";
                    break;
            }
            
            if (!string.IsNullOrEmpty(skillMemory))
            {
                memorySystem.AddMemory(new Memory
                {
                    content = skillMemory,
                    importance = 7f,
                    emotionalValue = 2f,
                    timestamp = DateTime.Now,
                    memoryType = MemoryType.Skill
                });
            }
        }
        
        #endregion
        
        #region 主要生命周期
        
        private System.Collections.IEnumerator AgentLifeCycle()
        {
            while (true)
            {
                float waitTime = GetActionInterval();
                bool hasError = false;
                
                // 启动异步生命周期任务
                var asyncTask = ExecuteAgentLifeCycleAsync();
                
                // 等待异步任务完成
                yield return new WaitUntil(() => asyncTask.IsCompleted);
                
                // 检查任务是否有异常
                if (asyncTask.IsFaulted && asyncTask.Exception != null)
                {
                    Debug.LogError($"Agent {agentName} lifecycle error: {asyncTask.Exception.GetBaseException().Message}");
                    hasError = true;
                }
                
                // 根据是否有错误调整等待时间
                if (hasError)
                {
                    waitTime = 1f; // 出错时等待1秒
                }
                
                yield return new WaitForSeconds(waitTime);
            }
        }
        
        private async Task ExecuteAgentLifeCycleAsync()
        {
            // 1. 感知环境变化
            PerceiveEnvironment();
            
            // 2. 更新记忆重要性
            memorySystem.UpdateImportanceScores();
            
            // 3. 反思和整合记忆
            await ReflectOnExperiences();
            
            // 4. 评估当前需求
            EvaluateNeeds();
            
            // 5. 生成或更新计划
            await GenerateOrUpdatePlan();
            
            // 6. 执行当前行动
            ExecuteCurrentAction();
            
            // 7. 社交互动
            await HandleSocialInteractions();
        }
        
        private float GetActionInterval()
        {
            // 根据代理状态调整行动间隔
            return currentState switch
            {
                AgentState.Emergency => 0.5f,
                AgentState.Working => 2f,
                AgentState.Socializing => 3f,
                AgentState.Resting => 5f,
                _ => 1f
            };
        }
        
        #endregion
        
        #region 感知系统
        
        private void UpdatePerception()
        {
            DetectNearbyObjects();
            DetectNearbyAgents();
            MonitorWeather();
        }
        
        private void PerceiveEnvironment()
        {
            // 感知环境变化并添加到记忆
            List<string> observations = new List<string>();
            
            // 天气感知
            if (currentWeather != null)
            {
                var weatherInfo = currentWeather.GetCurrentWeather();
                observations.Add($"当前天气：{weatherInfo.description}，温度：{weatherInfo.temperature}°C");
            }
            
            // 物体感知
            foreach (var obj in nearbyObjects)
            {
                if (obj != null)
                {
                    observations.Add($"我看到了{obj.name}");
                }
            }
            
            // 人物感知
            foreach (var agent in nearbyAgents)
            {
                if (agent != null && agent != this)
                {
                    observations.Add($"我看到{agent.AgentName}在附近，他看起来{DescribeAgentState(agent)}");
                }
            }
            
            // 将观察添加到记忆
            foreach (var observation in observations)
            {
                memorySystem.AddMemory(new Memory
                {
                    content = observation,
                    importance = CalculateObservationImportance(observation),
                    emotionalValue = CalculateObservationEmotion(observation),
                    timestamp = DateTime.Now,
                    memoryType = MemoryType.Observation
                });
            }
        }
        
        private void DetectNearbyObjects()
        {
            nearbyObjects.Clear();
            Collider[] colliders = Physics.OverlapSphere(transform.position, 10f);
            
            foreach (var collider in colliders)
            {
                if (collider.gameObject != gameObject)
                {
                    nearbyObjects.Add(collider.gameObject);
                }
            }
        }
        
        private void DetectNearbyAgents()
        {
            nearbyAgents.Clear();
            Collider[] colliders = Physics.OverlapSphere(transform.position, 15f);
            
            foreach (var collider in colliders)
            {
                var agent = collider.GetComponent<AIAgent>();
                if (agent != null && agent != this)
                {
                    nearbyAgents.Add(agent);
                }
            }
        }
        
        private void MonitorWeather()
        {
            currentWeather = FindObjectOfType<WeatherSystem>();
        }
        
        private string DescribeAgentState(AIAgent agent)
        {
            if (agent.Health < 30) return "虚弱";
            if (agent.Morale < 30) return "沮丧";
            if (agent.currentState == AgentState.Working) return "忙碌";
            if (agent.currentState == AgentState.Resting) return "在休息";
            return "还好";
        }
        
        private float CalculateObservationImportance(string observation)
        {
            // 基于观察内容计算重要性
            if (observation.Contains("紧急") || observation.Contains("危险"))
                return 9f;
            if (observation.Contains("食物") || observation.Contains("燃料"))
                return 7f;
            if (observation.Contains("天气"))
                return 5f;
            return 3f;
        }
        
        private float CalculateObservationEmotion(string observation)
        {
            // 基于观察内容计算情感价值
            if (observation.Contains("危险") || observation.Contains("受伤"))
                return -3f;
            if (observation.Contains("食物") || observation.Contains("温暖"))
                return 2f;
            if (observation.Contains("朋友") || observation.Contains("帮助"))
                return 1f;
            return 0f;
        }
        
        #endregion
        
        #region 记忆与反思
        
        private async Task ReflectOnExperiences()
        {
            // 如果重要记忆累积到阈值，进行反思
            float totalImportance = memorySystem.GetRecentImportanceSum();
            
            if (totalImportance > 15f) // 反思阈值
            {
                await memorySystem.GenerateReflection();
            }
        }
        
        #endregion
        
        #region 需求评估
        
        private void UpdateSurvivalNeeds()
        {
            // 随时间自然下降的需求
            hunger = Mathf.Max(0, hunger - Time.deltaTime * 0.5f);
            energy = Mathf.Max(0, energy - Time.deltaTime * 0.3f);
            warmth = Mathf.Max(0, warmth - Time.deltaTime * 0.8f); // 寒冷环境中体温下降快
            
            // 基于环境调整
            if (currentWeather != null)
            {
                var weather = currentWeather.GetCurrentWeather();
                if (weather.temperature < -10)
                {
                    warmth = Mathf.Max(0, warmth - Time.deltaTime * 1.5f);
                }
            }
            
            // 健康状态影响其他需求
            if (health < 50)
            {
                energy = Mathf.Max(0, energy - Time.deltaTime * 0.2f);
                morale = Mathf.Max(0, morale - Time.deltaTime * 0.1f);
            }
            
            // 需求过低时影响士气
            if (hunger < 20 || warmth < 20)
            {
                morale = Mathf.Max(0, morale - Time.deltaTime * 0.3f);
            }
        }
        
        private void EvaluateNeeds()
        {
            survivalNeeds.EvaluateCurrentNeeds();
        }
        
        #endregion
        
        #region 计划与决策
        
        private async Task GenerateOrUpdatePlan()
        {
            // 检查当前计划是否仍然有效
            if (IsCurrentPlanObsolete())
            {
                currentPlan = await planningSystem.GenerateNewPlan();
                currentPlanIndex = 0;
            }
        }
        
        private bool IsCurrentPlanObsolete()
        {
            if (currentPlan == null || currentPlan.Count == 0)
                return true;
                
            if (currentPlanIndex >= currentPlan.Count)
                return true;
                
            // 如果有紧急需求，重新规划
            if (health < 20 || hunger < 10 || warmth < 10)
                return true;
                
            return false;
        }
        
        #endregion
        
        #region 行动执行
        
        private void ExecuteCurrentAction()
        {
            if (currentPlan != null && currentPlanIndex < currentPlan.Count)
            {
                var action = currentPlan[currentPlanIndex];
                ExecuteAction(action);
            }
        }
        
        private void ExecuteAction(SurvivalAction action)
        {
            switch (action.actionType)
            {
                case ActionType.FindFood:
                    ExecuteFindFood();
                    break;
                case ActionType.FindShelter:
                    ExecuteFindShelter();
                    break;
                case ActionType.BuildFire:
                    ExecuteBuildFire();
                    break;
                case ActionType.HelpOthers:
                    ExecuteHelpOthers();
                    break;
                case ActionType.Rest:
                    ExecuteRest();
                    break;
                case ActionType.Socialize:
                    ExecuteSocialize();
                    break;
            }
            
            // 记录行动
            memorySystem.AddMemory(new Memory
            {
                content = $"我{action.description}",
                importance = action.importance,
                emotionalValue = action.emotionalImpact,
                timestamp = DateTime.Now,
                memoryType = MemoryType.Action
            });
            
            currentPlanIndex++;
        }
        
        private void ExecuteFindFood()
        {
            // 寻找食物的逻辑
            currentState = AgentState.Working;
            // 实际的寻找食物实现...
        }
        
        private void ExecuteFindShelter()
        {
            // 寻找避难所的逻辑
            currentState = AgentState.Working;
            // 实际的寻找避难所实现...
        }
        
        private void ExecuteBuildFire()
        {
            // 生火的逻辑
            currentState = AgentState.Working;
            // 实际的生火实现...
        }
        
        private void ExecuteHelpOthers()
        {
            // 帮助他人的逻辑
            currentState = AgentState.Socializing;
            // 实际的帮助他人实现...
        }
        
        private void ExecuteRest()
        {
            // 休息的逻辑
            currentState = AgentState.Resting;
            energy = Mathf.Min(100, energy + 20f);
        }
        
        private void ExecuteSocialize()
        {
            // 社交的逻辑
            currentState = AgentState.Socializing;
            // 实际的社交实现...
        }
        
        #endregion
        
        #region 社交系统
        
        private async Task HandleSocialInteractions()
        {
            foreach (var nearbyAgent in nearbyAgents)
            {
                if (ShouldInteractWith(nearbyAgent))
                {
                    await socialBehavior.InteractWith(nearbyAgent);
                }
            }
        }
        
        private bool ShouldInteractWith(AIAgent other)
        {
            // 基于关系、需求、距离等因素决定是否互动
            if (Vector3.Distance(transform.position, other.transform.position) > 5f)
                return false;
                
            if (relationships.ContainsKey(other) && relationships[other] < -5f)
                return false; // 关系太差不互动
                
            return UnityEngine.Random.Range(0f, 1f) < 0.3f; // 30%的概率互动
        }
        
        #endregion
        
        #region 公共方法
        
        public void AddRelationship(AIAgent other, float relationshipValue)
        {
            relationships[other] = Mathf.Clamp(relationshipValue, -10f, 10f);
        }
        
        public float GetRelationshipWith(AIAgent other)
        {
            return relationships.ContainsKey(other) ? relationships[other] : 0f;
        }
        
        public void ModifyHealth(float amount)
        {
            health = Mathf.Clamp(health + amount, 0f, 100f);
        }
        
        public void ModifyHunger(float amount)
        {
            hunger = Mathf.Clamp(hunger + amount, 0f, 100f);
        }
        
        public void ModifyWarmth(float amount)
        {
            warmth = Mathf.Clamp(warmth + amount, 0f, 100f);
        }
        
        public void ModifyMorale(float amount)
        {
            morale = Mathf.Clamp(morale + amount, 0f, 100f);
        }
        
        #endregion
    }
}
