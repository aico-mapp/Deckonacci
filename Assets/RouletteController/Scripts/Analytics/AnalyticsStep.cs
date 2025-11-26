using System;
using Mode.Scripts.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Mode.Scripts.Analytics
{
    [Serializable]
    [JsonObject("custom")]
    public class AnalyticsStep
    {
        [JsonProperty("event")] public string Source = "App";
        [JsonProperty("step")] public string Step;
        [JsonProperty("app_version")] public string Core = Versions.Product;
        [JsonProperty("app_name")] public string Name = Application.productName;
        [JsonProperty("platform")] public string Platform;
        [JsonProperty("start")] public long Start;
        [JsonProperty("stop")] public long Finish;
        [JsonProperty("result")] public long Difference;
        [JsonProperty("chain")] public string Guid;
        [JsonProperty("loading_number")] public int LoadingNumber;
        [JsonProperty("appsflyer_id")] public string AppsFlyerId;

        public AnalyticsStep(string step, string guid, int loadingNumber)
        {
            Step = step;
            Guid = guid;
            LoadingNumber = loadingNumber;
            Platform = Application.platform == RuntimePlatform.IPhonePlayer ? "iOS" : Application.platform.ToString();
            Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public void FinishStep()
        {
            Finish = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Difference = Finish - Start;
        }
    }
}