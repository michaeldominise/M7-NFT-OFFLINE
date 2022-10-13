using M7.GameData;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB.Account;
using M7.GameRuntime.Scripts.PlayerData;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.UnityPlayfab;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace M7
{
    public partial class SetPasswordSceneManager
    {
        #region BACKEND

        private void CreateAccount()
        {
            PlayerData.SaveEmail(LoginSignupSceneManager.Instance.Player.email);
            PlayerData.SavePassword(passwordInput.text);
            
            LoadScene(activationCodeScene, LoadSceneMode.Additive, overwriteSceneLayer: sceneLayer);
            // MessageBox.Create("Signing in...", MessageBox.ButtonType.Loading).Show();
            // PlayFabAccount.LoginCreate(LoginSignupSceneManager.Instance.Player.email, CreateAccountOkCallback,
            //     CreateAccountErrorCallback);
        }
        
        private void CreateAccountOkCallback(LoginResult obj)
        {
            var player = LoginSignupSceneManager.Instance.Player;
            player.password = passwordInput.text;
            player.playerStatus = PlayerStatus.New;
            player.playFabId = obj.PlayFabId;
            player.playerType = PlayerType.DEFAULT;
            player.deviceId = SystemInfo.deviceUniqueIdentifier;
            player.sessionTicket = obj.SessionTicket;

            if (!obj.NewlyCreated) return;
            AzureFunction.CreateNewUser(player, NewUserCreatedCallback, NewUserCreatedErrorCallback);
        }

        private void CreateAccountErrorCallback(PlayFabError obj)
        {
            Debug.Log($"Error {obj.Error}");
            MessageBox.HideCurrent();
            MessageBox.Create("Unable to create user", MessageBox.ButtonType.Ok, "User Account").Show();
        }

        private void NewUserCreatedCallback(string obj)
        {
            var responseResult = JsonConvert.DeserializeObject<Response>(obj);

            if (responseResult.ResponseResult == ResponseResult.OK)
            {
                PlayerDatabase.AccountProfile.OverwriteValues(responseResult.Data.ToString());
                CreateUserKeyss();
                return;
            }

            Debug.Log($"New User Created Error {responseResult.Message}");
        }
        
        private void NewUserCreatedErrorCallback(string obj)
        {
            var responseResult = JsonConvert.DeserializeObject<Response>(obj);
            Debug.Log($"New User Created Error {responseResult.Message}");
            MessageBox.Create(responseResult.Message, MessageBox.ButtonType.Ok, "Account Creation").Show();
        }
        public void CreateUserKeyss()
        {
            PlayFabFunctions.PlayFabCallFunction("CreateUserKey", false, "", "", KeyCallback);
        }
        void KeyCallback(ExecuteResult result)
        {
            MessageBox.HideCurrent();
            
            Debug.Log($" This is the result: {result}");
            LoadScene(activationCodeScene, LoadSceneMode.Additive, overwriteSceneLayer: sceneLayer);
        }
        #endregion
    }
}