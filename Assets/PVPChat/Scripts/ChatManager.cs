using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using ExitGames.Client.Photon;
public class ChatManager : MonoBehaviour, IChatClientListener
{

    [Header ("UI")]
    [SerializeField] GameObject ChatMessageTemplate;
    [SerializeField] TMP_InputField ChatInput;
    [SerializeField] GameObject ChatPanel;
    [SerializeField] TMP_Text ClientName;
    [SerializeField] TMP_Text ClientEmote;
    [SerializeField] GameObject EmotePanel;
    [SerializeField] GameObject EmoteButtonTemplate;
    [SerializeField] GameObject EmojiPanel;
    [SerializeField] GameObject EmojiButtonTemplate;
    [SerializeField] Button EmoteButton;


    [SerializeField] TMP_Text OpponentName;
    [SerializeField] TMP_Text OpponentEmote;

    [Header ("PUN CHAT")]
    [SerializeField] string UserID;
    [SerializeField] string RoomID;
    private ChatClient chatClient;

    #region Unity
    // Start is called before the first frame update
    void Start()
    {
        //Change null to the way we get user data
        PopulateEmotes();
        PopulateEmoji();
        ChatConnect();
    }

    private void Update()
    {
        chatClient.Service();
        if (ChatPanel.activeInHierarchy == true)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Invoke("ChatMessageSend",0);
            }
        }
    }

    private void PopulateEmotes()
    {
        //Debug.Log("Default sprite asset=" + TMPro.TMP_Settings.defaultSpriteAsset.spriteCharacterTable.Count);
        //Debug.Log("Number of Sprites=" + TMPro.TMP_Settings.GetSpriteAsset().spriteCharacterTable.Count);

        foreach (var SpriteElement in TMPro.TMP_Settings.GetSpriteAsset().spriteCharacterTable)
        {
            //Debug.Log(SpriteElement.name);
            GameObject gameObject = Instantiate(EmoteButtonTemplate) as GameObject;
            gameObject.SetActive(true);
            gameObject.transform.SetParent(EmoteButtonTemplate.transform.parent, false);
            gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "<sprite=" + SpriteElement.glyphIndex + ">";
            gameObject.GetComponent<Button>().onClick.AddListener(() => SendEmote("<sprite=" + SpriteElement.glyphIndex + ">"));
            
        }
    }
    private void PopulateEmoji()
    {
        //Debug.Log("Default sprite asset=" + TMPro.TMP_Settings.defaultSpriteAsset.spriteCharacterTable.Count);
        //Debug.Log("Number of Sprites=" + TMPro.TMP_Settings.GetSpriteAsset().spriteCharacterTable.Count);

        foreach (var SpriteElement in TMPro.TMP_Settings.GetSpriteAsset().spriteCharacterTable)
        {
            //Debug.Log(SpriteElement.name);
            GameObject gameObject = Instantiate(EmojiButtonTemplate) as GameObject;
            gameObject.SetActive(true);
            gameObject.transform.SetParent(EmojiButtonTemplate.transform.parent, false);
            gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "<sprite=" + SpriteElement.glyphIndex + ">";
            gameObject.GetComponent<Button>().onClick.AddListener(() => ChatInput.text+= "<sprite=" + SpriteElement.glyphIndex + ">");
        }
    }
    #endregion

    #region PUN Chat Interfaces

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("Debug  MSG Level:" + level+", MSG: "+ message);
    }

    public void OnDisconnected()
    {
        Debug.Log("Client has Disconnected");
        //ChatConnect();
    }
    public void OnConnected()
    {
        ConnectToMatchChat();
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("Chat State=" + state);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        Debug.Log("We got MSG's");
        Debug.Log("NUmber of Senders=" + senders.Length);
        if (channelName == RoomID)
        {
            for (int i = 0; i < senders.Length; i++)
            {
                //Check if the message is a emote or chat message.
                //Chat Message can be identified with message starting with  senders name
                //If it is not a chat message then we can be assured  that it is a emote.

                if (messages[i].ToString().StartsWith(ClientName.text) || messages[i].ToString().StartsWith(OpponentName.text))
                {
                    GameObject gameObject = Instantiate(ChatMessageTemplate) as GameObject;
                    gameObject.SetActive(true);
                    var MessageGameObject = gameObject.transform.Find("Message").gameObject;
                    MessageGameObject.GetComponent<TMP_Text>().text = messages[i].ToString();
                    gameObject.transform.SetParent(ChatMessageTemplate.transform.parent, false);
                }
                else
                {
                    //Coroutinues used cause Invoke does not support Parameters.
                    if (senders[i] == ClientName.text)
                    {
                        ClientEmote.text = messages[i].ToString();
                        StartCoroutine(RemoveEmote(ClientEmote));
                    }
                    else
                    {
                        OpponentEmote.text = messages[i].ToString();
                        StartCoroutine(RemoveEmote(OpponentEmote));
                    }
                }
                
            }
        }

    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("Connected to "+ channels);
        GetChatUsers();

    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log("User: " + user);

        if (channel == RoomID)
        {
            Debug.Log("User: " + user);
            OpponentName.text = user;
        }
    }
    public void OnUserUnsubscribed(string channel, string user)
    {
        if (channel == RoomID)
        {
            OpponentName.text = "";
        }

    }
    #endregion

    #region PUN Chat

    private void ChatConnect()
    {
        
        //UserID="Testing";
        this.chatClient = new ChatClient(this);
        chatClient.ChatRegion = "EU";//can be 'EU,US,ASIA'
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(UserID));
        Debug.Log("Chat Client Connected");
        
        //Connect to User
    }
    private void ConnectToMatchChat()
    {
        var options = new ChannelCreationOptions();
        options.PublishSubscribers = true;
        chatClient.Subscribe(RoomID ,0,0, options);
        ClientName.text = UserID;
        Debug.Log("Connected to Match Room");
    }
    IEnumerator RemoveEmote(TMP_Text Emote)
    {

        yield return new WaitForSeconds(5.0f);
        Emote.text = "";
        EmoteButton.interactable = true;

    }
    private void GetChatUsers()
    {
        ChatChannel chatChannel;
        chatClient.TryGetChannel(RoomID, out chatChannel);
        Debug.Log("Subs: " + chatChannel.Subscribers.Count);
        foreach (var user in chatChannel.Subscribers)
        {
            Debug.Log("USERS: " + user);
            if (user != UserID)
            {
                OpponentName.text = user;
            }
        }
    }
    #endregion
    #region UI
    public void ChatMessageSend()
    {
        ChatInput.text=ChatInput.text.Trim();
        if (ChatInput.text.Length != 0)
        {
            string Message = UserID + " : " + ChatInput.text;
            chatClient.PublishMessage(RoomID, Message);
            ChatInput.text = "";//Clear chat input after text has been sent
            Debug.Log("Chat Messeage sent");
        }
    }
    public void SendEmote(string Emote)
    {
        chatClient.PublishMessage(RoomID, Emote);
        EmotePanel.SetActive(false);
        EmoteButton.interactable = false;

    }
    public void OpenChatPanel()
    {
        ChatPanel.SetActive(true);
    }
    public void CloseChatPanel()
    {
        ChatPanel.SetActive(false);
    }

    public void EmotePanelState()
    {
        if (EmotePanel.activeSelf)
        {
            EmotePanel.SetActive(false);
        }
        else
        {
            EmotePanel.SetActive(true);
        }
    }
    public void EmojiPanelState()
    {
        if (EmojiPanel.activeSelf)
        {
            EmojiPanel.SetActive(false);
        }
        else
        {
            EmojiPanel.SetActive(true);
        }
    }
    #endregion
}
