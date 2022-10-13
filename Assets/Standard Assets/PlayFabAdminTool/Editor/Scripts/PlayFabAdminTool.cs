#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

using PlayFab;
using PlayFabSDK.Shared.Models;

#if ENABLE_PLAYFABADMIN_API
using PlayFab.AdminModels;
#endif

using Sirenix.OdinInspector;
using Array = System.Array;

#if UNITY_5_3_OR_NEWER
[CreateAssetMenu(fileName = "PlayFabAdminTool", menuName = "PlayFab/PlayFabAdminTool", order = 1)]
#endif
public class PlayFabAdminTool : ScriptableObject 
{
    [ReadOnly, ShowInInspector, BoxGroup]
    public string TitleId { get { return PlayFabSharedPrivate.TitleId; } }

    [ReadOnly, ShowInInspector, BoxGroup]
    private string DeveloperSecretKey { get { return PlayFabSharedPrivate.DeveloperSecretKey; } }

    [ShowInInspector, BoxGroup]
    public bool EnableTool
    {
        get 
        { 
            string[] defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');
            return defines.Contains("ENABLE_PLAYFABADMIN_API");
        }
        set 
        { 
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup) + ";ENABLE_PLAYFABADMIN_API";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
        }
    }

    private HashSet<string> m_PlayerIdList = new HashSet<string>();

    private static PlayFabSharedSettings s_PlayFabSharedSettings = null;
    private static PlayFabSharedSettings PlayFabSharedPrivate { get { if (s_PlayFabSharedSettings == null) s_PlayFabSharedSettings = GetSharedSettingsObjectPrivate(); return s_PlayFabSharedSettings; } }

    private const string SEGMENT_ALL_PLAYERS = "B40901EB47687A07";

    private static PlayFabSharedSettings GetSharedSettingsObjectPrivate()
    {
        PlayFabSharedSettings[] settingsList = Resources.LoadAll<PlayFabSharedSettings>("PlayFabSharedSettings");

        if(settingsList.Length != 1)
        {
            throw new System.Exception("The number of PlayFabSharedSettings objects should be 1: " + settingsList.Length);
        }

        return settingsList[0];
    }

    [Button, EnableIf("EnableTool")]
    public void DeleteAllMasterPlayerAccount(string segmentId = SEGMENT_ALL_PLAYERS, uint secondsToLive = 5, uint maxBatchSize = 500)
    {
#if ENABLE_PLAYFABADMIN_API
        m_PlayerIdList = new HashSet<string>();

        System.Action<GetPlayersInSegmentResult> success = (result) =>
        {
            int profilesLen = result.ProfilesInSegment;
            Debug.Log("Segment retrieved with   " + profilesLen + " players");

            for(int i = 0; i < profilesLen; i++)
            {
                PlayerProfile profile = result.PlayerProfiles[i];

                if(profile != null)
                {
                    m_PlayerIdList.Add(profile.PlayerId);
                    DeleteMasterPlayerAccount(profile.PlayerId, false);
                }
            }
        };

        Debug.Log("Fetching segment with id  " + segmentId);
        GetPlayersInSegmentRequest req = new GetPlayersInSegmentRequest();
        req.SegmentId = segmentId;
        req.SecondsToLive = secondsToLive;
        req.MaxBatchSize = maxBatchSize;
        PlayFabAdminAPI.GetPlayersInSegment(req, success, (err) => Debug.LogError(err.ErrorMessage));
#endif
    }

    [Button, EnableIf("EnableTool")]
    public void DeleteMasterPlayerAccount(string playfabId, bool isSinglePurge = true)
    {
#if ENABLE_PLAYFABADMIN_API
        System.Action<DeleteMasterPlayerAccountResult> success = (result) =>
        {
            if(isSinglePurge || m_PlayerIdList.Remove(playfabId))
            {
                Debug.Log("Delete player successful! " + result.JobReceiptId + " | " + playfabId);

                if(!isSinglePurge)
                {
                    Debug.Log("Remaining players .. " + m_PlayerIdList.Count);
                }
            }
        };

        DeleteMasterPlayerAccountRequest req = new DeleteMasterPlayerAccountRequest();
        req.PlayFabId = playfabId;
        req.MetaData = "Deleted player using admin call.";
        PlayFabAdminAPI.DeleteMasterPlayerAccount(req, success, (err) => Debug.LogError(err.ErrorMessage));
#endif
    }

    [Button, EnableIf("EnableTool")]
    public void DeleteAllTitlePlayerAccount(string segmentId = SEGMENT_ALL_PLAYERS, uint secondsToLive = 5, uint maxBatchSize = 500)
    {
#if ENABLE_PLAYFABADMIN_API
        m_PlayerIdList = new HashSet<string>();

        System.Action<GetPlayersInSegmentResult> success = (result) =>
        {
            int profilesLen = result.ProfilesInSegment;
            Debug.Log("Segment retrieved with   " + profilesLen + " players");

            for(int i = 0; i < profilesLen; i++)
            {
                PlayerProfile profile = result.PlayerProfiles[i];

                if(profile != null)
                {
                    m_PlayerIdList.Add(profile.PlayerId);
                    DeleteTitlePlayerAccount(profile.PlayerId, false);
                }
            }
        };

        Debug.Log("Fetching segment with id  " + segmentId);
        GetPlayersInSegmentRequest req = new GetPlayersInSegmentRequest();
        req.SegmentId = segmentId;
        req.SecondsToLive = secondsToLive;
        req.MaxBatchSize = maxBatchSize;
        PlayFabAdminAPI.GetPlayersInSegment(req, success, (err) => Debug.LogError(err.ErrorMessage));
#endif
    }

    [Button, EnableIf("EnableTool")]
    public void DeleteTitlePlayerAccount(string playfabId, bool isSinglePurge = true)
    {
#if ENABLE_PLAYFABADMIN_API
        System.Action<DeletePlayerResult> success = (result) =>
        {
            if(isSinglePurge || m_PlayerIdList.Remove(playfabId))
            {
                Debug.Log("Delete player successful! " + playfabId);

                if(!isSinglePurge)
                {
                    Debug.Log("Remaining players .. " + m_PlayerIdList.Count);
                }
            }
        };

        DeletePlayerRequest req = new DeletePlayerRequest();
        req.PlayFabId = playfabId;
        PlayFabAdminAPI.DeletePlayer(req, success, (err) => Debug.LogError(err.ErrorMessage));
#endif
    }
}

#endif
