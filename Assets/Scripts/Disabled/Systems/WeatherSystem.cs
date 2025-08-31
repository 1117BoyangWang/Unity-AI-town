using System;
using UnityEngine;
using IceStormSurvival.Core;

namespace IceStormSurvival.Systems
{
    /// <summary>
    /// 天气系统 - 模拟冰雪风暴末日环境
    /// </summary>
    public class WeatherSystem : MonoBehaviour
    {
        [Header("天气设置")]
        [SerializeField] private float weatherChangeInterval = 30f; // 天气变化间隔（秒）
        [SerializeField] private float baseTemperature = -15f; // 基础温度
        [SerializeField] private float temperatureVariation = 10f; // 温度变化范围
        
        [Header("当前天气")]
        [SerializeField] private WeatherInfo currentWeather;
        
        private float lastWeatherChange;
        private float timeScale = 1f; // 时间缩放，用于加速模拟
        
        public WeatherInfo GetCurrentWeather() => currentWeather;
        public event System.Action<WeatherInfo> OnWeatherChanged;
        
        private void Awake()
        {
            InitializeWeather();
        }
        
        private void Start()
        {
            InvokeRepeating(nameof(UpdateWeather), weatherChangeInterval, weatherChangeInterval);
        }
        
        private void InitializeWeather()
        {
            currentWeather = new WeatherInfo
            {
                weatherType = WeatherType.LightSnow,
                temperature = baseTemperature,
                windSpeed = 15f,
                visibility = 0.7f,
                humidity = 0.8f,
                description = "轻微的雪花飘落，天气阴冷",
                severity = 0.3f,
                duration = 2f,
                startTime = DateTime.Now
            };
        }
        
        private void UpdateWeather()
        {
            var newWeather = GenerateNewWeather();
            
            if (newWeather.weatherType != currentWeather.weatherType || 
                Mathf.Abs(newWeather.temperature - currentWeather.temperature) > 2f)
            {
                currentWeather = newWeather;
                OnWeatherChanged?.Invoke(currentWeather);
                
                Debug.Log($"天气变化: {currentWeather.description}, 温度: {currentWeather.temperature:F1}°C");
            }
        }
        
        private WeatherInfo GenerateNewWeather()
        {
            // 基于当前天气生成新天气，保持一定的连续性
            WeatherInfo newWeather = new WeatherInfo();
            
            // 生成天气类型
            newWeather.weatherType = GenerateWeatherType();
            
            // 基于天气类型设置参数
            SetWeatherParameters(newWeather);
            
            return newWeather;
        }
        
        private WeatherType GenerateWeatherType()
        {
            // 冰雪风暴环境中，恶劣天气较多
            float random = UnityEngine.Random.Range(0f, 1f);
            
            // 基于当前天气调整概率
            switch (currentWeather.weatherType)
            {
                case WeatherType.Clear:
                    if (random < 0.3f) return WeatherType.Clear;
                    if (random < 0.6f) return WeatherType.Cloudy;
                    if (random < 0.9f) return WeatherType.LightSnow;
                    return WeatherType.HeavySnow;
                    
                case WeatherType.Cloudy:
                    if (random < 0.2f) return WeatherType.Clear;
                    if (random < 0.4f) return WeatherType.Cloudy;
                    if (random < 0.7f) return WeatherType.LightSnow;
                    if (random < 0.9f) return WeatherType.HeavySnow;
                    return WeatherType.Blizzard;
                    
                case WeatherType.LightSnow:
                    if (random < 0.1f) return WeatherType.Cloudy;
                    if (random < 0.4f) return WeatherType.LightSnow;
                    if (random < 0.7f) return WeatherType.HeavySnow;
                    if (random < 0.9f) return WeatherType.Blizzard;
                    return WeatherType.Fog;
                    
                case WeatherType.HeavySnow:
                    if (random < 0.2f) return WeatherType.LightSnow;
                    if (random < 0.5f) return WeatherType.HeavySnow;
                    if (random < 0.8f) return WeatherType.Blizzard;
                    if (random < 0.95f) return WeatherType.Wind;
                    return WeatherType.ExtremeStorm;
                    
                case WeatherType.Blizzard:
                    if (random < 0.3f) return WeatherType.HeavySnow;
                    if (random < 0.6f) return WeatherType.Blizzard;
                    if (random < 0.9f) return WeatherType.Wind;
                    return WeatherType.ExtremeStorm;
                    
                default:
                    return WeatherType.LightSnow;
            }
        }
        
