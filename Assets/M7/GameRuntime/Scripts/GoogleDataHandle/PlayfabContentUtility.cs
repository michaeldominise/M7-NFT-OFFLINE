#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using System;


using PlayFab;
using PlayFab.ClientModels;

#if ENABLE_PLAYFABSERVER_API
using ServerModels = PlayFab.ServerModels;
#endif
namespace M7.ToolDownload
{
    public static class PlayfabContentUtility
    {
        public static GUITable table;

        // public static void UpdateServerTitleData(string key, string data, Action<SetTitleDataResult> OnSuccess = null, Action<PlayFabError> OnFail = null)
        // {
        //     var title = "Upload Data";
        //     var message = string.Format("Are you sure you want to upload this data to the {0} Server?", PlayFabSettings.TitleId);
        //     var ok = "Okay";
        //     var cancel = "Cancel";

        //     if (!EditorUtility.DisplayDialog(title, message, ok, cancel))
        //         return;

        //     PlayFabAdminAPI.SetTitleData(
        //         new SetTitleDataRequest
        //         {
        //             Key = key,
        //             Value = data
        //         }
        //         ,
        //         Success =>
        //         {
        //             Debug.Log("UPLOAD SUCCESS");
        //             var successTitle = "Upload Data";
        //             var successMessage = "Upload Success";

        //             EditorUtility.DisplayDialog(successTitle, successMessage, ok);
        //         },
        //         Fail =>
        //         {
        //             var failTitle = "Upload Data";
        //             var failMessage = Fail.ErrorMessage;

        //             EditorUtility.DisplayDialog(failTitle, failMessage, ok);
        //         });
        // }

        // public static void UpdateInternalServerData(string key, string data, Action<SetTitleDataResult> OnSuccess = null, Action<PlayFabError> OnFail = null)
        // {
        //     var title = "Upload Data";
        //     var message = string.Format("Are you sure you want to upload this INTERNAL data to the {0} Server?", PlayFabSettings.TitleId);
        //     var ok = "Okay";
        //     var cancel = "Cancel";

        //     if (!EditorUtility.DisplayDialog(title, message, ok, cancel))
        //         return;

        //     PlayFabAdminAPI.SetTitleInternalData(
        //         new SetTitleDataRequest
        //         {
        //             Key = key,
        //             Value = data
        //         }
        //         ,
        //         Success =>
        //         {
        //             Debug.Log("UPLOAD SUCCESS");
        //             var successTitle = "Upload Data";
        //             var successMessage = "Upload Success";

        //             EditorUtility.DisplayDialog(successTitle, successMessage, ok);
        //         },
        //         Fail =>
        //         {
        //             var failTitle = "Upload Data";
        //             var failMessage = Fail.ErrorMessage;

        //             EditorUtility.DisplayDialog(failTitle, failMessage, ok);
        //         });
        // }

        // public static void UploadContent(string key, object data, TitleType type, Action<SetTitleDataResult> onFinish = null)
        // {
        //     string convertedData = Newtonsoft.Json.JsonConvert.SerializeObject(data);

        //     switch (type)
        //     {
        //         case TitleType.TitleData:
        //             UpdateServerTitleData(key, convertedData, onFinish);
        //             break;
        //         case TitleType.InternalTitleData:
        //             UpdateInternalServerData(key, convertedData, onFinish);
        //             break;
        //     }
        // }

        // public static void UploadDropTable(UpdateRandomResultTablesRequest dropTable)
        // {
        //     var title = "Upload Data";
        //     var message = string.Format("Are you sure you want to upload this DROP TABLE to the {0} Server?\n WARNING!!! This will overwrite existing drop tables in the server!", PlayFabSettings.TitleId);
        //     var ok = "Ok";
        //     var cancel = "Cancel";

        //     if (!EditorUtility.DisplayDialog(title, message, ok, cancel))
        //         return;

        //     PlayFabAdminAPI.UpdateRandomResultTables(
        //         dropTable
        //         ,
        //         Success =>
        //         {
        //             Debug.Log("UPLOAD SUCCESS");
        //             var successTitle = "Upload Data";
        //             var successMessage = "Upload Success";

        //             EditorUtility.DisplayDialog(successTitle, successMessage, ok);
        //         },
        //         Fail =>
        //         {
        //             var failTitle = "Upload Data";
        //             var failMessage = Fail.ErrorMessage + "\nHTTP Status:" + Fail.HttpCode + Fail.HttpStatus;
        //             EditorUtility.DisplayDialog(failTitle, failMessage, ok);
        //         });
        // }


#if ENABLE_PLAYFABSERVER_API
        /// <summary>
        /// Retrieves the server time in Playfab
        /// </summary>
        /// <param name="serverTime">returns the server time</param>
        public static void GetServerTime(Action<ServerModels.GetTimeResult> OnGetTimeSuccess)
        {
            PlayFabServerAPI.GetTime(new ServerModels.GetTimeRequest(), OnGetTimeSuccess,
                Fail =>
                {
                    var failTitle = "Fetch Time";
                    var failMessage = Fail.ErrorMessage;
                    var ok = "Ok";

                    EditorUtility.DisplayDialog(failTitle, failMessage, ok);
                });
        }
#endif

        public enum TitleType
        {
            TitleData,
            InternalTitleData
        }
    }
}
#endif