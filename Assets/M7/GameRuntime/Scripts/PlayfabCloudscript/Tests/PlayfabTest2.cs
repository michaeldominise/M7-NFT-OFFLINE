#if UNITY_EDITOR
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using Sirenix.Utilities;
using UnityEngine;

namespace M7.GameRuntime.Scripts.Energy.Tests
{
    public class PlayfabTest2 : MonoBehaviour
    {
        [SerializeField] string customId;        

        [SerializeField] private PlayFabFunction playFabFunction;

        // private void Start()
        // {
        //     var request = new LoginWithCustomIDRequest()
        //     {
        //         TitleId = PlayFabSettings.TitleId,
        //         CreateAccount = false,
        //         CustomId = customId,
        //         InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {GetPlayerProfile = true, GetUserData = true}
        //     };
        //     PlayFabClientAPI.LoginWithCustomID(request, (result) =>
        //         {
        //             Debug.Log("Got PlayFabID: " + request.CustomId);
        //             print(result.NewlyCreated);
        //             if (result.NewlyCreated)
        //             {
        //                 print($"New user {result.NewlyCreated}");
        //             }
        //             else
        //             {
        //                 StartCoroutine(CheckPlayFabSession(result));
        //             }
        //     
        //         },
        //         (error) =>
        //         {
        //             Debug.Log("Error logging in player with custom ID:");
        //             Debug.Log(error.ErrorMessage);
        //         });
        // }

        private IEnumerator CheckPlayFabSession(LoginResult result)
        {
            var DisplayName = result.InfoResultPayload.PlayerProfile.DisplayName;
                        
            print($"Hello {DisplayName}");
            print($"Playfab ID {result.PlayFabId}");
            // if (DisplayName.IsNullOrWhitespace())
            // {
            //     print($"No displayname");
            // }
            // else
            // {
            //     print($"Hello {DisplayName}");
            //     PlayerDataMachine.PlayerName = DisplayName;
            //     LoadingSceneManager.Instance.StartDownloadCDN();
            // }
            Debug.Log("(existing account)");
            yield break;
        }
    }
}
#endif
