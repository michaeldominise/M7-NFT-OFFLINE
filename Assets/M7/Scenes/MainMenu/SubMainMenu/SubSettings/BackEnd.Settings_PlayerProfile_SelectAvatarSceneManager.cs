using System.Collections.Generic;
using M7.GameData;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
using Newtonsoft.Json;

namespace M7
{
    public partial class Settings_PlayerProfile_SelectAvatarSceneManager
    {
        private void UpdateAvatar()
        {
            if (SelectedAvatarId != PlayerDatabase.AccountProfile.AvatarId)
            {
                var updateAvatar = new Dictionary<string, string>()
                {
                    { "email", PlayerDatabase.AccountProfile.Email },
                    { "avatarId", SelectedAvatarId },
                    { "sessionTicket", PlayerDatabase.AccountProfile.SessionTicket }
                };

                AzureFunction.UpdateAvatar(updateAvatar, OkResult, ErrorResult);    
            }
        }

        private void ErrorResult(string obj)
        {
            MessageBox.HideCurrent();
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(obj);
            MessageBox.Create(response.Message, MessageBox.ButtonType.Ok, "Avatar Update")
                .Show();
        }

        private void OkResult(string obj)
        {
            MessageBox.HideCurrent();
            
            var response = JsonConvert.DeserializeObject<Response>(obj);

            if (response.ResponseResult == ResponseResult.OK)
            {
                PlayerDatabase.AccountProfile.UpdateAvatar(SelectedAvatarId);

                UnloadAtSceneLayer(sceneLayer);
                return;
            }
        }
    }
}