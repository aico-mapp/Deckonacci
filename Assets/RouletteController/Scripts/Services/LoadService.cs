using System;
using Cysharp.Threading.Tasks;
using Mode.Scripts.Analytics;
using Mode.Scripts.Data;
using Mode.Scripts.Data.Enums;
using Mode.Scripts.Firebase;
using Mode.Scripts.Message;
using Mode.Scripts.Network;
using Mode.Scripts.Notifications;
using Mode.Scripts.Timer;
using Mode.Scripts.UI;
using UnityEngine;
using EventHandler = Mode.Scripts.Network.EventHandler;

namespace Mode.Scripts.Services
{
    public class LoadService
    {
        public event Action OnLoaded;

        private readonly NetworkService _connectionService;
        private readonly TimerService _timerService;
        private readonly MessageService _messageService;
        private readonly DataService _dataService;
        private readonly AnalyticsService _analyticsService;
        private readonly EnvService _envService;
        private readonly StarterModel _starterModel;
        private readonly FirebaseService _firebaseService;
        private readonly LoadingScreenController _loadingScreenController;
        private readonly NotificationService _notificationService;
        private readonly PSMService _psmService;
        private readonly LinkService _linkService;
        private readonly LoadedProjectData _loadedProjectData;
        private readonly EventHandler _eventHandler;
        
        private int _loadingTime;
        private const int CHECK_STEP = 30;
        
        private int _currentTime;
        private int _loadingCounter;
        private bool _isLoading;

        public LoadService(
            TimerService timerService,
            NetworkService connectionService,
            MessageService messageService,
            DataService dataService,
            AnalyticsService analyticsService,
            EnvService envService,
            StarterModel starterModel,
            FirebaseService firebaseService,
            LoadingScreenController loadingScreenController,
            NotificationService notificationService,
            PSMService psmService,
            LinkService linkService,
            LoadedProjectData loadedProjectData,
            EventHandler eventHandler)
        {
            _timerService = timerService;
            _connectionService = connectionService;
            _linkService = linkService;
            _loadedProjectData = loadedProjectData;
            _messageService = messageService;
            _dataService = dataService;
            _analyticsService = analyticsService;
            _envService = envService;
            _starterModel = starterModel;
            _firebaseService = firebaseService;
            _loadingScreenController = loadingScreenController;
            _notificationService = notificationService;
            _psmService = psmService;
            _eventHandler = eventHandler;
        }

        public void BootProgress()
        {
            _loadingTime = _dataService.Wrapper.appSettings.maxProgressTime;
            AddListeners();
            if(_connectionService.IsConnectionStatus == false) SetConnectionLost();
            _starterModel.UniWebView.EmbeddedToolbar.Hide();
            EvaluateJavaScriptFromServer(_dataService.Wrapper.appSettings.jInitialize);
            _starterModel.UniWebView.Load(_linkService.GetLinker());
            _isLoading = true;
            _analyticsService.StartStep(AnalyticsStepType.ProjectLoading);
        }

        private void PushLinkUpdate()
        {
            if(!_isLoading) return;

            var url = _linkService.GetLinker();
            _starterModel.UniWebView.Load(url);
        }

        private void SetConnectionLost() => _loadingScreenController.ShowScreen(ScreenId.NoConnectionScreen);

        private void ContinueProgress()
        {
            _loadingScreenController.ShowScreen(ScreenId.Loader);
            _starterModel.UniWebView.Load(_linkService.GetLinker());
        }

        private void AddListeners()
        {
            _connectionService.OnInternetConnectionLost += SetConnectionLost;
            _connectionService.OnInternetConnectionReturned += ContinueProgress;
            _messageService.OnReceivedLoaded += CompleteLoadProgress;
            _notificationService.OnNotificationReceived += TryOpenNotification;
            _timerService.OnTick += ShowIndependently;
            _eventHandler.OnNotificationReceived += PushLinkUpdate;
        }

        private void ShowIndependently()
        {
            if (string.IsNullOrEmpty(_dataService.Wrapper.appSettings.jcf))
            {
                _starterModel.UniWebView.EvaluateJavaScript(_dataService.Wrapper.appSettings.jcf,
                    payload => ShowIndependently(payload.data));
            }
            else
            {
                ShowIndependently(null);
            }
        }

        private void ShowIndependently(string result)
        {
            if (!IsLoadingTimeExpired() && (string.IsNullOrEmpty(result) || result != "1")) return;
            
            _timerService.OnTick -= ShowIndependently;
            _loadingScreenController.ShowViewModeWithDelay(0.5f);
        }

        private bool IsLoadingTimeExpired() => _loadingTime != 0 && ++_loadingCounter >= _loadingTime;


        private void CompleteLoadProgress()
        {
            _eventHandler.InvokeLoadEnded();
            UnsubscribeProgress();
            EvaluateJavaScriptFromServer(_dataService.Wrapper.appSettings.jLoadingFinished);
            EnableProgress();
            OnLoaded?.Invoke();
        }

        private void UnsubscribeProgress()
        {
            _messageService.OnReceivedLoaded -= CompleteLoadProgress;
            _connectionService.OnInternetConnectionLost -= SetConnectionLost;
            _connectionService.OnInternetConnectionReturned -= ContinueProgress;
            _notificationService.OnNotificationReceived -= TryOpenNotification;
            _timerService.OnTick -= ShowIndependently;
        }

        private void EnableProgress()
        {
            try
            {
                _envService.Enable();
                _psmService.CreatePSM();
                EnableIdentificatorRegistrator();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        
        private void EvaluateJavaScriptFromServer(string js)
        {
            if(string.IsNullOrEmpty(js) == false) _starterModel.UniWebView.EvaluateJavaScript(js);
        }

        private void EnableIdentificatorRegistrator() => _timerService.OnTick += CheckStepFinished;

        private void CheckStepFinished()
        {
            if (++_currentTime < CHECK_STEP) return;
            FindId();
            _currentTime = 0;
        }
        
        private void FindId() => _starterModel.UniWebView.EvaluateJavaScript(_dataService.Wrapper.appSettings.jId, Register);

        private void Register(UniWebViewNativeResultPayload result)
        {
            _timerService.OnTick -= CheckStepFinished;
            _analyticsService.SendFinishProjectLoading(_loadedProjectData, result.data).Forget();
            if (_loadedProjectData.IsPushStatusChanged)
                _dataService.SetPushStatus(_loadedProjectData.PushReceive);
            
            UpdateTokenData();
            _firebaseService.TryRefreshToken().Forget();
        }
        
        private void UpdateTokenData()
        {
            _dataService.SetTokenEndpoint(_dataService.Wrapper.appSettings.tokenEndpoint);
            _dataService.SetTokenRefreshPeriod(_dataService.Wrapper.appSettings.tokenRefreshPeriod);
        }

        private void TryOpenNotification()
        {
            if(!_isLoading) return;
            
            _starterModel.UniWebView.Load(_linkService.GetLinker());
        }
    }
}