using M7.GameBuildSettings.AzureConfigSettings;
using UnityEngine;
using Sirenix.OdinInspector;

namespace M7.GameRuntime.Scripts.Managers.AzureFunctions
{
    public class AzureSettingsManager : MonoBehaviour
    {
        public static AzureSettingsManager Instance;

        [ShowInInspector] private AzureSettings _azureSettings;

        public AzureSettings AzureSettings => _azureSettings;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                _azureSettings = Resources.Load<AzureSettings>("AzureSettings");
                return;
            }
            
            Destroy(gameObject);
        }

        [Button]
        private void CheckSettings()
        {
            Debug.Log($"Current Settings. {AzureSettings.profileName}, {AzureSettings.hostKey}");
        }
    }
}
