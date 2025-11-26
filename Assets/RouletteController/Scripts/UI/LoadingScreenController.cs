using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mode.Scripts.Data.Enums;
using Mode.Scripts.Localization;
using Mode.Scripts.Services;
using UnityEngine;

namespace Mode.Scripts.UI
{
    public class LoadingScreenController
    {
        private readonly StarterModel _starterModel;
        private readonly LoadingScreenView _loadingScreenView;
        private readonly LoadingScreenModel _loadingScreenModel;

        public LoadingScreenController(StarterModel starterModel, LoadingScreenView loadingScreenView, LoadingScreenModel loadingScreenModel)
        {
            _starterModel = starterModel;
            _loadingScreenView = loadingScreenView;
            _loadingScreenModel = loadingScreenModel;
        }

        public void Start()
        {
            _loadingScreenModel.ActiveScreen = ScreenId.Loader;
            _loadingScreenModel.Screens = new Dictionary<ScreenId, GameObject>
            {
                [ScreenId.Loader] = _loadingScreenView.LoadingScreen,
                [ScreenId.NoConnectionScreen] = _loadingScreenView.ConnectionLostScreen
            };
        }

        public void LocalizeScreen(AppLanguage appLanguage)
        {
            _loadingScreenView.SetLostText(appLanguage.ConnectionLostText);
        }

        public void Construct()
        {
            _starterModel.UniWebView.Frame = _loadingScreenModel.ViewFrame;
        }

        public void ShowScreen(ScreenId screenId)
        {
            _loadingScreenView.gameObject.SetActive(true);
            _starterModel.UniWebView?.Hide();
            if (_loadingScreenModel.ActiveScreen == screenId)
            {
                _loadingScreenModel.Screens[_loadingScreenModel.ActiveScreen].SetActive(true);
                return;
            }

            _loadingScreenModel.Screens[_loadingScreenModel.ActiveScreen].SetActive(false);
            _loadingScreenModel.ActiveScreen = screenId;
            _loadingScreenModel.Screens[_loadingScreenModel.ActiveScreen].SetActive(true);
        }

        public void Hide()
        {
            _loadingScreenView.gameObject.SetActive(false);
            _loadingScreenModel.Screens[_loadingScreenModel.ActiveScreen].SetActive(false);
        }

        public async void ShowViewModeWithDelay(float delay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            ShowViewMode();
        }

        private void ShowViewMode()
        {
            Hide();
            _starterModel.UniWebView?.Show();
        }
    }
}