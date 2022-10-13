using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.CDN.Addressable
{
    public class AddressableAssetUtilityLoader : MonoBehaviour
    {
        public void WaitForAddressableAsset<T>(AssetReferenceT<T> assetReference, System.Action<T> onfinish) where T : Object
        {
            StartCoroutine(_WaitForAddressableAsset(assetReference, onfinish));
        }

        IEnumerator _WaitForAddressableAsset<T>(AssetReferenceT<T> assetReference, System.Action<T> onfinish) where T : Object
        {
            yield return new WaitWhile(() => assetReference.Asset == null);
            onfinish?.Invoke(assetReference.Asset as T);
        }
    }
}
