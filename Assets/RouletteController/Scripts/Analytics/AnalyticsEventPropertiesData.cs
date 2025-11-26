using System;
using Newtonsoft.Json;

namespace Mode.Scripts.Analytics
{
    [Serializable]
    public class AnalyticsEventPropertiesData
    {
        [JsonProperty("custom")] public AnalyticsStep analyticsStep;
        [JsonProperty("standart")] public StandartData standartData;
    }
}