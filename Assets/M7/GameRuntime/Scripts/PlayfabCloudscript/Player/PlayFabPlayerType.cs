using M7.GameRuntime.Scripts.PlayfabCloudscript.Model;
using UnityEngine;

namespace M7.GameRuntime.Scripts.PlayfabCloudscript.Player
{
    public static class PlayFabPlayerType
    {
        public static void GetPlayerType()
        {
            PlayFabFunctions.PlayFabCallFunction("GetUserData", false, "playerType", "", PlayerTypeCallback);
        }
        
        public static void SetPlayerType(PlayerType playerType)
        {
            PlayFabFunctions.PlayFabCallFunction("SetUserData", false, "playerType", playerType.ToString(), PlayerTypeCallback);
        }
        
        private static void PlayerTypeCallback(ExecuteResult obj)
        {
            Debug.Log($"player type {obj.Result.FunctionResult}");
        }
    }
}