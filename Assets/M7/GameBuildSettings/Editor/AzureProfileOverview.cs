using System;
using System.Collections.Generic;
using PlayFab;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine.Serialization;

namespace M7.Build.Editor
{
    [GlobalConfig("Assets/M7/GameBuildSettings")]
    public class AzureProfileOverview : GlobalConfig<AzureProfileOverview>
    {
        [Serializable]
        public class AzureFunctionKey
        {
            public string profileName = "Dev";
            public string hostKey = "pmRbFB-Enim8LM7iB8phK4tikB42eWgmiYwf1qjkdF9HAzFu8w2sAw==";
            [FormerlySerializedAs("uri")] public string azureFunctionUri = PlayFabSettings.LocalApiServer;
            public string azureCdnUri = "pmRbFB-Enim8LM7iB8phK4tikB42eWgmiYwf1qjkdF9HAzFu8w2sAw==";
        }

        public List<AzureFunctionKey> profileList;

        internal AzureFunctionKey GetProfile(string profileName)
        {
            return profileList?.Find(data => data.profileName == profileName);
        }

        [Button]
        private void Set()
        {
            foreach (var profile in profileList)
            {
                profile.azureFunctionUri = PlayFabSettings.LocalApiServer;
            }
        }
    }
}