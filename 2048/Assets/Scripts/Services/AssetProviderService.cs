using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services
{
    public class AssetProviderService : IAssetProviderService
    {
        public T LoadAssetFromResources<T>(string path) where T : UnityEngine.Object =>
            Resources.Load<T>(path);
        
        public async UniTask<T> LoadAssetAsync<T>(string path) where T : Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(path);
            await request.ToUniTask();
            return request.asset as T;
        }
    }
}