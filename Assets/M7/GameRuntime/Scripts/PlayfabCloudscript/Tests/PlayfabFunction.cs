using System;
using System.Collections.Generic;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.BackEnd.CosmosDB;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.PlayfabCloudscript.Maintenance;
using M7.GameRuntime.Scripts.PlayfabCloudscript.Model;
using M7.GameRuntime.Scripts.PlayfabCloudscript.Player;
using M7.GameRuntime.Scripts.UnityPlayfab;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.GameRuntime.Scripts.Energy.Tests
{
    #if UNITY_EDITOR

    [Serializable]
    public class PlayFabFunction
    {
        [Button]
        private void GetHeroCollection()
        {
            FunctionTest();
        }
        
        private void FunctionTest()
        {
            PlayFabFunctions.PlayFabCallFunction("GetUserData", false, "HeroCollection", "", GetResult);
        }

        private void GetResult(ExecuteResult executeResult)
        {
            Debug.Log($"Callback result {executeResult.Result.FunctionResult}");
        }

        [Button]
        private void AddHero(string json)
        {
            PlayFabFunctions.PlayFabCallFunction("AddHero", false, "characters", json, AddHeroResult);
        }

        private void AddHeroResult(ExecuteResult executeResult)
        {
            Debug.Log($"Callback add hero result {executeResult.Result.FunctionResult}");
        }

        [Button]
        private void GetHighestLevel()
        {
            PlayFabFunctions.PlayFabCallFunction("GetUserData", false, "HighestLevel", "", GetHighestLevelResult);
        }

        private void GetHighestLevelResult(ExecuteResult executeResult)
        {
            Debug.Log($"Highest Level, {executeResult.Result.FunctionResult}");
        }

        [Button]
        private void SetHighestLevel(string highestLevel)
        {
            var jsonString = $"{{\"Amount\" : {highestLevel}}}";
            PlayFabFunctions.PlayFabCallFunction("SetUserData", false, "HighestLevel", jsonString, SetHighestLevelResult);
        }

        private void SetHighestLevelResult(ExecuteResult executeResult)
        {
            Debug.Log($"Highest Level, {executeResult.Result.FunctionResult}");
        }

        [Button]
        private void GetAllUser()
        {
            PlayFabFunctions.PlayFabCallFunction("GetAllUserData", false, "", "{\"name\": \"kit\",\"age\": \"32\"}", GetAllUserCallback);
        }

        private void GetAllUserCallback(ExecuteResult executeResult)
        {
            Debug.Log($"Hello callback {executeResult.Result}");
        }

        [Button]
        private void IsMaintenance()
        {
            PlayFabFunctions.PlayFabCallFunction("GameStatus", false, "isMaintenance", "", GenericCallback);
        }

        private void GenericCallback(ExecuteResult executeResult)
        {
            Debug.Log($"FunctionName {executeResult.Result.FunctionName}. Generic callback result {executeResult.Result.FunctionResult}");
        }

        [Button]
        private void CheckApplicationVersion(string args)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(args);
            PlayFabFunctions.PlayFabCallFunction("CheckApplicationVersion", false, dict, VersionCallback);
        }

        private void VersionCallback(ExecuteResult obj)
        {
            Debug.Log($"Version callback {obj.Result.FunctionResult}");
        }

        #region PLAYERTYPE
        [Button]
        private void GetPlayerType()
        {
            PlayFabPlayerType.GetPlayerType();
        }

        [Button]
        private void SetPlayerType(PlayerType playerType)
        {
            PlayFabPlayerType.SetPlayerType(playerType);
        }
        
        private void PlayerTypeCallback(ExecuteResult obj)
        {
            Debug.Log($"player type {obj.Result.FunctionResult}");
        }
        #endregion

        #region MAINTENANCE
        [Button]
        private void Maintenance()
        {
            CheckMaintenance.GetMaintenanceStatus(MaintenanceCallback);
        }

        private void MaintenanceCallback(ExecuteResult obj)
        {
            var maintenanceResult =
                JsonConvert.DeserializeObject<MaintenanceModel>(obj.Result.FunctionResult.ToString());
            
            Debug.Log($"Is Maintenance {maintenanceResult.isMaintenance}, message {maintenanceResult.message}");
        }
        #endregion

        #region REGISTER
        [SerializeField, FoldoutGroup("Register")] private string email, password;
        [Button, FoldoutGroup("Register")]
        private void Register()
        {
            PlayFabAccount.Register(email, password, ResultCallback, ErrorCallback);
        }

        private void ResultCallback(RegisterPlayFabUserResult obj)
        {
            Debug.Log($"Player Registered. PlayFabID {obj.PlayFabId}");
        }
        
        private void ErrorCallback(PlayFabError obj)
        {
            Debug.Log($"Error. {obj.ErrorMessage}");
        }
        #endregion

        #region LOGIN
        [FoldoutGroup("Login"),SerializeField] private string loginEmail;
        [FoldoutGroup("Login"), Button]
        private void Login()
        {
            PlayFabAccount.Login(loginEmail, LoginResultCallback, LoginErrorCallback);
        }

        private void LoginResultCallback(LoginResult obj)
        {
            Debug.Log($"Player LoggedIn. PlayFabID {obj.PlayFabId}");
        }
        
        private void LoginErrorCallback(PlayFabError obj)
        {
            Debug.Log($"Error. {obj.ErrorMessage}");
        }
        #endregion

        #region DISPLAYNAME

        [FoldoutGroup("DisplayName"), SerializeField]
        private string newDisplayName;

        [FoldoutGroup("DisplayName"), Button]
        private void ChangeDisplayName()
        {
            PlayFabAccount.ChangeDisplayName(newDisplayName, ChangeDisplayResultCallback, ErrorCallback);
        }

        private void ChangeDisplayResultCallback(UpdateUserTitleDisplayNameResult obj)
        {
            Debug.Log($"New Display Name. {obj.Request}");
        }
        
        #endregion

        #region CHANGEEMAIL
        [FoldoutGroup("Change email"), SerializeField]
        private string oldEmail;
        [FoldoutGroup("Change email"), SerializeField]
        private string newEmail;
        [FoldoutGroup("Change email"), Button]
        private void ChangeEmail()
        {
            PlayFabAccount.ChangeEmail(oldEmail, newEmail, ChangeEmailCallback, ErrorCallback);
        }

        private void ChangeEmailCallback(LinkCustomIDResult linkCustomIDResult)
        {
            MessageBox.Create("email successfully changed", MessageBox.ButtonType.Cancel, "Email").Show();
        }
        
        #endregion
        
        #region UNLINKID
        [FoldoutGroup("Unlink Custom ID"), SerializeField]
        private string id;

        [FoldoutGroup("Unlink Custom ID"), Button]
        private void UnlinkCustomId()
        {
            PlayFabAccount.UnlinkCustomId(id, ResultCallback, ErrorCallback);
        }

        private void ResultCallback(LinkCustomIDResult obj)
        {
            Debug.Log(obj);
        }

        #endregion
        
        #region EMAILVERIFICATION

        [FoldoutGroup("Email verification"), SerializeField]
        private string sentToEmail;

        [FoldoutGroup("Email verification"), Button]
        private void EmailVerification()
        {
            var theEmail = new Dictionary<string, object> { { "email", sentToEmail } };
            AzureFunction.EmailVerification(theEmail, EmailOkResult, EmailErrorResult);
        }

        private void EmailErrorResult(string obj)
        {
            var response = JsonConvert.DeserializeObject<Response>(obj);
            MessageBox.Create($"EmailVerification error {response.Message}", MessageBox.ButtonType.Ok,
                "EmailVerification", "OK").Show();
        }

        private void EmailOkResult(string obj)
        {
            var response = JsonConvert.DeserializeObject<Response>(obj);
            MessageBox.Create($"{response.Message}", MessageBox.ButtonType.Ok, "EmailVerification", "OK").Show();
        }

        #endregion
        
        #region VERIFYEMAIL

        [FoldoutGroup("Verify Email"), SerializeField]
        private string emailToVerify;
        [FoldoutGroup("Verify Email"), SerializeField]
        private string activationCode;
        

        [FoldoutGroup("Verify Email"), Button]
        private void VerifyEmail()
        {
            var theEmail = new Dictionary<string, string> 
            { 
                { "email", emailToVerify }, 
                { "verificationCode", activationCode } 
            };
            AzureFunction.LoginViaVerificationCode(theEmail, OkResult, ErrorResult);
        }
        
        private void OkResult(string obj)
        {
            var responseResult = JsonConvert.DeserializeObject<Response>(obj);

            if (responseResult.ResponseResult == ResponseResult.OK) 
            {
                var result = JsonConvert.DeserializeObject<Dictionary<string, bool>>(responseResult.Data.ToString());
                Debug.Log($"Verification done, does the user exist? {result != null && result["userExists"]}");
                return;
            }
            
            Debug.Log($"Verification failed, {responseResult.Message}");
        }

        private void ErrorResult(string obj)
        {
            var errorResponse = JsonConvert.DeserializeObject<Response>(obj);
            Debug.Log($"Error,  {errorResponse.Message}");
            // PlayFabSettings.staticSettings.
        }

        #endregion

        #region SavePuzzleGameOnBoarding

        [FoldoutGroup("SavePuzzleGameOnBoarding"), Button]
        private void SavePuzzleGameOnBoarding(string json)
        {
            var param = new Dictionary<string, object>
            {
                { "json", json }
            };
            
            PlayFabFunctions.PlayFabCallFunction("UpdatePuzzleBoardOnBoarding", false, param, PuzzleGameOnBoardingCallback, ErrorCallback);
        }

        private void PuzzleGameOnBoardingCallback(ExecuteResult obj)
        {
            Debug.Log($"PuzzleGameOnBoardingCallback, {obj}");
        }

        #endregion

        #region TEST_Code
        [FoldoutGroup("Test Code"), Button]
        private void TestCode()
        {
            var theEmail = new Dictionary<string, string> 
            { 
                { "name", "kit" } 
            };
            AzureFunction.Test(theEmail, TestOk, ErrorResult);
        }
        
        private void TestOk(string obj)
        {
            var responseResult = JsonConvert.DeserializeObject<Response>(obj);
            Debug.Log(responseResult.Message);
        }
        
        #endregion
    }
#endif
}