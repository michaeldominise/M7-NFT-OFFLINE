using M7;
using M7.CDN;
using M7.GameData;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.PlayfabCloudscript.PlayerDatabase;
using M7.ServerTestScripts;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;

public class GetAllPlayerData: MonoBehaviour
{
    /// <summary>
    /// GET ALL MAIN DATA FUNCTION
    /// </summary>
    /// 

    Tweener progressTween;
    float nextProgressUpdate;
    const string loadingPlayerDataDescription = "Loading player data";

    public void GetAllMainData()
    {
        //var progress = 0f;
        //progressTween = DOTween.To(() => 0f, x => progress = x, 1f, 3).SetEase(Ease.InSine);
        //progressTween.onUpdate += () =>
        //{
        //    if (Time.time < nextProgressUpdate && progress != 1)
        //        return;
        //    nextProgressUpdate = Time.time + Random.Range(0.05f, 0.4f);
        //    LoadingSceneManager.Instance.UpdateProgressBar(loadingPlayerDataDescription, progress);
        //};
        //LoadingSceneManager.Instance.UpdateProgressBar(loadingPlayerDataDescription, 1);
        PlayerDatabase.AccountProfile.IsInitialized = true;
        PlayfabDataManager.Instance.IsInitialized = true;
        LoadingSceneManager.Instance.StartCDNDownload();
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

        if(!PlayerDatabase.DontLoadServerData)
        {
            // kit
            // if (!string.IsNullOrWhiteSpace(PlayerDatabase.AccountProfile.WalletAddress))
            // {
            //     PlayerDatabase.Inventories.Characters.OverwriteValues(playerdata.characters);
            //     PlayerDatabase.Inventories.Incubators.OverwriteValues(playerdata.incubators);
            //     PlayerDatabase.Inventories.Gems.OverwriteValues(playerdata.gems);
            //     PlayerDatabase.Inventories.Currencies.OverwriteValues(playerdata.currencies);
            //     PlayerDatabase.Inventories.Boosters.OverwriteValues(playerdata.boosters);    
            // }

            //PlayerDatabase.Inventories.SystemCurrencies.OverwriteValues(playerdata.systemCurrencies);
            //PlayerDatabase.Inventories.Boosters.OverwriteValues(playerdata.boosters);
            //PlayerDatabase.Teams.OverwriteValues(playerdata.teams);
            //PlayerDatabase.CampaignData.OverwriteValues(playerdata.campaignData);

            GetNfts();
        }
        
        //PlayerDatabase.GlobalDataSetting.StageAllDataSettings.OverWrite_Stage(playerdata.stageDataSettingsList);
        //PlayerDatabase.GlobalDataSetting.PlayerAllLevelChartData.OverWrite_Stage(playerdata.playerLevelChartDataSettings);
        //PlayerDatabase.GlobalDataSetting.RecoveryCostAllChartData.OverWrite_Recovery(playerdata.recoveryCostDataSettings);

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
        progressTween.Kill();
        LoadingSceneManager.Instance.UpdateProgressBar(loadingPlayerDataDescription, 1);
        LoadingSceneManager.Instance.StartCDNDownload();
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

        progressTween.Kill();
        LoadingSceneManager.Instance.UpdateProgressBar(loadingPlayerDataDescription, 1);
        LoadingSceneManager.Instance.StartCDNDownload();

    }

}
