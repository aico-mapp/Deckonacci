using Calendar.Scripts.Services.Factories.BaseFactory;
using Cysharp.Threading.Tasks;
using Game.Calendar.Scripts.Game.UI;
using UnityEngine;

namespace Game.Calendar.Scripts.Services.Factories.UIFactory
{
    public interface IUIFactory : IBaseFactory, IGlobalService
    {
        UniTask<GameObject> CreateRootCanvas();
        UniTask WarmUpPersistent();
        UniTask CreateMainViews(Transform parent);
    }
}