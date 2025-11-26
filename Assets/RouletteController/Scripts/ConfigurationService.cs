using System.Linq;
using Mode.Scripts.Data;
using Mode.Scripts.Firebase;
using Mode.Scripts.Localization;
using Mode.Scripts.UI;
using Newtonsoft.Json;
using UnityEngine;

namespace Mode.Scripts
{
    public class ConfigurationService
    {
        private readonly DataService _dataService;
        private readonly LocalizationService _localizationService;
        private readonly LoadingScreenController _loadingScreenController;

        public ConfigurationService(
            DataService dataService,
            LocalizationService localizationService,
            LoadingScreenController loadingScreenController)
        {
            _dataService = dataService;
            _localizationService = localizationService;
            _loadingScreenController = loadingScreenController;
        }

        public bool LoadConfiguration()
        {
            UpdateConfiguration();
            return ValidateLists();
        }

        public void UpdateConfiguration()
        {
            if (_dataService.Wrapper != null) return;
            
            _dataService.Wrapper = JsonConvert.DeserializeObject<FirebaseConfigurationWrapper>(_dataService.Configuration); 
            _localizationService.SetFromDictionary(_dataService.Wrapper.localizations);
            _loadingScreenController.LocalizeScreen(_localizationService.Word);
        }

        private bool ValidateLists()
        {
            var whiteList = _dataService.Wrapper.appSettings.positiveList;
            var blackList = _dataService.Wrapper.appSettings.negativeList;
            var locale = PreciseLocale.GetRegion();
            if (whiteList != null && whiteList.Length != 0) return whiteList.Contains(locale);
            if (blackList != null && blackList.Length != 0) return !blackList.Contains(locale);
            return true;
        }
    }
}