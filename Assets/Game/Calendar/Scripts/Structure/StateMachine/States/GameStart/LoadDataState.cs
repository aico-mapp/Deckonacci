using Calendar.Scripts.Services.EntityContainer;
using Calendar.Scripts.Services.SceneLoader;
using Calendar.Scripts.Structure.StateMachine.GameStateMachine;
using Calendar.Scripts.Structure.StateMachine.States;
using Cysharp.Threading.Tasks;
using Game.Calendar.Scripts.Game.UI;
using Game.Calendar.Scripts.Services.Factories.GameFactory;
using Game.Calendar.Scripts.Services.Factories.UIFactory;
using Game.Calendar.Scripts.Services.SaveLoad;
using Game.Calendar.Scripts.Services.Sound;
using Game.Calendar.Scripts.Services.StaticData;
using UnityEngine;

namespace Game.Calendar.Scripts.Structure.StateMachine.States.GameStart
{
    public class LoadDataState : IState
    {
        private readonly IStateMachine _stateMachine;
        private readonly IStaticData _staticData;
        private readonly ISaveLoad _saveLoad;
        private readonly IUIFactory _uiFactory;
        private readonly IGameFactory _gameFactory;
        private readonly ISoundService _soundService;
        private readonly ISceneLoader _sceneLoader;
        private readonly IEntityContainer _entityContainer;
        private const string MenuScene = "Menu";

        public LoadDataState(IStateMachine stateMachine, IStaticData staticData, ISaveLoad saveLoad,
            IUIFactory uiFactory, IGameFactory gameFactory, ISoundService soundService, ISceneLoader sceneLoader, IEntityContainer entityContainer)
        {
            _staticData = staticData;
            _saveLoad = saveLoad;
            _uiFactory = uiFactory;
            _gameFactory = gameFactory;
            _soundService = soundService;
            _stateMachine = stateMachine;
            _sceneLoader = sceneLoader;
            _entityContainer = entityContainer;
        }

        public async void Enter()
        {
            _saveLoad.Load();
            _soundService.Construct(_saveLoad, _staticData.SoundData);
            //_soundService.PlayBackgroundMusic();
            await CreatePersistentEntities();
            _sceneLoader.LoadScene(MenuScene, PrepareGame);
        }

        private async void PrepareGame()
        {
            await CreateGame();
        }


        private async UniTask CreateGame()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            
            //await _uiFactory.WarmUpMainMenu();
            GameObject rootCanvas = await _uiFactory.CreateRootCanvas();
            _stateMachine.Enter<LoadViewsState>();
        }

        public void Exit()
        {
        }

        private async UniTask CreatePersistentEntities()
        {
            await _uiFactory.WarmUpPersistent();
            await CreatePersistentCanvas();
        }

        private async UniTask<GameObject> CreatePersistentCanvas()
        {
            GameObject persistentCanvas = await _uiFactory.CreateRootCanvas();
            persistentCanvas.GetComponent<Canvas>().sortingOrder = 10;
            persistentCanvas.name = "PersistentCanvas";
            Object.DontDestroyOnLoad(persistentCanvas);
            return persistentCanvas;
        }
    }
}