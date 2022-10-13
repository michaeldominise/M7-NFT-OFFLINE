using System;
using M7.GameRuntime.Scripts.Managers.AzureFunctions;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using M7.Tools;

namespace M7.CDN
{
    [CreateAssetMenu(fileName = "M7AddressableProfile.asset", menuName = "Addressables/M7AddressableProfile")]
    public class M7AddressableProfile : M7Settings
    {
        static M7AddressableProfile _Instance;
        public static M7AddressableProfile Instance
        {
            get
            {
                _Instance = _Instance ?? Resources.Load<M7AddressableProfile>("M7AddressableProfile");
                return _Instance;
            }
        }

        #if UNITY_EDITOR
        public static string BuildTarget => UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString().ToLower();
        #endif
        
        public static string AppVersion => Application.version;
        public static string AddressableBuildNumber => Instance._AddressableBuildNumber;
        public static string PlayfabCDNKey => Instance._PlayfabCDNKey;
        public static string CDNCurrentVersionKey => Instance._CDNCurrentVersionKey;

        [SerializeField] string _AddressableBuildNumber = "02";
        [SerializeField] string _PlayfabCDNKey = "https://playfab/";
        [SerializeField] string _CDNCurrentVersionKey = "$CDNCurrentVersion";
        [SerializeField] bool useCDNCurrentVersionOverride;
        string _CDNCurrentVersion { get; set; }
        
        public static string CDNCurrentVersion
        {
            //get => Instance._CDNCurrentVersion;
            get => Instance._AddressableBuildNumber;
            set
            {
                Debug.LogError($"Playfab cdn version: {value}");
                if (Instance.useCDNCurrentVersionOverride)
                {
                    Instance._CDNCurrentVersion = $"{Instance._AddressableBuildNumber}";
                    Debug.LogError($"Use override cnd version: {Instance._CDNCurrentVersion}");
                }
                else
                {
                    Instance._CDNCurrentVersion = value;
                }
            }
        }

        public static string InternalIdTransformFunc(IResourceLocation location)
        {
            var path = location.InternalId;
#if UNITY_EDITOR
            var filePrefix = "file://";
            if (path.StartsWith(filePrefix))
                path = path.Substring(filePrefix.Length);
#endif
            if (!path.StartsWith(PlayfabCDNKey))
                return path;
            
            return path.Replace(CDNCurrentVersionKey, CDNCurrentVersion);
        }

        public static void GetWebRequestPathOverride(string path, Action<string> onFinish)
        {
          //  CDNCurrentVersion = "6";
            if (!path.StartsWith(PlayfabCDNKey))
                onFinish?.Invoke(path);
            //var addressableId = path.Replace(CDNCurrentVersionKey, CDNCurrentVersion).Replace(PlayfabCDNKey, AzureSettingsManager.Instance.AzureSettings.azureCdnUri);
            var addressableId = path.Replace(CDNCurrentVersionKey, CDNCurrentVersion).Replace(PlayfabCDNKey, $"{Application.streamingAssetsPath}/");
            Debug.Log(addressableId);
            onFinish?.Invoke(addressableId);
        }
    }
}