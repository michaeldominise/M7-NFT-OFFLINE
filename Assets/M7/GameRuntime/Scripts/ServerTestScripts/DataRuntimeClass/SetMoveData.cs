using M7;
using M7.GameData;
using M7.GameRuntime;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.PlayfabCloudscript.PlayerDatabase;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMoveData : MonoBehaviour
{
    public void MoveCheckData(string jsonData)
    {
        MessageBox.Create("Loading stage...", MessageBox.ButtonType.Loading).Show();
        PlayFabFunctions.PlayFabCallFunction("MoveCheck", false, "", jsonData, OnSuccess);
    }

    public void OnSuccess(ExecuteResult result)
    {
        Debug.Log(result.Result.FunctionResult);
        if (result.Result.FunctionResult.ToString() == "Not Enough Gaiananite")
        {
            MessageBox.HideCurrent();
        }
        else
        {
            //var playerdata = JsonConvert.DeserializeObject<PlayerData>(result.Result.FunctionResult.ToString());

            //Debug.Log(result.Result.FunctionResult);

            //if (result.Status != ResultStatus.Ok) return;

            //if (playerdata == null) return;

            //PlayerDatabase.Inventories.Currencies.OverwriteValues(playerdata.currencies);

            BattleManager.Instance.MoveCounterManager.GiveAdditionalMoves();

            BattleManager.Instance.MoveCounterManager.DisableMovesPanel();

            MessageBox.HideCurrent();
        }
    }
}
