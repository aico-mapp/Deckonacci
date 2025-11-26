using Game.Scripts.Game.DeviceAdaptation;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Calendar.Scripts.Game.DeviceAdaptation
{
    public abstract class BaseResolutionManager : DeviceSelection, IDeviceAdjustable
    {
        [SerializeField] protected AspectRatioFitter _aspectRatioFitter;
    
        [Header("Ratio Settings")]
        [SerializeField] protected float _iPhonePortraitAspectRatio = 0.46f;
        [SerializeField] protected float _iPadPortraitAspectRatio = 0.75f;
        [SerializeField] protected float _iPadLandscapeAspectRatio = 1.2f;
        [SerializeField] protected float _iPhoneLandscapeAspectRatio = 2.2f;
        [SerializeField] protected float _iPhoneXRRatio = 1.87f;

       
        private HorizontalLayoutGroup _horizontalLayoutGroup;

        protected virtual void Awake()
        {
            _horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
        }

        protected virtual void Start()
        {
            SetAspectRatioByLandscape(IsIpad(), false);
            ConfigureLayoutGroup();
        }

        private void ConfigureLayoutGroup()
        {
            if (_horizontalLayoutGroup != null)
            {
                _horizontalLayoutGroup.childControlWidth = true;
                _horizontalLayoutGroup.childControlWidth = false;
            }
        }
        
        protected void SetAspectRatioByLandscape(bool isiPad, bool setAspectMode)
        {
            if(setAspectMode)
                _aspectRatioFitter.aspectMode = isiPad ? 
                    AspectRatioFitter.AspectMode.WidthControlsHeight : 
                    AspectRatioFitter.AspectMode.HeightControlsWidth;
            _aspectRatioFitter.aspectRatio = isiPad ? _iPadLandscapeAspectRatio : _iPhoneLandscapeAspectRatio;
        }

        protected void SetAspectRatioByPortrait(bool isiPad)
        {
            _aspectRatioFitter.aspectMode = isiPad
                ? AspectRatioFitter.AspectMode.HeightControlsWidth
                : AspectRatioFitter.AspectMode.WidthControlsHeight;
            _aspectRatioFitter.aspectRatio = isiPad ? _iPadPortraitAspectRatio : _iPhonePortraitAspectRatio;
        }


        protected void ConfigureAspectRatioFitter(bool isiPad)
        {
            if (!_isPortraitOrientation)
                SetAspectRatioByLandscape(isiPad, true);
            else
                SetAspectRatioByPortrait(isiPad);
        }

        protected float GetAspectRatio()
        {
            Vector2Int screenSize = _isPortraitOrientation
                ? new Vector2Int(Screen.height, Screen.width)
                : new Vector2Int(Screen.width, Screen.height);
        
            return (float)screenSize.x / screenSize.y;
        }

        public abstract void AdjustResolution();
    }
}