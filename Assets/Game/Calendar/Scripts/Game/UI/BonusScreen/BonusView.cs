using System;
using Calendar.Scripts.Data.Enums;
using Calendar.Scripts.Game.UI.Base;
using Calendar.Scripts.Services.EntityContainer;
using Game.Calendar.Scripts.Services.Sound;
using Game.Scripts;
using Game.Calendar.Scripts.Data.Progress;
using Game.Calendar.Scripts.Game.Wheel;
using Game.Scripts.Animations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Calendar.Scripts.Game.UI.BonusScreen
{
    public class BonusView : BaseView, IFactoryEntity
    {
        public UnityEvent OnGetClick => _getButton.onClick;
        public UnityEvent OnBackClick => _backButton.onClick;
        public event Action<int> OnRewardAdded;
        
        [Header("Buttons")] 
        [SerializeField] private Button _getButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private SoundToggleButton _soundToggleButton;
        
        [Header("Bonus Wheel")]
        [SerializeField] private BonusWheelManager _bonusWheelManager;

        [Header("Popup Animation")]
        [SerializeField] private BonusResultInfo _resultInfo;
        [SerializeField] private PopupAnimation _resultPopup;
        
        private UserProgress _userProgress;
        private ISoundService _soundService;

        private int _currentReward;

        public void Construct(ISoundService soundService, UserProgress userProgress)
        {
            _soundService = soundService;
            _userProgress = userProgress;
            
            _soundToggleButton.Construct(_soundService);
            _bonusWheelManager.Initialize(_soundService, _userProgress);
        }
        
        public void SubscribeView()
        {
            _bonusWheelManager.SubscribeWheel();
            _bonusWheelManager.OnRewardShown += ShowResultPopup;
            
            OnGetClick.AddListener(AddReward);
        }

        public void UnsubscribeView()
        {
            _bonusWheelManager.UnsubscribeWheel();
            _bonusWheelManager.OnRewardShown -= ShowResultPopup;
            
            OnGetClick.RemoveAllListeners();
        }
        
        public void UpdateSound(bool isMuted)
        {
            _soundToggleButton.UpdateImage(isMuted);
        }

        private void AddReward()
        {
            OnRewardAdded?.Invoke(_currentReward);
        }

        private void ShowResultPopup(int reward)
        {
            _currentReward = reward;
            
            _soundService.PlayEffectSound(SoundId.PopupWin);
            
            _resultInfo.SetBonusText(reward);
            _resultPopup.Show();
        }
        
        public void HideResultPopup()
        {
            _resultPopup.Hide();
        }
    }
}