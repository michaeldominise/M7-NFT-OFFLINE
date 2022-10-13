using PlayFab;
using PlayFab.ClientModels;
using Sirenix.Utilities;
using UnityEngine;
using M7.ServerTestScripts;

    namespace M7.GameRuntime.Scripts.Energy.Tests
{
    public class PlayfabTest : MonoBehaviour
    {
        [SerializeField] string customId;        

        //[SerializeField] private PlayfabFunction playfabFunction;

        private void Start()
        {
            // LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
            // {
            //     TitleId = PlayFabSettings.TitleId,
            //     CreateAccount = true,
            //     CustomId = customId,
            //     InfoRequestParameters = new GetPlayerCombinedInfoRequestParams {GetPlayerProfile = true, GetUserData = true}
            // };
            // PlayFabClientAPI.LoginWithCustomID(request, (result) =>
            //     {
            //         Debug.Log("Got PlayFabID: " + request.CustomId);
            //         print(result.NewlyCreated);
            //         if (result.NewlyCreated)
            //         {
            //             print($"New user {result.NewlyCreated}");
            //         }
            //         else
            //         {
            //             string DisplayName = result.InfoResultPayload.PlayerProfile.DisplayName;
            //             if (DisplayName.IsNullOrWhitespace())
            //             {
            //                 print($"Hello {DisplayName}");
            //             }
            //             else
            //             {
            //                 PlayerDataMachine.PlayerName = DisplayName;
            //                 //LoadingSceneManager.Instance.StartDownloadCDN();
            //             }
            //             Debug.Log("(existing account)");
            //         }
            //
            //         PlayfabDataManager.Instance.Init();
            //     },
            //     (error) =>
            //     {
            //         Debug.Log("Error logging in player with custom ID:");
            //         Debug.Log(error.ErrorMessage);
            //     });
        }
    }
}
