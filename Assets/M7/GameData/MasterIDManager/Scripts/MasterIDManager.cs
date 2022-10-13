
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M7.CDN.Addressable;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "MasterIDManager", menuName = "Assets/M7/Data/MasterIDManager")]
    public partial class MasterIDManager : ScriptableObject
    {
        static MasterIDManager Instance => Resources.Load<MasterIDManager>("MasterIDManager");

        [SerializeField] MasterIDManager_DefaultResources defaultResources;
        public static MasterIDManager_DefaultResources DefaultResources => Instance.defaultResources;

        [SerializeField, ReadOnly] AddressableAssetOverview<RPGObject> rpgObjectReference = new AddressableAssetOverview<RPGObject>();
        public static AddressableAssetOverview<RPGObject> RPGObjectReference => Instance.rpgObjectReference;

#if UNITY_EDITOR
        [Button]
        public static void UpdateOverview()
        {
            RPGObjectReference.UpdateOverview();
        }
#endif
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class MasterIDManagerUpdateOverview
    {
        static MasterIDManagerUpdateOverview()
        {
            MasterIDManager.UpdateOverview();
        }
    }
#endif
}