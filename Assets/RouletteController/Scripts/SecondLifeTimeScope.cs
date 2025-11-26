using Mode.Scripts.Services;
using Mode.Scripts.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Mode.Scripts
{
    public class SecondLifeTimeScope : LifetimeScope
    {
        [SerializeField] private OrientationChanger orientationChanger;
        [SerializeField] private WebViewResizer webViewResizer;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(orientationChanger).As<OrientationChanger>();
            builder.RegisterInstance(webViewResizer).As<WebViewResizer>();
            builder.Register<Setter>(Lifetime.Singleton);
        }

        private void Start()
        {
            Container.Resolve<Setter>().Set();
        }
    }
}