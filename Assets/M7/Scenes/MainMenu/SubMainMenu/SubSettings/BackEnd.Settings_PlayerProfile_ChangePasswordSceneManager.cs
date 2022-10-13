using System.Collections.Generic;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
using M7.GameData;
using Newtonsoft.Json;

namespace M7
{
    public  partial class Settings_PlayerProfile_ChangePasswordSceneManager
    {
        private void UpdatePassword()
        {
            var user = new Dictionary<string, string>
            {
                { "email", PlayerDatabase.AccountProfile.Email },
                { "password", passwordInput.text },
                { "code", emailVerificationCodeInput.text },
                { "sessionTicket", PlayerDatabase.AccountProfile.SessionTicket }
            };

            MessageBox.Create("Updating...", MessageBox.ButtonType.Loading, "Change Password").Show();
            
            AzureFunction.UpdatePassword(user, OkResult, ErrorResult);
        }

        private void OkResult(string obj)
        {
            MessageBox.HideCurrent();
            
            var response = JsonConvert.DeserializeObject<Response>(obj);

            if (response.ResponseResult == ResponseResult.OK)
            {
                MessageBox.Create(response.Message, MessageBox.ButtonType.Ok, "Change Password", "OK",
                    onResult: MessageBoxOk).Show();
                return;
            }
            
            MessageBox.Create(response.Message, MessageBox.ButtonType.Ok, "Change Password", "OK").Show();
        }

        private void MessageBoxOk(MessageBox.ButtonType obj)
        {
            if ((obj & MessageBox.ButtonType.Ok) != 0)
                UnloadAtSceneLayer(sceneLayer);
        }

        private void ErrorResult(string obj)
        {
            var response = JsonConvert.DeserializeObject<Response>(obj);
            MessageBox.HideCurrent();
            MessageBox.Create(response.Message, MessageBox.ButtonType.Ok, "Change Password", "OK").Show();
        }
    }
}