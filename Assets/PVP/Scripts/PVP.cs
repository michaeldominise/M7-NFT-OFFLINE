using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using PlayFab;

using PlayFab.ClientModels;
using UnityEngine.UI.Extensions;
public class PVP : MonoBehaviourPunCallbacks
{
    //PlayerInfo Class will Contain the info of Client and the Opponnent they face when matchmaking.Unused for now.
    [Header ("Player Info")]
    [SerializeField,ReadOnly] PlayerInfo Client= new PlayerInfo();
    [SerializeField, ReadOnly] PlayerInfo Opponnet= new PlayerInfo();
    [SerializeField, ReadOnly] PlayerMMR ClientMMR = new PlayerMMR(); 


    [Header ("PlayerMatch")]
    [SerializeField] GameObject Player1NameText;
    [SerializeField] GameObject Player2NameText;
    [SerializeField] GameObject Player1Avatar;
    [SerializeField] GameObject Player2Avatar;
    [SerializeField] GameObject Player1;
    [SerializeField] GameObject Player2;

    [Header("MenuScenes")]
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject WaitingRoom;

    [Header("Login")]
    [SerializeField] GameObject LoginPanel;
    [SerializeField] GameObject UsernameText;

    [Header("Avatars")]
    [SerializeField] List<Sprite> Avatars = new List<Sprite>();
    
    [Header("Matchmaking")]
    public const string MMR_KEY = "C0";
    private TypedLobby SqlLobby = new TypedLobby("MMRLobby", LobbyType.SqlLobby);

    [Header("Leaderboard")]
    [SerializeField, ReadOnly] Leaderbord leaderBoard = new Leaderbord();

    private GameObject CurrentPanel;

    [SerializeField, ReadOnly] string gameverison = "1.0";
    private void Awake()
    {
        //Client = new PlayerInfo();
        Connect();
        CurrentPanel = MainMenu;
    }


