
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.GUI;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
#endif
using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace M7.CDN.Addressable
{
	public static class AddressableAssetUtility
    {
        static AddressableAssetUtilityLoader _AddressableAssetUtilityLoader;
        static AddressableAssetUtilityLoader AddressableAssetUtilityLoader
        {
            get
            {
                if (!_AddressableAssetUtilityLoader)
                {
                    _AddressableAssetUtilityLoader = new GameObject().AddComponent<AddressableAssetUtilityLoader>();
                    _AddressableAssetUtilityLoader.gameObject.name = "AddressableAssetUtilityLoader";
                }
                return _AddressableAssetUtilityLoader;
            }
        }

        public static void LoadAssetAsync<TObject>(this AssetReferenceT<TObject> assetReference, System.Action<TObject> onComplete) where TObject : Object
        {
            if(assetReference == null)
            {
                onComplete(default);
                return;
            }

            if (assetReference.IsValid())
            {
                if (assetReference.Asset != null)
                    onComplete?.Invoke(assetReference.Asset as TObject);
                else
                    AddressableAssetUtilityLoader.WaitForAddressableAsset(assetReference, onComplete);
            }
            else if (!string.IsNullOrWhiteSpace(assetReference.AssetGUID))
                assetReference.LoadAssetAsync<TObject>().Completed += operationHandle => onComplete?.Invoke(operationHandle.Result);
            else
                onComplete(default);
        }

        public static void LoadAddressableAssets<TObject>(AssetReferenceT<TObject>[] assetReferences, System.Action<AsyncOperationStatus> onComplete = null) where TObject : Object
        {
            if (assetReferences == null)
            {
                onComplete?.Invoke(AsyncOperationStatus.None);
                return;
            }

            var loadedCount = 0;
            foreach (var assetReference in assetReferences)
            {
                if(assetReference == null)
                {
                    loadedCount++;
                    if (loadedCount == assetReferences.Length)
                        onComplete?.Invoke(AsyncOperationStatus.Succeeded);
                    continue;
                }

                assetReference.LoadAssetAsync(result =>
                {
                    loadedCount++;
                    if (loadedCount == assetReferences.Length)
                        onComplete?.Invoke(AsyncOperationStatus.Succeeded);
                });
            }
        }
    }
}