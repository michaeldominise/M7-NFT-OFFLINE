using M7;
using M7.GameData;
using M7.GameData.Scripts.Saveables;
using M7.GameRuntime;
using M7.GameRuntime.Scripts.Energy;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.ServerTestScripts;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEnergyData : MonoBehaviour
{
    public enum EnergyStateEvent { PlayGame, TryAgainGame}

    MessageBox mBox;
    public void PlayCheckEnergy(EnergyStateEvent state)
    {
        mBox = MessageBox.Create("Checking energy.", MessageBox.ButtonType.Loading).Show();
        switch (state)
        {
            case EnergyStateEvent.PlayGame:
                PlayFabFunctions.PlayFabCallFunction("EnergyCheck", false, "", "", PlayGame);
                break;
            case EnergyStateEvent.TryAgainGame:
                PlayFabFunctions.PlayFabCallFunction("EnergyCheck", false, "", "", TryAgain);
                break;
        }
    }
    public void PlayGame(ExecuteResult result)
    {
        if (result.Result.FunctionResult.ToString() == "Not Enough Energy")
        {
            mBox.Hide();
            mBox = MessageBox.Create(result.Result.FunctionResult.ToString(), MessageBox.ButtonType.Ok).Show();
        }
        else if (result.Result.FunctionResult.ToString() == "A hero in your team does not have enough durability to enter this stage.")
        {
            mBox.Hide();
            mBox = MessageBox.Create(result.Result.FunctionResult.ToString(), MessageBox.ButtonType.Ok).Show();
        }
        else if (result.Result.FunctionResult.ToString() == "Not Enough Energy & A hero in your team does not have enough durability to enter this stage.")
        {
            mBox.Hide();
            mBox = MessageBox.Create(result.Result.FunctionResult.ToString(), MessageBox.ButtonType.Ok).Show();
        }
        else
        {
            mBox.Hide();
            DownloadDataRuntime.Instance.isLoadScene = true;
            DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.GetStageData);
        }
    }
    public void TryAgain(ExecuteResult result)
    {
        if (result.Result.FunctionResult.ToString() == "Not Enough Energy")
        {
            mBox.Hide();
            BattleManager.Instance.MoveCounterManager.DisableMovesPanel();
            BattleManager.Instance.MoveCounterManager.OutOfMovesPanel[4].SetActive(true);
        }
        else if (result.Result.FunctionResult.ToString() == "A hero in your team does not have enough durability to enter this stage.")
        {
            mBox.Hide();
            BattleManager.Instance.MoveCounterManager.DisableMovesPanel();
            BattleManager.Instance.MoveCounterManager.OutOfMovesPanel[4].SetActive(true);
            mBox = MessageBox.Create(result.Result.FunctionResult.ToString(), MessageBox.ButtonType.Ok).Show();
        }
        else if (result.Result.FunctionResult.ToString() == "Not Enough Energy & A hero in your team does not have enough durability to enter this stage.")
        {
            mBox.Hide();
            BattleManager.Instance.MoveCounterManager.DisableMovesPanel();
            BattleManager.Instance.MoveCounterManager.OutOfMovesPanel[4].SetActive(true);
            mBox = MessageBox.Create(result.Result.FunctionResult.ToString(), MessageBox.ButtonType.Ok).Show();
        }
        else
        {
            InitialMenuManager.Instance.GameInventoryManager.multiplier = 1;
            BattleManager.Instance.GameFinishManager.LoadScene(BattleManager.Instance.GameFinishManager.battleScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
    //IEnumerator MboxTimer(string message)
    //{
    //    mBox = MessageBox.Create(message, MessageBox.ButtonType.Ok).Show();
    //    yield return new WaitForSeconds(2);
    //    mBox.Hide();
    //}

}
