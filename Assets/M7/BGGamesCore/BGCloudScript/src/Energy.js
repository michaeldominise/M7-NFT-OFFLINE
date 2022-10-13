/* ======================== Energy Methods ============================== */

'use strict'

let App = {
    
}

handlers.GetEnery = function(args, content)
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
            "EventName" : "GetEnery_error",
            "Error" : err
        });

        return err.apiErrorInfo.apiError;
    }
}

/* ======================== Energy Methods ============================== */