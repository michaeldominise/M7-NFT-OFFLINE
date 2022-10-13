using M7;
using M7.GameData;
using M7.GameRuntime;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.PlayfabCloudscript.PlayerDatabase;
using M7.ServerTestScripts;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetGaianite : SceneManagerBase
{
    /// <summary>
    /// SET COLLECTED CUBE
    /// </summary>
    public void SetCollectedCubes(int[] cubes)
    {
        MessageBox.Create("Saving game progress...", MessageBox.ButtonType.Loading).Show();
        BattleManager.Instance.EndGameSet.SetGaianite();
        MessageBox.HideCurrent();
        TransitionOverlay.Show(0);
        LoadScene(BattleManager.Instance.GameFinishManager.mainScene, LoadSceneMode.Single, overwriteSceneLayer: 0, forceLoad: true);
        //float sum = cubes.Sum();
        //string jsonString = $"{{ \"cubesDestroyed\" : {sum}}}";
        //PlayFabFunctions.PlayFabCallFunction("SetCollectedGaianiteData", false, "", jsonString, GetAllMainDataCallback);
    }
    public void GetAllMainDataCallback(ExecuteResult result)
    {
        GetPlayerData(result);
    }

    void GetPlayerData(ExecuteResult result)
    {
        var playerdata = JsonConvert.DeserializeObject<PlayerData>(result.Result.FunctionResult.ToString());

        if (result.Status != ResultStatus.Ok) return;

        if (playerdata == null) return;

        PlayerDatabase.Teams.OverwriteValues(playerdata.teams);
        PlayerDatabase.CampaignData.OverwriteValues(playerdata.campaignData);
        PlayerDatabase.Inventories.SystemCurrencies.OverwriteValues(playerdata.systemCurrencies);

        PlayerDatabase.GlobalDataSetting.StageAllDataSettings.OverWrite_Stage(playerdata.stageDataSettingsList);
        PlayerDatabase.GlobalDataSetting.PlayerAllLevelChartData.OverWrite_Stage(playerdata.playerLevelChartDataSettings);

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
        TransitionOverlay.Show(0);
        LoadScene(BattleManager.Instance.GameFinishManager.mainScene, LoadSceneMode.Single, overwriteSceneLayer: 0, forceLoad: true);

    }
}

