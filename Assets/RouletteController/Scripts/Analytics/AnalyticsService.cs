using Cysharp.Threading.Tasks;
using Firebase;
using Mode.Scripts.Data;
using Mode.Scripts.Http;
using Newtonsoft.Json;
using UnityEngine;

namespace Mode.Scripts.Analytics
{
    public class AnalyticsService
    {
        private readonly AnalyticsModel _analyticsModel;
        private readonly AppConfiguration _appConfiguration;
        private readonly HttpService _httpService;
        private readonly DataService _dataService;

        private bool IsInitialized => !string.IsNullOrEmpty(_analyticsModel.Location) ||
                                      !string.IsNullOrEmpty(_analyticsModel.Common);

        public AnalyticsService(AnalyticsModel analyticsModel,
            AppConfiguration appConfiguration,
            HttpService httpService,
            DataService dataService)
        {
            _analyticsModel = analyticsModel;
            _appConfiguration = appConfiguration;
            _httpService = httpService;
            _dataService = dataService;
        }
        
        public void Configure(string common, string location)
        {
            _analyticsModel.Location = location;
            _analyticsModel.Common = common;
            foreach (var step in _analyticsModel.DeferredSteps)
                PostStep(step);
            
            _analyticsModel.DeferredSteps.Clear();
        }

        public void StartStep(AnalyticsStepType stepType)
        {
            var stepName = _analyticsModel.StepNames[stepType];
            if (_analyticsModel.Steps.ContainsKey(stepName)) 
                return;
            
            _analyticsModel.AddStep(stepName);
        }

        public void FinishStep(AnalyticsStepType stepType)
        {
            var finishedAnalyticsStep = _analyticsModel.GetStepWithRemove(stepType);
            finishedAnalyticsStep.FinishStep();
            _analyticsModel.AnalyticsData.analyticsEventProperties.analyticsStep = finishedAnalyticsStep;
            var analyticsJson = JsonConvert.SerializeObject(_analyticsModel.AnalyticsData);
            if (IsInitialized) PostStep(analyticsJson);
            else _analyticsModel.DeferredSteps.Add(analyticsJson);
        }

        public void SetAgent(string agent) =>
            _analyticsModel.AnalyticsData.analyticsEventProperties.standartData.userAgent = agent;
        
        public async UniTask SendFinishProjectLoading(LoadedProjectData loadedProjectData, string uuid)
        {
            var conversionTag = _dataService.ConversionTag;
            var json = JsonUtility.ToJson(new AnalyticsLoadedAppData()
            {
                host = loadedProjectData.Host,
                project_name = _dataService.ProjectName,
                fb_token = _dataService.PushToken,
                fb_projectId = FirebaseApp.DefaultInstance.Options.ProjectId,
                fb_project_name = Application.identifier,
                appsflyer_id = conversionTag,
                ctag = conversionTag,
                platform = SystemInfo.operatingSystem,
                locale = PreciseLocale.GetLanguage() + '_' + PreciseLocale.GetRegion(),
                release_version = Versions.Product,
                push_receive = loadedProjectData.PushReceive,
                uuid = uuid
            });
            await _httpService.Send(GetAdata(), json);
        }

        private string GetAdata()
        {
            return _analyticsModel.Common + "/adata/" + _appConfiguration.appId + '/' + _analyticsModel.Chain;
        }
        
        private void PostStep(string json)
        {
            if (!string.IsNullOrEmpty(_analyticsModel.Location)) 
                _httpService.Send(_analyticsModel.Location, json).Forget();

            if (!string.IsNullOrEmpty(_analyticsModel.Common))
                _httpService.Send(_analyticsModel.Common + "/anlz", json).Forget();
        }
    }
}