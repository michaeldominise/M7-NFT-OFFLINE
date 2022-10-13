using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Linq;
using UnityEngine.AddressableAssets.ResourceLocators;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.GUI;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine.AddressableAssets;
#endif
using UnityEngine;

namespace M7.CDN.Addressable
{
    public static class CDN_LABLES_EXTENSION
    {
        public static string ToAddressableLabel(this AddressableAssetHelper.CDN_LABELS cdnLabel)
        {
            if (cdnLabel == AddressableAssetHelper.CDN_LABELS.default_additive || cdnLabel == AddressableAssetHelper.CDN_LABELS.default_replace)
                return "default";
            else
                return cdnLabel.ToString();
        }
    }

    public class AddressableAssetHelper : ScriptableObject
    {
        const string remoteGroupName = "Remote Group";
        const string helperDirectoryPath = "Assets/AddressableAssetsData/AddressableAssetHelper/";
        const string helperFileName = "AddressableAssetHelper.asset";
        const string helperPath = helperDirectoryPath + helperFileName;

        public enum CDN_LABELS
        {
            default_additive,
            default_replace,
            cdn1,
            cdn2,
            cdn3
        }

        static AddressableAssetHelper _Instance;
        public static AddressableAssetHelper Instance
        {
            get
            {
#if UNITY_EDITOR
                if(_Instance == null)
                {
                    _Instance = AssetDatabase.LoadAssetAtPath<AddressableAssetHelper>(helperPath);
                    if (_Instance == null)
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(helperDirectoryPath)))
                            Directory.CreateDirectory(helperDirectoryPath);
                        AssetDatabase.CreateAsset(CreateInstance<AddressableAssetHelper>(), helperPath);
                    }
                }
#endif
                return Instance;
            }
        }

#if UNITY_EDITOR
        [SerializeField] AddressableAssetGroup _addressableAssetGroupReference;

        static AddressableAssetSettings DefaultAddressableSettings { get { return AddressableAssetSettingsDefaultObject.Settings; } }
#endif

        public static void MarkAssetAsAddressable(Object[] assetObjects, bool isMarkAsAssetAddressable, CDN_LABELS label = CDN_LABELS.default_additive)
        {
#if UNITY_EDITOR
            for (int i = 0; i < assetObjects.Length; i++)
            {
                Object assetObject = assetObjects[i];
                EditorUtility.DisplayProgressBar("Mark Asset As Addressables", string.Format("{0} [{1}/{2}]", assetObject, i, assetObjects.Length), (float)i / assetObjects.Length);

                if(assetObject != null)
                {
                    MarkAssetAsAddressable(assetObject, isMarkAsAssetAddressable, label);
                }
            }
            EditorUtility.ClearProgressBar();
#endif
        }

        public static void MarkAssetAsAddressable(Object assetObject, bool isMarkAsAssetAddressable, CDN_LABELS label = CDN_LABELS.default_additive)
        {
#if UNITY_EDITOR
            var assetPath = assetObject == null ? "" : AssetDatabase.GetAssetPath(assetObject);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError(string.Format("{0} is not in the asset folder.", assetObject));
                return;
            }

            var assetGUID = AssetDatabase.AssetPathToGUID(assetPath);

            if (!isMarkAsAssetAddressable)
                DefaultAddressableSettings.RemoveAssetEntry(assetGUID);
            else
            {
                var existingAssetEntry = DefaultAddressableSettings.FindAssetEntry(assetGUID);
                if (existingAssetEntry != null)
                    return;
                var assetEntry = DefaultAddressableSettings.CreateOrMoveEntry(assetGUID, DefaultAddressableSettings.DefaultGroup);
                if (label != CDN_LABELS.default_additive)
                    assetEntry.labels.Clear();
                if (!assetEntry.labels.Contains("default"))
                    assetEntry.labels.Add("default");
                if (label != CDN_LABELS.default_additive && label != CDN_LABELS.default_replace)
                    assetEntry.labels.Add(label.ToString());
                assetEntry.OnBeforeSerialize();
            }

            foreach (var group in DefaultAddressableSettings.groups)
            {
                group.OnBeforeSerialize();
                EditorUtility.SetDirty(group);
            }
#endif
        }

        public static void CreateAssetGroupByAssetName(Object[] assetObjects)
        {
            foreach (var assetObject in assetObjects)
                CreateAssetGroupByAssetName(assetObject);
        }

        [Button]
        public static void CreateAssetGroupByAssetName(Object assetObject)
        {
#if UNITY_EDITOR
            var assetPath = AssetDatabase.GetAssetPath(assetObject);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError(string.Format("{0} is not in the asset folder.", assetObject));
                return;
            }
            if (DefaultAddressableSettings.FindGroup(assetObject.name) == null)
                DefaultAddressableSettings.CreateGroup(assetObject.name, false, false, true, null);
#endif
        }
    }
}