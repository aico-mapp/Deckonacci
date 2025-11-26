using System;
using Cysharp.Threading.Tasks;
using Mode.Scripts.Analytics;
using Mode.Scripts.Data;
using Mode.Scripts.Localization;
using Mode.Scripts.Message;
using Mode.Scripts.UI;
using UnityEngine;

namespace Mode.Scripts.Services
{
    public class StarterService
    {
        private readonly StarterModel _starterModel;
        private readonly AnalyticsService _analyticsService;
        private readonly LocalizationService _localizationService;
        private readonly ToolbarController _toolbarController;
        private readonly MessageService _messageService;
        private readonly LoadingScreenController _loadingScreenController;
        private readonly ConversionService _conversionService;
        private readonly ToolbarView _toolbarView;
        private readonly LoadService _loadService;
        private readonly DataService _dataService;

        public StarterService(
            StarterModel starterModel,
            AnalyticsService analyticsService,
            LocalizationService localizationService,
            ToolbarController toolbarController,
            MessageService messageService,
            LoadingScreenController loadingScreenController,
            ConversionService conversionService,
            ToolbarView toolbarView,
            LoadService loadService,
            DataService dataService)
        {
            _starterModel = starterModel;
            _analyticsService = analyticsService;
            _localizationService = localizationService;
            _toolbarController = toolbarController;
            _messageService = messageService;
            _loadingScreenController = loadingScreenController;
            _conversionService = conversionService;
            _toolbarView = toolbarView;
            _loadService = loadService;
            _dataService = dataService;
        }

        public void Run()
        {
            Initialize();
            Subscribe();
            LoadProgress();
        }

        private void Initialize()
        {
            _starterModel.UniWebView = CreateWebView();
            _messageService.Initialize();
            _starterModel.WebViewResizer.Construct(_toolbarView);
            _loadingScreenController.Construct();
        }

        private UniWebView CreateWebView()
        {
            Screen.orientation = ScreenOrientation.AutoRotation;
            UniWebView.SetJavaScriptEnabled(true);
            UniWebView.SetAllowInlinePlay(true);
            UniWebView.SetAllowAutoPlay(true);
            var webView = _starterModel.WebViewResizer.gameObject.AddComponent<UniWebView>();
            _starterModel.WebViewResizer.SetFullSizeModePortrait();
            webView.ReferenceRectTransform = _starterModel.WebViewResizer.GetRect();
            webView.UpdateFrame();
            webView.SetCalloutEnabled(false);
            webView.SetSupportMultipleWindows(true, true);
            webView.SetUseWideViewPort(false);
            webView.RegisterShouldHandleRequest(request =>
            {
                if (request.Url.StartsWith("mailto:"))
                {
                    webView.GoBack();
                    Application.OpenURL(request.Url);
                    return false;
                }
                
                if (request.Url.EndsWith(".pdf"))
                {
                    webView.GoBack();
                    Application.OpenURL(request.Url);
                    return false;
                }
        
                return true;
            });
            SetAg(webView);
            ConfigureEmbedded(webView);
            return webView;
        }

        private void SetAg(UniWebView webView)
        {
            var webAg = webView.GetUserAgent();
            var fullAg = _dataService.Wrapper.appSettings.fullUserAgent;
            var ag = _dataService.Wrapper.appSettings.agent;
            if (string.IsNullOrEmpty(fullAg) == false) webAg = fullAg;
            else if (string.IsNullOrEmpty(ag) == false) webAg += " " + ag;
            webView.SetUserAgent(webAg);
            _analyticsService.SetAgent(webAg);
        }
        
        private void ConfigureEmbedded(UniWebView webView)
        {
            _toolbarController.SetText(_localizationService.Word.BackText);
            _toolbarController.SetToolbarColors();
            webView.EmbeddedToolbar.SetPosition(UniWebViewToolbarPosition.Bottom);
            webView.EmbeddedToolbar.Hide();
        }

        private void Subscribe()
        {
            _starterModel.OrientationChanger.OnOrientationChanged += ChangeOrientation;
            //_starterModel.UniWebView.OnLoadingErrorReceived += OnPageErrorReceived;
        }

        private void ChangeOrientation(DeviceOrientation orientation)
        {
            _toolbarController.SetPanelInSafeArea();
            if(_toolbarController.ActiveSelf) _starterModel.WebViewResizer.SetModeSizeWithToolbar();
            else _starterModel.WebViewResizer.SetFullSizeModePortrait();

            _starterModel.UniWebView.ReferenceRectTransform = _starterModel.WebViewResizer.GetRect();
            _starterModel.UniWebView.UpdateFrame();
        }
        
        private void OnPageErrorReceived(UniWebView webView, int errorCode, string errorMessage, UniWebViewNativeResultPayload payload)
        {
            if (errorCode == 102) webView.GoBack();
        }

        private void LoadProgress()
        {
            _loadService.OnLoaded += EndAnalytics;
            _loadService.BootProgress();
        }

        private void EndAnalytics()
        {
            _loadService.OnLoaded -= EndAnalytics;
            try
            {
                _conversionService.SendConversionData().Forget();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}