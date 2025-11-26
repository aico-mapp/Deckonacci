using System.Collections.Generic;
using Calendar.Scripts.Services.Assets;
using Calendar.Scripts.Services.EntityContainer;
using Calendar.Scripts.Services.Factories.BaseFactory;
using Calendar.Scripts.Structure.StateMachine.GameStateMachine;
using Cysharp.Threading.Tasks;
using Game.Calendar.Scripts.Game;
using Game.Calendar.Scripts.Game.UI.BonusScreen;
using Game.Calendar.Scripts.Game.UI.FibonacciScreen;
using Game.Calendar.Scripts.Game.UI.GameScreen;
using Game.Calendar.Scripts.Game.UI.MainScreen;
using Game.Calendar.Scripts.Game.UI.RouletteScreen;
using Game.Calendar.Scripts.Game.UI.RulesScreen;
using Game.Calendar.Scripts.Services.SaveLoad;
using Game.Calendar.Scripts.Services.Sound;
using Game.Calendar.Scripts.Services.StaticData;
using Game.Scripts.Game;
using UnityEngine;


namespace Game.Calendar.Scripts.Services.Factories.UIFactory
{
    public class UIFactory: BaseFactory, IUIFactory
    {
        private readonly IStaticData _staticData;
        private readonly IStateMachine _stateMachine;
        private readonly ISoundService _soundService;
        private readonly ISaveLoad _saveLoad;
        
        private const string RootCanvasKey = "RootCanvas";
        
        public UIFactory(IStaticData staticData, IAssets assets, IEntityContainer entityContainer, 
            ISoundService soundService, ISaveLoad saveLoad, IStateMachine stateMachine)
            : base(assets, entityContainer)
        {
            _staticData = staticData;
            _soundService = soundService;
            _saveLoad = saveLoad;
            _stateMachine = stateMachine;
        }

        public async UniTask WarmUpPersistent()
        {
            await _assets.LoadPersistent<GameObject>(RootCanvasKey);
        }

        public async UniTask<GameObject> CreateRootCanvas() => await _assets.Instantiate<GameObject>(RootCanvasKey);

        public async UniTask CreateMainViews(Transform parent)
        {
            await CreateMainScreenView(parent);
            await CreateRulesScreenView(parent);
            await CreateGameScreenView(parent);
            await CreateBonusScreenView(parent);
            await CreateRouletteScreenView(parent);
            await CreateFibonacciScreenView(parent);
        }
        
        private async UniTask CreateMainScreenView(Transform parent)
        {
            MainView view = await InstantiateAsRegistered<MainView>(parent);
            view.Construct(_soundService);
            view.Hide();
        }
        
        private async UniTask CreateRulesScreenView(Transform parent)
        {
            RulesView view = await InstantiateAsRegistered<RulesView>(parent);
            view.Construct(_soundService);
            view.Hide();
        }
        
        private async UniTask CreateGameScreenView(Transform parent)
        {
            GameView view = await InstantiateAsRegistered<GameView>(parent);
            view.Construct(_soundService);
            view.Hide();
        }
        
        private async UniTask CreateBonusScreenView(Transform parent)
        {
            BonusView view = await InstantiateAsRegistered<BonusView>(parent);
            view.Construct(_soundService, _saveLoad.Progress);
            view.Hide();
        }
        
        private async UniTask CreateRouletteScreenView(Transform parent)
        {
            RouletteView view = await InstantiateAsRegistered<RouletteView>(parent);
            view.Construct(_soundService);
            view.Hide();
        }
        
        private async UniTask CreateFibonacciScreenView(Transform parent)
        {
            FibonacciView view = await InstantiateAsRegistered<FibonacciView>(parent);
            view.Construct(_soundService);
            view.Hide();
        }
    }
}