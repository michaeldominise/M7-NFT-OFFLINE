using System.Collections.Generic;
using M7.GameData;
using M7.GameRuntime;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
using M7.GameRuntime.Scripts.Energy.Tests;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.Settings;
using M7.GameRuntime.Scripts.UnityPlayfab;
using Newtonsoft.Json;

namespace M7
{
    public partial class Settings_PlayerProfile_DeleteAccountSceneManager
    {
        public void DeleteAccount()
        {
            var emailAndCode = new Dictionary<string, string>
            {
                { "email", PlayerDatabase.AccountProfile.Email},
                { "verificationCode", emailVerificationCodeInput.text },
                { "sessionTicket", PlayerDatabase.AccountProfile.SessionTicket }
            };

            MessageBox.Create("Deleting...", MessageBox.ButtonType.Loading).Show();
            AzureFunction.DeleteAccount(emailAndCode, OkResult, ErrorResult);
        }

        private void OkResult(string obj)
        {
            MessageBox.HideCurrent();
            
            var response = JsonConvert.DeserializeObject<Response>(obj);

            if (response.ResponseResult == ResponseResult.FAILED)
            {
                MessageBox.Create(response.Message, MessageBox.ButtonType.Ok, "Delete Account", "OK").Show();
                return;
            }
                
            PlayFabFunctions.PlayFabCallFunction("DeletePlayFabAccount");
            
            MessageBox.Create(response.Message, MessageBox.ButtonType.Ok, "Delete Account", "OK", onResult:OnResult).Show();
        }

        private void OnResult(MessageBox.ButtonType obj)
        {
            if ((obj & MessageBox.ButtonType.Ok) == 0) return;
            
            LocalData.DeleteLoggedInPlayer();
            GameManager.RestartGameDialog(false);
        }

        private void ErrorResult(string obj)
        {
            var response = JsonConvert.DeserializeObject<Response>(obj);
            MessageBox.Create(response.Message, MessageBox.ButtonType.Ok, "Delete Account", "OK").Show();
        }
    }
}