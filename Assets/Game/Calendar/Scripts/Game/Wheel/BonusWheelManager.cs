using System;
using Calendar.Scripts.Data.Enums;
using DG.Tweening;
using Game.Calendar.Scripts.Data.Progress;
using Game.Calendar.Scripts.Services.Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Calendar.Scripts.Game.Wheel
{
    public class BonusWheelManager : MonoBehaviour
    {
        public event Action<int> OnRewardShown;
        [Header("References")]
        [SerializeField] private SpinWheelController _wheelController;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private Button _spinButton;
        
        [Header("Cooldown Settings")]
        [SerializeField] private float _cooldownHours = 12f;

        private ISoundService _soundService;
        private UserProgress _userProgress;
        
        private bool _isOnCooldown;
        private DateTime _nextAvailableTime;

        public void Initialize(ISoundService soundService, UserProgress userProgress)
        {
            _soundService = soundService;
            _userProgress = userProgress;
            
            CheckCooldownStatus();
            _wheelController.InitializeWheel();
        }

        public void SubscribeWheel()
        {
            _spinButton.onClick.AddListener(OnSpinButtonClicked);
        }
        
        public void UnsubscribeWheel()
        {
            _spinButton.onClick.RemoveAllListeners();
        }

        private void Update()
        {
            if (_isOnCooldown)
            {
                UpdateTimerDisplay();
            }
        }

        public void CheckCooldownStatus()
        {
            if (string.IsNullOrEmpty(_userProgress.BonusWheelData.LastSpinTime))
            {
                SetSpinAvailable();
                return;
            }

            DateTime lastSpinTime = DateTime.Parse(_userProgress.BonusWheelData.LastSpinTime);
            _nextAvailableTime = lastSpinTime.AddHours(_cooldownHours);

            if (DateTime.Now >= _nextAvailableTime)
            {
                SetSpinAvailable();
            }
            else
            {
                SetSpinOnCooldown();
            }
        }

        private void OnSpinButtonClicked()
        {
            if (_isOnCooldown || _wheelController.IsSpinning()) return;

            _wheelController.Spin(OnSpinComplete);
            _spinButton.interactable = false;
            
            _soundService.PlayEffectSound(SoundId.BonusWheel);
        }

        private void OnSpinComplete(int reward)
        {
            _userProgress.BonusWheelData.LastSpinTime = DateTime.Now.ToString("o");
            _userProgress.SendPropertyChanged();
            
            Debug.Log($"You won {reward} coins!");
            
            OnRewardShown?.Invoke(reward);
            
            _nextAvailableTime = DateTime.Now.AddHours(_cooldownHours);
            SetSpinOnCooldown();
        }

        private void SetSpinAvailable()
        {
            _isOnCooldown = false;
            _spinButton.interactable = true;
            _timerText.enabled = false;
        }

        private void SetSpinOnCooldown()
        {
            _isOnCooldown = true;
            _spinButton.interactable = false;
            UpdateTimerDisplay();
        }

        private void UpdateTimerDisplay()
        {
            TimeSpan timeRemaining = _nextAvailableTime - DateTime.Now;

            if (timeRemaining.TotalSeconds <= 0)
            {
                SetSpinAvailable();
                return;
            }
            
            string timeText;
            if (timeRemaining.TotalHours >= 1)
            {
                timeText = $"{timeRemaining.Hours}h {timeRemaining.Minutes}m {timeRemaining.Seconds}s";
            }
            else if (timeRemaining.TotalMinutes >= 1)
            {
                timeText = $"{timeRemaining.Minutes}m {timeRemaining.Seconds}s";
            }
            else
            {
                timeText = $"{timeRemaining.Seconds}s";
            }

            _timerText.enabled = true;
            _timerText.text = $"Next spin in: {timeText}";
        }

        private void OnDestroy()
        {
            if (_spinButton != null)
                _spinButton.onClick.RemoveListener(OnSpinButtonClicked);
        }
    }
}