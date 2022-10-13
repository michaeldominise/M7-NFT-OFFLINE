using M7;
using M7.GameData;
using M7.GameRuntime;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.PlayfabCloudscript.PlayerDatabase;
using M7.ServerTestScripts;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroInventoryData : MonoBehaviour
{
    public enum HeroInventoryState { OnReadyLevel, OnConfirmLevel, OnBoostLevel, OnFinishTime }
    public HeroInventoryState heroInventoryState;

    public void HeroInventoryEvent(HeroInventoryState state)
    {
        heroInventoryState = state;

        MessageBox.Create("Loading..", MessageBox.ButtonType.Loading).Show();

        switch (heroInventoryState)
        {
            case HeroInventoryState.OnReadyLevel:
                PlayFabFunctions.PlayFabCallFunction("GetLevelUpData", false, "", "", LevelListOnSuccess);
                break;
            case HeroInventoryState.OnConfirmLevel:
                PlayFabFunctions.PlayFabCallFunction("ConfirmLevelUp", false, "", CharacterInfoManager.Instance.sCharacterData[0].InstanceID, ConfirmLevelSuccess);
                break;
            case HeroInventoryState.OnBoostLevel:
                PlayFabFunctions.PlayFabCallFunction("BoostLevelUp", false, "", CharacterInfoManager.Instance.sCharacterData[0].InstanceID, BoostLevelSuccess);
                break; 
            case HeroInventoryState.OnFinishTime:
                PlayFabFunctions.PlayFabCallFunction("FinishedLeveling", false, "", CharacterInfoManager.Instance.sCharacterData[0].InstanceID, FinishTimeSuccess);
                break;
        }
    }
    void LevelListOnSuccess(ExecuteResult result)
    {
        Debug.Log(result);
        MainMenuSceneManager.Instance.ExecuteButton(NavigationData.NavigationType_MainMenu.Inventory);
        MessageBox.HideCurrent();
    }

    void ConfirmLevelSuccess(ExecuteResult result)
    {
        heroInventoryState = HeroInventoryState.OnConfirmLevel;
        GetSystemCurrencies();
    }

    void BoostLevelSuccess(ExecuteResult result)
    {
        heroInventoryState = HeroInventoryState.OnBoostLevel;
        GetSystemCurrencies();
    }

    void FinishTimeSuccess(ExecuteResult result)
    {
        heroInventoryState = HeroInventoryState.OnFinishTime;
        GetSystemCurrencies();
    }

    public void GetNfts(HeroInventoryState state)
    {
        var email = new Dictionary<string, string>
        {
            { "email", PlayerDatabase.AccountProfile.Email }
        };

        switch (state)
        {
            case HeroInventoryState.OnConfirmLevel:
                AzureFunction.GetNfts(email, OkConfirmLevelResult, ErrorResult);
                break;
            case HeroInventoryState.OnBoostLevel:
                AzureFunction.GetNfts(email, OkBoostLevelResult, ErrorResult);
                break;
            case HeroInventoryState.OnFinishTime:
                AzureFunction.GetNfts(email, OkFinishTimeLevelResult, ErrorResult);
                break;
        }
    }

    private void ErrorResult(string errorResult)
    {
        Debug.Log($"Get NFT error {errorResult}");
    }

    private void OkConfirmLevelResult(string result)
    {
        Debug.Log($"Get NFT ok {result}");
        var nft = JsonConvert.DeserializeObject<Nfts>(result);

        if (nft == null)
            return;

        PlayerDatabase.Inventories.Characters.OverwriteValues(nft.characters);
        PlayerDatabase.Inventories.Currencies.OverwriteValues(nft.currencies);

        CharacterInfoManager.Instance.OnConfirmLevel();
        StaticCurrenciesUIManager.Instance.InitInstanceUI();
        MessageBox.HideCurrent();

    }
    private void OkBoostLevelResult(string result)
    {
        Debug.Log($"Get NFT ok {result}");
        var nft = JsonConvert.DeserializeObject<Nfts>(result);

        if (nft == null)
            return;

        PlayerDatabase.Inventories.Characters.OverwriteValues(nft.characters);
        PlayerDatabase.Inventories.Currencies.OverwriteValues(nft.currencies);

        CharacterInfoManager.Instance.LevelBoosted();
        StaticCurrenciesUIManager.Instance.InitInstanceUI();
        MessageBox.HideCurrent();

    }
    private void OkFinishTimeLevelResult(string result)
    {
        Debug.Log($"Get NFT ok {result}");
        var nft = JsonConvert.DeserializeObject<Nfts>(result);

        if (nft == null)
            return;

        PlayerDatabase.Inventories.Characters.OverwriteValues(nft.characters);
        PlayerDatabase.Inventories.Currencies.OverwriteValues(nft.currencies);

        CharacterInfoManager.Instance.LevelBoosted();
        MessageBox.HideCurrent();

    }

    void GetSystemCurrencies()
    {
        PlayFabFunctions.PlayFabCallFunction("GetAllMainData", false, "", "", GetPlayerData);
    }

    void GetPlayerData(ExecuteResult result)
    {
        var playerdata = JsonConvert.DeserializeObject<PlayerData>(result.Result.FunctionResult.ToString());

        Debug.Log(result.Result.FunctionResult);

        if (result.Status != ResultStatus.Ok) return;

        if (playerdata == null) return;

        PlayerDatabase.Inventories.SystemCurrencies.OverwriteValues(playerdata.systemCurrencies);

        GetNfts(heroInventoryState);
    }
}