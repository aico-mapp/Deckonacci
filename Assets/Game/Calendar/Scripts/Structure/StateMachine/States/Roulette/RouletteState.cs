using System.Collections.Generic;
using System.Threading;
using Calendar.Scripts.Data.Enums;
using Calendar.Scripts.Services.EntityContainer;
using Calendar.Scripts.Services.LoadingCurtain;
using Calendar.Scripts.Structure.StateMachine.GameStateMachine;
using Calendar.Scripts.Structure.StateMachine.States;
using Cysharp.Threading.Tasks;
using Game.Calendar.Scripts.Data;
using Game.Calendar.Scripts.Data.StaticData;
using Game.Calendar.Scripts.Game.Table;
using Game.Calendar.Scripts.Game.UI.GameScreen;
using Game.Calendar.Scripts.Game.UI.RouletteScreen;
using Game.Calendar.Scripts.Services.Factories.GameFactory;
using Game.Calendar.Scripts.Services.SaveLoad;
using Game.Calendar.Scripts.Services.Sound;
using Game.Calendar.Scripts.Services.StaticData;
using Game.Calendar.Scripts.Structure.StateMachine.States.Fibonacci;
using Game.Calendar.Scripts.Structure.StateMachine.States.Game;
using Game.Calendar.Scripts.Structure.StateMachine.States.Main;
using UnityEngine;

namespace Game.Calendar.Scripts.Structure.StateMachine.States.Roulette
{
    public class RouletteState: IPayloadedState<List<BetData>>
    {
        protected ISaveLoad _saveLoad;
        protected ISoundService _soundService;
        protected IStaticData _staticData;
        protected IEntityContainer _entityContainer;
        protected ILoadingCurtain _loadingCurtain;
        protected IGameFactory _gameFactory;
        
        private readonly IStateMachine _stateMachine;
        private RouletteView _mainView;

        private List<BetData> _betsData;
        private RouletteReward _currentReward;
        
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        
        public RouletteState(IStateMachine stateMachine, IEntityContainer entityContainer, 
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
        
        public void Enter(List<BetData> betsData)
        {
            _betsData = new List<BetData>(betsData);
            
            _mainView = _entityContainer.GetEntity<RouletteView>();
            _mainView.SubscribeView();
            
            _mainView.OnBackClick.AddListener(SwitchBackState);
            _mainView.OnRouletteResulted += SetResult;
            _mainView.OnPayoutCalculated += SetCurrentReward;
            
            _mainView.UpdateSound(_soundService.IsSoundMuted);
            _mainView.SetBets(_betsData);
            
            SpinRoulette();
            
            _mainView.Show();
            _loadingCurtain.Hide();
        }
        
        public void Exit()
        {
            _loadingCurtain.Show();
            _mainView.Hide();
            
            _betsData.Clear();
            
            _mainView.OnBackClick.RemoveAllListeners();
            _mainView.OnRouletteResulted -= SetResult;
            _mainView.OnPayoutCalculated -= SetCurrentReward;
            
            _mainView.HideResult();

            _mainView.UnsubscribeView();
        }
        
        private void SetResult()
        {
            if(_currentReward.RewardType == CategoryType.Fibonacci)
                SwitchFibonacciState();
            else
                SwitchBackState();
        }

        private void SetCurrentReward(RouletteReward reward, int payout)
        {
            _currentReward = reward;
            _saveLoad.Progress.CurrentBalance += payout;
        }
        
        private async void SpinRoulette()
        {
            await UniTask.DelayFrame(1);
            _mainView.TrySpin();
        }

        public void SwitchBackState()
        {
            _soundService.PlayEffectSound(SoundId.Click);
            _stateMachine.Enter<GameState>();
        }
        
        public void SwitchFibonacciState()
        {
            _soundService.PlayEffectSound(SoundId.Click);
            _stateMachine.Enter<FibonacciState>();
        }
    }
}