using System;
using Newtonsoft.Json;

namespace Mode.Scripts.Localization
{
    [Serializable]
    public class AppLanguage
    {
        [JsonProperty("back")] public string BackText;
        [JsonProperty("internet_connection_lost")] public string ConnectionLostText;
        [JsonProperty("alias")] public string AliasLang;
    }
}