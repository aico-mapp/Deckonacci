using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mode.Scripts.Analytics;
using Mode.Scripts.Data;
using Mode.Scripts.Http;
using Newtonsoft.Json;
using UnityEngine;

namespace Mode.Scripts.Services
{
    public class ConversionService
    {
        private readonly DataService _dataService;
        private readonly HttpService _httpService;
        private readonly AppConfiguration _appConfiguration;
        private readonly AnalyticsModel _analyticsModel;

        public ConversionService(DataService dataService, HttpService httpService, AppConfiguration appConfiguration, AnalyticsModel analyticsModel)
        {
            _dataService = dataService;
            _httpService = httpService;
            _appConfiguration = appConfiguration;
            _analyticsModel = analyticsModel;
        }

        public async UniTask SendConversionData()
        {
            var conversionData = CollectConversionData();
            await _httpService.Send(_dataService.Wrapper.conversionLocation + "/cd", conversionData);
        }

        private string CollectConversionData()
        {
            var conversionDataDictionary = new Dictionary<string, object>(10);
            conversionDataDictionary.TryAdd("bundleIdentifier", Application.identifier);
#if UNITY_IOS
            conversionDataDictionary.TryAdd("appleAppID", _appConfiguration.appId);
#endif
            conversionDataDictionary.TryAdd("os", SystemInfo.operatingSystem);
            conversionDataDictionary.TryAdd("appsflyer_id", _dataService.ConversionTag);
            conversionDataDictionary.TryAdd("is_first_launch", _dataService.LoadingCounter == 1);
            conversionDataDictionary.TryAdd("af_status", "light");
            conversionDataDictionary.TryAdd("ctag", _dataService.ConversionTag);
            conversionDataDictionary.TryAdd("localName", PreciseLocale.GetLanguage() + '_' + PreciseLocale.GetRegion());
            conversionDataDictionary.TryAdd("wv_version", Versions.Product);
            conversionDataDictionary.TryAdd("chain", _analyticsModel.Chain);
            return JsonConvert.SerializeObject(conversionDataDictionary);
        }
    }
}