        private void SetWeatherParameters(WeatherInfo weather)
        {
            weather.startTime = DateTime.Now;
            
            switch (weather.weatherType)
            {
                case WeatherType.Clear:
                    weather.temperature = baseTemperature + UnityEngine.Random.Range(0f, 5f);
                    weather.windSpeed = UnityEngine.Random.Range(5f, 15f);
                    weather.visibility = UnityEngine.Random.Range(0.9f, 1f);
                    weather.humidity = UnityEngine.Random.Range(0.4f, 0.6f);
                    weather.severity = UnityEngine.Random.Range(0.1f, 0.3f);
                    weather.description = "天空相对晴朗，但依然寒冷";
                    weather.duration = UnityEngine.Random.Range(1f, 3f);
                    break;
                    
                case WeatherType.Cloudy:
                    weather.temperature = baseTemperature + UnityEngine.Random.Range(-2f, 3f);
                    weather.windSpeed = UnityEngine.Random.Range(10f, 20f);
                    weather.visibility = UnityEngine.Random.Range(0.7f, 0.9f);
                    weather.humidity = UnityEngine.Random.Range(0.6f, 0.8f);
                    weather.severity = UnityEngine.Random.Range(0.2f, 0.4f);
                    weather.description = "阴云密布，空气中弥漫着寒意";
                    weather.duration = UnityEngine.Random.Range(2f, 4f);
                    break;
                    
                case WeatherType.LightSnow:
                    weather.temperature = baseTemperature + UnityEngine.Random.Range(-3f, 2f);
                    weather.windSpeed = UnityEngine.Random.Range(15f, 25f);
                    weather.visibility = UnityEngine.Random.Range(0.6f, 0.8f);
                    weather.humidity = UnityEngine.Random.Range(0.7f, 0.9f);
                    weather.severity = UnityEngine.Random.Range(0.3f, 0.5f);
                    weather.description = "雪花轻柔地飘落，地面开始积雪";
                    weather.duration = UnityEngine.Random.Range(2f, 6f);
                    break;
                    
                case WeatherType.HeavySnow:
                    weather.temperature = baseTemperature + UnityEngine.Random.Range(-5f, 0f);
                    weather.windSpeed = UnityEngine.Random.Range(25f, 40f);
                    weather.visibility = UnityEngine.Random.Range(0.3f, 0.6f);
                    weather.humidity = UnityEngine.Random.Range(0.8f, 0.95f);
                    weather.severity = UnityEngine.Random.Range(0.5f, 0.7f);
                    weather.description = "大雪纷飞，视线严重受阻";
                    weather.duration = UnityEngine.Random.Range(3f, 8f);
                    break;
                    
                case WeatherType.Blizzard:
                    weather.temperature = baseTemperature + UnityEngine.Random.Range(-8f, -2f);
                    weather.windSpeed = UnityEngine.Random.Range(50f, 80f);
                    weather.visibility = UnityEngine.Random.Range(0.1f, 0.3f);
                    weather.humidity = UnityEngine.Random.Range(0.9f, 1f);
                    weather.severity = UnityEngine.Random.Range(0.7f, 0.9f);
                    weather.description = "猛烈的暴风雪肆虐，极其危险";
                    weather.duration = UnityEngine.Random.Range(1f, 4f);
                    break;
                    
                case WeatherType.Fog:
                    weather.temperature = baseTemperature + UnityEngine.Random.Range(-2f, 2f);
                    weather.windSpeed = UnityEngine.Random.Range(5f, 15f);
                    weather.visibility = UnityEngine.Random.Range(0.2f, 0.5f);
                    weather.humidity = UnityEngine.Random.Range(0.9f, 1f);
                    weather.severity = UnityEngine.Random.Range(0.4f, 0.6f);
                    weather.description = "浓雾弥漫，能见度极低";
                    weather.duration = UnityEngine.Random.Range(2f, 5f);
                    break;
                    
                case WeatherType.Wind:
                    weather.temperature = baseTemperature + UnityEngine.Random.Range(-5f, -1f);
                    weather.windSpeed = UnityEngine.Random.Range(60f, 100f);
                    weather.visibility = UnityEngine.Random.Range(0.4f, 0.7f);
                    weather.humidity = UnityEngine.Random.Range(0.6f, 0.8f);
                    weather.severity = UnityEngine.Random.Range(0.6f, 0.8f);
                    weather.description = "强风呼啸，卷起大量雪花";
                    weather.duration = UnityEngine.Random.Range(1f, 3f);
                    break;
                    
                case WeatherType.ExtremeStorm:
                    weather.temperature = baseTemperature + UnityEngine.Random.Range(-12f, -5f);
                    weather.windSpeed = UnityEngine.Random.Range(100f, 150f);
                    weather.visibility = UnityEngine.Random.Range(0.05f, 0.2f);
                    weather.humidity = UnityEngine.Random.Range(0.95f, 1f);
                    weather.severity = UnityEngine.Random.Range(0.9f, 1f);
                    weather.description = "史无前例的极端风暴，生存面临严峻挑战";
                    weather.duration = UnityEngine.Random.Range(0.5f, 2f);
                    break;
            }
        }
        
