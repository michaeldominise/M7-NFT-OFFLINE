using System.Collections.Generic;
using System.Runtime.InteropServices;
using M7.GameData;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB.Account;
using M7.GameRuntime.Scripts.PlayerData;
using M7.GameRuntime.Scripts.Settings;
using M7.GameRuntime.Scripts.UnityPlayfab;
using M7.ServerTestScripts;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace M7
{
    public partial class LoginWithPasswordSceneManager
    {
        private MessageBox _messageBox;
        private void LoginWithPassword(string email, string password)
        {
            // var emailPassword = new Dictionary<string, string>
            // {
            //     { "email", email },
            //     { "password", password },
            //     { "deviceId", SystemInfo.deviceUniqueIdentifier }
            // };
            //
            // _messageBox = MessageBox.Create("Signing in...", MessageBox.ButtonType.Loading).Show();
            //
            // AzureFunction.LoginViaPassword(emailPassword, OkResult, ErrorResult);

            if (string.IsNullOrWhiteSpace(PlayerData.LoadEmail()) || 
                string.IsNullOrWhiteSpace(PlayerData.LoadPassword()))
            {
                MessageBox.Create("Invalid email or password", MessageBox.ButtonType.Ok).Show();
            }
            else if (PlayerData.LoadEmail() == email && PlayerData.LoadPassword() == password)
            {
                PlayfabDataManager.Instance.Init();
                UnloadAtSceneLayer(Instance.sceneLayer);
            }
            else
            {
                MessageBox.Create("Invalid email or password", MessageBox.ButtonType.Ok).Show();
            }
        }

        private void OkResult(string obj)
        {
            var responseResult = JsonConvert.DeserializeObject<Response>(obj);

            if (responseResult.ResponseResult == ResponseResult.OK) 
            {
                _messageBox.Hide();
                
                PlayerDatabase.AccountProfile.OverwriteValues(responseResult.Data.ToString());
                PlayerDatabase.AccountProfile.SetDeviceId();
                
                // playfablogin
                PlayFabAccount.Login(PlayerDatabase.AccountProfile.Email, PlayFabLoginOkCallback, PlayFabLoginErrorCallback);
                return;
            }
            _messageBox.InitValues(responseResult.Message, MessageBox.ButtonType.Ok).Show();
            Debug.Log($"Verification failed, {responseResult.Message}");
        }

        private void PlayFabLoginErrorCallback(PlayFabError obj)
        {
            MessageBox.HideCurrent();
            MessageBox.Create(obj.ErrorMessage, MessageBox.ButtonType.Ok, "Login").Show();
        }

        private void PlayFabLoginOkCallback(LoginResult obj)
        {
            PlayfabDataManager.Instance.Init();
            UnloadAtSceneLayer(Instance.sceneLayer);
        }

        private void ErrorResult(string obj)
        {
            var responseResult = JsonConvert.DeserializeObject<Response>(obj);
            _messageBox.Hide();
            _messageBox.InitValues(responseResult.Message, MessageBox.ButtonType.Ok).Show();
        }
    }
}