using Calendar.Scripts.Data.Enums;
using Calendar.Scripts.Services.EntityContainer;
using Calendar.Scripts.Services.LoadingCurtain;
using Calendar.Scripts.Structure.StateMachine.GameStateMachine;
using Calendar.Scripts.Structure.StateMachine.States;
using Game.Calendar.Scripts.Game.UI.MainScreen;
using Game.Calendar.Scripts.Game.UI.RulesScreen;
using Game.Calendar.Scripts.Services.SaveLoad;
using Game.Calendar.Scripts.Services.Sound;
using Game.Calendar.Scripts.Services.StaticData;
using Game.Calendar.Scripts.Structure.StateMachine.States.Bonus;
using Game.Calendar.Scripts.Structure.StateMachine.States.Game;

namespace Game.Calendar.Scripts.Structure.StateMachine.States.Main
{
    public class RulesState: IState
    {
        protected ISaveLoad _saveLoad;
        protected ISoundService _soundService;
        protected IStaticData _staticData;
        protected IEntityContainer _entityContainer;
        protected ILoadingCurtain _loadingCurtain;
        
        private readonly IStateMachine _stateMachine;
        private RulesView _mainView;
        
        public RulesState(IStateMachine stateMachine, IEntityContainer entityContainer, 
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
            _mainView = _entityContainer.GetEntity<RulesView>();
            _mainView.SubscribeView();
            
            _mainView.OnBackClick.AddListener(SwitchBackState);
            
            _mainView.UpdateSound(_soundService.IsSoundMuted);
            _mainView.ResetRules();
            
            _mainView.Show();
            _loadingCurtain.Hide();
        }
        
        public void Exit()
        {
            _loadingCurtain.Show();
            _mainView.Hide();
            
            _mainView.OnBackClick.RemoveAllListeners();
            
            _mainView.UnsubscribeView();
        }

        private void SwitchBackState()
        {
            _soundService.PlayEffectSound(SoundId.Click);
            _stateMachine.Enter<MainState>();
        }
        
        private void SwitchBonusState()
        {
            _soundService.PlayEffectSound(SoundId.Click);
            _stateMachine.Enter<BonusState>();
        }
    }
}