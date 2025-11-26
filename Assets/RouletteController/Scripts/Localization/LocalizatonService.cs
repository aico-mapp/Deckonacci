using System.Collections.Generic;
using Mode.Scripts.Data;
using Mode.Scripts.Firebase;
using Newtonsoft.Json;
using UnityEngine;

namespace Mode.Scripts.Localization
{
    public class LocalizationService
    {
        public AppLanguage Word { get; private set; }

        private readonly DataService _dataService;
        
        private const string DEFAULT_LANG_PATH = "localization_en";
        
        public LocalizationService(DataService dataService)
        {
            _dataService = dataService;
        }

        public void Init()
        {
            if (string.IsNullOrEmpty(_dataService.Configuration))
            {
                SetDefaultLang();
            }
            else
            {
                var firebaseConfigurationWrapper = JsonConvert.DeserializeObject<FirebaseConfigurationWrapper>(_dataService.Configuration);
                if (firebaseConfigurationWrapper is { localizations: not null })
                    SetFromDictionary(firebaseConfigurationWrapper.localizations);
                else
                    SetDefaultLang();
            }
        }

        public void SetFromDictionary(Dictionary<string, AppLanguage> localizations)
        {
            if (localizations.TryGetValue(PreciseLocale.GetLanguage(), out var language))
                Word = language;
            else
                SetDefaultLang();
        }

        private void SetDefaultLang()
        {
            var localizationFile = LoadLocalizationFile();
            Word = JsonConvert.DeserializeObject<AppLanguage>(localizationFile.ToString());
        }

        private TextAsset LoadLocalizationFile() => Resources.Load(DEFAULT_LANG_PATH) as TextAsset;
    }
}