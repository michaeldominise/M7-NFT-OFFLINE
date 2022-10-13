using M7;
using M7.GameData;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.PlayfabCloudscript.PlayerDatabase;
using M7.ServerTestScripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
// using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;

public class GetPlayerBattleData: MonoBehaviour
{
    /// <summary>
    /// GET BATTLE DATA
    /// </summary>
    public void GetBattleGameData()
    {
        MessageBox.Create("Loading stage.", MessageBox.ButtonType.Loading).Show();
        TeamInfoSceneManager.Instance.LoadNextSceneCallBack(DownloadDataRuntime.Instance.isLoadScene);
        //string jsonData = PlayerDatabase.Teams.selectedTeamName;
        //PlayFabFunctions.PlayFabCallFunction("GetBattleData", false, "", jsonData, GetAllMainDataCallback);
    }
    public void GetAllMainDataCallback(ExecuteResult result)
    {
        GetPlayerData(result);
    }

    void GetPlayerData(ExecuteResult result)
    {
        var playerdata = JsonConvert.DeserializeObject<PlayerData>(result.Result.FunctionResult.ToString());

        Debug.Log(result.Result.FunctionResult);

        if (result.Status != ResultStatus.Ok) return;

        if (playerdata == null) return;

        PlayerDatabase.Inventories.SystemCurrencies.OverwriteValues(playerdata.systemCurrencies);
        PlayerDatabase.Teams.OverwriteValues(playerdata.teams);
        PlayerDatabase.CampaignData.OverwriteValues(playerdata.campaignData);
        PlayerDatabase.Inventories.Boosters.OverwriteValues(playerdata.boosters);

        //PlayerDatabase.GlobalDataSetting.StageAllDataSettings.OverWrite_Stage(playerdata.stageDataSettingsList);
        //PlayerDatabase.GlobalDataSetting.PlayerAllLevelChartData.OverWrite_Stage(playerdata.playerLevelChartDataSettings);

        GetNfts();
    }

    public void GetNfts()
    {
        // call for NFT
        var email = new Dictionary<string, string>
        {
            { "email", PlayerDatabase.AccountProfile.Email }
        };

        AzureFunction.GetNfts(email, OkResult, ErrorResult);
    }

    private void ErrorResult(string errorResult)
    {
        Debug.Log($"Get NFT error {errorResult}");
    }

    private void OkResult(string result)
    {
        Debug.Log($"Get NFT ok {result}");
        var nft = JsonConvert.DeserializeObject<Nfts>(result);

        if (nft == null)
            return;

        PlayerDatabase.Inventories.Characters.OverwriteValues(nft.characters);
        PlayerDatabase.Inventories.Incubators.OverwriteValues(nft.incubators);
        PlayerDatabase.Inventories.Gems.OverwriteValues(nft.gems);

        MessageBox.HideCurrent();
        TeamInfoSceneManager.Instance.LoadNextSceneCallBack(DownloadDataRuntime.Instance.isLoadScene);

    }


}
