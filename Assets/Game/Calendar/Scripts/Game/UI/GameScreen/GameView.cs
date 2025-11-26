using System.Collections.Generic;
using Calendar.Scripts.Data.Enums;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Calendar.Scripts.Game.UI.Base;
using Calendar.Scripts.Services.EntityContainer;
using Game.Calendar.Scripts.Game.Chip;
using Game.Calendar.Scripts.Game.Table;
using Game.Calendar.Scripts.Services.Sound;
using Game.Scripts;
using System;
using Game.Calendar.Scripts.Data;
using Game.Calendar.Scripts.Services.SaveLoad;

namespace Game.Calendar.Scripts.Game.UI.GameScreen
{
    public class GameView : BaseView, IFactoryEntity
    {
        public UnityEvent OnBackClick => _backButton.onClick;
        public UnityEvent OnSpinClick => _spinButton.onClick;
        public UnityEvent OnClearClick => _clearButton.onClick;
        public event Action<List<BetData>> OnBetsUpdated;
        
        [Header("Game References")]
        [SerializeField] private TableController _tableController;
        [SerializeField] private ChipsController _chipsController;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _balanceText;
        [SerializeField] private TextMeshProUGUI _betText;
        
        [Header("Buttons")] 
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _spinButton;
        [SerializeField] private Button _clearButton;
        [SerializeField] private SoundToggleButton _soundToggleButton;

        [Header("DEBUG")]
        [SerializeField] private Button _testButton;
        public UnityEvent OnTestClick => _testButton.onClick;
        
        private BetTable _betTable;
        private ISoundService _soundService;
        
        public void Construct(ISoundService soundService)
        {
            _soundService = soundService;
            
            _soundToggleButton.Construct(_soundService);
        }
        
        public void SubscribeView()
        {
            if (_betTable != null)
            {
                _betTable.SubscribeTable();
                _betTable.OnBetsChanged += HandleBetsChanged;
            }
        }

        public void UnsubscribeView()
        {
            _chipsController.UnsubscribeChips();
            
            if (_betTable != null)
            {
                _betTable.UnsubscribeTable();
                _betTable.OnBetsChanged -= HandleBetsChanged;
            }
        }

        private void HandleBetsChanged(List<BetData> bets)
        {
            int totalBet = _betTable.GetTotalBetAmount();
    
            UpdateBet(totalBet);
    
            OnBetsUpdated?.Invoke(bets);
        }

        public void StartGame()
        {
            _betTable = _tableController.SetupTable();
            _betTable.Initialize(_soundService, _chipsController);

            _chipsController.SpawnAllChips();
        }

        public void ResetGame()
        {
            if(_betTable != null)
            {
                _betTable.ClearBets();
                _betTable = null;
            }
            
            _tableController.ResetTable();
            _chipsController.ClearAllChips();
        }

        public void ClearAllBets()
        {
            _betTable.ClearBetsWithAnimation();
            _chipsController.ResetBetTotal();
        }

        public void UpdateSound(bool isMuted)
        {
            _soundToggleButton.UpdateImage(isMuted);
        }

        public void UpdateBalance(int balance)
        {
            _balanceText.text = $"Balance : {balance}";
        }
        
        public void UpdateBet(int bet)
        {
            _betText.text = $"Bet: {bet}";
            SetSpinButtonState(bet > 0);
        }

        public void SetSpinButtonState(bool interactable)
        {
            _spinButton.interactable = interactable;
        }

        public List<BetData> GetCurrentBets()
        {
            return _betTable?.GetCurrentBets() ?? new List<BetData>();
        }
    }
}