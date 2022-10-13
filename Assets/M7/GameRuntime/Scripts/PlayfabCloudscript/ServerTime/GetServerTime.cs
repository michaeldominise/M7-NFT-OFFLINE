using M7.GameRuntime;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using Sirenix.OdinInspector;
using Newtonsoft.Json;

namespace M7.PlayfabCloudscript.ServerTime
{
    public class GetServerTime : MonoBehaviour
    {
        [Button]
        public void InitServerTime()
        {
            CheckServerTime();
        }
        public void CheckServerTime()
        {
            PlayFabFunctions.PlayFabCallFunction("GetServerTime", false, "", "", OnSuccessTime);
        }
        void OnSuccessTime(ExecuteResult result)
        {
            var timeResult = JsonConvert.SerializeObject(result);
            Debug.Log($"Success - The Time is: {timeResult}");

        }
    }
}
