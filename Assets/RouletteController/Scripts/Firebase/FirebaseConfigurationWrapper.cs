using System;
using System.Collections.Generic;
using Mode.Scripts.Localization;
using Mode.Scripts.Network;
using Newtonsoft.Json;

namespace Mode.Scripts.Firebase
{
    [Serializable]
    public class FirebaseConfigurationWrapper
    {
        [JsonProperty("dopparams")] public string additionalParametrs;
        [JsonProperty("serverUrl")] public string conversionLocation;
        [JsonProperty("analyticEndpoint")] public string analysesLocation;
        [JsonProperty("urls")] public Dictionary<string, LinkWrapper> urls;
        [JsonProperty("pushUrls")] public Dictionary<string, LinkWrapper> pushUrls;
        [JsonProperty("localizations")] public Dictionary<string, AppLanguage> localizations;
        [JsonProperty("settings")] public ProgressSettings appSettings;
        [JsonProperty("backButtonTextColor")] public string backButtonTextColor;
        [JsonProperty("backButtonPanelColor")] public string backButtonPanelColor;
        [JsonProperty("backButtonImageUrl")] public string backButtonImageUrl;
    }
}