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