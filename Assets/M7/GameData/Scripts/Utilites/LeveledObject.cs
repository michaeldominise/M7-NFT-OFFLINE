using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using M7.CDN.Addressable;

namespace M7.GameData
{
    [System.Serializable]
    public class LeveledAssetReferenceObject<T> where T : Object
    {
        [TableList]
        public AssetReferenceDataArray<T> assetReferennces;

        public AssetReferenceData<T> GetAssetReference(int level)
        {
            return assetReferennces.Length == 0 ? null : assetReferennces[level];
        }
    }
}