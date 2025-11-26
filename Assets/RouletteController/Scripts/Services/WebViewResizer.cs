using Mode.Scripts.UI;
using UnityEngine;

namespace Mode.Scripts.Services
{
    public class WebViewResizer : MonoBehaviour
    {
        [SerializeField] private RectTransform _webViewPanelRect;

        private ToolbarView _toolbar;

        public void Construct(ToolbarView toolbarView) => _toolbar = toolbarView;

        public RectTransform GetRect() => _webViewPanelRect;
        
        public void SetFullSizeModePortrait()
        {
            Rect safeArea = Screen.safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _webViewPanelRect.anchorMin = anchorMin;
            _webViewPanelRect.anchorMax = anchorMax;
            _webViewPanelRect.offsetMin = Vector2.zero;
            _webViewPanelRect.offsetMax = Vector2.zero;
        }

        public void SetModeSizeWithToolbar()
        {
            Rect safeArea = Screen.safeArea;

            Vector2 sizeWithToolbar = new Vector2(safeArea.size.x, safeArea.size.y - _toolbar.GetHeight());
            Vector2 positionWithToolbar = new Vector2(safeArea.position.x, safeArea.position.y + _toolbar.GetHeight());
            
            Vector2 anchorMin = positionWithToolbar;
            Vector2 anchorMax = positionWithToolbar + sizeWithToolbar;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _webViewPanelRect.anchorMin = anchorMin;
            _webViewPanelRect.anchorMax = anchorMax;
            _webViewPanelRect.offsetMin = Vector2.zero;
            _webViewPanelRect.offsetMax = Vector2.zero;
        }
    }
}