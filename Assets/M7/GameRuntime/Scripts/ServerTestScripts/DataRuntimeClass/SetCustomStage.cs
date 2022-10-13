using M7;
using M7.GameRuntime;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCustomStage : MonoBehaviour
{
    MessageBox mBox;

    public void SetChosenStage(string jsonString)
    {
        mBox = MessageBox.Create("Checking Data..", MessageBox.ButtonType.Loading).Show();
        PlayFabFunctions.PlayFabCallFunction("UpdateCustomStage", false, "", jsonString, PlayGame);
    }
    public void PlayGame(ExecuteResult result)
    {
        mBox.Hide();
    }
}