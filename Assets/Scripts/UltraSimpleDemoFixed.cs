using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 超简单AI小镇演示 - 修复版本
/// 包含对话功能
/// </summary>
public class UltraSimpleDemoFixed : MonoBehaviour
{
    [System.Serializable]
    public class SimpleAgent
    {
        public string name;
        public float health;
        public string activity;
        public GameObject gameObject;
        public bool isAlive;
        public string personality;
        public string profession;
        public float socialTimer;
        public string lastDialogue;
        
        public SimpleAgent(string agentName, string agentPersonality, string agentProfession)
        {
            name = agentName;
            health = 100f;
            activity = "休息";
            isAlive = true;
            personality = agentPersonality;
            profession = agentProfession;
            socialTimer = 0f;
            lastDialogue = "";
        }
    }
    
    [Header("设置")]
    public int agentCount = 3;
    
    private SimpleAgent[] agents;
    private float timer = 0f;
    private float statusTimer = 0f;
    
    void Start()
    {
        Debug.Log("🏘️ 超简单AI小镇启动！(对话版)");
        CreateAgents();
        StartCoroutine(SimulationLoop());
    }
    
    void CreateAgents()
    {
        string[] names = { "小明", "小红", "小李" };
        string[] personalities = { "乐观开朗", "细心谨慎", "聪明机智" };
        string[] professions = { "医生", "工程师", "教师" };
        Color[] colors = { Color.red, Color.blue, Color.green };
        
        agents = new SimpleAgent[agentCount];
        
        for (int i = 0; i < agentCount; i++)
        {
            // 创建代理数据
            agents[i] = new SimpleAgent(names[i], personalities[i], professions[i]);
            
            // 创建3D对象
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = names[i];
            cube.transform.position = new Vector3(i * 2f, 0.5f, 0);
            cube.GetComponent<Renderer>().material.color = colors[i];
            
            agents[i].gameObject = cube;
            
            Debug.Log($"✅ 创建代理: {names[i]} ({professions[i]}, {personalities[i]})");
        }
    }
    
    IEnumerator SimulationLoop()
    {
        while (true)
        {
            timer += 1f;
            
            // 更新每个代理
            for (int i = 0; i < agents.Length; i++)
            {
                if (agents[i].isAlive)
                {
                    UpdateAgent(agents[i]);
                    agents[i].socialTimer += 1f;
                }
            }
            
            // 触发对话（每5秒检查一次）
            if (timer % 5f == 0)
            {
                CheckForDialogue();
            }
            
            yield return new WaitForSeconds(1f);
        }
    }
    
    void UpdateAgent(SimpleAgent agent)
    {
        // 随机活动
        float rand = Random.value;
        
        if (rand < 0.3f)
        {
            agent.activity = "寻找食物";
            agent.health += Random.Range(5f, 10f);
        }
        else if (rand < 0.6f)
        {
            agent.activity = "休息";
            agent.health += Random.Range(2f, 5f);
        }
        else
        {
            agent.activity = "工作";
            agent.health -= Random.Range(1f, 3f);
        }
        
        // 限制健康值
        agent.health = Mathf.Clamp(agent.health, 0f, 100f);
        
        // 检查生存状态
        if (agent.health <= 0f)
        {
            agent.isAlive = false;
            agent.activity = "死亡";
            agent.gameObject.GetComponent<Renderer>().material.color = Color.black;
        }
        
        Debug.Log($"🤖 {agent.name}: {agent.activity} (健康: {agent.health:F0}%)");
    }
    
    void CheckForDialogue()
    {
        // 随机选择两个活着的代理进行对话
        var aliveAgents = new List<SimpleAgent>();
        for (int i = 0; i < agents.Length; i++)
        {
            if (agents[i].isAlive)
            {
                aliveAgents.Add(agents[i]);
            }
        }
        
        if (aliveAgents.Count >= 2)
        {
            var agent1 = aliveAgents[Random.Range(0, aliveAgents.Count)];
            var agent2 = aliveAgents[Random.Range(0, aliveAgents.Count)];
            
            // 确保不是同一个代理
            while (agent2 == agent1 && aliveAgents.Count > 1)
            {
                agent2 = aliveAgents[Random.Range(0, aliveAgents.Count)];
            }
            
            if (agent1 != agent2)
            {
                CreateDialogue(agent1, agent2);
            }
        }
    }
    
