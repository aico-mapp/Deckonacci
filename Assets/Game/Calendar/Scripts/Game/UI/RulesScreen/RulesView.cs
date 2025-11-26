using Calendar.Scripts.Game.UI.Base;
using Calendar.Scripts.Services.EntityContainer;
using Game.Calendar.Scripts.Services.Sound;
using Game.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Calendar.Scripts.Game.UI.RulesScreen
{
    public class RulesView: BaseView, IFactoryEntity
    {
        public UnityEvent OnBackClick => _backButton.onClick;
        
        [Header("Buttons")] 
        [SerializeField] private Button _backButton;
        [SerializeField] private SoundToggleButton _soundToggleButton;

        [Header("Rules Transform")]
        [SerializeField] private RectTransform _iPhoneRules;
        [SerializeField] private RectTransform _iPadRules;

        private Vector3 _currentIPhonePos;
        private Vector3 _currentIPadPos;
        
        public void Construct(ISoundService soundService)
        {
            _soundToggleButton.Construct(soundService);
            
            _currentIPhonePos = _iPhoneRules.anchoredPosition;
            _currentIPadPos = _iPadRules.anchoredPosition;
        }

        public void SubscribeView()
        {
            
        }

        public void UnsubscribeView()
        {
            
        }
        
        public void ResetRules()
        {
            _iPhoneRules.anchoredPosition = _currentIPhonePos;
            _iPadRules.anchoredPosition = _currentIPadPos;
        }
        
        public void UpdateSound(bool isMuted)
        {
            _soundToggleButton.UpdateImage(isMuted);
        }
    }
}