using System.Collections;
using System.Collections.Generic;
using M7.GameBuildSettings.AzureConfigSettings;
using M7.GameRuntime.Scripts.Managers.AzureFunctions;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace M7.CDN
{
    public class M7StorageJsonAssetProvider : JsonAssetProvider
    {
        public override string ProviderId => typeof(JsonAssetProvider).FullName;

        public override void Provide(ProvideHandle provideHandle)
        {
            if (!provideHandle.Location.InternalId.StartsWith(M7AddressableProfile.PlayfabCDNKey))
            {
                base.Provide(provideHandle);
                return;
            }

            M7AddressableProfile.GetWebRequestPathOverride(provideHandle.Location.InternalId,
                resultURL =>
                {
                    Assert.IsTrue(provideHandle.Location.ResourceType == typeof(ContentCatalogData), "Only catalogs supported");
                    var resourceLocation = new ResourceLocationBase(resultURL, resultURL, typeof(JsonAssetProvider).FullName, typeof(string));
                    var asyncHandle = provideHandle.ResourceManager.ProvideResource<ContentCatalogData>(resourceLocation);
                    provideHandle.SetDownloadProgressCallbacks(asyncHandle.GetDownloadStatus);
                    asyncHandle.Completed += handle =>
                    {
                        if (asyncHandle.Status == AsyncOperationStatus.Failed)
                        {
                            Provide(provideHandle);
                            return;
                        }
                        var contents = handle.Result;
                        provideHandle.Complete(contents, true, handle.OperationException);
                    };
                });
        }
    }
}