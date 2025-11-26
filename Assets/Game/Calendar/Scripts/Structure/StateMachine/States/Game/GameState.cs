using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Calendar.Scripts.Data.Enums;
using Calendar.Scripts.Services.EntityContainer;
using Calendar.Scripts.Services.LoadingCurtain;
using Calendar.Scripts.Structure.StateMachine.GameStateMachine;
using Calendar.Scripts.Structure.StateMachine.States;
using Cysharp.Threading.Tasks;
using Game.Calendar.Scripts.Data;
using Game.Calendar.Scripts.Data.Enums;
using Game.Calendar.Scripts.Game.UI.GameScreen;
using Game.Calendar.Scripts.Services.Factories.GameFactory;
using Game.Calendar.Scripts.Services.SaveLoad;
using Game.Calendar.Scripts.Services.Sound;
using Game.Calendar.Scripts.Services.StaticData;
using Game.Calendar.Scripts.Structure.StateMachine.States.Fibonacci;
using Game.Calendar.Scripts.Structure.StateMachine.States.Main;
using Game.Calendar.Scripts.Structure.StateMachine.States.Roulette;
using UnityEngine;

namespace Game.Calendar.Scripts.Structure.StateMachine.States.Game
{
    public class GameState : IState
    {
        protected ISaveLoad _saveLoad;
        protected ISoundService _soundService;
        protected IStaticData _staticData;
        protected IEntityContainer _entityContainer;
        protected ILoadingCurtain _loadingCurtain;
        protected IGameFactory _gameFactory;
        
        private readonly IStateMachine _stateMachine;
        private GameView _mainView;
        
        private List<BetData> _currentBets = new List<BetData>();
        private int _startingBalance; // Track the balance at state entry
        
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        
        public GameState(IStateMachine stateMachine, IEntityContainer entityContainer, 
            ILoadingCurtain loadingCurtain, IStaticData staticData, ISaveLoad saveLoad, 
            ISoundService soundService, IGameFactory gameFactory)
        {
            _soundService = soundService;
            _stateMachine = stateMachine;
            _staticData = staticData;
            _entityContainer = entityContainer;
            _loadingCurtain = loadingCurtain;
            _saveLoad = saveLoad;
            _gameFactory = gameFactory;
        }
        
        public void Enter()
        {
            _mainView = _entityContainer.GetEntity<GameView>();
            
            _mainView.StartGame();
            
            _mainView.SubscribeView();
            _mainView.OnBetsUpdated += HandleBetsUpdated;
            
            _mainView.OnBackClick.AddListener(SwitchBackState);
            _mainView.OnSpinClick.AddListener(SwitchRouletteState);
            _mainView.OnClearClick.AddListener(ClearBets);
            //DEBUG
            _mainView.OnTestClick.AddListener(SwitchFibonacciState);
            
            _mainView.UpdateSound(_soundService.IsSoundMuted);
            
            _startingBalance = _saveLoad.Progress.CurrentBalance;
            _mainView.UpdateBalance(_startingBalance);
            _mainView.UpdateBet(0);
            
            _mainView.Show();
            _loadingCurtain.Hide();
        }
        
        public void Exit()
        {
            _loadingCurtain.Show();
            _mainView.Hide();
            
            _mainView.UnsubscribeView();
            _mainView.OnBetsUpdated -= HandleBetsUpdated;

            _mainView.OnBackClick.RemoveAllListeners();
            _mainView.OnSpinClick.RemoveAllListeners();
            _mainView.OnClearClick.RemoveAllListeners();
            //DEBUG
            _mainView.OnTestClick.RemoveAllListeners();
            
            _mainView.ResetGame();
            
            _currentBets.Clear();
        }

        private void HandleBetsUpdated(List<BetData> bets)
        {
            _currentBets = bets;
            
            _mainView.UpdateBalance(GetBetInfo().balance);
            _mainView.UpdateBet(GetBetInfo().bet);

            Debug.Log($"=== Current Bets ({_currentBets.Count}) ===");
            Debug.Log($"Starting Balance: {_startingBalance}, Total Bet: {GetBetInfo().bet}, Display Balance: {GetBetInfo().balance}");
            foreach (var bet in _currentBets)
            {
                Debug.Log($"Type: {bet.Type}, Color: {bet.Color}, Suit: {bet.Suit}, " +
                         $"Value: {bet.TableValue}, ChipValue: {bet.ChipValue}, Count: {bet.BetCount}");
            }
        }

        private (int bet, int balance) GetBetInfo()
        {
            int totalBet = _currentBets.Sum(bet => bet.ChipValue * bet.BetCount);
            return (totalBet, _startingBalance - totalBet);
        }

        private void ClearBets()
        {
            _mainView.ClearAllBets();
        }

        public void SwitchBackState()
        {
            _soundService.PlayEffectSound(SoundId.Click);
            _stateMachine.Enter<MainState>();
        }
        
        public void SwitchRouletteState()
        {
            _soundService.PlayEffectSound(SoundId.Click);
            
            _saveLoad.Progress.CurrentBalance = GetBetInfo().balance;
            
            List<BetData> betsCopy = _currentBets.Select(bet => bet.Clone()).ToList();
            
            _mainView.ResetGame();
            _currentBets.Clear();
            
            _stateMachine.Enter<RouletteState, List<BetData>>(betsCopy);
        }
        
        public void SwitchFibonacciState()
        {
            _soundService.PlayEffectSound(SoundId.Click);
            _stateMachine.Enter<FibonacciState>();
        }

        public List<BetData> GetCurrentBets()
        {
            return new List<BetData>(_currentBets);
        }
    }
}