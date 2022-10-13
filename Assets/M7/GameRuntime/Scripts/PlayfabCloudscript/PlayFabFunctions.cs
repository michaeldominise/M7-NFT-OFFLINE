//This snippet assumes that your game client is already logged into PlayFab.

using System;
using System.Collections.Generic;
using M7.GameBuildSettings.AzureConfigSettings;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.Managers.AzureFunctions;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;
using EntityKey = PlayFab.CloudScriptModels.EntityKey;
using PlayFab.ClientModels;
using PlayFabSDK.Shared.Models;

namespace M7.GameRuntime.Scripts.PlayfabCloudscript
{
    public static class PlayFabFunctions
    {
        public static void PlayFabCallFunction(string functionName, bool generateStreamEvent = false, string key = "",
        string json = "",
        Action<ExecuteResult> callBack = null,
        Action<PlayFabError> errorCallBack = null)
        {
            if (AzureSettingsManager.Instance.AzureSettings.profileName.Contains("LocalHost"))
                functionName = $"p_{functionName}";
            
            PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
            {
                Entity = new EntityKey
                {
                    Id = PlayFabSettings.staticPlayer.EntityId, //Get this from when you logged in,
                    Type = PlayFabSettings.staticPlayer.EntityType //Get this from when you logged in
                },
                FunctionName = functionName, //This should be the name of your Azure Function that you created.
                FunctionParameter = new Dictionary<string, object> { { "key", key }, { "value", json } }, //This is the data that you would want to pass into your function.
                GeneratePlayStreamEvent = generateStreamEvent //Set this to true if you would like this call to show up in PlayStream
            }, result =>
            {
                if (result.FunctionResultTooLarge ?? false)
                {
                    Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                    callBack?.Invoke(new ExecuteResult
                    {
                        Result = null,
                        Status = ResultStatus.Error
                    });
                }
                Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
                Debug.Log($"Result: {result.FunctionResult}");

                callBack?.Invoke(new ExecuteResult
                {
                    Result = result,
                    Status = ResultStatus.Ok
                });
            }, error =>
            {
                Debug.Log($"Playfab: Title ID {PlayFabSettings.staticSettings.TitleId}, Key {PlayFabSettings.staticSettings.DeveloperSecretKey}");
                Debug.Log($"Azure: URI {AzureSettingsManager.Instance.AzureSettings.uri}, " +
                          $"Host Key {AzureSettingsManager.Instance.AzureSettings.hostKey}, Azure CDN {AzureSettingsManager.Instance.AzureSettings.azureCdnUri}, " +
                          $"Profile Name {AzureSettingsManager.Instance.AzureSettings.profileName}");
                Debug.Log($"Oops Something went wrong: {error.GenerateErrorReport()}, {functionName}");
                errorCallBack?.Invoke(error);
            });
        }

        public static void PlayFabCallFunction(string functionName, bool generateStreamEvent, Dictionary<string, object> functionParameters,
            Action<ExecuteResult> callBack = null,
            Action<PlayFabError> errorCallBack = null)
        {
            if (AzureSettingsManager.Instance.AzureSettings.profileName.Contains("LocalHost"))
                functionName = $"p_{functionName}";
            
            PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
            {
                Entity = new EntityKey
                {
                    Id = PlayFabSettings.staticPlayer.EntityId, //Get this from when you logged in,
                    Type = PlayFabSettings.staticPlayer.EntityType //Get this from when you logged in
                },
                FunctionName = functionName, //This should be the name of your Azure Function that you created.
                FunctionParameter = functionParameters, //This is the data that you would want to pass into your function.
                GeneratePlayStreamEvent = generateStreamEvent //Set this to true if you would like this call to show up in PlayStream
            }, result =>
            {
                if (result.FunctionResultTooLarge ?? false)
                {
                    Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                    callBack?.Invoke(new ExecuteResult
                    {
                        Result = null,
                        Status = ResultStatus.Error
                    });
                }
                Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
                Debug.Log($"Result: {result.FunctionResult}");

                callBack?.Invoke(new ExecuteResult
                {
                    Result = result,
                    Status = ResultStatus.Ok
                });
            }, error =>
            {
                Debug.Log($"Playfab: Title ID {PlayFabSettings.staticSettings.TitleId}, Key {PlayFabSettings.staticSettings.DeveloperSecretKey}");
                Debug.Log($"Azure: URI {AzureSettingsManager.Instance.AzureSettings.uri}, " +
                          $"Host Key {AzureSettingsManager.Instance.AzureSettings.hostKey}, Azure CDN {AzureSettingsManager.Instance.AzureSettings.azureCdnUri}, " +
                          $"Profile Name {AzureSettingsManager.Instance.AzureSettings.profileName}");
                Debug.Log($"Oops Something went wrong: {error.GenerateErrorReport()}, {functionName}");
                errorCallBack?.Invoke(error);
            });
        }
    }
    public class ExecuteResult
    {
        public ResultStatus Status;
        public ExecuteFunctionResult Result;
    }

    public enum ResultStatus
    {
        Ok, Error
    }
}
