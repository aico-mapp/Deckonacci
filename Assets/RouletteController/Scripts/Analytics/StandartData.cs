using System;
using Newtonsoft.Json;

namespace Mode.Scripts.Analytics
{
    [Serializable]
    [JsonObject("standart")]
    public class StandartData
    {
        [JsonProperty("user_agent")] public string userAgent;
    }
}