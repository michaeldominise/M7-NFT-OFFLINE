using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using M7.GameData;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB.Account;
using M7.GameRuntime.Scripts.Energy;
using M7.GameRuntime.Scripts.Managers.AzureFunctions;
using Newtonsoft.Json;
using PlayFab.ClientModels;
# if UNITY_EDITOR
using PlayFab;
using UnityEditorInternal;
# endif
using UnityEngine;
using UnityEngine.Networking;

namespace M7.GameRuntime.Scripts.BackEnd.Azurefunctions
{
    public static class AzureFunction
    {
        private static string Uri => AzureSettingsManager.Instance.AzureSettings.uri;
        private static readonly string sendEmailVerificationCode = $"{Uri}EmailVerification";
        private static readonly string endEmailVerificationDeleteAccount = $"{Uri}EmailVerificationDeleteAccount";
        private static readonly string loginViaVerificationCode = $"{Uri}LoginViaVerificationCode";
        private static readonly string createNewUser = $"{Uri}CreateNewUser";
        private static readonly string loginViaPassword = $"{Uri}LoginViaPassword";
        private static readonly string activateAccount = $"{Uri}ActivateAccount";
        private static readonly string updateEmail = $"{Uri}UpdateEmail";
        private static readonly string updatePassword = $"{Uri}UpdatePassword";
        private static readonly string updateAvatar = $"{Uri}UpdateAvatar";
        private static readonly string deleteAccount = $"{Uri}DeleteAccount";
        private static readonly string addWalletAddress = $"{Uri}AddWalletAddress";
        private static readonly string removeWalletAddress = $"{Uri}RemoveWalletAddress";
        private static readonly string updateSession = $"{Uri}UpdateSessionTicket";
        private static readonly string test = $"{Uri}Test";
        private static readonly string logout = $"{Uri}Logout";
        private static readonly string checkApplicationVersion = $"{Uri}CheckApplicationVersion";
        private static readonly string getNfts = $"{Uri}GetNfts";
        
        public static string uri => Uri;

        public static void EmailVerification(Dictionary<string, object> email, Action<string> okResult, Action<string> errorResult)
        {
            if (LoginSignupSceneManager.Instance != null)
            {
                LoginSignupSceneManager.Instance.StartCoroutine(EndPoint(sendEmailVerificationCode, email, okResult, errorResult));
                return;
            }

            if (Settings_PlayerProfile_EmailSceneManager.Instance != null)
            {
                Settings_PlayerProfile_EmailSceneManager.Instance.StartCoroutine(EndPoint(sendEmailVerificationCode, email, okResult, errorResult));
                return;
            }

            if (Settings_PlayerProfile_ChangePasswordSceneManager.Instance != null)
            {
                Settings_PlayerProfile_ChangePasswordSceneManager.Instance.StartCoroutine(
                    EndPoint(sendEmailVerificationCode, email, okResult, errorResult));   
            }

            
            Settings_PlayerProfile_DeleteAccountSceneManager.Instance.StartCoroutine(
                EndPoint(sendEmailVerificationCode, email, okResult, errorResult));
        }
        
        public static void EmailVerificationDeleteAccount(Dictionary<string, object> email, Action<string> okResult, Action<string> errorResult)
        {
            Settings_PlayerProfile_DeleteAccountSceneManager.Instance.StartCoroutine(
                EndPoint(endEmailVerificationDeleteAccount, email, okResult, errorResult));
        }

        public static void LoginViaVerificationCode(Dictionary<string, string> emailAndCode, Action<string> okResult, Action<string> errorResult)
        {
            LoginSignupSceneManager.Instance.StartCoroutine(EndPoint(loginViaVerificationCode, emailAndCode, okResult, errorResult));
        }

        public static void CreateNewUser(Player player, Action<string> okResult, Action<string> errorResult)
        {
            SetPasswordSceneManager.Instance.StartCoroutine(EndPoint(createNewUser, player, okResult, errorResult));
        }

        public static void LoginViaPassword(Dictionary<string, string> emailAndPassword, Action<string> okResult, Action<string> errorResult)
        {
            LoginWithPasswordSceneManager.Instance.StartCoroutine(EndPoint(loginViaPassword, emailAndPassword, okResult, errorResult));
        }
        
        public static void Logout(Dictionary<string, string> email, Action<string> okResult, Action<string> errorResult)
        {
            MainMenuSceneManager.Instance.StartCoroutine(EndPoint(logout,
                email, okResult, errorResult));
        }
        
