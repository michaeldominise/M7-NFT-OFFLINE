


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


// The Following are in Case Player Data will  be used for Matchmaking
/*
NewPlayerCreated:
On Signing up, a Playfab Rule executes this function to Set the PlayerData from TitleData;
*/
handlers.NewPlayerCreated=function(args,context)
{
    log.debug("New Player Joined");
    var TitleDataRequest = {"Keys":["DefaultMMR"]};
    var TitleDataResponse = server.GetTitleData(TitleDataRequest);
    var ResponseResult= JSON.parse(JSON.stringify(TitleDataResponse));
    var TitleData=ResponseResult.Data;
    var MMRData=TitleData.DefaultMMR;
    log.debug("Data "+MMRData);
    var StoreData={"PlayerMMR":MMRData};
    SetUserInternalData(StoreData);
}
/*
UpdatePlayerMMR:
The function gets the playerData for the user who calls this function and
then updates the Player MMR which is stored in the Player Data.
*/
handlers.UpdatePlayerMMR=function(args,context)
{
    log.debug("MMR Changed");
    var PlayerInternalDataRequest = {"PlayFabId": currentPlayerId,"Keys":["PlayerMMR"]};
    var PlayerInternalDataResponse = server.GetUserInternalData(PlayerInternalDataRequest);
    var ResponseResult= JSON.parse(JSON.stringify(PlayerInternalDataResponse));
    var PlayerData=ResponseResult.Data;
    var MMRValue=JSON.parse(PlayerData.PlayerMMR.Value);
    var newMMR=MMRValue.MMR+args.AdjustMMR;
    MMRValue.MMR=newMMR;
    PlayerData.PlayerMMR.Value=MMRValue;
    var StoreData= {};
    StoreData["PlayerMMR"]=JSON.stringify(PlayerData.PlayerMMR.Value);
    SetUserInternalData(StoreData);

}
/*
GetPlayerMMR returns the Player MMR for the user who called this
*/
handlers.GetPlayerMMR=function(args,context)
{
    log.debug("MMR Sent to Client");
    var PlayerInternalDataRequest = {"PlayFabId": currentPlayerId,"Keys":["PlayerMMR"]};
    var PlayerInternalDataResponse = server.GetUserInternalData(PlayerInternalDataRequest);
    var ResponseResult= JSON.parse(JSON.stringify(PlayerInternalDataResponse));
    var PlayerData=ResponseResult.Data;
    var MMRValue=PlayerData.PlayerMMR;
    MMRValue=MMRValue.Value;
    log.debug("MMR= "+ (MMRValue.MMR));
    return MMRValue;
}
/*
UpdatePlayerData is a Schulded Task which will add the PlayeData for players who dont have the required Data
*/
handlers.UpdatePlayerData=function(args,context)
{
    var PlayerInternalDataRequest = {"PlayFabId": currentPlayerId,"Keys":["PlayerMMR"]};
    var PlayerInternalDataResponse = server.GetUserInternalData(PlayerInternalDataRequest);
    var ResponseResult= JSON.parse(JSON.stringify(PlayerInternalDataResponse));
    log.debug("Result null? "+ResponseResult.Data==null)
    if(ResponseResult.Data.PlayerMMR==null)
    {
        var TitleDataRequest = {"Keys":["DefaultMMR"]};
        var TitleDataResponse = server.GetTitleData(TitleDataRequest);
        var ResponseResult= JSON.parse(JSON.stringify(TitleDataResponse));
        var TitleData=ResponseResult.Data;
        var MMRData=TitleData.DefaultMMR;
        log.debug("Data "+MMRData);
        var StoreData={"PlayerMMR":MMRData};
        SetUserInternalData(StoreData);

    }
}

