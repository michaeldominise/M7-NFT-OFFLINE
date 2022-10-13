using Fasterflect;
using M7.GameData;
using M7.Tools.Utility;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace M7.Tools
{
    public class StageInfoDrawableWindow : M7DrawableWindow<LevelData>
    {
        public StageInfoDrawableWindow(System.Func<LevelData, bool> filterCondition = null, bool showImport = false) : base(filterCondition, showImport) { }

        public override void ImportData()
        {
            GoogleTSVDownloader.Download(
                spreadSheetID: SheetIDs.MasterListId,
                sheetId: SheetIDs.GetFromType(typeof(LevelData)),
                onDownloadComplete: (result) =>
                {
                    if (result.result == ResultState.Success)
                    {
                        EditorUtility.DisplayProgressBar("Import Data", "Translating sheet data...", 0f);
                        var data = TSVTranslator.Translate<LevelData>(result.downloadedResult) as List<LevelData>;
                        GetStageWaveAndCombine(data);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", result.error, "Ok");
                    }
                });
        }

        private void GetStageWaveAndCombine(List<LevelData> levelDataList)
        {
            GoogleTSVDownloader.Download(
                spreadSheetID: SheetIDs.MasterListId,
                sheetId: SheetIDs.LevelWaveId,
                onDownloadComplete: (result) =>
                {
                    if (result.result == ResultState.Success)
                    {
                        var data = TSVTranslator.TranslateToStageWave(result.downloadedResult);
                        foreach(LevelData levelData in levelDataList)
                        {
                            TeamData_Enemy enemies;
                            if(data.TryGetValue(levelData.MasterID.Replace("_", ""), out enemies))
                            {
                                if(enemies != null)
                                {
                                    levelData.SetFieldValue("teamData", enemies);
                                }
                            }
                        }
                        AssetUtility.CreateAssets(AssetPaths.PathWithType(typeof(LevelData)), levelDataList);
                        EditorUtility.ClearProgressBar();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", result.error, "Ok");
                    }
                });
        }
    }

    public class StageWave
    {

    }
}