using M7.GameRuntime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using System.Collections;
using M7.GameData;
using M7;

public partial class SROptions
{
    static string AddNftCharacterUri = $"{AzureFunction.uri}AddNftCharacter";
   
    static string DeleteNftCharacterUri = $"{AzureFunction.uri}DeleteNftCharacter";
    static string AddNftIncubatorUri = $"{AzureFunction.uri}AddNftIncubator";

    int randomHeroCount = 30;
    [Category("AddHeroes_Random")] public void AddHeroes_Random() => GameManager.Instance.StartCoroutine(_AddHeroes(1, 1000, randomHeroCount));
    [Category("AddHeroes_Random")] public int RandomHeroCount { get => randomHeroCount; set => randomHeroCount = value; }

    int tokenIdStart = 1;
    int heroCount = 1;
    [Category("AddHeroes_TokenId")] public void AddHeroes_TokenId() => GameManager.Instance.StartCoroutine(_AddHeroes(tokenIdStart, tokenIdStart + heroCount - 1, heroCount));
    [Category("AddHeroes_TokenId"), NumberRange(1, 1000)] public int TokenIdStart { get => tokenIdStart; set => tokenIdStart = value; }
    [Category("AddHeroes_TokenId"), NumberRange(1, 1000)] public int HeroCount { get => heroCount; set => heroCount = value; }

    IEnumerator _AddHeroes(int startTokenId, int maxTokenId, int heroCount)
    {
        if (string.IsNullOrWhiteSpace(PlayerDatabase.AccountProfile.WalletAddress))
            yield break;

        SRDebug.Instance.HideDebugPanel();
        var messsageBox = MessageBox.Create("AddHeroes_TokenId.", MessageBox.ButtonType.Loading).Show();

        var randomHeroTokenIdList = new List<int>();
        for (var x = startTokenId; x <= maxTokenId; x++)
            randomHeroTokenIdList.Add(x);

        var icubatorTokenIdList = new List<int>();
        for (var i = 0; i < heroCount; i++)
        {
            var rnd = UnityEngine.Random.Range(0, randomHeroTokenIdList.Count);
            var tokenId = randomHeroTokenIdList[rnd];
            randomHeroTokenIdList.RemoveAt(rnd);

            messsageBox.InitValues($"AddHeroes_TokenId: tokenId:{tokenId} {i + 1}/{heroCount}.", MessageBox.ButtonType.Loading).Show();
            yield return AddCharacterNft(tokenId, null, errorResult => icubatorTokenIdList.Add(tokenId), true);
        }

        foreach (var icubatorTokenId in icubatorTokenIdList)
        {
            messsageBox.InitValues($"AddRandomIncubators: tokenId:{icubatorTokenId}.", MessageBox.ButtonType.Loading).Show();
            yield return AddCharacterNft(icubatorTokenId, null, null, false);
        }
        messsageBox.Hide();

        GameManager.RestartGameDialog();
    }

    public static IEnumerator AddCharacterNft(int tokenId, Action<string> okResult, Action<string> errorResult, bool isHero)
    {
        var nftData = new Dictionary<string, object>();
        nftData["locationAddress"] = "test";
        //nftData["walletAddress"] = PlayerDatabase.AccountProfile.WalletAddress;
        nftData["hashID"] = PlayerDatabase.AccountProfile.Email;
        nftData["tokenId"] = tokenId;
        // yield return AzureFunction.EndPoint(isHero ? AddNftCharacterUri : AddNftIncubatorUri, nftData, okResult, errorResult);
        yield return null;

        var instanceId = $"{PlayerDatabase.Inventories.Characters.GetSortedItems().Count + 1}";
        SaveableCharacterData savChar = null;
        yield return GameManager.Instance.AddHeroNftManager.GenerateHero(instanceId, result => savChar = result);
        PlayerDatabase.Inventories.Characters.AddItem(savChar);
    }
}