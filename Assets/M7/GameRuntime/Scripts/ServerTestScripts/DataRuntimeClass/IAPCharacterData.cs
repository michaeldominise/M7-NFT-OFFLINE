using M7;
using M7.GameData;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.PlayfabCloudscript.PlayerDatabase;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPCharacterData : MonoBehaviour
{
    public Shop_SceneManager Shop_SceneManager => Shop_SceneManager.Instance;

    public void BuyCharacter()
    {
        MessageBox.Create("Loading...", MessageBox.ButtonType.Loading).Show();
        StartCoroutine(AddNonCharacterNft(null, null));
    }

    public IEnumerator AddNonCharacterNft(Action<string> okResult, Action<string> errorResult)
    {
        var nftData = new Dictionary<string, object>();
        nftData["locationAddress"] = "test";
        nftData["hashID"] = PlayerDatabase.AccountProfile.Email;
        yield return AzureFunction.EndPoint($"{AzureFunction.uri}AddNonNftCharacter", nftData, okResult, errorResult);

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

        MessageBox.HideCurrent();
        Shop_SceneManager.ShowGatcha();
    }
}
