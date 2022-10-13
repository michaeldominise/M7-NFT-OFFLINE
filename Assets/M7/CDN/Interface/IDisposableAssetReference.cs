using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.CDN
{
    public interface IDisposableAssetReference
    {
        AssetReference AssetReference { get; }
        void DisposeAssetReference();
        void ReleaseAsset();
    }
}