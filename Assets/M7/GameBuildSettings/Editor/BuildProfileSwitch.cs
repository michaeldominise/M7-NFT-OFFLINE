using M7.GameBuildSettings.AzureConfigSettings;
using PlayFabSDK.Shared.Models;
using UnityEditor;
using UnityEngine;

namespace M7.Build.Editor
{
    public class BuildProfileSwitch : MonoBehaviour
    {
        private const string Directory = "Build/Profile/Change Profile to ";
    
        public const string DEV_PROFILE = "Dev";
        public const string QA_PROFILE = "QA";
        public const string PROD = "Prod";
        // public const string LOCALHOST = "LocalHost";
        public const string LOCALHOST_DEV = "LocalHost_Dev";
        public const string LOCALHOST_QA = "LocalHost_QA";

        public static PlayFabSharedSettings GetPlayFabSharedSettings()
        {
            return Resources.Load("PlayFabSharedSettings") as PlayFabSharedSettings;
        }

        public static AzureSettings GetAzureSettings()
        {
            return Resources.Load("AzureSettings") as AzureSettings;
        }
        
        // [MenuItem(Directory + LOCALHOST)]
        // public static void ChangeSettingsToLocalHost()
        // {
        //     ChangeSettings(LOCALHOST);
        // }
        
        [MenuItem(Directory + LOCALHOST_DEV)]
        public static void ChangeSettingsToLocalHost_Dev()
        {
            ChangeSettings(LOCALHOST_DEV);
        }
        
        [MenuItem(Directory + LOCALHOST_QA)]
        public static void ChangeSettingsToLocalHost_QA()
        {
            ChangeSettings(LOCALHOST_QA);
        }
        
        [MenuItem(Directory + DEV_PROFILE)]
        public static void ChangeSettingsToDev()
        {
            ChangeSettings(DEV_PROFILE);
        }

        [MenuItem(Directory + QA_PROFILE)]
        public static void ChangeSettingsToQA()
        {
            ChangeSettings(QA_PROFILE);
        }

        [MenuItem(Directory + PROD)]
        public static void ChangeSettingsToRelease()
        {
            ChangeSettings(PROD);
        }

        static void ChangeSettings(string profileName)
        {
            // for playfab
            var playfabProfile = PlayfabProfileOverview.Instance.GetProfile(profileName);
            if (playfabProfile == null)
            {
                Debug.LogError($"Can't find {profileName} in PlayfabProfileOverview");
                return;
            }

            // for azure functions
            var azureProfile = AzureProfileOverview.Instance.GetProfile(profileName);
            if (azureProfile == null)
            {
                Debug.LogError($"Can't find {profileName} in AzureProfileOverview");
                return;
            }
        
            // playfab
            var settings = GetPlayFabSharedSettings();
            settings.TitleId = playfabProfile.titleId;

            // azure
            var azureSettings = GetAzureSettings();
            azureSettings.profileName = azureProfile.profileName;
            azureSettings.hostKey = azureProfile.hostKey;
            azureSettings.uri = azureProfile.azureFunctionUri;
            azureSettings.azureCdnUri = azureProfile.azureCdnUri;
            EditorUtility.SetDirty(azureSettings);
            
#if ENABLE_PLAYFABSERVER_API || ENABLE_PLAYFABADMIN_API || UNITY_EDITOR
            settings.DeveloperSecretKey = playfabProfile.DeveloperSecretKey;
#endif

            EditorUtility.SetDirty(settings);
            Debug.Log($"Playfab Shared Settings changed to {profileName} profile");
#if UNITY_EDITOR
            GBuildProfileSwitchEditor.SetChecked();
#endif
        }
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class GBuildProfileSwitchEditor
    {
        private const string Directory = "Build/Profile/Change Profile to ";
    
        static AzureSettings settings;
        // static AzureFun
        static GBuildProfileSwitchEditor()
        {
            EditorApplication.delayCall += () => {
                if (settings == null)
                    settings = Resources.Load("AzureSettings") as AzureSettings;
                SetChecked();
            };
        }

        public static void SetChecked()
        {
            // Menu.SetChecked(Directory + BuildProfileSwitch.LOCALHOST, settings.profileName == AzureProfileOverview.Instance.GetProfile(BuildProfileSwitch.LOCALHOST)?.profileName);
            Menu.SetChecked(Directory + BuildProfileSwitch.LOCALHOST_DEV, settings.profileName == AzureProfileOverview.Instance.GetProfile(BuildProfileSwitch.LOCALHOST_DEV)?.profileName);
            Menu.SetChecked(Directory + BuildProfileSwitch.LOCALHOST_QA, settings.profileName == AzureProfileOverview.Instance.GetProfile(BuildProfileSwitch.LOCALHOST_QA)?.profileName);
            Menu.SetChecked(Directory + BuildProfileSwitch.DEV_PROFILE, settings.profileName == AzureProfileOverview.Instance.GetProfile(BuildProfileSwitch.DEV_PROFILE)?.profileName);
            Menu.SetChecked(Directory + BuildProfileSwitch.QA_PROFILE, settings.profileName == AzureProfileOverview.Instance.GetProfile(BuildProfileSwitch.QA_PROFILE)?.profileName);
            Menu.SetChecked(Directory + BuildProfileSwitch.PROD, settings.profileName == AzureProfileOverview.Instance.GetProfile(BuildProfileSwitch.PROD)?.profileName);
        }
    }
#endif
}