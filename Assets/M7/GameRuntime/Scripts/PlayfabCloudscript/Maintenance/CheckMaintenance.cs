using System;

namespace M7.GameRuntime.Scripts.PlayfabCloudscript.Maintenance
{
    public static class CheckMaintenance
    {
        public static void GetMaintenanceStatus(Action<ExecuteResult> callback)
        {
            PlayFabFunctions.PlayFabCallFunction("GetInternalTitleData", false, "maintenance", "", callback);
        }
    }
}