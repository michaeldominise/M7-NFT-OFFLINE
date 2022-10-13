using System.Collections.Generic;
using M7.GameData;
using M7.GameRuntime;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
using M7.Settings;
using M7.GameRuntime.Scripts.Settings;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace M7
{
    public class SettingsSceneManager : SceneManagerBase
    {
        public static SettingsSceneManager Instance { get; private set; }

        [SerializeField] AssetReference playerProfileScene;
        [SerializeField] TextMeshProUGUI username;
        [SerializeField] TextMeshProUGUI playerEmailAdd;
        [SerializeField] Toggle soundToggle;
        [SerializeField] Toggle musicToggle;
        [SerializeField] CharacterInstance_AvatarBasic avatar;

        string MarketplaceLink => "https://www.murasaki.com";
        string HowToPlayLink => "https://murasaki7.gitbook.io/m7-puzzle-rpg-whitepaper-v1.0.0/how-to-play";
        string LinktreeLink => "https://linktr.ee/murasaki7";
        string FeedbackLink => " https://forms.gle/CjeC3LYQcGLvb6Ba9";

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        private void Start()
        {
            soundToggle.isOn = SettingsAPI.SoundsEnabled;
            musicToggle.isOn = SettingsAPI.MusicEnabled;
            PlayerDatabase.AccountProfile.onValuesChanged += RefreshAccountValues;
            RefreshAccountValues();
        }

        void RefreshAccountValues()
        {
            username.text = PlayerDatabase.AccountProfile.Username;
            playerEmailAdd.text = PlayerDatabase.AccountProfile.Email;
            avatar.Init(MasterIDManager.RPGObjectReference.FindAssetReference(PlayerDatabase.AccountProfile.AvatarId)?.GetAssetReference<RPGObject>(), null);
        }

        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch (gameObject.name)
            {
                case "Close_Button":
                    UnloadAtSceneLayer(sceneLayer);
                    break;
                case "PlayerProfileButton":
                    LoadScene(playerProfileScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    break;
                case "MarketplaceButton":
                    Application.OpenURL(MarketplaceLink);
                    break;
                case "HowToPlayButton":
                    Application.OpenURL(HowToPlayLink);
                    break;
                case "LinktreeButton":
                    Application.OpenURL(LinktreeLink);
                    break;
                case "FeedbackButton":
                    Application.OpenURL(FeedbackLink);
                    break;
                case "LogoutButton":
                    MessageBox.Create("Are you sure you want to logout?", MessageBox.ButtonType.Ok_No, okLabel: "Yes", noLabel:"No", onResult: result =>
                    {
                        if (result == MessageBox.ButtonType.Ok)
                        {
                            Logout();
                            return;
                        }
                        
                        MessageBox.HideCurrent();
                    }).Show();
                    break;
            }
        }

        protected override void ExecuteToggleEvent(Toggle toggle)
        {
            switch(toggle.name)
            {
                case "SoundToggle":
                    SettingsAPI.SoundsEnabled = toggle.isOn;
                    break;
                case "MusicToggle":
                    SettingsAPI.MusicEnabled = toggle.isOn;
                    break;
            }
        }
        
        private void Logout()
        {
            var email = new Dictionary<string, string>
            {
                { "email", PlayerDatabase.AccountProfile.Email },
                { "sessionTicket", PlayerDatabase.AccountProfile.SessionTicket }
            };

            MessageBox.Create("Logging out...", MessageBox.ButtonType.Loading).Show();
            
            AzureFunction.Logout(email, OkResult, ErrorResult);
        }

        private void OkResult(string obj)
        {
            MessageBox.HideCurrent();
            
            var response = JsonConvert.DeserializeObject<Response>(obj);

            if (response.ResponseResult == ResponseResult.OK)
            {
                LocalData.DeleteLoggedInPlayer();
                GameManager.RestartGameDialog(false);
                return;
            }

            MessageBox.Create("Unable to logout user", MessageBox.ButtonType.Ok, "Logout").Show();
        }
        
        private void ErrorResult(string obj)
        {
            MessageBox.HideCurrent();
            MessageBox.Create("Unable to logout user", MessageBox.ButtonType.Ok, "Logout").Show();
        }
    }
}