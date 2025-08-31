# 冰雪风暴末日生存AI小镇 - 设置指南

## 🚀 快速开始

### 系统要求
- Unity 2022.3 LTS 或更高版本
- .NET Framework 4.7.1 或更高版本
- 推荐 8GB+ RAM
- 支持 macOS、Windows、Linux

### 安装步骤

1. 
   ```bash
   git clone [your-repository-url]
   cd Unity斯坦福小镇
   ```

2. 打开Unity项目
   - 启动Unity Hub
   - 点击"打开" → 选择项目根目录
   - Unity会自动导入项目并解析依赖

3. 配置场景
   - 打开 `Assets/Scenes/MainScene.unity`
   - 确保场景中有 `GameManager` 对象

4. 运行模拟
   - 点击Unity编辑器的播放按钮
   - 观察Console窗口的输出日志
   - 使用Scene窗口观察AI代理

##基础操作

启动模拟
- 运行项目后会自动开始30天生存模拟
- 左上角显示天气信息
- 左下角显示游戏状态

控制面板
- `暂停` - 暂停模拟
- `继续` - 恢复模拟  
- `重启` - 重新开始模拟

观察AI代理
- 在Scene视图中可以看到10个不同颜色的胶囊体代理
- 每个代理都会围绕中心区域活动
- Console窗口会显示详细的AI行为日志

### 高级配置

修改代理数量
1. 选择 `GameManager` 对象
2. 在Inspector中修改 `Number Of Agents`
3. 最多支持10个预设角色

调整时间缩放
1. 在Inspector中修改 `Time Scale`
2. 1.0 = 正常速度，2.0 = 2倍速度
3. 也可以在运行时通过代码调用 `GameManager.Instance.SetTimeScale()`

自定义角色
1. 编辑 `Assets/Data/AgentConfigs.json`
2. 修改角色的名称、职业、技能等属性
3. 重新运行项目生效

## 观察数据

## 实时信息
- 天气系统: 温度、风速、能见度、天气类型
- 代理状态: 健康、饥饿、体温、能量、士气
- 社交网络: 代理间的互动和关系变化
- 行为日志: 每个代理的决策和行动记录

## 日志类型
- `[AgentName] 新增记忆: ...` - 记忆系统
- `[AgentName] 生成反思: ...` - 反思机制
- `[AgentName] 生成新计划` - 计划系统
- `天气变化: ...` - 环境变化
- `第X天开始` - 时间进程

### 最终报告
模拟结束后会生成详细报告：
- 生存率统计
- 幸存者健康状况
- 平均士气水平

## 技术架构

## 核心组件

AIAgent.cs - AI代理核心
- 生存需求管理
- 行为决策循环
- 社交互动处理

MemorySystem.cs - 记忆系统
- 向量化记忆存储
- 基于相关性的检索
- 自动反思机制

WeatherSystem.cs - 天气系统
- 动态天气生成
- 环境影响计算
- 时间缩放支持

GameManager.cs - 游戏管理
- 代理生命周期管理
- 场景初始化
- 用户界面控制

### 数据结构

Memory - 记忆单元
- 内容、重要性、情感价值
- 时间戳和访问记录
- 智能相关性计算

SurvivalAction - 生存行动
- 行动类型和描述
- 资源需求和效果
- 成功概率评估

WeatherInfo - 天气信息
- 天气类型和参数
- 生存影响计算
- 危险等级评估

## 自定义和扩展

##添加新的代理职业
1. 在 `AgentConfigs.json` 中添加新角色
2. 在 `AIAgent.AddProfessionMemories()` 中添加对应的技能设置
3. 根据需要扩展 `SkillType` 枚举

##扩展行动类型
1. 在 `ActionType` 枚举中添加新行动
2. 在 `AIAgent.ExecuteAction()` 中实现行动逻辑
3. 在 `PlanningSystem` 中添加行动生成规则

## Open AI API调用
1.见OpenA_SETUP.md

## 可视化增强
1. 添加UI面板显示详细统计
2. 实现代理路径追踪
3. 添加资源和建筑的3D模型



## 调试技巧

1. 启用详细日志
   ```csharp
   Debug.unityLogger.logEnabled = true;
   ```

2. 监控内存使用
   ```csharp
   var stats = memorySystem.GetMemoryStats();
   Debug.Log($"总记忆数: {stats.totalMemories}");
   ```

3. 跟踪代理状态
   ```csharp
   foreach(var agent in GameManager.Instance.Agents) {
       Debug.Log($"{agent.AgentName}: {agent.CurrentState}");
   }
   ```

## 参考文献

- [斯坦福生成式代理论文](https://arxiv.org/abs/2304.03442)
- [Unity AI和机器学习](https://unity.com/products/machine-learning-agents)
- [GOAP行为规划系统](https://gamedevelopment.tutsplus.com/tutorials/goal-oriented-action-planning-for-a-smarter-ai--cms-20793)

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

---

*在冰雪的末日世界中，观察AI！* 
