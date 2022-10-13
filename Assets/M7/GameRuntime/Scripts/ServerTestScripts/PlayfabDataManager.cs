using System;
using System.Collections.Generic;
using M7.GameData;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
using M7.GameRuntime.Scripts.Settings;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.Utilities;
using UnityEngine;

namespace M7.ServerTestScripts
{
    public class PlayfabDataManager : MonoBehaviour
    {
        public static PlayfabDataManager Instance;
        public bool IsInitialized { get; set; } = true;

        private void Awake()
        {
            Instance = this;
        }
        public void Init()
        {
            DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.GetAllPlayerData);            
        }

        public void PlayFabLogin(string customId, Action onLoginOkCallback, Action onLoginErrorCallback)
        {
            var request = new LoginWithCustomIDRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                CustomId = customId,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {GetPlayerProfile = true, GetUserData = true}
            };
            PlayFabClientAPI.LoginWithCustomID(request, (result) =>
                {
                    Debug.Log("Got PlayFabID: " + request.CustomId);
                    print(result.NewlyCreated);
                    
                    Debug.Log(JsonConvert.SerializeObject(result));
                    
                    // save session ticket to cosmos
                    AzureFunction.UpdateSession(result, OkResult, ErrorResult);
                    
                    if (result.NewlyCreated)
                    {
                        print($"New user {result.NewlyCreated}");
                    }
                    else
                    {
                        string DisplayName = result.InfoResultPayload.PlayerProfile.DisplayName;
                        if (!DisplayName.IsNullOrWhitespace())
                        {
                            PlayerDataMachine.PlayerName = DisplayName;
                            PlayerDatabase.AccountProfile.UpdateDisplayName(DisplayName);
                            //LoadingSceneManager.Instance.StartDownloadCDN();
                        }

                        Debug.Log("(existing account)");
                    }
                    onLoginOkCallback?.Invoke();
                },
                (error) =>
                {
                    Debug.Log("Error logging in player with custom ID:");
                    Debug.Log(error.ErrorMessage);
                    onLoginErrorCallback?.Invoke();
                });   
        }

        private void ErrorResult(string obj)
        {
            var response = JsonConvert.DeserializeObject<Response>(obj);
            Debug.LogError($"Error Creating Session, {response.Message}");
            MessageBox.Create("Error Creating Session", MessageBox.ButtonType.Ok, "Session").Show();
        }

        private void OkResult(string obj)
        {
            
        }
    }
}