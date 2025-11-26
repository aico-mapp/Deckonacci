using Game.Calendar.Scripts.Game.DeviceAdaptation;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Game.DeviceAdaptation
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(AspectRatioFitter))]
    public class BackResolution : BaseResolutionManager
    {
        [Header("Resolution Settings")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Sprite _iPhoneBackground, _iPadBackground;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Vector2 _sliceOffset;

        public Sprite IPhoneBackground
        {
            set
            {
                _iPhoneBackground = value;
                AdjustResolution();
            }
        }
    
        public Sprite IPadBackground
        {
            set
            {
                _iPadBackground = value;
                AdjustResolution();
            }
        }

        protected override void Start()
        {
            base.Start();
            AdjustResolution();
        }

        public override void AdjustResolution()
        {
            SetBackgroundOrientation();
            bool isiPad = IsIpad();
            SetImageResolution(isiPad);
            SetSlice(isiPad);
            _rectTransform.sizeDelta = Vector2.zero;
        }

        private void SetBackgroundOrientation()
        {
            _isPortraitOrientation = Screen.height > Screen.width;
        }
    
        private void SetImageResolution(bool isiPad)
        {
            _backgroundImage.sprite = isiPad ? _iPadBackground : _iPhoneBackground;
        }

        private void SetSlice(bool isiPad)
        {
            ConfigureAspectRatioFitter(isiPad);
            ApplyOffset(isiPad);
        }

        private void ApplyOffset(bool isiPad)
        {
            _rectTransform.anchoredPosition = isiPad || GetAspectRatio() >= _iPhoneXRRatio 
                ? Vector2.zero 
                : new Vector2(_sliceOffset.x, _sliceOffset.y);
        }
    }
}