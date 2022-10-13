using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using M7.GameData;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB.Account;
using M7.GameRuntime.Scripts.PlayerData;
using M7.GameRuntime.Scripts.UnityPlayfab;
using Newtonsoft.Json;
using M7.ServerTestScripts;
using PlayFab;
using PlayFab.ClientModels;

namespace M7
{
    public class LoginSignupSceneManager : SceneManagerBase
    {
        public static LoginSignupSceneManager Instance { get; private set; }

        [SerializeField] VerificationCode verificationCode;
        [SerializeField] AssetReference loginWithPasswordScene;
        [SerializeField] AssetReference setPasswordScene;
        [SerializeField] AssetReference activationCodeScene;
        [SerializeField] TMP_InputField emailInput;
        [SerializeField] TMP_InputField emailVerificationCodeInput;
        [SerializeField] Button loginSignUpButton;
        [SerializeField] Toggle agreeToggle;

        string TermsOfUseLink => "www.murasaki7.com";
        string PrivacyPolicyLink => "www.murasaki7.com";

        public Player Player;

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        private void Start() => CheckLoginSignUpButton();
        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch(gameObject.name)
            {
                case "TermsOfUseButton":
                    Application.OpenURL(TermsOfUseLink);
                    break;
                case "PrivacyPolicyButton":
                    Application.OpenURL(PrivacyPolicyLink);
                    break;
                case "SendCodeButton":
                    if (!string.IsNullOrWhiteSpace(emailInput.text))
                        verificationCode.OnClick(emailInput.text, VerificationCode.VerificationType.Login);
                    break;
                case "LoginSignUpButton":
                    LoginSignUp();
                    break;
                case "LoginWithPasswordButton":
                    LoadScene(loginWithPasswordScene, LoadSceneMode.Additive, overwriteSceneLayer: sceneLayer);
                    break;
            }
        }

        protected override void ExecuteToggleEvent(Toggle toggle)
        {
            switch (toggle.name)
            {
                case "IAggreeToggle":
                    CheckLoginSignUpButton();
                    break;
            }
        }

        protected override void ExecuteInputEvent(TMP_InputField input)
        {
            switch (input.name)
            {
                case "EmailInput":
                case "EmailVerificationCodeInput":
                    CheckLoginSignUpButton();
                    break;
            }
        }

        private void CheckLoginSignUpButton() => loginSignUpButton.interactable = agreeToggle.isOn && !string.IsNullOrWhiteSpace(emailInput.text) && !string.IsNullOrWhiteSpace(emailVerificationCodeInput.text);

        void LoginSignUp()
        {
            // var theEmail = new Dictionary<string, string> 
            // { 
            //     { "email", emailInput.text }, 
            //     { "verificationCode", emailVerificationCodeInput.text },
            //     { "deviceId", SystemInfo.deviceUniqueIdentifier }
            // };

            if (PlayerData.LoadEmail() == emailInput.text)
            {
                PlayfabDataManager.Instance.Init();
                UnloadAtSceneLayer(Instance.sceneLayer);
            }
            else
            {
                Player = new Player { email = emailInput.text };
                LoadScene(setPasswordScene, LoadSceneMode.Additive, overwriteSceneLayer: sceneLayer);
            }
            
            // MessageBox.Create("Signing in...", MessageBox.ButtonType.Loading).Show();
            //
            // AzureFunction.LoginViaVerificationCode(theEmail, LogInSignUpOkResult, LoginSignUpErrorResult);
        }
        private void LogInSignUpOkResult(string obj)
        {
            var responseResult = JsonConvert.DeserializeObject<Response>(obj);

            if (responseResult.ResponseResult == ResponseResult.OK)
            {                               
                MessageBox.HideCurrent();
                
                var result = responseResult.Data == null ? null : JsonConvert.DeserializeObject<Player>(responseResult.Data.ToString());
                var newPlayer = result == null;

                // already logged in
                if (responseResult.Data != null)
                {
                    PlayerDatabase.AccountProfile.OverwriteValues(responseResult.Data.ToString());
                    PlayerDatabase.AccountProfile.SetDeviceId();   
                }

                if (newPlayer)
                {
                    Player = new Player { email = emailInput.text };                    
                    LoadScene(setPasswordScene, LoadSceneMode.Additive, overwriteSceneLayer: sceneLayer);   
                    return;
                }
                
                if (result.playerStatus == PlayerStatus.New)
                {
                    LoadScene(activationCodeScene, LoadSceneMode.Additive, overwriteSceneLayer: sceneLayer);
                    return;
                }
                
                PlayFabAccount.Login(PlayerDatabase.AccountProfile.Email, ResultCallback, ErrorCallback);
                return;
            }
            
            MessageBox.HideCurrent();
            MessageBox.Create($"Verification failed, {responseResult.Message}", MessageBox.ButtonType.Ok).Show();
        }

        private void ErrorCallback(PlayFabError obj)
        {
            MessageBox.Create(obj.ErrorMessage, MessageBox.ButtonType.Ok).Show();
        }

        private void ResultCallback(LoginResult obj)
        {
            // save session ticket to cosmos
            AzureFunction.UpdateSession(obj);
            
            PlayfabDataManager.Instance.Init();
            UnloadAtSceneLayer(Instance.sceneLayer);
        }

        private void LoginSignUpErrorResult(string obj)
        {
            var errorResponse = JsonConvert.DeserializeObject<Response>(obj);
            MessageBox.HideCurrent();
            MessageBox.Create(errorResponse.Message, MessageBox.ButtonType.Ok).Show();
        }
    }
}
