using System.Threading;
using Calendar.Scripts.Data.Enums;
using Calendar.Scripts.Services.EntityContainer;
using Calendar.Scripts.Services.LoadingCurtain;
using Calendar.Scripts.Structure.StateMachine.GameStateMachine;
using Calendar.Scripts.Structure.StateMachine.States;
using Game.Calendar.Scripts.Game.UI.FibonacciScreen;
using Game.Calendar.Scripts.Services.Factories.GameFactory;
using Game.Calendar.Scripts.Services.SaveLoad;
using Game.Calendar.Scripts.Services.Sound;
using Game.Calendar.Scripts.Services.StaticData;
using Game.Calendar.Scripts.Structure.StateMachine.States.Game;
using Game.Calendar.Scripts.Structure.StateMachine.States.Main;
using Game.Scripts.Animations;

namespace Game.Calendar.Scripts.Structure.StateMachine.States.Fibonacci
{
    public class FibonacciState : IState
    {
        protected ISaveLoad _saveLoad;
        protected ISoundService _soundService;
        protected IStaticData _staticData;
        protected IEntityContainer _entityContainer;
        protected ILoadingCurtain _loadingCurtain;
        protected IGameFactory _gameFactory;
        
        private readonly IStateMachine _stateMachine;
        private FibonacciView _mainView;
        
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        
        public FibonacciState(IStateMachine stateMachine, IEntityContainer entityContainer, 
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
            _mainView = _entityContainer.GetEntity<FibonacciView>();
            _mainView.SubscribeView();
            
            _mainView.OnBackClick.AddListener(SwitchBackState);
            _mainView.OnGetClick.AddListener(SwitchBackState);
            
            _mainView.UpdateSound(_soundService.IsSoundMuted);
            
            _mainView.Show();
            _loadingCurtain.Hide();
            
            _mainView.StartGame(OnGameEnd);
        }

        private void OnGameEnd(int playerScore)
        {
            if (playerScore != 0)
            {
                _saveLoad.Progress.CurrentBalance *= playerScore;
            }
        }
        
        public void Exit()
        {
            _mainView.HideResult();
            
            _loadingCurtain.Show();
            _mainView.Hide();
            
            _mainView.OnBackClick.RemoveAllListeners();
            _mainView.OnGetClick.RemoveAllListeners();

            _mainView.UnsubscribeView();
        }

        public void SwitchBackState()
        {
            _soundService.PlayEffectSound(SoundId.Click);
            _stateMachine.Enter<GameState>();
        }
    }
}