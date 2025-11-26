using System;
using Newtonsoft.Json;

namespace Mode.Scripts.Analytics
{
    [Serializable]
    public class AnalyticsData
    {
        [JsonProperty("event_properties")] public AnalyticsEventPropertiesData analyticsEventProperties;
    }
}