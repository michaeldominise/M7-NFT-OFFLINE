
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace M7.GameData
{
    public partial class MasterIDManager
    {
        public static string GetAlternativeAssetName(string assetName)
        {
            if (string.IsNullOrWhiteSpace(assetName))
                return assetName;

            return assetName;
        }

        //public static bool IsVirtualCurrency(string masterID)
        //{
        //    foreach (var virtualCurrencyMasterID in Instance.virtualCurrencyMasterIDs)
        //        if (virtualCurrencyMasterID == masterID)
        //            return true;
        //    return false;
        //}
    }
}