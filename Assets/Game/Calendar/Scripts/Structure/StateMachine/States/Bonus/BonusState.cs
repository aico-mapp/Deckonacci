using System.Threading;
using Calendar.Scripts.Data.Enums;
using Calendar.Scripts.Services.EntityContainer;
using Calendar.Scripts.Services.LoadingCurtain;
using Calendar.Scripts.Structure.StateMachine.GameStateMachine;
using Calendar.Scripts.Structure.StateMachine.States;
using Game.Calendar.Scripts.Game.UI.BonusScreen;
using Game.Calendar.Scripts.Game.UI.GameScreen;
using Game.Calendar.Scripts.Services.Factories.GameFactory;
using Game.Calendar.Scripts.Services.SaveLoad;
using Game.Calendar.Scripts.Services.Sound;
using Game.Calendar.Scripts.Services.StaticData;
using Game.Calendar.Scripts.Structure.StateMachine.States.Main;

namespace Game.Calendar.Scripts.Structure.StateMachine.States.Bonus
{
    public class BonusState: IState
    {
        protected ISaveLoad _saveLoad;
        protected ISoundService _soundService;
        protected IStaticData _staticData;
        protected IEntityContainer _entityContainer;
        protected ILoadingCurtain _loadingCurtain;
        protected IGameFactory _gameFactory;
        
        private readonly IStateMachine _stateMachine;
        private BonusView _mainView;
        
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        
        public BonusState(IStateMachine stateMachine, IEntityContainer entityContainer, 
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
            _mainView = _entityContainer.GetEntity<BonusView>();
            _mainView.SubscribeView();
            
            _mainView.OnBackClick.AddListener(SwitchBackState);
            _mainView.OnRewardAdded += SetRewards;
            
            _mainView.UpdateSound(_soundService.IsSoundMuted);
            
            _mainView.Show();
            _loadingCurtain.Hide();
        }
        
        public void Exit()
        {
            _loadingCurtain.Show();
            _mainView.Hide();

            _mainView.HideResultPopup();
            
            _mainView.OnBackClick.RemoveAllListeners();
            _mainView.OnRewardAdded -= SetRewards;

            _mainView.UnsubscribeView();
        }
        
        private void SetRewards(int reward)
        {
            _saveLoad.Progress.CurrentBalance += reward;
            SwitchBackState();
        }

        public void SwitchBackState()
        {
            _soundService.PlayEffectSound(SoundId.Click);
            _stateMachine.Enter<MainState>();
        }
    }
}