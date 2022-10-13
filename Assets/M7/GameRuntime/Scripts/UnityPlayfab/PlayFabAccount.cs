using System;
using M7.GameData;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace M7.GameRuntime.Scripts.UnityPlayfab
{
    public static class PlayFabAccount
    {
        public static void Register(string email, string password, Action<RegisterPlayFabUserResult> resultCallback, 
            Action<PlayFabError> errorCallback)
        {
            var request = new RegisterPlayFabUserRequest
            {
                Email = email,
                Password = password,
                RequireBothUsernameAndEmail = false
            };

            PlayFabClientAPI.RegisterPlayFabUser(request,
            result =>
            {
                resultCallback?.Invoke(result);
            },
            error =>
            {
                errorCallback?.Invoke(error);
            });
        }
        
        public static void Login(string email, Action<LoginResult> resultCallback, 
            Action<PlayFabError> errorCallback)
        {
            var request = new LoginWithCustomIDRequest()
            {
                CustomId = email
            };

            PlayFabClientAPI.LoginWithCustomID(request,
                result =>
                {
                    AzureFunction.UpdateSession(result);
                    Debug.Log(JsonConvert.SerializeObject(result));
                    resultCallback?.Invoke(result);
                },
                error =>
                {
                    errorCallback?.Invoke(error);
                });
        }
        
        // login, create new user if account does not exist
        public static void LoginCreate(string email, Action<LoginResult> resultCallback, 
            Action<PlayFabError> errorCallback)
        {
            var request = new LoginWithCustomIDRequest
            { 
                CustomId = email,
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithCustomID(request,
                result =>
                {
                    resultCallback?.Invoke(result);
                },
                error =>
                {
                    errorCallback?.Invoke(error);
                });
        }

        public static void ChangeDisplayName(string newDisplayName, Action<UpdateUserTitleDisplayNameResult> resultCallback, 
            Action<PlayFabError> errorCallback)
        {
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = newDisplayName
            };
            
            PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            result =>
            {
                resultCallback?.Invoke(result);    
            },
            error =>
            {
                errorCallback?.Invoke(error);
            });
        }

        public static void ChangeEmail(string oldEmail, string newEmail, Action<LinkCustomIDResult> resultCallback, Action<PlayFabError> errorCallback)
        {
            var request = new UnlinkCustomIDRequest
            {
                CustomId = oldEmail
            };
            
            PlayFabClientAPI.UnlinkCustomID(request,
            result =>
            {
                var linkCustomIDRequest = new LinkCustomIDRequest
                {
                    CustomId = newEmail
                };
                
                PlayFabClientAPI.LinkCustomID(linkCustomIDRequest, 
                    idResult =>
                    {
                        resultCallback?.Invoke(idResult);
                    },
                    error =>
                    {
                        errorCallback?.Invoke(error);
                    });
            },
            error =>
            {
                errorCallback?.Invoke(error);
            });            
        }

        public static void UnlinkCustomId(string id, Action<LinkCustomIDResult> resultCallback, Action<PlayFabError> errorCallback)
        {
            var request = new UnlinkCustomIDRequest
            {
                CustomId = id
            };
            
            PlayFabClientAPI.UnlinkCustomID(request,
                result =>
                {
                    Debug.Log("unlink done");
                },
                error =>
                {
                    Debug.Log("unlink error");
                });            
        }

        public static void SavePuzzleGameOnBoarding(string stageName)
        {
            
        }
    }
}