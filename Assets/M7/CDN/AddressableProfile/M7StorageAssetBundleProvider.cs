using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace M7.CDN
{
    [DisplayName("M7StorageAssetBundleProvider")]
    public class M7StorageAssetBundleProvider : AssetBundleProvider
    {
        public override void Provide(ProvideHandle provideHandle)
        {
            if (!provideHandle.Location.InternalId.StartsWith(M7AddressableProfile.PlayfabCDNKey))
            {
                base.Provide(provideHandle);
                return;
            }

            M7AddressableProfile.GetWebRequestPathOverride(provideHandle.Location.InternalId, resultURL =>
                {
                    var dependenciesList = provideHandle.Location.Dependencies;
                    var dependenciesArray = provideHandle.Location.Dependencies == null ? new IResourceLocation[0] : new IResourceLocation[dependenciesList.Count];
                    dependenciesList?.CopyTo(dependenciesArray, 0);
                    var resourceLocation = new ResourceLocationBase(resultURL, resultURL, typeof(AssetBundleProvider).FullName, typeof(IResourceLocator), dependenciesArray)
                    {
                        Data = provideHandle.Location.Data,
                        PrimaryKey = provideHandle.Location.PrimaryKey
                    };

                    var asyncHandle = provideHandle.ResourceManager.ProvideResource<IAssetBundleResource>(resourceLocation);
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