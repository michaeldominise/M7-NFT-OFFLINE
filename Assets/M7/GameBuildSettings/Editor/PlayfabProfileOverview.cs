using System;
using System.Collections.Generic;
using Sirenix.Utilities;

namespace M7.Build.Editor
{
    [GlobalConfig("Editor/Playfab")]
    public class PlayfabProfileOverview : GlobalConfig<PlayfabProfileOverview>
    {
        [Serializable]
        public class PlayfabProfileData
        {
            public string profileName = "Dev";
            public string titleId = "D0D7";
            public string DeveloperSecretKey = "48OUQ83R5PMZS9NBMXIQFPEEDGDQT57JKE7ISPCU81MTGFX1TX";
        }

        public List<PlayfabProfileData> profileList;

        internal PlayfabProfileData GetProfile(string profileName)
        {
            return profileList?.Find(data => data.profileName == profileName);
        }
    }
}