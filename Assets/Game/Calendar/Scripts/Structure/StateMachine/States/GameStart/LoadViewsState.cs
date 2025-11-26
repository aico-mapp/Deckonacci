using Calendar.Scripts.Structure.StateMachine.GameStateMachine;
using Calendar.Scripts.Structure.StateMachine.States;
using Cysharp.Threading.Tasks;
using Game.Calendar.Scripts.Services.Factories.UIFactory;
using Game.Calendar.Scripts.Structure.StateMachine.States.Main;
using UnityEngine;

namespace Game.Calendar.Scripts.Structure.StateMachine.States.GameStart
{
    public class LoadViewsState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IUIFactory _uiFactory;

        public LoadViewsState(IStateMachine stateMachine, IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
            _stateMachine = stateMachine;
        }

        public async void Enter()
        {
            await CreateGame();
            _stateMachine.Enter<MainState>();
        }

        private async UniTask CreateGame()
        {
            GameObject rootCanvas = await _uiFactory.CreateRootCanvas();
            rootCanvas.GetComponent<Canvas>().worldCamera = Camera.current;
            rootCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
            await _uiFactory.CreateMainViews(rootCanvas.transform);
        }

        public void Exit()
        {
        }
    }
}