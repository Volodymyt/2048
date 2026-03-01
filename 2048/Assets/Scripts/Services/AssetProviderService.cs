using UnityEngine;

namespace Services
{
    public class AssetProviderService : IAssetProviderService
    {
        public T LoadAssetFromResources<T>(string path) where T : Object =>
            Resources.Load<T>(path);
    }
}