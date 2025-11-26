using System;
using System.Threading.Tasks;
using Mode.Scripts.Data;
using Mode.Scripts.Data.Enums;
using Mode.Scripts.Network;
using Mode.Scripts.Notifications;
using Mode.Scripts.Timer;
using Mode.Scripts.UI;
using UnityEngine;

namespace Mode.Scripts.Services
{
    public class EnvService
    {
        private readonly TimerService _timerService;
        private readonly StarterModel _starterModel;
        private readonly ToolbarController _toolbarController;
        private readonly NetworkService _networkService;
        private readonly LoadingScreenController _loadingScreenController;
        private readonly LinkService _linkService;
        private readonly NotificationService _notificationService;
        private readonly DataService _dataService;
        private string _initialState;
        private string _host;
        private string _current;
        private bool _isMultipleWindowOpened;

        public EnvService(
            TimerService timerService,
            StarterModel starterModel,
            ToolbarController toolbarController,
            NetworkService networkService,
            LoadingScreenController loadingScreenController,
            LinkService linkService,
            NotificationService notificationService,
            DataService dataService)
        {
            _timerService = timerService;
            _starterModel = starterModel;
            _toolbarController = toolbarController;
            _networkService = networkService;
            _loadingScreenController = loadingScreenController;
            _linkService = linkService;
            _notificationService = notificationService;
            _dataService = dataService;
        }

        public void Enable()
        {
            _isMultipleWindowOpened = false;
            DefineLocation(host => _host = host);
            _initialState = GetWithoutParams(_starterModel.UniWebView.Url);
            SubscribeMode();
            EnableChangeHandler();
            _loadingScreenController.ShowViewModeWithDelay(0.5f);
        }

        private void SubscribeMode()
        {
            _toolbarController.OnBackPressed += ReturnToHost;
            _starterModel.UniWebView.OnWebContentProcessTerminated += TryReloadByCrash;
            _starterModel.UniWebView.OnMultipleWindowOpened += OnMultipleWindowOpened;
            _starterModel.UniWebView.OnMultipleWindowClosed += OnMultipleWindowClosed;
            _networkService.OnInternetConnectionLost += SetConnectionLost;
            _networkService.OnInternetConnectionReturned += SetupMode;
            _notificationService.OnNotificationReceived += TryOpenNotification;
        }

        private void DefineLocation(Action<string> callback) =>
            _starterModel.UniWebView.EvaluateJavaScript("window.location.protocol + \"//\" + window.location.host;", 
                payload => callback.Invoke(payload.data));

        private void SetConnectionLost() => _loadingScreenController.ShowScreen(ScreenId.NoConnectionScreen);

        private void SetupMode()
        {
            _starterModel.UniWebView.Reload();
            _loadingScreenController.ShowViewModeWithDelay(0.5f);
        }
        
        private void OnMultipleWindowOpened(UniWebView webView, string multipleWindowId)
        {
            _isMultipleWindowOpened = true;
            _starterModel.UniWebView.SetAllowBackForwardNavigationGestures(false);
            if (webView.Url.EndsWith(".pdf"))
            {
                webView.GoBack();
                Application.OpenURL(webView.Url);
            }
            if (webView.Url.StartsWith("mailto:"))
            {
                webView.GoBack();
                Application.OpenURL(webView.Url);
            }
            
            _toolbarController.Show();
            SetSize(false);
        }
        
        private void OnMultipleWindowClosed(UniWebView webView, string multipleWindowId)
        {
            _isMultipleWindowOpened = false;
            TryToggleGestures();
            if(_starterModel.UniWebView.Url.StartsWith(_host) == false) return;
            _toolbarController.Hide();
            SetSize(true);
        }

        private void TryShowToolbar(string newUrl)
        {
            if (string.IsNullOrEmpty(_host) || string.IsNullOrEmpty(newUrl)) return;
            if (newUrl.StartsWith(_host) == false)
            {
                _toolbarController.Show();
                SetSize(false);
            }
            else
            {
                SetSize(true);
                _toolbarController.Hide();
            }
        }

        private void TryToggleGestures() =>
            _starterModel.UniWebView.SetAllowBackForwardNavigationGestures(string
                .CompareOrdinal(GetWithoutParams(_starterModel.UniWebView.Url), _initialState) != 0);

        private async void TryReloadByCrash(UniWebView webView)
        {
            if (!_dataService.Wrapper.appSettings.contentTerminatedHandle) return;
            await Task.Delay(1250);
            if (string.IsNullOrWhiteSpace(webView.Url)) ReturnToHost();
        }

        private void ReturnToHost()
        {
            SetSize(true);
            _toolbarController.Hide();
            LoadStartPage();
        }

        private async void LoadStartPage()
        {
            while (_starterModel.UniWebView.CanGoBack)
            {
                _starterModel.UniWebView.GoBack();
                await Task.Delay(TimeSpan.FromMilliseconds(5));
            }
            _starterModel.UniWebView.Load(_host);
            _starterModel.UniWebView.UpdateFrame();
            TryShowToolbar(_starterModel.UniWebView.Url);
        }

        private void EnableChangeHandler()
        {
            _current = _starterModel.UniWebView.Url;
            _timerService.OnTick += CheckForNewLink;
        }

        private void CheckForNewLink()
        {
            if (_starterModel.UniWebView.Url == _current) return;
            _current = _starterModel.UniWebView.Url;
            TryShowToolbar(_starterModel.UniWebView.Url);
            if (!_isMultipleWindowOpened) TryToggleGestures();
            else _starterModel.UniWebView.SetAllowBackForwardNavigationGestures(false);
        }

        private string GetWithoutParams(string stroke) =>
            stroke.IndexOf('?') == -1 ? stroke : stroke.Substring(0, stroke.IndexOf('?'));

        private void TryOpenNotification()
        {
            _starterModel.UniWebView.Load(_linkService.GetLinker());
        }

        private void SetSize(bool isFull)
        {
            if (isFull)
                _starterModel.WebViewResizer.SetFullSizeModePortrait();
            else
                _starterModel.WebViewResizer.SetModeSizeWithToolbar();
            _starterModel.UniWebView.ReferenceRectTransform = _starterModel.WebViewResizer.GetRect();
            _starterModel.UniWebView.UpdateFrame();
        }
    }
}