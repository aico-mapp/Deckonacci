using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mode.Scripts.Data;
using Mode.Scripts.Http;
using Newtonsoft.Json;

namespace Mode.Scripts.Firebase
{
    public class RefreshService
    {
        private readonly DataService _dataService;
        private readonly HttpService _httpService;

        public RefreshService(
            DataService dataService,
            HttpService httpService)
        {
            _dataService = dataService;
            _httpService = httpService;
        }

        public async UniTask SendRefresh(string oldToken, string newToken)
        {
            var tokenData = new Dictionary<string, object>(10);
            tokenData.TryAdd("oldtoken", oldToken ?? string.Empty);
            tokenData.TryAdd("newtoken", newToken ?? string.Empty);
            tokenData.TryAdd("ctag", _dataService.ConversionTag ?? string.Empty);
            tokenData.TryAdd("project", _dataService.ProjectName ?? string.Empty);
            JsonConvert.SerializeObject(tokenData);
            var jsonData = JsonConvert.SerializeObject(tokenData);

            try
            {
                await _httpService.Send(_dataService.TokenEndpoint, jsonData);
            }
            catch (Exception)
            {
                return;
            }
            
            _dataService.SetToken(newToken);
        }
    }
}