// The Following  Functions are in Case Leaderboard Data will  be used for Matchmaking
/*
NewPlayerStats function creates the Leaderboard Statistics for the New User
*/
handlers.NewPlayerStats=function(args,context)
{
    var Version= GetCurrentLeaderboardVersion();
    log.debug("New Player Stats Created");
    var TitleDataRequest = {"Keys":["DefaultMMR"]};
    var TitleDataResponse = server.GetTitleData(TitleDataRequest);
    var ResponseResult= JSON.parse(JSON.stringify(TitleDataResponse));
    var TitleData=ResponseResult.Data;
    var MMRData=JSON.parse(TitleData.DefaultMMR);
    var PlayerMMR=MMRData.MMR;
    var request={
        PlayFabId: currentPlayerId,
        "Statistics": [{"StatisticName": "Player Rating","Version": Version,"Value": PlayerMMR}]
    }
    var GetUpdatePlayerStatisticsResponse=server.UpdatePlayerStatistics(request);
}
/*
GetPlayerStats gets the Leaderboard Statistics for the user who called the function
*/
handlers.GetPlayerStats=function(args,context)
{
    var request={
        PlayFabId: currentPlayerId,
        "StatisticNames": ["Player Rating"]
    }
    var GetPlayerStatisticsResponse=server.GetPlayerStatistics(request);
    var Response=JSON.parse(JSON.stringify(GetPlayerStatisticsResponse));
    var Data=result=Response.Statistics;
    log.debug(Data);
    var PlayerStats=Data[0]
    return PlayerStats.Value;
}
/*
UpdatePlayerStats updates the Leaderboard Statistics for the user who calls this function with the arguments sent
*/
handlers.UpdatePlayerStats=function(args,context)
{
    var request={
        PlayFabId: currentPlayerId,
        "Statistics": [{"StatisticName": "Player Rating","Version": 0,"Value": args.AdjustMMR}]
    }
    var GetUpdatePlayerStatisticsResponse=server.UpdatePlayerStatistics(request);
}
/*
GetLeaderBoard returns the Top 10 Leaderboard values 
*/
handlers.GetLeaderBoard=function(args,context)
{
    var request=
    {
        "StatisticName" : "Player Rating",
        "StartPosition" : 0,
        "MaxResultsCount" : 10
    }
    var GetLeaderboardResponse=server.GetLeaderboard(request);
    var Response=JSON.parse(JSON.stringify(GetLeaderboardResponse));
    var LeaderboardData=[];
    log.debug(Response.Leaderboard);
    Response.Leaderboard.forEach(function (item) {
    LeaderboardData.push({"PlayerID":item.PlayFabId,"MMR":item.StatValue}); 
        
    });
    log.debug(LeaderboardData);
    return LeaderboardData;
}
handlers.AddExistingUsersStatsitcs=function(args,context)
{
    var request={
        PlayFabId: currentPlayerId,
        "StatisticNames": ["Player Rating"]
    }
    var GetPlayerStatisticsResponse=server.GetPlayerStatistics(request);
    var Response=JSON.parse(JSON.stringify(GetPlayerStatisticsResponse));
    log.debug(JSON.stringify(Response));
    var Data=Response.Statistics;
    var PlayerStats=Data[0];
    if(PlayerStats==undefined)
    {
        request=
        {
            PlayFabId: currentPlayerId,
            "FunctionName": "NewPlayerStats"
        }
        server.ExecuteCloudScript(request);
    }
}
function GetCurrentLeaderboardVersion()
{
    var request={
        "StatisticName": "Player Rating",
        "StartPosition": 0,
        "MaxResultsCount": 10
    }
    var GetLeaderboardResponse = server.GetLeaderboard(request);
    var Response=(JSON.stringify(GetLeaderboardResponse));
    log.debug("Response="+GetLeaderboardResponse.Version);
    return GetLeaderboardResponse.Version;

}


/* ============================= Patch Versioning Methods =============================*/

const APPLICATION_MAINTENANCE_KEY = "applicationCurrentVersion";
const APPLICATION_CDN_ANDROID_KEY = "applicationCDNAndroidCurrentVersion";
const APPLICATION_CDN_IOS_KEY = "applicationCDNIOSCurrentVersion";
const APPLICATION_CDN_WEBGL_KEY = "applicationCDNWEBGLCurrentVersion";

