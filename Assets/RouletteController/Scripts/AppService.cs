using Cysharp.Threading.Tasks;
using Firebase;
using Mode.Scripts.Analytics;
using Mode.Scripts.Data;
using Mode.Scripts.Data.Enums;
using Mode.Scripts.Firebase;
using Mode.Scripts.Localization;
using Mode.Scripts.Network;
using Mode.Scripts.Notifications;
using Mode.Scripts.Timer;
using Mode.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mode.Scripts
{
    public class AppService
    {
        private readonly LocalizationService _localizationService;
        private readonly LoadingScreenController _loadingScreenController;
        private readonly ConfigurationService _configurationService;
        private readonly FirebaseService _firebaseService;
        private readonly DataService _dataService;
        private readonly AppConfiguration _appConfiguration;
        private readonly AnalyticsService _analyticsService;
        private readonly NetworkService _networkService;
        private readonly ToolbarController _toolbarController;
        private readonly NotificationService _notificationService;
        private readonly TimerService _timerService;

        public AppService(
            LocalizationService localizationService,
            LoadingScreenController loadingScreenController,
            ConfigurationService configurationService,
            FirebaseService firebaseService,
            DataService dataService,
            AppConfiguration appConfiguration,
            AnalyticsService analyticsService,
            NetworkService networkService,
            ToolbarController toolbarController,
            NotificationService notificationService,
            TimerService timerService)
        {
            _localizationService = localizationService;
            _loadingScreenController = loadingScreenController;
            _configurationService = configurationService;
            _firebaseService = firebaseService;
            _dataService = dataService;
            _appConfiguration = appConfiguration;
            _analyticsService = analyticsService;
            _networkService = networkService;
            _toolbarController = toolbarController;
            _notificationService = notificationService;
            _timerService = timerService;
        }

        public void Run()
        {
            _loadingScreenController.Start();
            _toolbarController.Start();
            RunServices();
            StartAnalyticEvents();
            Initialize().Forget();
        }

        private void RunServices()
        {
            _localizationService.Init();
            _loadingScreenController.LocalizeScreen(_localizationService.Word);
            _timerService.Start();
            _firebaseService.Initialize(_appConfiguration.statusKey);
            _notificationService.Initialize();
        }

        private void StartAnalyticEvents()
        {
            _analyticsService.StartStep(AnalyticsStepType.TotalLoading);
            _analyticsService.StartStep(AnalyticsStepType.ConfigurationLoading);
        }

        private async UniTaskVoid Initialize()
        {
            Subscribe();
            _networkService.StartMonitoringAsync().Forget();
            await RunFirebase();
        }

        private void Subscribe()
        {
            _networkService.OnInternetConnectionLost += LostConnection;
            _networkService.OnInternetConnectionReturned += ReInitialize;
        }

        private void Unsubscribe()
        {
            _networkService.OnInternetConnectionLost -= LostConnection;
            _networkService.OnInternetConnectionReturned -= ReInitialize;
        }

        private void LostConnection()
        {
            _loadingScreenController.ShowScreen(ScreenId.NoConnectionScreen);
        }

        private async void ReInitialize()
        {
            await RunFirebase();
            _loadingScreenController.ShowScreen(ScreenId.Loader);
        }

        private async UniTask RunFirebase()
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (_dataService.TryGame || dependencyStatus != DependencyStatus.Available)
            {
                ReceiveGame(false);
                return;
            }

            await _firebaseService.ReceiveData(_appConfiguration.firebaseKey);
            ReceiveGame(!string.IsNullOrEmpty(_dataService.Configuration));
        }

        private void ReceiveGame(bool value)
        {
            _analyticsService.FinishStep(AnalyticsStepType.ConfigurationLoading);
            if (_dataService.Status == 1)
            {
                LoadWithConfig();
            }
            else if (_dataService.TryGame)
            {
                RunGame();
            }
            else
            {
                if (value)
                {
                    var validValue = _configurationService.LoadConfiguration();
                    if (validValue)
                    {
                        LoadWithConfig();
                    }
                    else
                    {
                        RunGame();
                    }
                }
                else
                {
                    RunGame();
                }
            }
        }

        private void RunGame()
        {
            Screen.orientation = _appConfiguration.gameScreenOrientation;
            Unsubscribe();
            _notificationService.Stop();
            _networkService.StopMonitoring();
            _loadingScreenController.Hide();
            _dataService.SetStatus(-1);
            SceneManager.LoadScene(_appConfiguration.gameSceneIndex);
        }

        private void LoadWithConfig()
        {
            _configurationService.UpdateConfiguration();
            _dataService.TrySetConversationTag();
            _analyticsService.Configure(_dataService.Wrapper.conversionLocation, _dataService.Wrapper.analysesLocation);
            SceneManager.LoadScene(_appConfiguration.mdlSceneIndex);
            Unsubscribe();
            _toolbarController.LoadImage();
        }
    }
}