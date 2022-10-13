using M7.GameRuntime;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB.Account;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.PlayfabCloudscript.PlayerDatabase;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace M7.GameData
{
    [CreateAssetMenu(fileName = "GlobalDatabase", menuName = "Assets/M7/GameData/GlobalDatabase")]
    public class GlobalDatabase : ScriptableObject
    {
        private static GlobalDatabase Instance => GameManager.Instance.GlobalDatabase ? GameManager.Instance.GlobalDatabase : Resources.Load<GlobalDatabase>("GlobalDatabase");

        [SerializeField] GlobalDataSetting globalSettings;
        [SerializeField] DurabilityCostSetting durabilityCostSetting;

        public static GlobalDataSetting GlobalDataSetting => Instance.globalSettings;
        public static DurabilityCostSetting DurabilityCostSetting => Instance.durabilityCostSetting;
    }
}