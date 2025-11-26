using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Calendar.Scripts.Services.Assets
{
    public class AssetProvider : IAssets
    {
        private readonly Dictionary<string, AsyncOperationHandle> _completedCache = new Dictionary<string, AsyncOperationHandle>(10);
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new Dictionary<string, List<AsyncOperationHandle>>(10);

        public AssetProvider() => Addressables.InitializeAsync().ToUniTask();

        public async UniTask<T> Instantiate<T>(string address, Transform parent = null) where T : Object
        {
            var uniTask = await Addressables.InstantiateAsync(address, parent).ToUniTask() as T;
            return uniTask;
        }
        
        public async UniTask<T> Instantiate<T>(string address, Vector3 at, Quaternion rotation, Transform parent = null) where T : Object =>
            await Addressables.InstantiateAsync(address, at, rotation, parent).ToUniTask() as T;

        public async UniTask<T> LoadPersistent<T>(string address) where T : class => await Addressables.LoadAssetAsync<T>(address);

        public async UniTask<T> Load<T>(string address) where T : class
        {
            Debug.Log($"Loaded asset - {address}");
            if (_completedCache.TryGetValue(address, out AsyncOperationHandle completedHandle))
                return completedHandle.Result as T;

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
            return await RunWithCacheOnComplete(address, handle);
        }
        
        public async UniTask<T> Load<T>(AssetReference assetReference) where T : class
        {
            if (_completedCache.TryGetValue(assetReference.AssetGUID, out AsyncOperationHandle completedHandle))
                return completedHandle.Result as T;
            
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetReference);
            return await RunWithCacheOnComplete(assetReference.AssetGUID, handle);
        }

        public void CleanUp()
        {
            foreach (List<AsyncOperationHandle> handles in _handles.Values)
            foreach (AsyncOperationHandle handle in handles)
                Addressables.Release(handle);
            
            _completedCache.Clear();
            _handles.Clear();
        }

        private async UniTask<T> RunWithCacheOnComplete<T>(string cacheKey, AsyncOperationHandle<T> handle) where T : class
        {
            handle.Completed += completeHandle => _completedCache[cacheKey] = completeHandle;
            AddHandle(cacheKey, handle);
            return await handle.ToUniTask();
        }

        private void AddHandle<T>(string key, AsyncOperationHandle<T> handle) where T : class
        {
            if (!_handles.TryGetValue(key, out List<AsyncOperationHandle> handles))
            {
                handles = new List<AsyncOperationHandle>();
                _handles[key] = handles;
            }

            handles.Add(handle);
        }
    }
}