using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace M7.CDN
{
    /// <summary>
    /// Build script that takes care of modifying the settings.xml to use our json provider for loading the remote hash
    /// </summary>
    [CreateAssetMenu(fileName = "M7StorageBuildScript.asset", menuName = "Addressables/Content Builders/M7 Build")]
    public class M7StorageBuildScript : BuildScriptPackedMode
    {
        public override string Name => "M7 Build";

        protected override TResult DoBuild<TResult>(AddressablesDataBuilderInput builderInput, AddressableAssetsBuildContext aaContext)
        {
            var buildResult = base.DoBuild<TResult>(builderInput, aaContext);
            if (aaContext.Settings.BuildRemoteCatalog)
            {
                PatchSettingsFile(builderInput);
            }
            else
            {
                Debug.Log("OnBuild");
                Debug.LogWarning("[TheGamedevGuru] PlayFab: Addressables Remote Catalog is not enabled, skipping patching of the settings file");
            }
            return buildResult;
        }

        private void PatchSettingsFile(AddressablesDataBuilderInput builderInput)
        {
            // Get the path to the settings.json file
            var settingsJsonPath = Addressables.BuildPath + "/" + builderInput.RuntimeSettingsFilename;

            // Parse the JSON document
            var settingsJson = JsonUtility.FromJson<ResourceManagerRuntimeData>(File.ReadAllText(settingsJsonPath));

            // Look for the remote hash section
            var originalRemoteHashCatalogLocation = settingsJson.CatalogLocations.Find(locationData => locationData.Keys[0] == "AddressablesMainContentCatalogRemoteHash");
            var isRemoteLoadPathValid = originalRemoteHashCatalogLocation.InternalId.StartsWith(M7AddressableProfile.PlayfabCDNKey);
            if (isRemoteLoadPathValid == false)
            {
                throw new BuildFailedException($"RemoteLoadPath must start with {M7AddressableProfile.PlayfabCDNKey}");
            }

            // Change the remote hash provider to our PlayFabStorageHashProvider
            var newRemoteHashCatalogLocation = new ResourceLocationData(originalRemoteHashCatalogLocation.Keys, originalRemoteHashCatalogLocation.InternalId, typeof(M7StorageHashProvider), originalRemoteHashCatalogLocation.ResourceType, originalRemoteHashCatalogLocation.Dependencies);
            settingsJson.CatalogLocations.Remove(originalRemoteHashCatalogLocation);
            settingsJson.CatalogLocations.Add(newRemoteHashCatalogLocation);

            File.WriteAllText(settingsJsonPath, JsonUtility.ToJson(settingsJson));
            Debug.Log(settingsJsonPath);
        }
    }
}
