using System.Collections.Generic;
using System.Text;
using Firebase;
using Mode.Scripts.Analytics;
using Mode.Scripts.Data;
using Mode.Scripts.Services;
using UnityEngine;

namespace Mode.Scripts.Message
{
    public class PSMService
    {
        private readonly LoadedProjectData _loadedProjectData;
        private readonly AnalyticsService _analyticsService;
        private readonly StarterModel _starterModel;
        private readonly DataService _dataService;

        public PSMService(LoadedProjectData loadedProjectData,
            AnalyticsService analyticsService,
            StarterModel starterModel,
            DataService dataService)
        {
            _loadedProjectData = loadedProjectData;
            _analyticsService = analyticsService;
            _starterModel = starterModel;
            _dataService = dataService;
        }
        
        public void CreatePSM()
        {
            SendPSM(BuildPSM(_loadedProjectData.PushReceive), _loadedProjectData.Host);
            _analyticsService.FinishStep(AnalyticsStepType.ProjectLoading);
            _analyticsService.FinishStep(AnalyticsStepType.TotalLoading);
        }
        
        private Dictionary<string, string> BuildPSM(string pushReceive)
        {
            return new Dictionary<string, string>{
                {LoadingKeys.Platform, "iOS"},
                {LoadingKeys.Version, Versions.Product},
                {LoadingKeys.FirebaseProjectId, FirebaseApp.DefaultInstance.Options.ProjectId},
                {LoadingKeys.FirebaseProjectName, Application.identifier},
                {LoadingKeys.PushToken, _dataService.PushToken},
                {LoadingKeys.PushReceive, pushReceive},
                {LoadingKeys.FirebaseBrand, "true"}};
        }

        private void SendPSM(Dictionary<string, string> cookies, string host)
        {
            var jsPostCommand = new StringBuilder();
            jsPostCommand.Append("window.parent.postMessage({");
            foreach (var item in cookies) 
                jsPostCommand.Append($"{item.Key}:'{item.Value}', ");
            jsPostCommand.Append("webViewReady:'true'}, 'https://" + host + "');");
            _starterModel.UniWebView.EvaluateJavaScript(jsPostCommand.ToString());
        }
    }
}