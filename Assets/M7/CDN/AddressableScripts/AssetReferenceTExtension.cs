using M7.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace M7.CDN.Addressable
{
    public static class AssetReferenceTExtension
    {
        //public static bool IsCharacter(this AssetReferenceObject assetReference) { return assetReference != null && assetReference.IsTypeOf(typeof(CharacterObject)); }
        //public static bool IsEquipable(this AssetReferenceObject assetReference) { return assetReference != null && assetReference.IsTypeOf(typeof(EquipableObject)); }
        //public static bool IsCurrency(this AssetReferenceObject assetReference) { return assetReference != null && assetReference.IsTypeOf(typeof(CurrencyObject)); }
        //public static bool IsBuilding(this AssetReferenceObject assetReference) { return assetReference != null && assetReference.IsTypeOf(typeof(BuildingObject)); }

        public static AssetReferenceData<T> FindAssetReference<T>(this AssetReferenceDataArray<T> rpgObjectReferenceArray, string assetName) where T : Object
        {
            return rpgObjectReferenceArray.AssetReferences.FindAssetReferences(new string[] { assetName })[0];
        }

        public static AssetReferenceData<T>[] FindAssetReferences<T>(this AssetReferenceDataArray<T> rpgObjectReferenceArray, string[] assetNames) where T : Object
        {
            return rpgObjectReferenceArray.AssetReferences.FindAssetReferences(assetNames);
        }

        public static AssetReferenceData<T> FindAssetReference<T>(this ICollection<AssetReferenceData<T>> RPGObjectReference, string assetName) where T : Object
        {
            return RPGObjectReference.FindAssetReferences(new string[] { assetName })[0];
        }

        public static AssetReferenceData<T>[] FindAssetReferences<T>(this ICollection<AssetReferenceData<T>> RPGObjectReferences, string[] assetNames) where T : Object
        {
            var dataList = new AssetReferenceData<T>[assetNames?.Length ?? 0];
            for (var i1 = 0; i1 < dataList.Length; i1++)
            {
                var assetName = MasterIDManager.GetAlternativeAssetName(assetNames[i1]);
                if (string.IsNullOrEmpty(assetName?.Trim()))
                {
                    dataList[i1] = null;
                    continue;
                }

                var enumerator = RPGObjectReferences.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var RPGObjectReference = enumerator.Current;
                    if (RPGObjectReference == null)
                        continue;

                    if (RPGObjectReference.AssetName == assetName)
                    {
                        dataList[i1] = RPGObjectReference;
                        break;
                    }
                }
                //Debug.Assert(dataList[i1] != null, string.Format("Can't find the assetName: \"{0}\" in the RPGObjectReference List", assetName));
            }
            return dataList;
        }
    }
}
