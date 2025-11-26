using Cysharp.Threading.Tasks;
using Game.Calendar.Scripts.Services;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Calendar.Scripts.Services.Assets
{
    public interface IAssets : IGlobalService
    {
        UniTask<T> Load<T>(string address) where T : class;
        UniTask<T> Load<T>(AssetReference assetReference) where T : class;
        void CleanUp();
        UniTask<T> Instantiate<T>(string address, Transform parent = null) where T : Object;
        UniTask<T> Instantiate<T>(string address, Vector3 at, Quaternion rotation, Transform parent = null) where T : Object;
        UniTask<T> LoadPersistent<T>(string address) where T : class;
    }
}