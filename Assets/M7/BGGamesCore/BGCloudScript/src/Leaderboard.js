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