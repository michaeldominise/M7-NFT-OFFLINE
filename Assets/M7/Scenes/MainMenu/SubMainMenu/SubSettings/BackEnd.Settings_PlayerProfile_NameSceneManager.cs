using M7.GameData;
using M7.GameRuntime.Scripts.UnityPlayfab;
using PlayFab;
using PlayFab.ClientModels;

namespace M7
{
    public partial class Settings_PlayerProfile_NameSceneManager
    {
        private void ChangeDisplayName()
        {
            MessageBox.Create("Updating...", MessageBox.ButtonType.Ok, "Name", onResult: OnOkClick).Show();
            PlayFabAccount.ChangeDisplayName(usernameInput.text, ResultCallback, ErrorCallback);
        }

        private void ResultCallback(UpdateUserTitleDisplayNameResult obj)
        {
            MessageBox.HideCurrent();
            PlayerDatabase.AccountProfile.UpdateDisplayName(usernameInput.text);
            MessageBox.Create("Username successfully changed!", MessageBox.ButtonType.Ok, "Name", onResult: OnOkClick).Show();
        }

        private void ErrorCallback(PlayFabError obj)
        {
            MessageBox.HideCurrent();
            MessageBox.Create("Username successfully changed!", MessageBox.ButtonType.Ok, "Name").Show();
        }
        
        private void OnOkClick(MessageBox.ButtonType obj)
        {
            UnloadAtSceneLayer(sceneLayer);
        }
    }
}