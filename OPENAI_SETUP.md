# OpenAI API 集成指南

本项目支持集成真实的OpenAI GPT模型，让AI代理拥有更真实、更丰富的对话和思考能力。

## 🔑 获取OpenAI API密钥

### 步骤1：注册OpenAI账户
1. 访问 [OpenAI官网](https://openai.com/)
2. 点击"Sign up"注册账户
3. 验证邮箱并完成账户设置

### 步骤2：获取API密钥
1. 登录后访问 [API Keys页面](https://platform.openai.com/api-keys)
2. 点击"Create new secret key"
3. 给密钥起个名字（如"Ice Storm Survival Town"）
4. **重要**：复制并安全保存API密钥（只会显示一次）

### 步骤3：充值账户（如需要）
- 新账户通常有免费额度
- 如需更多使用量，可在 [Billing页面](https://platform.openai.com/account/billing) 充值

## ⚙️ 配置项目

### 方法1：配置文件（推荐）

1. **运行项目一次**
   - 打开Unity项目
   - 运行MainScene
   - 如果未配置API，界面会显示配置提示

2. **创建配置文件**
   - 点击"创建配置文件"按钮，或
   - 手动在以下路径创建`config.json`文件：
     ```
     Windows: %USERPROFILE%/AppData/LocalLow/DefaultCompany/IceStormSurvivalTown/config.json
     macOS: ~/Library/Application Support/DefaultCompany/IceStormSurvivalTown/config.json
     Linux: ~/.config/unity3d/DefaultCompany/IceStormSurvivalTown/config.json
     ```

3. **编辑配置文件**
   ```json
   {
     "apiKey": "sk-your-actual-api-key-here",
     "model": "gpt-3.5-turbo",
     "temperature": 0.7,
     "maxTokens": 150,
     "requestTimeout": 30,
     "maxRetries": 3,
     "useProxy": false,
     "proxyUrl": ""
   }
   ```

4. **重启项目**
   - 配置生效后，代理将使用真实的OpenAI API

### 方法2：环境变量

设置以下环境变量：
```bash
export OPENAI_API_KEY="sk-your-actual-api-key-here"
export OPENAI_MODEL="gpt-3.5-turbo"
export OPENAI_TEMPERATURE="0.7"
export OPENAI_MAX_TOKENS="150"
```

**Windows (PowerShell):**
```powershell
$env:OPENAI_API_KEY="sk-your-actual-api-key-here"
```

**Windows (命令提示符):**
```cmd
set OPENAI_API_KEY=sk-your-actual-api-key-here
```

## 🎛️ 参数说明

### 核心设置
- **apiKey**: OpenAI API密钥（必需）
- **model**: 使用的模型名称
  - `gpt-3.5-turbo`: 成本低，速度快（推荐）
  - `gpt-4`: 质量更高，但成本更高
  - `gpt-4-turbo-preview`: 最新的GPT-4模型

### 生成参数
- **temperature**: 创造性控制 (0.0-2.0)
  - `0.0`: 非常确定性的回答
  - `0.7`: 平衡创造性和一致性（推荐）
  - `1.0`: 更有创造性
  
- **maxTokens**: 最大生成长度
  - `50-100`: 简短回应
  - `150`: 中等长度（推荐）
  - `300+`: 详细回应

### 高级设置
- **requestTimeout**: 请求超时时间（秒）
- **maxRetries**: 失败重试次数
- **useProxy**: 是否使用代理
- **proxyUrl**: 代理服务器地址

## 💰 成本控制

### 预算规划
- **gpt-3.5-turbo**: ~$0.002/1K tokens
- **gpt-4**: ~$0.03/1K tokens
- 一次对话通常消耗50-200 tokens

### 节省成本技巧
1. **使用gpt-3.5-turbo**: 性价比最高
2. **控制maxTokens**: 避免生成过长文本
3. **合理设置temperature**: 0.7已足够
4. **监控使用量**: 在OpenAI控制台查看

### 设置使用限制
在OpenAI控制台设置月度支出限制：
1. 访问 [Usage页面](https://platform.openai.com/account/usage)
2. 设置"Hard limit"防止超支

## 🛠️ 故障排除

### 常见错误

**API密钥无效**
```
错误: HTTP 401: Unauthorized
解决: 检查API密钥是否正确复制，是否过期
```

**配额超限**
```
错误: HTTP 429: Rate limit exceeded
解决: 等待一段时间或升级API套餐
```

**网络连接问题**
```
错误: 请求超时
解决: 检查网络连接，考虑使用代理
```

**余额不足**
```
错误: HTTP 402: Payment Required
解决: 在OpenAI控制台充值账户
```

### 调试步骤

1. **检查配置**
   ```csharp
   Debug.Log($"API配置状态: {ConfigurationManager.Instance.IsConfigured()}");
   ```

2. **查看详细日志**
   - Unity Console会显示API调用状态
   - 成功时显示token使用情况
   - 失败时显示具体错误信息

3. **测试API连接**
   - 项目包含自动重试机制
   - 失败时会自动降级到模拟服务

## 🔧 高级功能

### 角色定制化
项目支持为不同职业的代理定制专门的提示词：

```csharp
// 在EnhancedOpenAIService中
private string BuildReflectionSystemPrompt(string agentContext)
{
    return $@"你是一个{agentContext}...";
}
```

### 对话历史
系统会维护对话历史，让AI回应更连贯：
- 反思历史：帮助生成更深层的洞察
- 回应历史：保持角色一致性
- 规划历史：确保计划的连续性

### 智能重试
- 指数退避重试策略
- 自动错误恢复
- 备用回应系统

## 📊 监控和分析

### API使用统计
项目会记录：
- Token使用量
- 请求成功率
- 平均响应时间
- 错误类型分布

### 效果评估
观察AI代理使用真实LLM后的改进：
- 更自然的对话
- 更合理的反思
- 更灵活的规划
- 更丰富的个性表达

## 🌍 国际化支持

### 中国大陆用户
由于网络限制，可能需要：
1. 使用VPN或代理
2. 配置代理设置：
   ```json
   {
     "useProxy": true,
     "proxyUrl": "http://your-proxy:port"
   }
   ```

### API替代方案
也可以使用兼容OpenAI格式的其他服务：
- Azure OpenAI
- 通义千问
- 文心一言
- 本地部署的模型

只需修改`API_URL`即可。

## 🔐 安全注意事项

1. **保护API密钥**
   - 不要提交到代码仓库
   - 不要分享给他人
   - 定期轮换密钥

2. **访问控制**
   - 设置使用限制
   - 监控异常使用
   - 及时撤销泄露的密钥

3. **数据隐私**
   - OpenAI可能会记录API请求
   - 避免发送敏感信息
   - 了解数据使用政策

---

*配置完成后，你的AI代理将拥有真正的"智慧"！* 🧠✨
