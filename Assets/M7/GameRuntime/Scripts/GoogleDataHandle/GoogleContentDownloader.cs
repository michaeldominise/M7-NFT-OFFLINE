#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System;
using Newtonsoft.Json;

namespace M7.ToolDownload
{
    public static class GoogleContentDownloader
    {
        static readonly string SkillObject_Sheet_Id = "1fd_G68oI5lHnrq-SeFDQYfFmPo7gFWpDT6LLk7CZMlY";
        static readonly string PLAYFAB_SkillObject_REQUIREMENT_KEY = "campaignEnergyRequirement";
        static readonly string PLAYFAB_SPECIAL_OFFER_KEY = "specialOffers";
        
        public static void updateSKills(string sheetId, Action<SkillObjectList> Success)
        {
            //GoogleTSVDownloader.Download(
            //    SkillObject_Sheet_Id,
            //       sheetId,
            //       (result, isSuccess) =>
            //       {
            //           Debug.Log(result);
            //           SkillObjectReader.ReadCSV(result,
            //               ConvertedData =>
            //               {
            //                   var title = "Download";
            //                   var message = string.Format("Download success!");
            //                   var ok = "Ok";
                       
            //                   EditorUtility.DisplayDialog(title, message, ok);
                       
            //                   Success(ConvertedData);
                       
            //               });
        
            //       }
            //   );
        }
        // public static void UpdateDropTable(string sheetId, Action<List<RandomResultTable>> Success)
        // {
        //     TSVDownloader.Download(
        //            PLAYFAB_SPREADSHEET_ID,
        //            sheetId,
        //            (result, isSuccess) =>
        //            {
        //                Debug.Log(result);
        //                PlayFabTableReader.ReadCSV(result,
        //                    ConvertedData =>
        //                    {
        //                        Debug.Log("[PlayfabContentTool] UpdateDropTable ");
        //                        var title = "Download";
        //                        var message = string.Format("Download success!");
        //                        var ok = "Ok";
        //
        //                        EditorUtility.DisplayDialog(title, message, ok);
        //
        //                        Success(ConvertedData.Tables);
        //
        //                    });
        //
        //            }
        //        );
        // }

        
        
        // public static void UploadEnergyRequirements(List<SkillObjectData> data, Action<SetTitleDataResult> onFinish)
        // {
        //     PlayfabContentUtility.UploadContent(PLAYFAB_SkillObject_REQUIREMENT_KEY, data, PlayfabContentUtility.TitleType.InternalTitleData, onFinish);
        // }
    }
}
#endif
