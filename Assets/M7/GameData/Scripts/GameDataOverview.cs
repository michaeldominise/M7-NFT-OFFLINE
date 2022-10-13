using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace M7.GameData
{
    public class GameDataOverview<T1> : GameDataOverview where T1 : ScriptableObject
    {
        [SerializeField, ReadOnly, ListDrawerSettings(Expanded = true)]
        public T1[] AllData;
        [ShowInInspector, ReadOnly] T1 defaultValueCache;


        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        public override void UpdateOverview()
        {
#if UNITY_EDITOR
            // Finds and assigns all scriptable objects of type
            AllData = AssetDatabase.FindAssets("t:" + typeof(T1).Name)
                .Select(guid => AssetDatabase.LoadAssetAtPath<T1>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();
#endif
        }

        public virtual void Save()
        {
#if UNITY_EDITOR
            var allDataAssetEditor = AllData;
            for (int i = 0; i < allDataAssetEditor.Length; i++)
            {
                if (allDataAssetEditor[i] != null)
                {
                    EditorUtility.SetDirty(allDataAssetEditor[i]);
                }
            }
#endif
        }

        /// <summary>
        /// Finds the data.
        /// </summary>
        /// <returns>The data with the same asset name.</returns>
        /// <param name="assetName">Asset name.</param>
        /// <param name="useDefaultIfNotFound">If set to <c>true</c> use the zero index of AllData if not found or if empty null.</param>
        public virtual T1 FindData(string assetName, bool useDefaultIfNotFound = true)
        {
            return FindData<T1>(assetName, useDefaultIfNotFound);
        }

        /// <summary>
        /// Finds the data.
        /// </summary>
        /// <returns>The data with the same asset name.</returns>
        /// <param name="assetName">Asset name.</param>
        /// <param name="useDefaultIfNotFound">If set to <c>true</c> use the zero index of AllData if not found or if empty null.</param>
        /// <typeparam name="T2">The return type parameter.</typeparam>
        public virtual T2 FindData<T2>(string assetName, bool useDefaultIfNotFound = true) where T2 : T1
        {
            return FindDataList<T2>(new string[] { assetName }, useDefaultIfNotFound)[0];
        }

        /// <summary>
        /// Finds the data list.
        /// </summary>
        /// <returns>The data with the same asset name.</returns>
        /// <param name="assetNames">Assets name.</param>
        /// <param name="useDefaultIfNotFound">If set to <c>true</c> use the zero index of AllData if not found or if empty null.</param>
        public T1[] FindDataList(string[] assetNames, bool useDefaultIfNotFound = true)
        {
            return FindDataList<T1>(assetNames, useDefaultIfNotFound);
        }

        /// <summary>
        /// Finds the data list.
        /// </summary>
        /// <returns>The data with the same asset name.</returns>
        /// <param name="assetNames">Asset names.</param>
        /// <param name="useDefaultIfNotFound">If set to <c>true</c> use the zero index of AllData if not found or if empty null.</param>
        /// <typeparam name="T2">The return type parameter.</typeparam>
        public T2[] FindDataList<T2>(string[] assetNames, bool useDefaultIfNotFound = true) where T2 : T1
        {
            var dataList = new T2[assetNames.Length];
            for (var i1 = 0; i1 < dataList.Length; i1++)
            {
                var assetName = assetNames[i1];
                if (assetName == null)
                    continue;

                if (string.IsNullOrEmpty(assetName) && !useDefaultIfNotFound)
                    dataList[i1] = null;

                assetName = MasterIDManager.GetAlternativeAssetName(assetName);
                for (int i2 = 0; i2 < AllData.Length; i2++)
                {
                    if (AllData[i2] == null)
                        continue;

                    var assetObject = AllData[i2] as T2;
                    if (assetObject == null)
                        continue;

                    if (defaultValueCache == null || defaultValueCache as T2 == null)
                        defaultValueCache = useDefaultIfNotFound && AllData.Length > 0 ? assetObject : null;

                    if (string.IsNullOrEmpty(assetName) && defaultValueCache != null)
                        break;

                    if (AllData[i2].name.ToLower() == assetName.ToLower())
                    {
                        dataList[i1] = assetObject;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(assetName))
                    Debug.LogWarning(string.Format("Will use default:'{0}'.", defaultValueCache == null ? "Null" : defaultValueCache.name));
                else if (dataList[i1] == null)
                    Debug.LogWarning(string.Format("Can't find '{0}' in the asset.{1}", assetName, useDefaultIfNotFound ? string.Format(" Will use default:'{0}' instead.", defaultValueCache == null ? "Null" : defaultValueCache.name) : ""));
                dataList[i1] = dataList[i1] ?? (useDefaultIfNotFound ? defaultValueCache as T2 : null);
            }
            return dataList;
        }

        public T1 GetDefault()
        {
            return GetDefault<T1>();
        }

        public T2 GetDefault<T2>() where T2 : T1
        {
            return FindData<T2>("");
        }
    }

    public class GameDataOverview
    {
        public virtual void UpdateOverview() { }
        public virtual void UpdateLocKeys() { }
    }
}
