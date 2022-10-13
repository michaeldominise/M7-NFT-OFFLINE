using M7;
using M7.GameData;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.PlayfabCloudscript.PlayerDatabase;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPPackData : MonoBehaviour
{
    string jsonTempProductID;

    public void BuyPack(string jsonValue)
    {
        jsonTempProductID = jsonValue;
        MessageBox.Create("Loading...", MessageBox.ButtonType.Loading).Show();
        PlayFabFunctions.PlayFabCallFunction("PurchasePack", false, "", jsonValue, GetAllMainDataCallback);
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

        PlayerDatabase.Inventories.SystemCurrencies.OverwriteValues(playerdata.systemCurrencies);
        MessageBox.HideCurrent();
        ShopUIManager.Instance.SuccessPopUp(jsonTempProductID);

    }
}
