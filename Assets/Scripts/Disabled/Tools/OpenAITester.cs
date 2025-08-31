using UnityEngine;
using IceStormSurvival.Services;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IceStormSurvival.Tools
{
    /// <summary>
    /// OpenAI API 测试工具
    /// 用于验证API配置和连接
    /// </summary>
    public class OpenAITester : MonoBehaviour
    {
        [Header("测试设置")]
        [SerializeField] private string testPrompt = "在这个冰雪风暴的末日世界中，我应该怎样生存？";
        
        private ILLMService llmService;
        private bool isTesting = false;
        
        private void Start()
        {
            // 初始化LLM服务
            llmService = new EnhancedOpenAIService();
            
            Debug.Log("OpenAI测试工具已初始化，按T键测试API连接");
        }
        
        private void Update()
        {
            // 按T键测试API
            if (Input.GetKeyDown(KeyCode.T) && !isTesting)
            {
                _ = TestAPI(); // 启动异步任务但不等待
            }
        }
        
        public async Task TestAPI()
        {
            if (isTesting) return;
            
            isTesting = true;
            Debug.Log("开始测试OpenAI API...");
            
            try
            {
                // 测试不同类型的API调用
                await TestReflection(testPrompt);
                await TestResponse(testPrompt);
                await TestPlan(testPrompt);
                
                Debug.Log("✅ 所有API测试完成！");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ API测试失败: {e.Message}");
            }
            finally
            {
                isTesting = false;
            }
        }
        
        private async Task TestReflection(string prompt)
        {
            try
            {
                string reflection = await llmService.GenerateReflection($"基于这个情况进行反思: {prompt}");
                Debug.Log($"🧠 反思测试成功: {reflection}");
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
                Debug.Log($"💬 回应测试成功: {response}");
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
                Debug.Log($"📋 计划测试成功: {plan}");
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
            GUILayout.BeginArea(new Rect(10, 380, 400, 150));
            
#if UNITY_EDITOR
            GUILayout.Label("OpenAI API 测试工具", EditorStyles.boldLabel);
#else
            GUILayout.Label("OpenAI API 测试工具");
#endif
            
            var config = ConfigurationManager.Instance;
            bool isConfigured = config.IsConfigured();
            
            GUI.color = isConfigured ? Color.green : Color.red;
            GUILayout.Label(isConfigured ? "✅ API已配置" : "❌ API未配置");
            GUI.color = Color.white;
            
            GUI.enabled = isConfigured && !isTesting;
            if (GUILayout.Button(isTesting ? "测试中..." : "测试API连接 (或按T键)"))
            {
                _ = TestAPI(); // 启动异步任务但不等待
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
                    var configManager = ConfigurationManager.Instance.GetComponent<ConfigurationManager>();
                    var method = configManager.GetType().GetMethod("CreateSampleConfigFile", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    method?.Invoke(configManager, null);
                }
            }
            
            GUILayout.EndArea();
        }
    }
}
