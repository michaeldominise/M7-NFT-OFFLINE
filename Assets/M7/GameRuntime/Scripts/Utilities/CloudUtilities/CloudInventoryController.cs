using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using System;
using UnityEngine.Events;



using System.Collections;
using System.ComponentModel;
using BGGamesCore.Models.Response;

public class CloudInventoryController : MonoBehaviour
{
    public static int API_CALL_LIMIT = 20;
    public UnityEvent OnSyncingFinish;
    public UnityEvent OnSyncingFailed;

    [SerializeField] private bool grantInitialItemsOnSyncFail = true;


    public static Action OnRetrieveInventory;
    public event Action OnInventoryFetched;


    private void OnEnable()
    {
 //       OnRetrieveInventory += RetrieveCloudInventory;
    }
    private void OnDisable()
    {
    //    OnRetrieveInventory -= RetrieveCloudInventory;
    }






    public static string KeyToVirtualCurrencyChecker(string key)
    {
        string name = " ";

        if (key.Equals(VirtualCurrencyKeys.EN.ToString()))
        {
            name = "Energy";
        }
        else if (key.Equals(VirtualCurrencyKeys.EM.ToString()))
        {
            name = "Emmerium";
        }
        else if (key.Equals(VirtualCurrencyKeys.GM.ToString()))
        {
            name = "Gems";
        }
        else if (key.Equals(VirtualCurrencyKeys.GO.ToString()))
        {
            name = "Gold";
        }
        else if (key.Equals(VirtualCurrencyKeys.DE.ToString()))
        {
            name = "DarkEmmerium";
        }
        else if (key.Equals(VirtualCurrencyKeys.AP.ToString()))
        {
            name = "AffinityMissionPass";
        }
        else if (key.Equals(VirtualCurrencyKeys.EP.ToString()))
        {
            name = "ExpMissionPass";
        }
        else if (key.Equals(VirtualCurrencyKeys.GP.ToString()))
        {
            name = "GaianiteRushMissionPass";
        }
        else if (key.Equals(VirtualCurrencyKeys.PE.ToString()))
        {
            name = "PurchasedEnergy";
        }
        else if (key.Equals(VirtualCurrencyKeys.PT.ToString()))
        {
            name = "PVPTicket";
        }
        else if (key.Equals(VirtualCurrencyKeys.FT.ToString()))
        {
            name = "FriendshipQuestTicket";
        }
        return name;
    }
    public static string VirtualCurrencyToKeyChecker(string virtualCurrency)
    {
        string key = " ";

        switch (virtualCurrency)
        {
            case "Energy":
                key = VirtualCurrencyKeys.EN.ToString();
                break;
            case "Emmerium":
                key = VirtualCurrencyKeys.EM.ToString();
                break;
            case "Gems":
                key = VirtualCurrencyKeys.GM.ToString();
                break;
            case "Gold":
                key = VirtualCurrencyKeys.GO.ToString();
                break;
            case "DarkEmmerium":
                key = VirtualCurrencyKeys.DE.ToString();
                break;
            case "AffinityMissionPass":
                key = VirtualCurrencyKeys.AP.ToString();
                break;
            case "ExpMissionPass":
                key = VirtualCurrencyKeys.EP.ToString();
                break;
            case "GaianiteRushMissionPass":
                key = VirtualCurrencyKeys.GP.ToString();
                break;
            case "PurchasedEnergy":
                key = VirtualCurrencyKeys.PE.ToString();
                break;
            case "PVPTicket":
                key = VirtualCurrencyKeys.PT.ToString();
                break;
            case "FriendshipQuestTicket":
                key = VirtualCurrencyKeys.FT.ToString();
                break;

        }

        return key;
    }

    public static void AddCurrency(string virtualCurrency, int amount, Action<Currency> OnResult)
    {
        if (amount <= 0)
        {
            //Debug.LogWarning("The amount you are trying to add is less than equal to 0");
            return;
        }

        string key = VirtualCurrencyToKeyChecker(virtualCurrency);

        CloudInventoryUtility.AddCurrency(
            key,
            amount,
            result =>
            {
                OnResult(result);
            });
    }

    public static void RemoveCurrency(string virtualCurrency, int amount, Action<Currency> OnResult)
    {
        if (amount <= 0)
        {
            //Debug.LogWarning("The amount you are trying to subtract is less than equal to 0");
            return;
        }

        string key = VirtualCurrencyToKeyChecker(virtualCurrency);

        CloudInventoryUtility.RemoveCurrency(
            key,
            amount,
            result =>
            {
                OnResult(result);
            });
    }

}


