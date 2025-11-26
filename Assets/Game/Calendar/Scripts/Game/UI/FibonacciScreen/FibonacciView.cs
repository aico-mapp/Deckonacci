using System;
using Calendar.Scripts.Data.Enums;
using Calendar.Scripts.Game.UI.Base;
using Calendar.Scripts.Services.EntityContainer;
using Game.Calendar.Scripts.Game.Fibonacci;
using Game.Calendar.Scripts.Services.Sound;
using Game.Scripts;
using Game.Scripts.Animations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Calendar.Scripts.Game.UI.FibonacciScreen
{
    public class FibonacciView : BaseView, IFactoryEntity
    {
        public UnityEvent OnBackClick => _backButton.onClick;
        public UnityEvent OnGetClick => _getButton.onClick;
        
        [Header("Buttons")] 
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _getButton;
        [SerializeField] private SoundToggleButton _soundToggleButton;
        
        [Header("UI")]
        [SerializeField] private Image _getImage;
        [SerializeField] private Sprite _payoutSprite;
        [SerializeField] private Sprite _noPayoutSprite;
        
        [Header("Game Controller")]
        [SerializeField] private FibonacciGameController _gameController;
        
        [Header("Score UI")]
        [SerializeField] private TextMeshProUGUI _playerScoreText;
        [SerializeField] private TextMeshProUGUI _bot1ScoreText;
        [SerializeField] private TextMeshProUGUI _bot2ScoreText;
        [SerializeField] private TextMeshProUGUI _bot3ScoreText;

        [Header("Result UI")]
        [SerializeField] private PopupAnimation _resultPopup;
        [SerializeField] private TextMeshProUGUI _roundsResultText;
        [SerializeField] private TextMeshProUGUI _winsResultText;
        
        private event Action<int> OnGameEnd;
        private ISoundService _soundService;
        
        public void Construct(ISoundService soundService)
        {
            _soundService = soundService;
            _soundToggleButton.Construct(_soundService);
        }
        
        public void StartGame(Action<int> onGameEnd)
        {
            OnGameEnd = onGameEnd;
            _gameController.Initialize(_soundService, OnGameFinished, UpdateScore);
            _gameController.StartGame();
        }

        private void OnGameFinished(int playerScore)
        {
            ShowResult();
            
            _roundsResultText.text = playerScore == 1 ? $"You won {playerScore} round" : $"You won {playerScore} rounds";
            _winsResultText.text = playerScore == 0 ? "Balance x1" : $"Balance x{playerScore}";
            _getImage.sprite = playerScore is 0 or 1 ? _noPayoutSprite : _payoutSprite;

            _soundService.PlayEffectSound(playerScore != 0 ? SoundId.PopupWin : SoundId.PopupLose);

            OnGameEnd?.Invoke(playerScore);
        }
        
        public void UpdateSound(bool isMuted)
        {
            _soundToggleButton.UpdateImage(isMuted);
        }

        private void UpdateScore(int playerScore, int bot1Score, int bot2Score, int bot3Score)
        {
            if (_playerScoreText != null)
                _playerScoreText.text = $"{playerScore}";
            
            if (_bot1ScoreText != null)
                _bot1ScoreText.text = $"{bot1Score}";
            
            if (_bot2ScoreText != null)
                _bot2ScoreText.text = $"{bot2Score}";
            
            if (_bot3ScoreText != null)
                _bot3ScoreText.text = $"{bot3Score}";
        }
        
        public void SubscribeView()
        {
            // Subscribe to any necessary events
        }

        public void UnsubscribeView()
        {
            // Unsubscribe from events
        }

        private void ShowResult()
        {
            _resultPopup.Show();
        }

        public void HideResult()
        {
            _resultPopup.Hide();
        }
    }
}