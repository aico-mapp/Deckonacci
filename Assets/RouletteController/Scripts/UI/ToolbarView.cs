using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mode.Scripts.UI
{
    public class ToolbarView : MonoBehaviour
    {
        public event Action OnBackPressed;

        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private Image _background;
        [SerializeField] private Image _backIcon;
        [SerializeField] private RectTransform _panelRect;
        [SerializeField] private RectTransform _toolbarRect;
        
        public RectTransform PanelRect => _panelRect;
        public RectTransform ToolbarRect => _toolbarRect;
        
        public void SetBackButtonSprite(Sprite sprite) => _backIcon.sprite = sprite;

        public void SetTextColor(Color textColor) => _buttonText.color = textColor;

        public void SetBackgroundColor(Color backgroundColor) => _background.color = backgroundColor;
        
        public void SetText(string text) => _buttonText.text = text;
        
        public float GetHeight() => _toolbarRect.rect.height;

        private void Awake()
        {
            _button.onClick.AddListener(PressBack);
            DontDestroyOnLoad(this);
            gameObject.SetActive(false);
        }

        private void OnDestroy() => _button.onClick.RemoveListener(PressBack);

        private void PressBack() => OnBackPressed?.Invoke();
    }
}