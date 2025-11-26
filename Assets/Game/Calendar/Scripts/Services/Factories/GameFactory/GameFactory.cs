using Calendar.Scripts.Services.Assets;
using Calendar.Scripts.Services.EntityContainer;
using Calendar.Scripts.Services.Factories.BaseFactory;
using Calendar.Scripts.Structure.StateMachine.GameStateMachine;
using Cysharp.Threading.Tasks;
using Game.Calendar.Scripts.Services.SaveLoad;
using Game.Calendar.Scripts.Services.Sound;
using Game.Calendar.Scripts.Services.StaticData;
using Game.Scripts.Game;
using UnityEngine;

namespace Game.Calendar.Scripts.Services.Factories.GameFactory
{
    public class GameFactory: BaseFactory, IGameFactory
    {
        private readonly ISoundService _soundService;
        private readonly IStaticData _staticData;
        private readonly ISaveLoad _saveLoad;
        private readonly IStateMachine _stateMachine;
        
        private const string SphereObjectKey = "GameSphere";
        
        private GameObject _gameSphere;

        public GameFactory(IAssets assets, IEntityContainer entityContainer, ISoundService soundService, 
            IStaticData staticData, ISaveLoad saveLoad, IStateMachine stateMachine) : base(assets, entityContainer)
        {
            _soundService = soundService;
            _staticData = staticData;
            _saveLoad = saveLoad;
            _stateMachine = stateMachine;
        }
    }
}