using M7.CDN;
using M7.CDN.Addressable;
using M7.GameData;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.GameRuntime
{
    [System.Serializable]
    public abstract class BaseRPGObjectInstance<BaseSaveableDataType> : InitializableInstance<BaseSaveableDataType>, IDisposableAssetReference where BaseSaveableDataType : BaseSaveableData
    {
        public BaseSaveableDataType SaveableData => ObjectData;
        public AssetReference AssetReference { get; private set; }

        public override void Init(BaseSaveableDataType objectData, Action onFinish)
        {
            if(SaveableData == objectData)
            {
                onFinish?.Invoke();
                return;
            }

            CleanInstance();

            ObjectData = objectData;
            SaveableData.onValuesChanged += RefreshNonAssetReferenceDisplay;

            var assetReferenceT = SaveableData.AssetReferenceData.GetAssetReference<RPGObject>();
            AssetReference = assetReferenceT;
            assetReferenceT.LoadAssetAsync(result =>
            {
                AddressableAssetDisposeManager.AddDisposableAssetReference(this);
                OnBaseRPGObjectReferenceLoaded(onFinish);
            });
            RefreshNonAssetReferenceDisplay();
        }

        public virtual void CleanInstance()
        {
            if(SaveableData != null)
                SaveableData.onValuesChanged -= RefreshNonAssetReferenceDisplay;
            DisposeAssetReference();
        }

        public virtual void DisposeAssetReference() => AddressableAssetDisposeManager.DisposeAssetReference(this);
        public virtual void ReleaseAsset() => AssetReference?.ReleaseAsset();
        public virtual void OnBaseRPGObjectReferenceLoaded(System.Action onFinish) => onFinish?.Invoke();
        public abstract void RefreshNonAssetReferenceDisplay();

        public void SetLayerRecursively(GameObject obj, int layer)
        {
            if (!obj)
                return;
            obj.layer = layer;
            foreach (Transform child in obj.transform)
                SetLayerRecursively(child.gameObject, layer);
        }
    }
}