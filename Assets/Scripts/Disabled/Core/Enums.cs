namespace IceStormSurvival.Core
{
    /// <summary>
    /// 代理状态枚举
    /// </summary>
    public enum AgentState
    {
        Idle,           // 空闲
        Working,        // 工作中
        Socializing,    // 社交中
        Resting,        // 休息中
        Emergency,      // 紧急状态
        Searching,      // 搜索中
        Planning,       // 规划中
        Dead            // 死亡
    }

    /// <summary>
    /// 行动类型枚举
    /// </summary>
    public enum ActionType
    {
        // 基础生存
        FindFood,       // 寻找食物
        FindWater,      // 寻找水源
        FindShelter,    // 寻找避难所
        BuildFire,      // 生火
        Rest,           // 休息
        Sleep,          // 睡觉

        // 建设活动
        BuildShelter,   // 建造避难所
        RepairShelter,  // 修理避难所
        GatherResources,// 收集资源
        CraftItems,     // 制作物品

        // 社交活动
        Socialize,      // 社交
        ShareInfo,      // 分享信息
        HelpOthers,     // 帮助他人
        Trade,          // 交易
        Argue,          // 争论
        Comfort,        // 安慰他人

        // 医疗活动
        TreatInjured,   // 治疗伤者
        CheckHealth,    // 检查健康

        // 搜索活动
        Scout,          // 侦察
        Hunt,           // 狩猎
        Forage,         // 觅食

        // 防御活动
        Guard,          // 警戒
        Patrol,         // 巡逻

        // 紧急活动
        Evacuate,       // 撤离
        RescueOthers,   // 救援他人
        CallForHelp     // 求助
    }

    /// <summary>
    /// 记忆类型枚举
    /// </summary>
    public enum MemoryType
    {
        Core,           // 核心记忆（身份、技能等）
        Observation,    // 观察记忆
        Action,         // 行动记忆
        Social,         // 社交记忆
        Skill,          // 技能记忆
        Reflection,     // 反思记忆
        Planning,       // 规划记忆
        Emergency,      // 紧急记忆
        Emotional       // 情感记忆
    }

    /// <summary>
    /// 需求类型枚举
    /// </summary>
    public enum NeedType
    {
        Hunger,         // 饥饿
        Thirst,         // 口渴
        Warmth,         // 温暖
        Rest,           // 休息
        Safety,         // 安全
        Social,         // 社交
        Purpose,        // 目标感
        Health,         // 健康
        Shelter         // 避难所
    }

    /// <summary>
    /// 情感类型枚举
    /// </summary>
    public enum EmotionType
    {
        Happy,          // 快乐
        Sad,            // 悲伤
        Angry,          // 愤怒
        Fear,           // 恐惧
        Anxiety,        // 焦虑
        Hope,           // 希望
        Despair,        // 绝望
        Gratitude,      // 感激
        Guilt,          // 内疚
        Pride,          // 骄傲
        Shame,          // 羞耻
        Love,           // 爱
        Hate,           // 恨
        Trust,          // 信任
        Distrust,       // 不信任
        Calm,           // 平静
        Excited,        // 兴奋
        Bored,          // 无聊
        Curious,        // 好奇
        Confused        // 困惑
    }

    /// <summary>
    /// 天气类型枚举
    /// </summary>
    public enum WeatherType
    {
        Clear,          // 晴朗
        Cloudy,         // 多云
        LightSnow,      // 小雪
        HeavySnow,      // 大雪
        Blizzard,       // 暴风雪
        Fog,            // 雾
        Wind,           // 大风
        ExtremeStorm    // 极端风暴
    }

    /// <summary>
    /// 资源类型枚举
    /// </summary>
    public enum ResourceType
    {
        Food,           // 食物
        Water,          // 水
        Wood,           // 木材
        Fuel,           // 燃料
        Medicine,       // 药物
        Tools,          // 工具
        Clothing,       // 衣物
        Blankets,       // 毯子
        Metal,          // 金属
        Plastic,        // 塑料
        Electronics,    // 电子设备
        Weapons,        // 武器
        Ammunition,     // 弹药
        Books,          // 书籍
        Seeds,          // 种子
        Rope,           // 绳索
        Containers,     // 容器
        FirstAid        // 急救用品
    }

    /// <summary>
    /// 建筑类型枚举
    /// </summary>
    public enum BuildingType
    {
        Shelter,        // 避难所
        Storage,        // 储藏室
        Kitchen,        // 厨房
        Workshop,       // 工作间
        Medical,        // 医疗室
        Greenhouse,     // 温室
        WatchTower,     // 瞭望塔
        Wall,           // 围墙
        Gate,           // 大门
        Well,           // 水井
        Fireplace,      // 壁炉
        Latrine         // 厕所
    }

    /// <summary>
    /// 关系类型枚举
    /// </summary>
    public enum RelationshipType
    {
        Stranger,       // 陌生人
        Acquaintance,   // 熟人
        Friend,         // 朋友
        BestFriend,     // 最好的朋友
        Rival,          // 对手
        Enemy,          // 敌人
        Family,         // 家人
        Lover,          // 恋人
        Leader,         // 领导者
        Follower,       // 追随者
        Mentor,         // 导师
        Student,        // 学生
        Colleague,      // 同事
        Subordinate,    // 下属
        Superior        // 上级
    }

    /// <summary>
    /// 技能类型枚举
    /// </summary>
    public enum SkillType
    {
        Medical,        // 医疗
        Construction,   // 建筑
        Hunting,        // 狩猎
        Cooking,        // 烹饪
        Leadership,     // 领导力
        Engineering,    // 工程
        Teaching,       // 教学
        Combat,         // 战斗
        Crafting,       // 制作
        Farming,        // 农业
        Electronics,    // 电子
        Mechanical,     // 机械
        Chemistry,      // 化学
        Psychology,     // 心理学
        Negotiation,    // 谈判
        Strategy,       // 策略
        Athletics,      // 体能
        Stealth,        // 潜行
        Survival,       // 生存
        FirstAid        // 急救
    }

    /// <summary>
    /// 优先级枚举
    /// </summary>
    public enum Priority
    {
        Critical = 5,   // 关键
        High = 4,       // 高
        Medium = 3,     // 中等
        Low = 2,        // 低
        Trivial = 1     // 微不足道
    }

    /// <summary>
    /// 事件类型枚举
    /// </summary>
    public enum EventType
    {
        // 环境事件
        WeatherChange,      // 天气变化
        ResourceDiscovered, // 发现资源
        AnimalSighting,     // 动物目击
        
        // 社交事件
        AgentMeeting,       // 代理会面
        Conversation,       // 对话
        Conflict,           // 冲突
        Cooperation,        // 合作
        
        // 生存事件
        Injury,             // 受伤
        Illness,            // 生病
        Death,              // 死亡
        Recovery,           // 康复
        
        // 建设事件
        ConstructionStart,  // 开始建设
        ConstructionComplete, // 完成建设
        BuildingCollapse,   // 建筑倒塌
        
        // 资源事件
        ResourceGathered,   // 收集资源
        ResourceConsumed,   // 消耗资源
        ResourceTraded,     // 交易资源
        ResourceLost,       // 丢失资源
        
        // 特殊事件
        Rescue,             // 救援
        Discovery,          // 发现
        Disaster,           // 灾难
        Miracle             // 奇迹
    }
}
