using System;
using System.IO;
using UnityEngine;

namespace IceStormSurvival.Services
{
    /// <summary>
    /// 配置管理器 - 管理API密钥和各种设置
    /// </summary>
    public class ConfigurationManager : MonoBehaviour
    {
        private static ConfigurationManager _instance;
        public static ConfigurationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("ConfigurationManager");
                    _instance = go.AddComponent<ConfigurationManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        [Header("OpenAI设置")]
        [SerializeField] private string configFileName = "config.json";
        
        private OpenAIConfig openAIConfig;
        private bool configLoaded = false;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                LoadConfiguration();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        public OpenAIConfig GetOpenAIConfig()
        {
            if (!configLoaded)
            {
                LoadConfiguration();
            }
            return openAIConfig;
        }
        
        private void LoadConfiguration()
        {
            try
            {
                // 1. 尝试从配置文件加载
                if (LoadFromConfigFile())
                {
                    Debug.Log("从配置文件加载OpenAI设置成功");
                    configLoaded = true;
                    return;
                }
                
                // 2. 尝试从环境变量加载
                if (LoadFromEnvironmentVariables())
                {
                    Debug.Log("从环境变量加载OpenAI设置成功");
                    configLoaded = true;
                    return;
                }
                
                // 3. 使用默认配置
                LoadDefaultConfiguration();
                Debug.LogWarning("使用默认OpenAI配置，请设置API密钥");
                configLoaded = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"加载配置失败: {e.Message}");
                LoadDefaultConfiguration();
                configLoaded = true;
            }
        }
        
        private bool LoadFromConfigFile()
        {
            string configPath = Path.Combine(Application.persistentDataPath, configFileName);
            
            // 如果没有配置文件，尝试从StreamingAssets加载
            if (!File.Exists(configPath))
            {
                string streamingConfigPath = Path.Combine(Application.streamingAssetsPath, configFileName);
                if (File.Exists(streamingConfigPath))
                {
                    // 复制到可写目录
                    File.Copy(streamingConfigPath, configPath);
                }
                else
                {
                    return false;
                }
            }
            
            try
            {
                string configJson = File.ReadAllText(configPath);
                openAIConfig = JsonUtility.FromJson<OpenAIConfig>(configJson);
                
                // 验证配置
                if (openAIConfig != null && !string.IsNullOrEmpty(openAIConfig.apiKey))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"读取配置文件失败: {e.Message}");
            }
            
            return false;
        }
        
        private bool LoadFromEnvironmentVariables()
        {
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            
            if (!string.IsNullOrEmpty(apiKey))
            {
                openAIConfig = new OpenAIConfig
                {
                    apiKey = apiKey,
                    model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-3.5-turbo",
                    temperature = float.TryParse(Environment.GetEnvironmentVariable("OPENAI_TEMPERATURE"), out float temp) ? temp : 0.7f,
                    maxTokens = int.TryParse(Environment.GetEnvironmentVariable("OPENAI_MAX_TOKENS"), out int tokens) ? tokens : 150
                };
                return true;
            }
            
            return false;
        }
        
        private void LoadDefaultConfiguration()
        {
            openAIConfig = new OpenAIConfig
            {
                apiKey = "", // 需要用户设置
                model = "gpt-3.5-turbo",
                temperature = 0.7f,
                maxTokens = 150
            };
        }
        
        public void SaveConfiguration()
        {
            try
            {
                string configPath = Path.Combine(Application.persistentDataPath, configFileName);
                string configJson = JsonUtility.ToJson(openAIConfig, true);
                File.WriteAllText(configPath, configJson);
                Debug.Log($"配置已保存到: {configPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"保存配置失败: {e.Message}");
            }
        }
        
        public void SetAPIKey(string apiKey)
        {
            if (openAIConfig == null)
            {
                LoadDefaultConfiguration();
            }
            
            openAIConfig.apiKey = apiKey;
            SaveConfiguration();
        }
        
        public bool IsConfigured()
        {
            return openAIConfig != null && !string.IsNullOrEmpty(openAIConfig.apiKey);
        }
        
        // 在编辑器中显示配置界面
        private void OnGUI()
        {
            if (!IsConfigured())
            {
                // 显示配置提示
                GUI.Box(new Rect(10, 250, 400, 120), "");
                GUI.Label(new Rect(20, 270, 380, 20), "OpenAI API 配置");
                GUI.Label(new Rect(20, 290, 380, 20), "请设置API密钥以启用真实AI对话:");
                
                if (GUI.Button(new Rect(20, 310, 120, 25), "打开配置目录"))
                {
                    Application.OpenURL(Application.persistentDataPath);
                }
                
                if (GUI.Button(new Rect(150, 310, 120, 25), "创建配置文件"))
                {
                    CreateSampleConfigFile();
                }
                
                GUI.Label(new Rect(20, 340, 280, 20), "配置文件路径:");
                GUI.Label(new Rect(20, 355, 380, 20), Application.persistentDataPath);
            }
        }
        
        private void CreateSampleConfigFile()
        {
            try
            {
                string configPath = Path.Combine(Application.persistentDataPath, configFileName);
                var sampleConfig = new OpenAIConfig
                {
                    apiKey = "your-openai-api-key-here",
                    model = "gpt-3.5-turbo",
                    temperature = 0.7f,
                    maxTokens = 150
                };
                
                string configJson = JsonUtility.ToJson(sampleConfig, true);
                File.WriteAllText(configPath, configJson);
                
                Debug.Log($"示例配置文件已创建: {configPath}");
                Debug.Log("请编辑config.json文件并填入你的OpenAI API密钥");
            }
            catch (Exception e)
            {
                Debug.LogError($"创建配置文件失败: {e.Message}");
            }
        }
    }

}
