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