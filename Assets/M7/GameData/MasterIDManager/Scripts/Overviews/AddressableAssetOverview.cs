using M7.CDN.Addressable;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace M7.GameData
{
    [System.Serializable]
    public class AddressableAssetOverview<T> : AddressableAssetOverview where T : ScriptableObject
    {
        [SerializeField, ReadOnly] AssetReferenceDataArray<T> AllData = new AssetReferenceDataArray<T>();

#if UNITY_EDITOR
        public override void UpdateOverview()
        {
            AllData.Assets = AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
        }
#endif

        public AssetReferenceData<T> FindAssetReference(string assetName) => AllData.FindAssetReference(assetName);
        public AssetReferenceData<T>[] FindAssetReferences(string[] assetNames) => AllData.FindAssetReferences(assetNames);
    }

    public class AddressableAssetOverview : IUpdateOverview, IUpdateLocalizationKeys
    {
        public virtual void UpdateOverview() { }
        public virtual void UpdateLocalizationKeys() { }
    }
}