handlers.CheckApplicationVersion = function(args, content)
{
  try 
  {
      
    let Type = args.Type;
    let Platform = args.Platform;
    let CurrentVersion = args.Version;
    let ServerVersion  = "";
    
    let payload = "";
    let flag = false;
    
    switch(Type)
    {
        case "Maintenance":
            ServerVersion = GetServerInternalData([APPLICATION_MAINTENANCE_KEY])[APPLICATION_MAINTENANCE_KEY];
            log.info("server/current: " + ServerVersion + "/" + CurrentVersion);
            var server = ServerVersion.split(".");
            var client = CurrentVersion.split(".");
            flag = server <= client ? true:false;
        break;
        case "CDN":
            switch(Platform)
            {
                case "Android":
                    ServerVersion = GetServerInternalData([APPLICATION_CDN_ANDROID_KEY])[APPLICATION_CDN_ANDROID_KEY];
                    break;
                case "iOS":
                    ServerVersion = GetServerInternalData([APPLICATION_CDN_IOS_KEY])[APPLICATION_CDN_IOS_KEY];
                    break;
                case "WEBGL":
                    ServerVersion = GetServerInternalData([APPLICATION_CDN_WEBGL_KEY])[APPLICATION_CDN_WEBGL_KEY];
                    break;
            }
            flag = CurrentVersion === ServerVersion ? true:false;
            payload = ServerVersion;
        break;
    }
    
    let data = {};
    data.Flag = flag;
    data.Payload = payload;
    return data;
  } 
  catch (err) 
  {
    LogError({
        "EventName" : "CheckApplicationVersion",
        "Error" : err
    });

    return err.apiErrorInfo.apiError;
  }
    
}
/* ============================= Patch Versioning Methods =============================*/


