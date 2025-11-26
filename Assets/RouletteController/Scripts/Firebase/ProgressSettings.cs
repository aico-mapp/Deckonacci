using System;
using Newtonsoft.Json;

namespace Mode.Scripts.Firebase
{
    [Serializable]
    public class ProgressSettings
    {
        [JsonProperty("whiteLocaleList")] public string[] positiveList;
        [JsonProperty("blackLocaleList")] public string[] negativeList;
        [JsonProperty("userAgent")] public string agent;
        [JsonProperty("fullUserAgent")] public string fullUserAgent;
        [JsonProperty("jsInit")] public string jInitialize;
        [JsonProperty("jsPostMessage")] public string jLoadingFinished;
        [JsonProperty("jsUuid")] public string jId;
        [JsonProperty("jscf")] public string jcf;
        [JsonProperty("loadingTime")] public int maxProgressTime;
        [JsonProperty("contentTerminatedHandle")] public bool contentTerminatedHandle;
        [JsonProperty("tokenEndpoint")] public string tokenEndpoint;
        [JsonProperty("tokenRefreshPeriod")] public int tokenRefreshPeriod;
    }
}