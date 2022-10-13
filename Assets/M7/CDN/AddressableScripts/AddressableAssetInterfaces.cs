using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.CDN.Addressable
{
    public interface IAddressableAssetLoader
    {
        void LoadAddressableReferences();
        void UnloadAddressableReferences();
    }

    public interface IAddressableAssetListLoader
    {
        float LoadProgress { get; }
        void LoadAddressableReferences(params string[] masterID);
        void UnloadAddressableReferences(params string[] masterID);
    }

    public interface IAddressableAssetSetter
    {
        void SetAddressableReferences();
    }

    public interface IAddressableAssetReferences<T> where T : Object
    {
        List<AssetReferenceData<T>> AssetReferences { get; }
    }

    public interface IAddressableAssetMasterIDs
    {
        string[] AddressableAssetMasterIDs { get; }
    }
}
