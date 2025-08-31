using System.Collections.Generic;
using UnityEngine;
using IceStormSurvival.Core;
using IceStormSurvival.Systems;

namespace IceStormSurvival.Managers
{
    /// <summary>
    /// 游戏主管理器 - 管理AI代理和游戏世界
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("游戏设置")]
        [SerializeField] private int numberOfAgents = 10;
        [SerializeField] private GameObject agentPrefab;
        [SerializeField] private float spawnRadius = 20f;
        [SerializeField] private bool autoStart = true;
        
        [Header("模拟参数")]
        [SerializeField] private float timeScale = 1f;
        [SerializeField] private int simulationDays = 30;
        
        private List<AIAgent> agents = new List<AIAgent>();
        private WeatherSystem weatherSystem;
        private float startTime;
        private int currentDay = 1;
        
        // 预设的角色配置
        private readonly AgentConfig[] agentConfigs = {
            new AgentConfig { name = "李医生", profession = "医生", age = 42, personality = "冷静、理性、关心他人" },
            new AgentConfig { name = "张工程师", profession = "工程师", age = 35, personality = "逻辑性强、动手能力强、喜欢解决问题" },
            new AgentConfig { name = "王老师", profession = "教师", age = 38, personality = "耐心、组织能力强、善于沟通" },
            new AgentConfig { name = "刘猎人", profession = "猎人", age = 45, personality = "警觉、独立、经验丰富" },
            new AgentConfig { name = "陈厨师", profession = "厨师", age = 29, personality = "创造力强、乐观、善于团队合作" },
            new AgentConfig { name = "赵学生", profession = "学生", age = 22, personality = "好奇、学习能力强、容易紧张" },
            new AgentConfig { name = "孙退休工人", profession = "工人", age = 58, personality = "经验丰富、踏实、有点固执" },
            new AgentConfig { name = "周护士", profession = "护士", age = 27, personality = "细心、有爱心、容易情绪化" },
            new AgentConfig { name = "吴农民", profession = "农民", age = 51, personality = "勤劳、朴实、了解自然" },
            new AgentConfig { name = "郑司机", profession = "司机", age = 33, personality = "果断、熟悉地形、喜欢冒险" }
        };
        
        public static GameManager Instance { get; private set; }
        public List<AIAgent> Agents => agents;
        public int CurrentDay => currentDay;
        public float TimeScale => timeScale;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            InitializeGame();
            
            if (autoStart)
            {
                StartSimulation();
            }
        }
        
        private void Update()
        {
            UpdateDayCounter();
        }
        
        private void InitializeGame()
        {
            Debug.Log("=== 冰雪风暴生存小镇 - 游戏开始 ===");
            
            // 初始化天气系统
            weatherSystem = FindObjectOfType<WeatherSystem>();
            if (weatherSystem == null)
            {
                var weatherGO = new GameObject("WeatherSystem");
                weatherSystem = weatherGO.AddComponent<WeatherSystem>();
            }
            
            // 创建AI代理
            CreateAgents();
            
            startTime = Time.time;
            
            Debug.Log($"游戏初始化完成 - 创建了 {agents.Count} 个AI代理");
        }
        
        private void CreateAgents()
        {
            for (int i = 0; i < numberOfAgents && i < agentConfigs.Length; i++)
            {
                CreateAgent(agentConfigs[i], i);
            }
        }
        
        private void CreateAgent(AgentConfig config, int index)
        {
            // 计算生成位置
            float angle = (float)index / numberOfAgents * 2f * Mathf.PI;
            Vector3 spawnPosition = new Vector3(
                Mathf.Cos(angle) * spawnRadius,
                0f,
                Mathf.Sin(angle) * spawnRadius
            );
            
            // 创建代理GameObject
            GameObject agentGO;
            if (agentPrefab != null)
            {
                agentGO = Instantiate(agentPrefab, spawnPosition, Quaternion.identity);
            }
            else
            {
                agentGO = CreateDefaultAgentGameObject(spawnPosition);
            }
            
            agentGO.name = $"Agent_{config.name}";
            
            // 添加AIAgent组件
            var agent = agentGO.GetComponent<AIAgent>();
            if (agent == null)
            {
                agent = agentGO.AddComponent<AIAgent>();
            }
            
            // 配置代理
            ConfigureAgent(agent, config);
            
            agents.Add(agent);
            
            Debug.Log($"创建AI代理: {config.name} ({config.profession})");
        }
        
        private GameObject CreateDefaultAgentGameObject(Vector3 position)
        {
            var agentGO = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            agentGO.transform.position = position;
            
            // 设置随机颜色以区分代理
            var renderer = agentGO.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(
                    Random.Range(0.3f, 1f),
                    Random.Range(0.3f, 1f),
                    Random.Range(0.3f, 1f)
                );
            }
            
            // 添加碰撞器和刚体
            var collider = agentGO.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = false;
            }
            
            var rigidbody = agentGO.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = agentGO.AddComponent<Rigidbody>();
            }
            rigidbody.mass = 70f;
            rigidbody.freezeRotation = true;
            
            return agentGO;
        }
        
        private void ConfigureAgent(AIAgent agent, AgentConfig config)
        {
            // 通过反射设置私有字段（在实际项目中应该使用公共属性或方法）
            var agentType = typeof(AIAgent);
            
            var nameField = agentType.GetField("agentName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            nameField?.SetValue(agent, config.name);
            
            var professionField = agentType.GetField("profession", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            professionField?.SetValue(agent, config.profession);
            
            var ageField = agentType.GetField("age", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ageField?.SetValue(agent, config.age);
            
            var personalityField = agentType.GetField("personality", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            personalityField?.SetValue(agent, config.personality);
        }
        
        private void StartSimulation()
        {
            Debug.Log("=== 开始30天生存模拟 ===");
            
            // 设置时间缩放
            if (weatherSystem != null)
            {
                weatherSystem.SetTimeScale(timeScale);
            }
            
            // 启动日志记录
            InvokeRepeating(nameof(LogDailyStatus), 60f / timeScale, 60f / timeScale); // 每分钟记录一次状态
        }
        
        private void UpdateDayCounter()
        {
            float elapsedTime = Time.time - startTime;
            int newDay = Mathf.FloorToInt(elapsedTime * timeScale / 86400f) + 1; // 86400秒 = 1天
            
            if (newDay != currentDay)
            {
                currentDay = newDay;
                OnNewDay();
            }
        }
        
        private void OnNewDay()
        {
            Debug.Log($"=== 第 {currentDay} 天开始 ===");
            
            if (currentDay > simulationDays)
            {
                EndSimulation();
            }
        }
        
        private void LogDailyStatus()
        {
            Debug.Log($"--- 第 {currentDay} 天状态报告 ---");
            
            foreach (var agent in agents)
            {
                if (agent != null)
                {
                    Debug.Log($"{agent.AgentName}: 健康={agent.Health:F1}, 士气={agent.Morale:F1}, 状态={agent.CurrentState}");
                }
            }
            
            if (weatherSystem != null)
            {
                var weather = weatherSystem.GetCurrentWeather();
                Debug.Log($"天气: {weather.description}, 温度: {weather.temperature:F1}°C");
            }
        }
        
        private void EndSimulation()
        {
            Debug.Log("=== 30天生存模拟结束 ===");
            
            CancelInvoke();
            
            // 生成最终报告
            GenerateFinalReport();
        }
        
        private void GenerateFinalReport()
        {
            Debug.Log("=== 最终生存报告 ===");
            
            int survivorCount = 0;
            float totalHealth = 0f;
            float totalMorale = 0f;
            
            foreach (var agent in agents)
            {
                if (agent != null && agent.Health > 0)
                {
                    survivorCount++;
                    totalHealth += agent.Health;
                    totalMorale += agent.Morale;
                    
                    Debug.Log($"幸存者: {agent.AgentName} - 健康: {agent.Health:F1}, 士气: {agent.Morale:F1}");
                }
            }
            
            float survivalRate = (float)survivorCount / agents.Count * 100f;
            float avgHealth = survivorCount > 0 ? totalHealth / survivorCount : 0f;
            float avgMorale = survivorCount > 0 ? totalMorale / survivorCount : 0f;
            
            Debug.Log($"生存率: {survivalRate:F1}% ({survivorCount}/{agents.Count})");
            Debug.Log($"平均健康: {avgHealth:F1}");
            Debug.Log($"平均士气: {avgMorale:F1}");
        }
        
        #region 公共接口
        
        public void SetTimeScale(float scale)
        {
            timeScale = Mathf.Max(0.1f, scale);
            if (weatherSystem != null)
            {
                weatherSystem.SetTimeScale(timeScale);
            }
        }
        
        public void PauseSimulation()
        {
            Time.timeScale = 0f;
        }
        
        public void ResumeSimulation()
        {
            Time.timeScale = 1f;
        }
        
        public void RestartSimulation()
        {
            // 清理现有代理
            foreach (var agent in agents)
            {
                if (agent != null)
                {
                    Destroy(agent.gameObject);
                }
            }
            agents.Clear();
            
            // 重新初始化
            currentDay = 1;
            startTime = Time.time;
            InitializeGame();
            StartSimulation();
        }
        
        #endregion
        
        private void OnGUI()
        {
            GUI.Box(new Rect(10, 140, 300, 100), "");
            GUI.Label(new Rect(20, 160, 280, 20), $"冰雪风暴生存小镇 - 第 {currentDay} 天");
            GUI.Label(new Rect(20, 180, 280, 20), $"AI代理数量: {agents.Count}");
            GUI.Label(new Rect(20, 200, 280, 20), $"时间缩放: {timeScale:F1}x");
            GUI.Label(new Rect(20, 220, 280, 20), $"目标: {simulationDays} 天生存挑战");
            
            // 控制按钮
            if (GUI.Button(new Rect(320, 160, 80, 25), "暂停"))
            {
                PauseSimulation();
            }
            
            if (GUI.Button(new Rect(410, 160, 80, 25), "继续"))
            {
                ResumeSimulation();
            }
            
            if (GUI.Button(new Rect(500, 160, 80, 25), "重启"))
            {
                RestartSimulation();
            }
        }
    }
    
    /// <summary>
    /// 代理配置数据结构
    /// </summary>
    [System.Serializable]
    public class AgentConfig
    {
        public string name;
        public string profession;
        public int age;
        public string personality;
    }
}
