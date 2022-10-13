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
using UnityEngine;

public class SetRecoveryData : MonoBehaviour
{
    MessageBox mBox;
    public void CheckRecovery(string jsonData)
    {
        mBox = MessageBox.Create("Loading..", MessageBox.ButtonType.Loading).Show();
        PlayFabFunctions.PlayFabCallFunction("CheckSetRecovery", false, "", jsonData, SuccessAddRecovery);
    }
    public void SuccessAddRecovery(ExecuteResult result)
    {
        if (result.Result.FunctionResult.ToString() == "Not Enough Gaianite")
        {
            mBox.Hide();
            mBox = MessageBox.Create(result.Result.FunctionResult.ToString(), MessageBox.ButtonType.Ok).Show();
            CharacterInfoManager.Instance.RecoveryAddCallback(false);
        }
        else
        {
            GetNfts();
        }
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

        CharacterInfoManager.Instance.RecoveryAddCallback(true);

        StaticCurrenciesUIManager.Instance.InitInstanceUI();

        mBox.Hide();

    }
}
