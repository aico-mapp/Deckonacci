using Calendar.Scripts.Services.Factories.StateFactory;
using Calendar.Scripts.Structure.StateMachine.GameStateMachine;
using Game.Calendar.Scripts.Structure.StateMachine.States.GameStart;
using VContainer.Unity;

namespace Calendar.Scripts.Structure.Entry
{
    public class GameEntry : IStartable
    {
        private readonly IStateMachine _stateMachine;
        private readonly IStateFactory _stateFactory;

        public GameEntry(IStateMachine stateMachine, IStateFactory stateFactory)
        {
            _stateMachine = stateMachine;
            _stateFactory = stateFactory;
        }
        
        public void Start()
        {
            _stateFactory.CreateAllStates();
            _stateMachine.Enter<LoadDataState>(); 
        }
    }
}