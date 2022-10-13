/* ======================= Cloud Inventory Methods for client========================= */


handlers.RetrieveCloudInventory = function(args, content)
{
    try
    {
        var result = server.GetUserInventory(
        {
            PlayFabId: currentPlayerId,
        });

        return result;
    }
    catch(err)
    {
        LogError({
            "EventName" : "RetrieveCloudInventory_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

function SetUserInternalData(StoreData)
{
    var request = {
        PlayFabId: currentPlayerId,
        "Data": StoreData
        };
    var GetUserReadOnlyDataResponse = server.UpdateUserInternalData(request);
}

function AddItemData(itemList)
{
    for(var i = 0; i < itemList.length; i++)
    {
        itemList[i].PlayFabId = currentPlayerId;
    }

    try
    {
        var itemData = server.GrantItemsToUsers(
        {
            ItemGrants : itemList

        });

        return itemData.ItemGrantResults;
    }
    catch(err)
    {
        LogError({
            "EventName" : "AddItemData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}
const INITIAL_ITEMS_BUNDLE_KEY = "Bundle_InitialItems"
handlers.GrantInitialItems = function(args, content)
{
    try
    {
        //var curTime = GetCurrentServerTime();
        // var key = "initializeItemsKey"
        // var keyList = [key];
        // var rawData= GetServerInternalData(keyList);
        var initBundle = INITIAL_ITEMS_BUNDLE_KEY;
        var catalog = GetCatalog();
        var items = catalog[initBundle].Bundle.BundledItems;
        var finalItems = [{}];
        log.info("Function Initialized");
        if(items.length > 0)
        {
            for(var i = 0; i < items.length; i++)
            {
                // var customData = new ItemCustomData();
                let customData = {};
                var currentItem = items[i];
                finalItems[i] = { };
                log.info("CHECK ================ :" + currentItem + " : " + catalog[items[i]].ItemClass);
                if(catalog[items[i]].ItemClass === "character")
                {

                    finalItems[i].PlayFabId = " ";
                    finalItems[i].ItemId = currentItem;
                    finalItems[i].Data = customData;
                    log.info("CHECK ================ :" + currentItem);
                }
                else
                {
                    finalItems[i].PlayFabId = " ";
                    finalItems[i].ItemId = currentItem;
                }
            }
        }

        var grantedItems = AddItemData(finalItems);
        log.info(JSON.stringify(grantedItems));
        return grantedItems;
    }
    catch(err)
    {
        LogError({
            "EventName" : "GrantInitialItems_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

function GetCatalog()
{
    try
    {
        var result = server.GetCatalogItems({}).Catalog;
        var catalog = { };

        for(var i = 0; i < result.length; i++)
        {
            catalog[result[i].ItemId] = result[i];
        }
        return catalog;
    }
    catch(err)
    {
        LogError({
            "EventName" : "GetCatalogItems_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }

}
handlers.AddCurrency = function(args, content)
{
    try
    {
        var result = server.AddUserVirtualCurrency(
        {
            PlayFabId: currentPlayerId,
            VirtualCurrency : args.VirtualCurrency,
            Amount : args.Amount
        });

        return result;
    }
    catch(err)
    {
        LogError({
            "EventName" : "AddCurrency_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

handlers.RemoveCurrency = function(args, content)
{
    try
    {
        var result = server.SubtractUserVirtualCurrency(
        {
            PlayFabId: currentPlayerId,
            VirtualCurrency : args.VirtualCurrency,
            Amount : args.Amount
        });

        return result;
    }
    catch(err)
    {
        LogError({
            "EventName" : "RemoveCurrency_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}
/* ======================= Cloud Inventory Methods for client========================= */