using UnityEngine;
using UnityEngine.UI;
using IceStormSurvival.Services;
using System.Threading.Tasks;

namespace IceStormSurvival.Tools
{
    /// <summary>
    /// OpenAI API 测试工具
    /// 用于验证API配置和连接
    /// </summary>
    public class OpenAITester : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private InputField inputField;
        [SerializeField] private Text outputText;
        [SerializeField] private Button testButton;
        [SerializeField] private Text statusText;
        
        private ILLMService llmService;
        private bool isTesting = false;
        
        private void Start()
        {
            // 初始化LLM服务
            llmService = new EnhancedOpenAIService();
            
            // 设置UI
            if (testButton != null)
            {
                testButton.onClick.AddListener(TestAPI);
            }
            
            if (inputField != null)
            {
                inputField.text = "在这个冰雪风暴的末日世界中，我应该怎样生存？";
            }
            
            UpdateStatus();
        }
        
        private void UpdateStatus()
        {
            var config = ConfigurationManager.Instance;
            bool isConfigured = config.IsConfigured();
            
            if (statusText != null)
            {
                if (isConfigured)
                {
                    statusText.text = "✅ OpenAI API 已配置";
                    statusText.color = Color.green;
                }
                else
                {
                    statusText.text = "❌ OpenAI API 未配置";
                    statusText.color = Color.red;
                }
            }
            
            if (testButton != null)
            {
                testButton.interactable = isConfigured && !isTesting;
                testButton.GetComponentInChildren<Text>().text = isTesting ? "测试中..." : "测试API";
            }
        }
        
        public async void TestAPI()
        {
            if (isTesting) return;
            
            isTesting = true;
            UpdateStatus();
            
            try
            {
                string prompt = inputField != null ? inputField.text : "测试提示";
                
                if (outputText != null)
                {
                    outputText.text = "正在调用OpenAI API...";
                }
                
                // 测试不同类型的API调用
                await TestReflection(prompt);
                await TestResponse(prompt);
                await TestPlan(prompt);
                
                if (outputText != null)
                {
                    outputText.text += "\n\n✅ 所有API测试完成！";
                }
            }
            catch (System.Exception e)
            {
                if (outputText != null)
                {
                    outputText.text = $"❌ API测试失败:\n{e.Message}";
                }
                Debug.LogError($"API测试失败: {e.Message}");
            }
            finally
            {
                isTesting = false;
                UpdateStatus();
            }
        }
        
        private async Task TestReflection(string prompt)
        {
            try
            {
                string reflection = await llmService.GenerateReflection($"基于这个情况进行反思: {prompt}");
                
                if (outputText != null)
                {
                    outputText.text = $"🧠 反思测试:\n{reflection}\n\n";
                }
                
                Debug.Log($"反思测试成功: {reflection}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"反思测试失败: {e.Message}");
                throw;
            }
        }
        
        private async Task TestResponse(string prompt)
        {
            try
            {
                string response = await llmService.GenerateResponse(prompt);
                
                if (outputText != null)
                {
                    outputText.text += $"💬 回应测试:\n{response}\n\n";
                }
                
                Debug.Log($"回应测试成功: {response}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"回应测试失败: {e.Message}");
                throw;
            }
        }
        
        private async Task TestPlan(string prompt)
        {
            try
            {
                string plan = await llmService.GeneratePlan($"制定行动计划: {prompt}");
                
                if (outputText != null)
                {
                    outputText.text += $"📋 计划测试:\n{plan}";
                }
                
                Debug.Log($"计划测试成功: {plan}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"计划测试失败: {e.Message}");
                throw;
            }
        }
        
        // 快速测试按钮（在编辑器中使用）
        [ContextMenu("快速测试API")]
        private async void QuickTest()
        {
            if (Application.isPlaying)
            {
                await TestAPI();
            }
            else
            {
                Debug.LogWarning("请在运行时测试API");
            }
        }
        
        private void OnGUI()
        {
            // 如果没有UI组件，使用OnGUI创建简单界面
            if (testButton == null)
            {
                GUILayout.BeginArea(new Rect(10, 380, 400, 200));
                
                GUILayout.Label("OpenAI API 测试工具", EditorStyles.boldLabel);
                
                var config = ConfigurationManager.Instance;
                bool isConfigured = config.IsConfigured();
                
                GUI.color = isConfigured ? Color.green : Color.red;
                GUILayout.Label(isConfigured ? "✅ API已配置" : "❌ API未配置");
                GUI.color = Color.white;
                
                GUI.enabled = isConfigured && !isTesting;
                if (GUILayout.Button(isTesting ? "测试中..." : "测试API连接"))
                {
                    _ = TestAPI();
                }
                GUI.enabled = true;
                
                if (!isConfigured)
                {
                    if (GUILayout.Button("打开配置目录"))
                    {
                        Application.OpenURL(Application.persistentDataPath);
                    }
                    
                    if (GUILayout.Button("创建配置文件"))
                    {
                        ConfigurationManager.Instance.GetComponent<ConfigurationManager>()
                            .GetType()
                            .GetMethod("CreateSampleConfigFile", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            ?.Invoke(ConfigurationManager.Instance, null);
                    }
                }
                
                GUILayout.EndArea();
            }
        }
    }
}
