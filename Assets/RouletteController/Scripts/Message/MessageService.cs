using System;
using Mode.Scripts.Data;
using Mode.Scripts.Services;
using UnityEngine;

namespace Mode.Scripts.Message
{
    public class MessageService
    {
        public event Action OnReceivedLoaded;
        
        private readonly DataService _dataService;
        private readonly LoadedProjectData _loadedProjectData;
        private readonly StarterModel _starterModel;

        public MessageService(
            DataService dataService,
            LoadedProjectData loadedProjectData,
            StarterModel starterModel)
        {
            _dataService = dataService;
            _loadedProjectData = loadedProjectData;
            _starterModel = starterModel;
        }

        public void Initialize()
        {
            _starterModel.UniWebView.OnMessageReceived += HandleMessageReceived;
            _loadedProjectData.SetPushStatus();
        }
        
        private void HandleMessageReceived(UniWebView webView, UniWebViewMessage message)
        {
            var eventData = message.Args;
            var host = eventData.TryGetValue(LoadingKeys.Host, out var hostValue);
            if (string.IsNullOrEmpty(_loadedProjectData.Host) && host) _loadedProjectData.Host = hostValue;
            
            if (eventData[LoadingKeys.Key] != LoadingKeys.LoadFinished) return;
            if (eventData.TryGetValue(LoadingKeys.Name, out var projectName))
            {
                _dataService.SetProjectName(projectName);
            }
            
            OnReceivedLoaded?.Invoke();
        }
    }
}