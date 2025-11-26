using System;
using Calendar.Scripts.Game.UI.Base;
using Calendar.Scripts.Services.EntityContainer;
using Game.Calendar.Scripts.Services.Sound;
using Game.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Calendar.Scripts.Game.UI.MainScreen
{
    public class MainView: BaseView, IFactoryEntity
    {
        public UnityEvent OnPlayClick => _playButton.onClick;
        public UnityEvent OnRulesClick => _rulesButton.onClick;
        public UnityEvent OnBonusClick => _bonusButton.onClick;
        
        [Header("Buttons")] 
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _rulesButton;
        [SerializeField] private Button _bonusButton;
        [SerializeField] private SoundToggleButton _soundToggleButton;
        
        public void Construct(ISoundService soundService)
        {
            _soundToggleButton.Construct(soundService);
        }

        public void SubscribeView()
        {
            
        }

        public void UnsubscribeView()
        {
            
        }
        
        public void UpdateSound(bool isMuted)
        {
            _soundToggleButton.UpdateImage(isMuted);
        }
    }
}