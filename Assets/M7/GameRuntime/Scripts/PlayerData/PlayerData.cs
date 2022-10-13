using System;
using UnityEngine;

namespace M7.GameRuntime.Scripts.PlayerData
{
    [Serializable]
    public class PlayerData
    {
        public static void SaveEmail(string email)
        {
            PlayerPrefs.SetString("email", email);
            PlayerPrefs.Save();
        }
        
        public static void SavePassword(string password)
        {
            PlayerPrefs.SetString("password", password);
            PlayerPrefs.Save();
        }
        
        public static string LoadEmail()
        {
            return PlayerPrefs.GetString("email");
        }
        
        public static string LoadPassword()
        {
            return PlayerPrefs.GetString("password");
        }
    }
}
