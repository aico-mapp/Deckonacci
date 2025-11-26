using System;
using Cysharp.Threading.Tasks;
using Mode.Scripts.Data;
using UnityEngine;
using UnityEngine.Networking;

namespace Mode.Scripts.UI
{
    public class ToolbarController
    {
        private readonly DataService _dataService;
        private readonly ToolbarView _toolbarView;
        
        public event Action OnBackPressed;
        
        public bool ActiveSelf => _toolbarView.gameObject.activeSelf;

        public ToolbarController(
            DataService dataService,
            ToolbarView toolbarView)
        {
            _dataService = dataService;
            _toolbarView = toolbarView;
        }

        public void Start()
        {
            SetToolbarInPanel();
            SetPanelInSafeArea();
            _toolbarView.OnBackPressed += ToolbarClick;
        }
        
        public void Show() => _toolbarView.gameObject.SetActive(true);

        public void Hide() => _toolbarView.gameObject.SetActive(false);

        public void LoadImage()
        {
            var imageUrl = _dataService.Wrapper.backButtonImageUrl;
            if (string.IsNullOrWhiteSpace(imageUrl)) return;
            
            try
            {
                LoadImageAsync(imageUrl).Forget();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void SetToolbarColors()
        {
            if (IsColorCorrect(_dataService.Wrapper.backButtonTextColor))
                _toolbarView.SetTextColor(ParseColor(_dataService.Wrapper.backButtonTextColor));
            if (IsColorCorrect(_dataService.Wrapper.backButtonPanelColor))
                _toolbarView.SetBackgroundColor(ParseColor(_dataService.Wrapper.backButtonPanelColor));
        }

        public void SetPanelInSafeArea()
        {
            Rect safeArea = Screen.safeArea;
        
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _toolbarView.PanelRect.anchorMin = anchorMin;
            _toolbarView.PanelRect.anchorMax = anchorMax;
            _toolbarView.PanelRect.offsetMin = Vector2.zero;
            _toolbarView.PanelRect.offsetMax = Vector2.zero;
        }

        public void SetText(string text) => _toolbarView.SetText(text);

        private void ToolbarClick()
        {
            OnBackPressed?.Invoke();
        }
        
        private void SetToolbarInPanel()
        {
            _toolbarView.ToolbarRect.anchorMin = new Vector2(0, 0);
            _toolbarView.ToolbarRect.anchorMax = new Vector2(1, 0);
            _toolbarView.ToolbarRect.pivot = new Vector2(0.5f, 0.5f);
            
            _toolbarView.ToolbarRect.anchoredPosition = new Vector2(0, _toolbarView.ToolbarRect.rect.height / 2);
        }
        
        private async UniTask LoadImageAsync(string url)
        {
            using var req = UnityWebRequestTexture.GetTexture(url);

            await req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success) return;

            var tex = DownloadHandlerTexture.GetContent(req);
            if (tex == null) return;

            var sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero);
            _toolbarView.SetBackButtonSprite(sprite);
        }

        private bool IsColorCorrect(string hexColor)
        {
            if (string.IsNullOrEmpty(hexColor))
                return false;

            if (hexColor.StartsWith("#"))
                hexColor = hexColor.Substring(1);

            if (hexColor.Length != 6 && hexColor.Length != 8)
                return false;

            if (!System.Text.RegularExpressions.Regex.IsMatch(hexColor, "^[0-9A-Fa-f]+$"))
                return false;

            return true;
        }

        private Color ParseColor(string hexColor)
        {
            if (hexColor.StartsWith("#"))
                hexColor = hexColor.Substring(1);

            var r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            var a = 1f;

            if (hexColor.Length == 8)
                a = byte.Parse(hexColor.Substring(6, 2), System.Globalization.NumberStyles.HexNumber) / 255f;

            return new Color(r / 255f, g / 255f, b / 255f, a);
        }
    }
}