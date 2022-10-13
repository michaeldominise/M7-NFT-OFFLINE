using M7;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStatsData : MonoBehaviour
{
    MessageBox mBox;
    public void CheckStats(string jsonData)
    {
        mBox = MessageBox.Create("Saving Data..", MessageBox.ButtonType.Loading).Show();
        PlayFabFunctions.PlayFabCallFunction("StatsDataChecker", false, "", jsonData, OnSuccessStats);
    }

    void OnSuccessStats(ExecuteResult result)
    {
        if (result.Result.FunctionResult.ToString() == "HACK")
        {
            mBox.Hide();
            mBox = MessageBox.Create(result.Result.FunctionResult.ToString(), MessageBox.ButtonType.Ok).Show();
        }
        else
        {
            mBox.Hide();
            AdditionalStatsSceneManager.Instance.SetData();
        }
    }
}
