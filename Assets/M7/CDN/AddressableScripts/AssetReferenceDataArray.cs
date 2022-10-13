using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace M7.CDN.Addressable
{
    [Serializable]
    public class AssetReferenceDataArray<T> where T : UnityEngine.Object
    {
        public static implicit operator AssetReferenceData<T>[](AssetReferenceDataArray<T> assetReferenceDataArray) => assetReferenceDataArray.assetReferences;

        [ReadOnly, SerializeField] AssetReferenceData<T>[] assetReferences = new AssetReferenceData<T>[0];
        public AssetReferenceData<T>[] AssetReferences => assetReferences;

        public int Length => assetReferences?.Length ?? 0;
        public AssetReferenceData<T> this[int key]
        {
            get => AssetReferences[key];
            set => AssetReferences[key] = value;
        }

#if UNITY_EDITOR
        [ShowInInspector]
        public T[] Assets
        {
            get => assetReferences.Select(assetReferene => assetReferene.Asset).ToArray();
            set
            {
                assetReferences = value.Select(obj 
                    => new AssetReferenceData<T>(obj))
                    .ToArray();
                AddressableAssetHelper.MarkAssetAsAddressable(value, true);
            }
        }
#endif
    }
}