    void CreateDialogue(SimpleAgent speaker, SimpleAgent listener)
    {
        string dialogue = GetDialogue(speaker, listener);
        speaker.lastDialogue = dialogue;
        
        Debug.Log($"💬 {speaker.name} 对 {listener.name} 说: \"{dialogue}\"");
        
        // 监听者的回应
        string response = GetResponse(listener, speaker, dialogue);
        listener.lastDialogue = response;
        
        Debug.Log($"💭 {listener.name} 回应: \"{response}\"");
        
        // 重置社交计时器
        speaker.socialTimer = 0f;
        listener.socialTimer = 0f;
    }
    
    string GetDialogue(SimpleAgent speaker, SimpleAgent listener)
    {
        string[] greetings = { "你好", "嗨", "最近怎么样" };
        string[] healthTopics = { "你看起来有点累", "需要休息一下吗", "身体还好吧" };
        string[] workTopics = { "工作很辛苦呢", "今天的任务完成了吗", "我们一起努力吧" };
        string[] encouragement = { "加油！", "我们能挺过去的", "团结就是力量" };
        
        if (speaker.health < 30)
        {
            return healthTopics[Random.Range(0, healthTopics.Length)];
        }
        else if (speaker.activity == "工作")
        {
            return workTopics[Random.Range(0, workTopics.Length)];
        }
        else if (listener.health < 50)
        {
            return encouragement[Random.Range(0, encouragement.Length)];
        }
        else
        {
            return greetings[Random.Range(0, greetings.Length)];
        }
    }
    
    string GetResponse(SimpleAgent responder, SimpleAgent original, string originalMessage)
    {
        string[] positiveResponses = { "谢谢关心", "我很好", "一起加油" };
        string[] concernedResponses = { "你也要照顾好自己", "我们互相帮助", "团结一致" };
        string[] professionalResponses = { "作为医生，我建议多休息", "作为工程师，我觉得效率很重要", "作为教师，我认为学习永无止境" };
        
        if (originalMessage.Contains("累") || originalMessage.Contains("休息"))
        {
            return concernedResponses[Random.Range(0, concernedResponses.Length)];
        }
        else if (originalMessage.Contains("工作") || originalMessage.Contains("任务"))
        {
            return professionalResponses[Random.Range(0, professionalResponses.Length)];
        }
        else
        {
            return positiveResponses[Random.Range(0, positiveResponses.Length)];
        }
    }
    
    void Update()
    {
        // 按键控制
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowAgentStatus();
        }
        
        // 定时显示状态（每10秒自动显示一次）
        statusTimer += Time.deltaTime;
        if (statusTimer >= 10f)
        {
            ShowAgentStatus();
            statusTimer = 0f;
        }
    }
    
    void ShowAgentStatus()
    {
        Debug.Log("=== 🏘️ AI小镇状态报告 ===");
        for (int i = 0; i < agents.Length; i++)
        {
            string status = $"📊 {agents[i].name} ({agents[i].profession}, {agents[i].personality}):\n";
            status += $"   健康: {agents[i].health:F0}% | 活动: {agents[i].activity}";
            if (!string.IsNullOrEmpty(agents[i].lastDialogue))
            {
                status += $"\n   💬 最近说话: \"{agents[i].lastDialogue}\"";
            }
            Debug.Log(status);
        }
    }
    
    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 250, 130), "AI小镇演示 (对话版)");
        GUI.Label(new Rect(20, 30, 180, 20), $"时间: {timer:F0}秒");
        GUI.Label(new Rect(20, 50, 180, 20), $"代理数: {agentCount}");
        GUI.Label(new Rect(20, 70, 180, 20), "按空格键或点击按钮");
        
        if (GUI.Button(new Rect(20, 90, 100, 25), "查看状态"))
        {
            ShowAgentStatus();
        }
        
        if (GUI.Button(new Rect(130, 90, 100, 25), "重新开始"))
        {
            RestartDemo();
        }
    }
    
    void RestartDemo()
    {
        // 清理现有代理
        if (agents != null)
        {
            for (int i = 0; i < agents.Length; i++)
            {
                if (agents[i] != null && agents[i].gameObject != null)
                {
                    DestroyImmediate(agents[i].gameObject);
                }
            }
        }
        
        // 重新创建
        CreateAgents();
        Debug.Log("🔄 AI小镇已重新启动！");
    }
}
