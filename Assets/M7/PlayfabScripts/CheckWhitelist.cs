using System;
using Chamoji.Social;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;


public static class CheckWhitelist 
    {
      public  static void VerifyAccount(Action onSuccess , Action onFail)
        {
            PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest()
            {
                FunctionName = "ValidateWhitelist",
                GeneratePlayStreamEvent = true
            }, result =>
            {
                bool IsWhitelist = JsonConvert.DeserializeObject<bool>(result.FunctionResult.ToString());
                Debug.Log(IsWhitelist);
                onSuccess?.Invoke();

            }, error =>
            {
                NetworkMethods.CheckNetworkError(error, () => { VerifyAccount( 
                    onSuccess,
                    onFail
                ); });
                onFail?.Invoke();
            });
        }
    }
