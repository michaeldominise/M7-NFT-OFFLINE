using System.Collections.Generic;
using M7.GameData;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
using M7.GameRuntime.Scripts.Settings;
using M7.GameRuntime.Scripts.UnityPlayfab;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEvents;

namespace M7
{
    public partial class Settings_PlayerProfile_EmailSceneManager
    {
        private void UpdateEmail()
        {
            var emailUpdate = new Dictionary<string, string>
            {
                { "newEmail", emailInput.text },
                { "oldEmail", PlayerDatabase.AccountProfile.Email },
                { "code", emailVerificationCodeInput.text },
                { "password", passwordInput.text },
                { "sessionTicket", PlayerDatabase.AccountProfile.SessionTicket }
            };
            MessageBox.Create("Updating...", MessageBox.ButtonType.Loading).Show();
            AzureFunction.UpdateEmail(emailUpdate, OkResult, ErrorResult);
        }

        private void OkResult(string obj)
        {
            MessageBox.HideCurrent();
            var response = JsonConvert.DeserializeObject<Response>(obj);

            if (response.ResponseResult == ResponseResult.OK)
            {
                PlayFabAccount.ChangeEmail(PlayerDatabase.AccountProfile.Email, emailInput.text, (LinkCustomIDResult customID) =>
                {
                    PlayerDatabase.AccountProfile.UpdateEmail(emailInput.text);

                    MessageBox.Create(response.Message, MessageBox.ButtonType.Ok, "Change Email", "OK", onResult:OnResult).Show();    
                }, ErrorCallback);
                
                return;
            }
            
            MessageBox.Create(response.Message, MessageBox.ButtonType.Ok, "Change Email", "OK").Show();
        }

        private void ErrorCallback(PlayFabError obj)
        {
            MessageBox.HideCurrent();
            MessageBox.Create(obj.ErrorMessage, MessageBox.ButtonType.Ok, "Change Email", "OK").Show();
        }

        private void OnResult(MessageBox.ButtonType obj)
        {
            if ((obj & MessageBox.ButtonType.Ok) != 0)
                UnloadAtSceneLayer(sceneLayer);
        }
        
        private void ErrorResult(string obj)
        {
            var response = JsonConvert.DeserializeObject<Response>(obj);
            
            MessageBox.Create(response.Message, MessageBox.ButtonType.Ok, "Change Email", "OK").Show();
        }
    }
}