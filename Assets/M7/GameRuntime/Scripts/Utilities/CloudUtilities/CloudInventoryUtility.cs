using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using System.Linq;
using Newtonsoft.Json;
using PlayFab.ClientModels;
using Chamoji.Social;
using CurrencyRequest = BGGamesCore.Models.Request.Currency;
using CurrencyResponse = BGGamesCore.Models.Response.Currency;

public class CloudInventoryUtility : MonoBehaviour
    {
    #region Error Handling

    public static Action<PlayFabError, NetworkMethods.RetryMethod> OnError = (error, retry) =>
    {
        NetworkMethods.CheckNetworkError(error, retry);
    };


    public static Action<ExecuteCloudScriptResult> OnInternalError = error =>
    {
        Debug.LogErrorFormat("{0}: {1}", error.Error.Error, error.Error.Message);
    };

    #endregion
    #region Data Retrieval Methods
    /// <summary>
    /// Retrieves the Cloud Inventory
    /// </summary>
    /// <param name="cloudInventory">Returns the Cloud Inventory</param>
    public static void RetrieveCloudInventory(Action<List<ItemInstance>> cloudInventory)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }
            PlayFabClientAPI.GetUserInventory(
                new GetUserInventoryRequest(), inventory =>
                {
                    if (inventory != null)
                    {
                        print(inventory.Inventory.Count);
                        List<ItemInstance> sortedInventory = inventory.Inventory.OrderBy(x => x.ItemClass == ItemTags.character.ToString())
                            .ThenBy(x => x.ItemClass == ItemTags.accessory.ToString())
                            .ThenBy(x => x.ItemClass == ItemTags.weapon.ToString())
                            .ThenBy(x => x.ItemClass == ItemTags.consumable.ToString() || x.ItemClass == ItemTags.limit.ToString() ||
                                         x.ItemClass == ItemTags.effect.ToString() || x.ItemClass == ItemTags.prizeBundle.ToString())
                            .ToList();
                        cloudInventory(sortedInventory);
                    }
                }, error => {  RetrieveCloudInventory(cloudInventory);  }
            );
        }

        /// <summary>
        /// Retrieves the Cloud Catalog
        /// </summary>
        /// <param name="CatalogData">Returns the cloud catalog dictionary by item name</param>
        public static void RetrieveCatalog(Action<Dictionary<string,CatalogItem>> CatalogData)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }
            Dictionary<string, CatalogItem> catalogContainer = new Dictionary<string, CatalogItem>();
            PlayFabClientAPI.GetCatalogItems(
                new GetCatalogItemsRequest()
                {
                }, result =>
                {
                    foreach(CatalogItem item in result.Catalog)
                    {
                        catalogContainer[item.ItemId] = item;
                    }

                    CatalogData(catalogContainer);

                }, error => { { RetrieveCatalog(CatalogData); } }
                );
                
        }
        #endregion

        #region Initialization Methods
        /// <summary>
        /// Grants the initial items set in the cloud script
        /// </summary>
        /// <param name="itemResult">Returns the items granted in player's inventory</param>
        public static void GrantInitialItems(Action<List<ItemInstance>> itemResult)
        {
            if (!PlayFabClientAPI.IsClientLoggedIn())
            {
                Debug.LogWarning("Client is not logged in to Playfab.");
                return;
            }
            PlayFabClientAPI.ExecuteCloudScript(
                new ExecuteCloudScriptRequest()
                {
                    FunctionName = "GrantInitialItems",
                    GeneratePlayStreamEvent = true,

                }, result =>
                {
                    if (result.FunctionResult != null)
                    {
                        var response = JsonConvert.DeserializeObject<List<ItemInstance>>(result.FunctionResult.ToString());


                        itemResult(response);
                        foreach (ItemInstance item in response)
                        {
                            Debug.Log("Added to cloud Inventory: " + item.ItemId);
                        }

                    }

                    if (result.Error != null)
                    {
                      //  OnInternalError(result);
                    }

                }, error => {  GrantInitialItems(itemResult); }
            );
        }
    #endregion

    #region Currency Methods
    public static void AddCurrency(string virtualCurrency, int amount, Action<CurrencyResponse> currencyData)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Client is not logged in to Playfab.");
            return;
        }

        PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest()
            {
                FunctionName = "AddCurrency",
                GeneratePlayStreamEvent = true,
                FunctionParameter = new CurrencyRequest()
                {
                    Amount = amount,
                    VirtualCurrency = virtualCurrency
                }
            }, result =>
            {
                if (result.FunctionResult != null)
                {
                    var response = JsonConvert.DeserializeObject<CurrencyResponse>(result.FunctionResult.ToString());

                    currencyData(response);
                }

                if (result.Error != null)
                {
                    OnInternalError(result);
                }


            }, error => { OnError(error, () => { AddCurrency(virtualCurrency, amount, currencyData); }); }
        );
    }

    public static void RemoveCurrency(string virtualCurrency, int amount, Action<CurrencyResponse> currencyData)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.LogWarning("Client is not logged in to Playfab.");
            return;
        }

        PlayFabClientAPI.ExecuteCloudScript(
            new ExecuteCloudScriptRequest()
            {
                FunctionName = "RemoveCurrency",
                GeneratePlayStreamEvent = true,
                FunctionParameter = new CurrencyRequest()
                {
                    Amount = amount,
                    VirtualCurrency = virtualCurrency
                }
            }, result =>
            {
                if (result.FunctionResult != null)
                {
                    var response = JsonConvert.DeserializeObject<CurrencyResponse>(result.FunctionResult.ToString());

                    currencyData(response);
                }

                if (result.Error != null)
                {
                    OnInternalError(result);
                }


            }, error => { OnError(error, () => { RemoveCurrency(virtualCurrency, amount, currencyData); }); }
        );
    }
    #endregion
    public enum ItemTags
        {
            character,
            building,
            gadget,
            consumable,
            prizeBundle,
            accessory,
            weapon,
            effect,
            limit,
            skin,
            generator
        }
    }

