using Calendar.Scripts.Data.Enums;
using Calendar.Scripts.Services.EntityContainer;
using Calendar.Scripts.Services.LoadingCurtain;
using Calendar.Scripts.Structure.StateMachine.GameStateMachine;
using Calendar.Scripts.Structure.StateMachine.States;
using Game.Calendar.Scripts.Game.UI.MainScreen;
using Game.Calendar.Scripts.Services.SaveLoad;
using Game.Calendar.Scripts.Services.Sound;
using Game.Calendar.Scripts.Services.StaticData;
using Game.Calendar.Scripts.Structure.StateMachine.States.Bonus;
using Game.Calendar.Scripts.Structure.StateMachine.States.Game;

namespace Game.Calendar.Scripts.Structure.StateMachine.States.Main
{
    public class MainState: IState
    {
        protected ISaveLoad _saveLoad;
        protected ISoundService _soundService;
        protected IStaticData _staticData;
        protected IEntityContainer _entityContainer;
        protected ILoadingCurtain _loadingCurtain;
        
        private readonly IStateMachine _stateMachine;
        private MainView _mainView;
        
        public MainState(IStateMachine stateMachine, IEntityContainer entityContainer, 
            ILoadingCurtain loadingCurtain, IStaticData staticData, ISaveLoad saveLoad, ISoundService soundService)
        {
            _soundService = soundService;
            _stateMachine = stateMachine;
            _staticData = staticData;
            _entityContainer = entityContainer;
            _loadingCurtain = loadingCurtain;
            _saveLoad = saveLoad;
        }
        
        public void Enter()
        {
            _mainView = _entityContainer.GetEntity<MainView>();
            _mainView.SubscribeView();
            
            _mainView.OnPlayClick.AddListener(SwitchGameState);
            _mainView.OnRulesClick.AddListener(SwitchRulesState);
            _mainView.OnBonusClick.AddListener(SwitchBonusState);
            
            _mainView.UpdateSound(_soundService.IsSoundMuted);
            
            _mainView.Show();
            _loadingCurtain.Hide();
        }
        
        public void Exit()
        {
            _loadingCurtain.Show();
            _mainView.Hide();
            
            _mainView.OnPlayClick.RemoveAllListeners();
            _mainView.OnRulesClick.RemoveAllListeners();
            _mainView.OnBonusClick.RemoveAllListeners();
            
            _mainView.UnsubscribeView();
        }

        private void SwitchGameState()
        {
            _soundService.PlayEffectSound(SoundId.Click);
            _stateMachine.Enter<GameState>();
        }
        
        private void SwitchRulesState()
        {
            _soundService.PlayEffectSound(SoundId.Click);
            _stateMachine.Enter<RulesState>();
        }
        
        private void SwitchBonusState()
        {
            _soundService.PlayEffectSound(SoundId.Click);
            _stateMachine.Enter<BonusState>();
        }
    }
}