using UnityEngine;
using System.Collections;
using UnityEngine.ResourceManagement.ResourceProviders;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace M7.CDN
{
    public class M7StorageHashProvider : ResourceProviderBase
    {
        public override void Provide(ProvideHandle provideHandle)
        {
            if (!provideHandle.Location.InternalId.StartsWith(M7AddressableProfile.PlayfabCDNKey))
                return;

            M7AddressableProfile.GetWebRequestPathOverride(provideHandle.Location.InternalId,
                resultURL =>
                {
                    var resourceLocation = new ResourceLocationBase(resultURL, resultURL, typeof(TextDataProvider).FullName, typeof(string));
                    var asyncHandle = provideHandle.ResourceManager.ProvideResource<string>(resourceLocation);
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