        /// <summary>
        /// 获取天气对生存的影响
        /// </summary>
        public WeatherSurvivalImpact GetSurvivalImpact()
        {
            return new WeatherSurvivalImpact
            {
                warmthDrainMultiplier = CalculateWarmthDrain(),
                visibilityReduction = 1f - currentWeather.visibility,
                movementSpeedMultiplier = CalculateMovementSpeed(),
                resourceGatheringMultiplier = CalculateResourceGathering(),
                shelterEffectiveness = CalculateShelterEffectiveness(),
                dangerLevel = currentWeather.severity
            };
        }
        
        private float CalculateWarmthDrain()
        {
            float tempFactor = Mathf.Max(0.5f, (10f - currentWeather.temperature) / 25f);
            float windFactor = 1f + (currentWeather.windSpeed / 100f);
            return tempFactor * windFactor;
        }
        
        private float CalculateMovementSpeed()
        {
            float visibilityFactor = currentWeather.visibility;
            float windFactor = Mathf.Max(0.3f, 1f - (currentWeather.windSpeed / 150f));
            return visibilityFactor * windFactor;
        }
        
        private float CalculateResourceGathering()
        {
            if (currentWeather.severity > 0.7f)
                return 0.2f; // 极端天气下几乎无法收集资源
            
            return Mathf.Max(0.3f, 1f - currentWeather.severity);
        }
        
        private float CalculateShelterEffectiveness()
        {
            // 恶劣天气下避难所更重要
            return Mathf.Min(2f, 1f + currentWeather.severity);
        }
        
        /// <summary>
        /// 设置时间缩放（用于加速模拟）
        /// </summary>
        public void SetTimeScale(float scale)
        {
            timeScale = Mathf.Max(0.1f, scale);
            CancelInvoke(nameof(UpdateWeather));
            InvokeRepeating(nameof(UpdateWeather), weatherChangeInterval / timeScale, weatherChangeInterval / timeScale);
        }
        
        /// <summary>
        /// 手动触发天气变化（用于测试）
        /// </summary>
        public void ForceWeatherChange()
        {
            UpdateWeather();
        }
        
        private void OnGUI()
        {
            if (currentWeather != null)
            {
                GUI.Box(new Rect(10, 10, 300, 120), "");
                GUI.Label(new Rect(20, 30, 280, 20), $"天气: {currentWeather.weatherType}");
                GUI.Label(new Rect(20, 50, 280, 20), $"描述: {currentWeather.description}");
                GUI.Label(new Rect(20, 70, 280, 20), $"温度: {currentWeather.temperature:F1}°C");
                GUI.Label(new Rect(20, 90, 280, 20), $"风速: {currentWeather.windSpeed:F1} km/h");
                GUI.Label(new Rect(20, 110, 280, 20), $"能见度: {currentWeather.visibility * 100:F0}%");
            }
        }
    }
    
    /// <summary>
    /// 天气对生存的影响
    /// </summary>
    public class WeatherSurvivalImpact
    {
        public float warmthDrainMultiplier;      // 体温流失倍数
        public float visibilityReduction;        // 视线降低
        public float movementSpeedMultiplier;    // 移动速度倍数
        public float resourceGatheringMultiplier; // 资源收集倍数
        public float shelterEffectiveness;       // 避难所有效性
        public float dangerLevel;                // 危险程度
    }
}
