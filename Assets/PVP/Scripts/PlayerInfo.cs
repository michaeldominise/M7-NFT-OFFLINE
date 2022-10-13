using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo 
{
    [SerializeField] string playerDisplayName;
    [SerializeField] int playerAvatarImageIndex;
    private int MMR;

    public void SetPlayerInfo(string PlayerName,int PlayerImage=-1)
    {
        playerDisplayName = PlayerName;
        playerAvatarImageIndex = PlayerImage;
    }
    public string PlayerDisplayName
    {
        get { return playerDisplayName; }
    }
    public int PlayerAvatarImage
    {
        get { return playerAvatarImageIndex; }
    }
}
