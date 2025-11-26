using System;
using Mode.Scripts.Firebase;
using Newtonsoft.Json;
using UnityEngine;

namespace Mode.Scripts.Data
{
    public class DataService
    {
        private const string PROGRESS_KEY = "mdlProgressData";
        
        private DataModel _dataModel;

        public FirebaseConfigurationWrapper Wrapper;
        public string PushToken;
        public string PushLink;
        
        public int Status => _dataModel.status;
        public string ProjectName => _dataModel.projectName;
        public string PushStatus => _dataModel.pushStatus;
        public string Configuration => _dataModel.configuration;
        public string ConversionTag => _dataModel.conversionTag;
        public int LoadingCounter => _dataModel.loadingCounter;
        public string Token => _dataModel.token;
        public string TokenEndpoint => _dataModel.tokenEndpoint;
        public int TokenRefreshPeriod => _dataModel.tokenRefreshPeriod;
        public bool TryGame => _dataModel.status == -1;

        public DataService()
        {
            LoadProgress();
            IncreaseLoadingCounter();
        }

        public void SetStatus(int status)
        {
            _dataModel.status = status;
            SaveProgress();
        }

        public void SetProjectName(string projectName)
        {
            _dataModel.projectName = projectName;
            SaveProgress();
        }

        public void SetPushStatus(string pushStatus)
        {
            _dataModel.pushStatus = pushStatus;
            SaveProgress();
        }

        public void SetConfiguration(string configuration)
        {
            _dataModel.configuration = configuration;
            SaveProgress();
        }

        public void IncreaseLoadingCounter()
        {
            _dataModel.loadingCounter++;
            SaveProgress();
        }

        public void SetToken(string token)
        {
            _dataModel.token = token;
            SaveProgress();
        }

        public void SetTokenEndpoint(string tokenEndpoint)
        {
            _dataModel.tokenEndpoint = tokenEndpoint;
            SaveProgress();
        }

        public void SetTokenRefreshPeriod(int tokenRefreshPeriod)
        {
            _dataModel.tokenRefreshPeriod = tokenRefreshPeriod;
            SaveProgress();
        }

        public void SetConversationTag(string conversationTag)
        {
            _dataModel.conversionTag = conversationTag;
            SaveProgress();
        }

        public void TrySetConversationTag()
        {
            if (!string.IsNullOrEmpty(_dataModel.conversionTag)) return;
            SetConversationTag(Guid.NewGuid().ToString());
            SetStatus(1);
        }

        private void SaveProgress()
        {
            PlayerPrefs.SetString(PROGRESS_KEY, JsonConvert.SerializeObject(_dataModel));
        }

        private void LoadProgress()
        {
            var progressJson = PlayerPrefs.GetString(PROGRESS_KEY);
            _dataModel = JsonConvert.DeserializeObject<DataModel>(progressJson) ?? new DataModel();
        }
    }
}