using System;
using System.Collections.Generic;
using System.Linq;
using M7.CloudDataUtility;
using M7.GameData;
using M7.GameRuntime;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;
using PlayFab.ClientModels;
//using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine.UI;
using GetUserDataRequest = PlayFab.ClientModels.GetUserDataRequest;
using ItemInstance = PlayFab.ClientModels.ItemInstance;
using UpdateUserDataRequest = PlayFab.ClientModels.UpdateUserDataRequest;
using UpdateUserDataResult = PlayFab.ClientModels.UpdateUserDataResult;
using UpdateUserTitleDisplayNameRequest = PlayFab.ClientModels.UpdateUserTitleDisplayNameRequest;

public static class PlayerDataMachine
{
    static string partyDataKey = "partyDataKey";

    static Action<List<ItemInstance>> RetrievedInventory;

    static UnityEvent OnFinish;

    public static event Action OnPlayerDataFetched;

    private static List<string> accountKeys;
    private static bool isEmptyExists = false;


    public static string playeFabID;
    public static string PlayerName;
   
    public static void CheckCampaignUpdates(Action<bool> OnSuccess)
    {
        PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest()
            {
                FunctionName = "CheckCampaignUpdates",
                GeneratePlayStreamEvent = true,

            }, result =>
            {
                if (result.FunctionResult != null)
                {
                    OnSuccess(true);
                }
                if (result.Error != null)
                {
                    OnSuccess(false);
                    CloudDataUtility.OnInternalError(result);
                }

            }, error=> { CloudDataUtility.OnError(error, () => { CheckCampaignUpdates(OnSuccess); }); }
        );
    }
    public static void CheckCodexUpdates(Action<bool> OnSuccess)
    {
        PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest()
            {
                FunctionName = "CheckCodexUpdate",
                GeneratePlayStreamEvent = true,

            }, result =>
            {
                if (result.FunctionResult != null)
                {
                    OnSuccess(true);
                }

            },error =>
            {
                CloudDataUtility.OnError(error, () => { CheckCodexUpdates(OnSuccess); }); 
                OnSuccess(false);
            }
        );
    }
    
    public static void InitDataInventory()
    {
        CloudInventoryUtility.RetrieveCloudInventory(RetrievedInventory);
    }

    public static  void OnInventoryRetrieved(List<ItemInstance> sortedInventory)
    {
        Debug.Log(sortedInventory.Count);
        for (int i = 0; i < PlayerDatabase.Inventories.Characters.GetItems().Count; i++)
        {
            if (sortedInventory[i].ItemClass ==CloudInventoryUtility.ItemTags.character.ToString()){
                //PlayerDatabase.Inventories.Characters.GetItems()[i].MasterID = sortedInventory[i].ItemId;
            }
        }
        
        CloudDataUtility.LoadPlayerCloudData("Teams", s =>
        {
            TeamsResponse t = JsonConvert.DeserializeObject<TeamsResponse>(s);
            // WE got the Teams
        });
    }



    public static void InitializePlayerTitleData()
    {
        PlayFabClientAPI.ExecuteCloudScript( new ExecuteCloudScriptRequest()
        {
            FunctionName  = "getServerTitleData",
            FunctionParameter = new { keys = "" },
            GeneratePlayStreamEvent = true

        }, result =>
        {
            var dataToSave = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.FunctionResult.ToString());
            CloudDataUtility.SaveCloud(dataToSave, saveCloudResult => {
                Debug.Log("Successfully uploaded the data");
        
            });
        }, error =>
        {
            Debug.Log(error.ErrorMessage);
        });
    }




    public static void GetPlayerData()
    {

        //       BGAnalytics.AnalyticsManager.Instance.SendPlayerEvent("CAMPAIGN_UPDATE_SUCCESS");

        CloudDataUtility.LoadPlayerCloudData(
            accountKeys,
            result =>
            {
                LoadData(result);
            }, RetrieveDefaultData);
    }
    private static void LoadData(Dictionary<string,string> result)
    {
        if( !isEmptyExists ) {
            OnFinish.Invoke();
            OnPlayerDataFetched?.Invoke();
        }
    }


    public static void RetrieveDefaultData(List<string> emptyDataKeys)
    {
        isEmptyExists = true;
        Dictionary<string, string> dataToSave = new Dictionary<string, string>();
        CloudDataUtility.LoadAllInternalCloudData(
            defaultData =>
            {
                foreach (string key in defaultData.Keys)
                {
                   
                    if (emptyDataKeys.Contains(key))
                    {
                        dataToSave[key] = defaultData[key];

                        if (emptyDataKeys.Contains(partyDataKey)){}
                    }
                }
              
                CloudDataUtility.SaveCloud(dataToSave, saveCloudResult => {
                });

                isEmptyExists = false;
                LoadData(dataToSave);
            });

        // OnFinish.Invoke();
    }
    public static void GrantInitialItems() 
    {
      
        CloudInventoryUtility.RetrieveCatalog(
            clog =>
            {
                CloudInventoryUtility.GrantInitialItems(
                    items =>
                    {
                        RetrievedInventory += OnInventoryRetrieved;
                        int isFirstTime = PlayerPrefs.GetInt("isFirstTime", 0);
                        if (!PlayerDatabase.DontLoadServerData && isFirstTime ==1) InitDataInventory();
                        PlayerPrefs.SetInt("isFirstTime", 1);
                        PlayerPrefs.Save();
                    });
            });
    }

    public static void ResultSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Data" + result + "has successfully been updated");
    }

    public static void ResultError(PlayFabError result)
    {
        Debug.LogError("Data has not been updated due to: " + result);
    }


    public static void UpdateUserTitle(string playerDisplayName,Action OnSucess)
    {
        UpdateUserTitleDisplayNameRequest displayNameRequest = new UpdateUserTitleDisplayNameRequest();
        displayNameRequest.DisplayName = playerDisplayName;
        PlayFabClientAPI.UpdateUserTitleDisplayName(displayNameRequest, (OnTitleUpdated =>
        {
            PlayerName = playerDisplayName;
            Debug.Log("UserName SuccessFully Updated");
            OnSucess?.Invoke();
        }), error =>
        {
            
        });
    }


    public static void SetUserData(string playfabId, string username, Action OnDataSaved)
    {
        playeFabID = playfabId;
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    {Constants.username, username},
                }
            },
            result =>
            {
                Debug.Log("Successfully updated user data");
                OnDataSaved?.Invoke();
            },
            error =>
            {
                Debug.Log("Got error setting user data username to " + username);
                Debug.Log(error.GenerateErrorReport());
            });
    }

    public static void GetUserData(string myPlayFabeId, Action OnDataSaveComplete = null)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myPlayFabeId,
            Keys = null
        }, result =>
        {
          //  initializeUserData(result.Data, myPlayFabeId);
            OnDataSaveComplete?.Invoke();
        }, (error) =>
        {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }
}