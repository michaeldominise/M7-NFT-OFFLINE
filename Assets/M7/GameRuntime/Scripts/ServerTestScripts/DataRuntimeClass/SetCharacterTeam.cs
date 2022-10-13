using M7;
using M7.GameData;
using M7.GameRuntime;
using M7.GameRuntime.Scripts.PlayfabCloudscript;
using M7.GameRuntime.Scripts.PlayfabCloudscript.PlayerDatabase;
using M7.ServerTestScripts;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCharacterTeam : MonoBehaviour
{
    public void SetCharacter()
    {
        //string JsonData = JsonConvert.SerializeObject(PlayerDatabase.Teams);
        //PlayFabFunctions.PlayFabCallFunction("SetCharacterTeam", false, "teams", JsonData, null);
        PlayerDatabase.SaveToLocal();
    }
}
