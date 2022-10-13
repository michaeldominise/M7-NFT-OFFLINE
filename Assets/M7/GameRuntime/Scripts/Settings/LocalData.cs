using UnityEngine;

namespace M7.GameRuntime.Scripts.Settings
{
    public class LocalData
    {
        private const string playerJsonKey = "playerJsonKey"; 
        
        public static string GetPlayer()
        {
            var jsonString = PlayerPrefs.GetString(playerJsonKey, null); 
            Debug.Log($"Get custom id {jsonString}");
            return jsonString;
        }

        public static void SetPlayer(string playerJson)
        {
            Debug.Log($"set custom id {playerJson}");
            PlayerPrefs.SetString(playerJsonKey, playerJson);
        }
        
        public static void DeleteLoggedInPlayer()
        {
            PlayerPrefs.DeleteKey(playerJsonKey);
        }
    }
}