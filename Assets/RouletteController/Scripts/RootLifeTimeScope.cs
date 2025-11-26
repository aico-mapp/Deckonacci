using Mode.Scripts.Analytics;
using Mode.Scripts.Data;
using Mode.Scripts.Firebase;
using Mode.Scripts.Http;
using Mode.Scripts.Localization;
using Mode.Scripts.Message;
using Mode.Scripts.Network;
using Mode.Scripts.Notifications;
using Mode.Scripts.Services;
using Mode.Scripts.Timer;
using Mode.Scripts.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mode.Scripts
{
    public class RootLifeTimeScope : LifetimeScope
    {
        [SerializeField] private AppConfiguration appConfiguration;
        [SerializeField] private ToolbarView toolbarView;
        [SerializeField] private LoadingScreenView loadingScreenView;
        [SerializeField] private NotificationHandler notificationHandler;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(appConfiguration).AsSelf();
            builder.RegisterInstance(toolbarView).AsSelf();
            builder.RegisterInstance(loadingScreenView).AsSelf();
            builder.RegisterInstance(notificationHandler).AsSelf();
            ConfigureModels(builder);
            ConfigureServices(builder);
        }

        private void ConfigureServices(IContainerBuilder builder)
        {
            builder.Register<HttpService>(Lifetime.Singleton);
            builder.Register<AnalyticsService>(Lifetime.Singleton);
            builder.Register<FirebaseService>(Lifetime.Singleton);
            builder.Register<RefreshService>(Lifetime.Singleton);
            builder.Register<LocalizationService>(Lifetime.Singleton);
            builder.Register<MessageService>(Lifetime.Singleton);
            builder.Register<AppService>(Lifetime.Singleton);
            builder.Register<DataService>(Lifetime.Singleton);
            builder.Register<PSMService>(Lifetime.Singleton);
            builder.Register<LinkService>(Lifetime.Singleton);
            builder.Register<NetworkService>(Lifetime.Singleton);
            builder.Register<ConversionService>(Lifetime.Singleton);
            builder.Register<EnvService>(Lifetime.Singleton);
            builder.Register<LoadService>(Lifetime.Singleton);
            builder.Register<ConfigurationService>(Lifetime.Singleton);
            builder.Register<StarterService>(Lifetime.Singleton);
            builder.Register<TimerService>(Lifetime.Singleton);
            builder.Register<LoadingScreenController>(Lifetime.Singleton);
            builder.Register<ToolbarController>(Lifetime.Singleton);
            builder.Register<NotificationService>(Lifetime.Singleton);
            builder.Register<EventHandler>(Lifetime.Singleton);
        }

        private void ConfigureModels(IContainerBuilder builder)
        {
            builder.Register<AnalyticsModel>(Lifetime.Singleton);
            builder.Register<LoadedProjectData>(Lifetime.Singleton);
            builder.Register<StarterModel>(Lifetime.Singleton);
            builder.Register<LoadingScreenModel>(Lifetime.Singleton);
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            Container.Resolve<AppService>().Run();
        }
    }
}