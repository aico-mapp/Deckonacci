using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Calendar.Scripts.Game.UI.Base
{
    public class ThemeSwitcher : MonoBehaviour
    {
        [SerializeField] private Image _targetImage;
        [SerializeField] private Sprite _darkImage;
        [SerializeField] private Sprite _lightImage;
        [SerializeField] private List<TMP_Text> _targetText;
        private Color _darkThemeTextColor, _lightThemeTextColor;

        public void Init(Color darkText, Color lightText)
        {
            _darkThemeTextColor = darkText;
            _lightThemeTextColor = lightText;
        }

        public void SwitchDark()
        {
            if (_targetImage != null)
                _targetImage.sprite = _darkImage;
            foreach (TMP_Text text in _targetText) text.color = _darkThemeTextColor;
        }

        public void SwitchLight()
        {
            if (_targetImage != null)
                _targetImage.sprite = _lightImage;
            foreach (TMP_Text text in _targetText) text.color = _lightThemeTextColor;
        }
    }
}