        public static void ActivateAccount(Dictionary<string, string> playFabIdAndCode, Action<string> okResult, Action<string> errorResult)
        {
            ActivationCodeSceneManager.Instance.StartCoroutine(EndPoint(activateAccount, playFabIdAndCode, okResult, errorResult));
        }
        
        public static void UpdateEmail(Dictionary<string, string> emailUpdate, Action<string> okResult, Action<string> errorResult)
        {
            Settings_PlayerProfile_EmailSceneManager.Instance.StartCoroutine(EndPoint(updateEmail,
                emailUpdate, okResult, errorResult));
        }
        
        public static void UpdatePassword(Dictionary<string, string> passwordUpdate, Action<string> okResult, Action<string> errorResult)
        {
            Settings_PlayerProfile_ChangePasswordSceneManager.Instance.StartCoroutine(EndPoint(updatePassword,
                passwordUpdate, okResult, errorResult));
        }
        
        public static void UpdateAvatar(Dictionary<string, string> avatarUpdate, Action<string> okResult, Action<string> errorResult)
        {
            Settings_PlayerProfile_SelectAvatarSceneManager.Instance.StartCoroutine(EndPoint(updateAvatar, avatarUpdate, okResult, errorResult));
        }
        
        public static void DeleteAccount(Dictionary<string, string> emailAndCode, Action<string> okResult, Action<string> errorResult)
        {
            Settings_PlayerProfile_DeleteAccountSceneManager.Instance.StartCoroutine(EndPoint(deleteAccount,
                emailAndCode, okResult, errorResult));
        }
        public static void AddWallet(Dictionary<string, string> emailAndWallet, Action<string> okResult, Action<string> errorResult)
        {
            MainMenuSceneManager.Instance.StartCoroutine(EndPoint(addWalletAddress,
                emailAndWallet, okResult, errorResult));
        }
        
        public static void RemoveWallet(Dictionary<string, string> emailAndWallet, Action<string> okResult, Action<string> errorResult)
        {
            MainMenuSceneManager.Instance.StartCoroutine(EndPoint(removeWalletAddress,
                emailAndWallet, okResult, errorResult));
        }
        
        public static void UpdateSession(LoginResult result, Action<string> okResult = null, Action<string> errorResult = null)
        {
            PlayerDatabase.AccountProfile.SetSessionTicket(result.SessionTicket);
            var emailAndSessionTicket = new Dictionary<string, string>
            {
                { "email", PlayerDatabase.AccountProfile.Email },
                { "sessionTicket", result.SessionTicket }
            };
            
            LoadingSceneManager.Instance.StartCoroutine(EndPoint(updateSession,
                emailAndSessionTicket, okResult, errorResult));
        }

        public static void Test(Dictionary<string, string> emailAndWallet, Action<string> okResult,
            Action<string> errorResult)
        {
            EnergyManager.Instance.StartCoroutine(EndPoint(test,
                emailAndWallet, okResult, errorResult));
        }
        
        public static void GetAppVersion(Dictionary<string, string> body, Action<string> okResult,
            Action<string> errorResult)
        {
            AzureSettingsManager.Instance.StartCoroutine(EndPoint(checkApplicationVersion,
                body, okResult, errorResult));
        }
        
        public static void GetNfts(Dictionary<string, string> body, Action<string> okResult,
            Action<string> errorResult)
        {
            AzureSettingsManager.Instance.StartCoroutine(EndPoint(getNfts,
                body, okResult, errorResult));
        }
        
        public static IEnumerator EndPoint(string endPoint, object postObject, Action<string> okResult, Action<string> errorResult)
        {
            using var webRequest = new UnityWebRequest(endPoint, "POST");

            var jsonBodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(postObject));
            webRequest.uploadHandler = new UploadHandlerRaw(jsonBodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            
            if(AzureSettingsManager.Instance.AzureSettings.profileName != "LocalHost")
                webRequest.SetRequestHeader("x-functions-key", AzureSettingsManager.Instance.AzureSettings.hostKey);

            Debug.Log($"Request end point {endPoint}, x-functions-key {AzureSettingsManager.Instance.AzureSettings.hostKey}");

            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                okResult?.Invoke(webRequest.downloadHandler.text);
                yield break;
            }
            
            errorResult?.Invoke(webRequest.downloadHandler.text);
        }
    }
}