    #region Pun
    private void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameverison;
        }
    }

    private void CreateRoom()
    {
        Debug.Log("Creating MMR Room with MMR of " + (ClientMMR.MMR));
        RoomOptions newRoomOptions = new RoomOptions();
        newRoomOptions.IsOpen = true;
        newRoomOptions.IsVisible = true;
        newRoomOptions.MaxPlayers = 2;
        newRoomOptions.PublishUserId = true;
        newRoomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { MMR_KEY, ClientMMR.MMR } };
        newRoomOptions.CustomRoomPropertiesForLobby = new string[] { MMR_KEY };
        PhotonNetwork.CreateRoom(null, newRoomOptions, SqlLobby);
    }
    private void JoinRandomRoom()
    {
        Debug.Log("Attempting joining MMR Room with MMR between of "+ (ClientMMR.MMR - 100) + "and" + (ClientMMR.MMR + 100));
        string sqlLobbyFilter = "C0 BETWEEN "+ (ClientMMR.MMR - 100) +" AND "+ (ClientMMR.MMR + 100) + " ";
        Debug.Log(sqlLobbyFilter);
        PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, SqlLobby, sqlLobbyFilter);
    }

    #endregion

    #region PUN Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        ErrorMessage(returnCode, message);
        //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers=2,PublishUserId=true});
        CreateRoom();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined");
        Debug.Log("Players in Room = " + PhotonNetwork.CurrentRoom.PlayerCount);
        SwitchPanesls( WaitingRoom);
        SetInfo(Player1,Player1Avatar,Player1NameText, Client.PlayerDisplayName, Client.PlayerAvatarImage);
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            var newPlayer = PhotonNetwork.PlayerListOthers[0];
            var Player2AvatarIndex = (int)newPlayer.CustomProperties["Avatar"];
            Opponnet.SetPlayerInfo(newPlayer.NickName, Player2AvatarIndex);
            SetInfo(Player2, Player2Avatar, Player2NameText, Opponnet.PlayerDisplayName, Opponnet.PlayerAvatarImage);
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //Set Other Players Info Here by retierving the user id of player from  cloudscipt playfab
        //SetInfo(Player2, );
        var Player2AvatarIndex = (int)newPlayer.CustomProperties["Avatar"];
        Opponnet.SetPlayerInfo(newPlayer.NickName, Player2AvatarIndex);
        SetInfo(Player2,Player2Avatar, Player2NameText, Opponnet.PlayerDisplayName, Opponnet.PlayerAvatarImage);
        Debug.Log("Players in Room =" + PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        int Player2AvatarIndex = 0;
        Opponnet.SetPlayerInfo(null, Player2AvatarIndex);
        Player2.SetActive(false);
        Debug.Log("Players in Room =" + PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ErrorMessage(returnCode, message);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Player ID= " + PhotonNetwork.LocalPlayer.UserId);
        
        //Client.SetPlayerInfo(PhotonNetwork.LocalPlayer.UserId);
    }
    public override void OnLeftRoom()
    {
        SwitchPanesls(MainMenu);
        Player1.SetActive(false);
        Player2.SetActive(false);
    }

    #endregion

    #region PlayfabCloudScript
    private void GetPlayerMMR()
    {
        var request = new ExecuteCloudScriptRequest { FunctionName = "GetPlayerMMR" };
        PlayFabClientAPI.ExecuteCloudScript(request, CloudScriptResult, CloudScriptError);
    }

    private void GetPlayerMMRStats()
    {
        var request = new ExecuteCloudScriptRequest { FunctionName = "GetPlayerStats" };
        PlayFabClientAPI.ExecuteCloudScript(request, CloudScriptResult, CloudScriptError);
        
    }

    void PlayfabUsernameLogin()
    {
        var request = new LoginWithCustomIDRequest { CustomId = SystemInfo.deviceName, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnCustomIDLoginSuccess, OnFailure);
        //Temp
        /*
        var username = "player1";
        Debug.Log("Username sent=" + username);
        var request = new LoginWithPlayFabRequest { Password = "manpower12", Username = username };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnFailure);
        */
    }
    public void ChangePlayerMMR(int Value)//For PlayerData
    {
        var data = new { adjustMMR = Value };
        var request = new ExecuteCloudScriptRequest { FunctionName = "UpdatePlayerMMR", FunctionParameter = data };
        PlayFabClientAPI.ExecuteCloudScript(request, CloudScriptResult, CloudScriptError);
        //GetPlayerMMR();
        //GetLeaderBoard();
    }
    public void ChangePlayerMMRStats(int Value)//For Leaderboard
    {
        var data = new { AdjustMMR = Value };
        var request = new ExecuteCloudScriptRequest { FunctionName = "UpdatePlayerStats", FunctionParameter = data };
        PlayFabClientAPI.ExecuteCloudScript(request, CloudScriptResult, CloudScriptError);
    }


    private void OnCustomIDLoginSuccess(LoginResult result)
    {
        Debug.Log("Loged In with CustomID");
        GetPlayerMMRStats();
        GetLeaderBoard();
    }

    private void GetLeaderBoard()
    {
        var request = new ExecuteCloudScriptRequest { FunctionName = "GetLeaderBoard"};
        PlayFabClientAPI.ExecuteCloudScript(request, CloudScriptResult, CloudScriptError);
    }


    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log(result);
        GetPlayerMMR();
    }
    private void OnFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong Playfab Login API.  :(");
        Debug.LogError(error.GenerateErrorReport());
    }
    void CloudScriptError(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with API CAll.  :(");
        Debug.LogError(error.GenerateErrorReport());
    }
    void CloudScriptResult(ExecuteCloudScriptResult result)
    {
        if (result.FunctionName == "GetPlayerMMR")
        {
            Debug.Log(result.FunctionResult);
            var Data = result.FunctionResult.ToString();
            ClientMMR = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerMMR>(Data);
        }
        else if (result.FunctionName == "GetPlayerStats")
        {
            Debug.Log(result.FunctionResult);
            ClientMMR.MMR = Int32.Parse(result.FunctionResult.ToString());
            GetLeaderBoard();
        }
        else if (result.FunctionName == "UpdatePlayerStats")
        {
            GetPlayerMMRStats();
        }
        else if (result.FunctionName == "GetLeaderBoard")
        {
            Debug.Log(result.FunctionResult);
            var Data = result.FunctionResult.ToString();
            leaderBoard.Board = null;
            leaderBoard.Board = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LeaderbordBase>>(Data); ;
        }
    }
    #endregion

    #region UI
    private void ErrorMessage(short returnCode, string message)
    {
        Debug.LogWarning("Error code:" + returnCode);
        Debug.LogWarning("Error Message:" + message);
    }

    private void SetInfo(GameObject Player,GameObject PlayerAvatar,GameObject PlayerName, string PlayerID,int PlayerAvatarIndex)
    {
        Player.SetActive(true);
        PlayerName.GetComponent<TMP_Text>().text = PlayerID;
        PlayerAvatar.GetComponent<Image>().sprite = Avatars[PlayerAvatarIndex];
    }
    private void SwitchPanesls(GameObject newPanel)
    {
        CurrentPanel.SetActive(false);
        newPanel.SetActive(true);
        CurrentPanel = newPanel;
    }

    public void StartMatchMaking()
    {
        //PhotonNetwork.JoinRandomRoom();
        JoinRandomRoom();
    }

    public void Back()
    {
        PhotonNetwork.LeaveRoom();
        //SetInfo(Player2Avatar, Player2NameText, "");
    }

    public void Login()
    {
        PhotonNetwork.LocalPlayer.NickName = UsernameText.GetComponent<TMP_InputField>().text;
        var playerAvatar =UnityEngine.Random.Range(0, Avatars.Count);
        var hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["Avatar"] = playerAvatar;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        Client.SetPlayerInfo(UsernameText.GetComponent<TMP_InputField>().text, playerAvatar);
        LoginPanel.SetActive(false);
        PlayfabUsernameLogin();
        
    }
    #endregion
}
