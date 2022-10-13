using UnityEngine;

namespace M7.GameBuildSettings.AzureConfigSettings
{
    [CreateAssetMenu(fileName = "AzureSettings", menuName = "Azure/CreateSettings", order = 1)]
    public class AzureSettings : ScriptableObject
    {
        public string profileName;
        public string hostKey;
        public string uri;
        public string azureCdnUri;
    }
}