/* ======================== ServerData Methods ============================== */
function GetServerCampaignData()
{
    var key = "campaignDataKey"
    var keyList = [];
    keyList.push(key);
    var rawContentCampaign = GetServerData(keyList);

    if(rawContentCampaign[key])
    {
        var contentCampaign = JSON.parse(rawContentCampaign[key]);
        return contentCampaign;
    }
    else
    {
        log.error("CAMPAIGN SERVER TITLE DATA EMPTY")
        throw "Campaign data in the server is empty. Please check server content!";
    }
}
function GetUserCampaignData()
{
    var key = "campaignDataKey"
    var keyList = [];
    keyList.push(key);
    var rawLocalCampaign = GetUserData(keyList);

    if(rawLocalCampaign[key])
    {
        var localCampaign = JSON.parse(rawLocalCampaign[key].Value);;
        return localCampaign;
    }
    else
    {
        log.error("CAMPAIGN USER TITLE DATA EMPTY")
        throw "User's campaign data is missing. Please check server!";
    }
}
function SetMultipleUserReadOnlyData(kvp)
{
        try
        {
            var itemData = server.UpdateUserReadOnlyData(
            {
                PlayFabId : currentPlayerId,
                Data: kvp
            });

        }
        catch(err)
        {
            LogError({
                "EventName" : "SetMultipleUserReadOnlyData_error",
                "Error" : err
            });

            return err.apiErrorInfo.apiError;
        }
}
function SetUserReadOnlyData(key, data)
{
    var dataToSend = {}
    dataToSend[key] = data;
        try
        {
            var itemData = server.UpdateUserReadOnlyData(
            {
                PlayFabId : currentPlayerId,
                Data: dataToSend
            });

        }
        catch(err)
        {
            LogError({
                "EventName" : "SetUserReadOnlyData_error",
                "Error" : err
            });

            return err.apiErrorInfo.apiError;
        }
}
function GetUserMultipleReadOnlyData(keys)
{
    try
    {
        var itemData = server.GetUserReadOnlyData(
        {
            PlayFabId : currentPlayerId,
            Keys : keys
        });

        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetUserMultipleReadOnlyData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}
function GetUserReadOnlyData(key)
{
    var dataToSend = {}
    dataToSend[key] = data;
    try
    {
        var itemData = server.GetUserReadOnlyData(
        {
            PlayFabId : currentPlayerId,
            Keys : keys
        });

        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetUserReadOnlyData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

function SetUserData(key, data)
{
    var dataToSend = {}
    dataToSend[key] = data;
        try
        {
            var itemData = server.UpdateUserData(
            {
                PlayFabId : currentPlayerId,
                Data: dataToSend
            });

        }
        catch(err)
        {
            LogError({
                "EventName" : "SetUserData_error",
                "Error" : err
            });

            return err.apiErrorInfo.apiError;
        }
}
function SetMultipleUserData(keys)
{
        try
        {
            var itemData = server.UpdateUserData(
            {
                PlayFabId : currentPlayerId,
                Data: keys
            });

        }
        catch(err)
        {
            LogError({
                "EventName" : "SetUserData_error",
                "Error" : err
            });

            return err.apiErrorInfo.apiError;
        }
}
function GetUserData(keys)
{
    try
    {
        var itemData = server.GetUserData(
        {
            PlayFabId : currentPlayerId,
            Keys  : keys
        });

        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetUserData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}
function GetSpecificUserData(keys, playerId)
{
    try
    {
        var itemData = server.GetUserData(
        {
            PlayFabId : playerId,
            Keys  : keys
        });
        log.info("specificuserdata")
        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetUserData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

function GetServerData(keys)
{
    try
    {
        var itemData = server.GetTitleData(
        {
            Keys  : keys
        });

        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetServerData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

function SetServerData(key, data)
{
    try
    {
        var itemData = server.SetTitleData(
        {
            Key : key,
            Value: data
        });
    }
    catch(err)
    {
        LogError({
            "EventName" : "SetServerData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

function GetServerInternalData(keys)
{
    try
    {
        var itemData = server.GetTitleInternalData(
        {
            Keys : keys
        });

        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetServerData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

handlers.getServerTitleData = function(args)
{
    let Keys = args.keys
    try
    {
        var itemData = server.GetTitleData({
            Keys : Keys
        });

        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetServerData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}




function GetSpecificUserInternalData(keys, playerId)
{
    try
    {
        var itemData = server.GetTitleInternalData(
        {
            PlayFabId : playerId,
            Keys  : keys
        });
        log.info("specificuserdata")
        return itemData.Data;

    }
    catch(err)
    {
        LogError({
            "EventName" : "GetUserData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}
function SetServerInternalData(key, data)
{
    try
    {
        var itemData = server.SetTitleInternalData(
        {
            Key : key,
            Value: data
        });


    }
    catch(err)
    {
        LogError({
            "EventName" : "GetServerData_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}
/* ======================== ServerData Methods ============================== */


/* ======================== Validation Methods ============================== */

const WHITELIST_KEY = "IsWhitelistMember"
handlers.ValidateWhitelist = function(args, context)
{
    try 
    {
        let whiteListData = GetUserData(WHITELIST_KEY)[WHITELIST_KEY];
        let whiteListFlag = whiteListData === undefined ? JSON.parse(GetServerData(WHITELIST_KEY)[WHITELIST_KEY]) : JSON.parse(whiteListData.Value);

        if(whiteListData === undefined)
            SetUserData(WHITELIST_KEY,JSON.stringify(GetServerData(WHITELIST_KEY)[WHITELIST_KEY]));

        if(whiteListFlag === undefined) whiteListFlag = false;
    
        
        return JSON.stringify(whiteListFlag);
    } 
    catch (err) 
    {
            LogError({
                "EventName" : "AssignPackageId",
                "Error" : err
            });

            return  err.apiErrorInfo.apiError;
            
    }
}
/* ======================== Validation Methods ============================== */
