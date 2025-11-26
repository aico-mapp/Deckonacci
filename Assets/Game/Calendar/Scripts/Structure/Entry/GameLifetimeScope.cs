using System;
using Calendar.Scripts.Services.LoadingCurtain;
using Calendar.Scripts.Structure.Entry;
using Game.Calendar.Scripts.Services.Sound;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using TypeExtensions = Calendar.Scripts.Extensions.TypeExtensions;

namespace Game.Calendar.Scripts.Structure.Entry
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private SoundService _soundService;
        [SerializeField] private LoadingCurtain _loadingCurtain;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterEntryPoint(builder);
            RegisterInstanceServices(builder);
            RegisterServices(builder);
            RegisterStates(builder);
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        private static void RegisterEntryPoint(IContainerBuilder builder) => builder.RegisterEntryPoint<GameEntry>();

        private void RegisterInstanceServices(IContainerBuilder builder)
        {
            builder.RegisterComponent(_soundService).AsImplementedInterfaces();
            builder.RegisterComponent(_loadingCurtain).AsImplementedInterfaces();
        }

        private static void RegisterServices(IContainerBuilder builder)
        {
            foreach ((Type serviceImplementation, Type serviceInterface) in TypeExtensions.GetAllGlobalServiceTypes())
                builder.Register(serviceImplementation, Lifetime.Singleton).As(serviceInterface);
        }

        private static void RegisterStates(IContainerBuilder builder)
        {
            foreach (Type stateType in TypeExtensions.GetAllStatesTypes())
                builder.Register(stateType, Lifetime.Singleton);
        }
    }
}