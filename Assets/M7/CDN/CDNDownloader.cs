using System;
using System.Collections;
using System.Collections.Generic;
using M7.GameRuntime.Scripts.BackEnd.Azurefunctions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace M7.CDN
{
    public class CDNDownloader
    {
        [TitleGroup("Localization Strings")]
        static private string m_LoadingCacheString => "Loading Cache";
        static private string m_ResourceFetchString => "Fetching Resources";
        static private string m_FetchCurrentDataVersion => "Fetching current data version...";
        static private string m_StartingGame => "Starting game...";
    
        
        
        const string CLEAR_CACHE_ONCE = "CLEAR_CACHE_ONCE";

		public static bool IsInitialized { get; private set; } = false;
		//static bool isCDNLoaded;
        static long m_AssetDownloadSize = 0;
        static AsyncOperationHandle<long> getAddresablesDownloadSize;
        public static CdnStatus CDNStatus { get; private set; } = new CdnStatus(CdnStatus.Status.None);
        public static IEnumerator Execute(string cdnLabel, Action<CdnStatus> onStatusUpdate)
        {
            // if (isCDNLoaded)
            //     yield break;

            // fetch
            CDNStatus.CurrenStatus = CdnStatus.Status.FetchVersion;
            onStatusUpdate?.Invoke(CDNStatus);
            yield return MainInit();

            // loading cache
            CDNStatus.CurrenStatus = CdnStatus.Status.LoadingCache;
            onStatusUpdate?.Invoke(CDNStatus);
            yield return AddressableInit();

            // Fetching Resources
            CDNStatus.CurrenStatus = CdnStatus.Status.ResourceFetch;
            onStatusUpdate?.Invoke(CDNStatus);
            yield return CheckDownloadSize(cdnLabel);

            if (m_AssetDownloadSize == 0)
            {
                CDNStatus.CurrenStatus = CdnStatus.Status.Done;
                onStatusUpdate?.Invoke(CDNStatus);
                yield break;
            }

            // start download
            CDNStatus.CurrenStatus = CdnStatus.Status.DownloadStarting;
            onStatusUpdate?.Invoke(CDNStatus);
            yield return DownloadPackedTogetherRoutine(cdnLabel, onStatusUpdate);
            onStatusUpdate?.Invoke(CDNStatus);

            CDNStatus.CurrenStatus = CdnStatus.Status.Done;
            onStatusUpdate?.Invoke(CDNStatus);

            CDNStatus.CurrenStatus = CdnStatus.Status.None;
            onStatusUpdate?.Invoke(CDNStatus);

            //yield return UnloadUnusedAssets();
        }
        
        static IEnumerator MainInit()
        {
            if (IsInitialized)
                yield break;
            bool versionValidatorDone = false;

            //VersionValidator.ValidateCurrentVersion(Application.version, VersionValidator.VersionValidatorType.CDN,
            //    versionResponse =>
            //    {
            //        Debug.Log(versionResponse.Flag);

            //        if (versionResponse.Flag)
            //        {
            //            M7AddressableProfile.CDNCurrentVersion = versionResponse.Payload;
            //            Debug.Log(M7AddressableProfile.CDNCurrentVersion);

            //            versionValidatorDone = true;
            //        }
            //    }
            //);

            versionValidatorDone = true;
            yield return new WaitUntil(() => versionValidatorDone);
            IsInitialized = true;
        }

        static IEnumerator AddressableInit()
        {
            // NOTE: HACK! this will remove the old cache data of the players on early access.
            int clearCacheOnce = PlayerPrefs.GetInt(CLEAR_CACHE_ONCE, 0);
            Debug.LogFormat("clearCacheOnce = {0}", clearCacheOnce);

            if (clearCacheOnce == 0)
            {
                if (Caching.ClearCache())
                {
                    Log("Cache Cleared!");
                    PlayerPrefs.SetInt(CLEAR_CACHE_ONCE, 1);
                }
            }

            Addressables.ResourceManager.InternalIdTransformFunc += M7AddressableProfile.InternalIdTransformFunc;
            Addressables.ResourceManager.ResourceProviders.Add(new AssetBundleProvider());
            Addressables.ResourceManager.ResourceProviders.Add(new M7StorageHashProvider());
            Addressables.ResourceManager.ResourceProviders.Add(new M7StorageAssetBundleProvider());
            Addressables.ResourceManager.ResourceProviders.Add(new M7StorageJsonAssetProvider());
            yield return Addressables.InitializeAsync();
        }

        static IEnumerator CheckDownloadSize(string cdnLabel)
        {
            //IEnumerable keys = new string[] { cdnLabel };
            //AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = Addressables.LoadResourceLocationsAsync(keys, Addressables.MergeMode.Intersection);
            //yield return locationsHandle;

            getAddresablesDownloadSize = Addressables.GetDownloadSizeAsync(cdnLabel);

            yield return new WaitUntil(() => getAddresablesDownloadSize.GetDownloadStatus().IsDone);
            m_AssetDownloadSize = getAddresablesDownloadSize.Result;
            Debug.Log($"AssetDownloadSize: {m_AssetDownloadSize.ToSizeString()}");
            Addressables.ReleaseInstance(getAddresablesDownloadSize);
        }
        
        static IEnumerator UnloadUnusedAssets()
        {
            Log("Cleanup started ..");

            yield return new WaitForEndOfFrame();
            // Unload unusued asset after unloading the scene!
            AsyncOperation unloadAssetOp = Resources.UnloadUnusedAssets();
            var waitOp = new WaitUntil(() => unloadAssetOp.isDone && unloadAssetOp.progress >= 1f);
            yield return waitOp;

            long memory = GC.GetTotalMemory(false);
            Log("Memory before GC: " + memory.ToSizeString());
        }

        static IEnumerator DownloadPackedTogetherRoutine(string cdnLabel, Action<CdnStatus> onStatusUpdate)
        {
            //IEnumerable keys = new string[] { cdnLabel };
            //AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = Addressables.LoadResourceLocationsAsync(keys, Addressables.MergeMode.Intersection);
            //yield return locationsHandle;
            //var downloadOp = Addressables.DownloadDependenciesAsync(locationsHandle.Result);

            var downloadOp = Addressables.DownloadDependenciesAsync(cdnLabel);

            // downloading
            while (!downloadOp.GetDownloadStatus().IsDone && downloadOp.GetDownloadStatus().Percent != 1)
            {
                CDNStatus.CurrenStatus = CdnStatus.Status.Downloading;
                CDNStatus.DownloadStatus = downloadOp.GetDownloadStatus();
                onStatusUpdate?.Invoke(CDNStatus);
                yield return new WaitForSeconds(0.1f);
            }

            //Addressables.ReleaseInstance(locationsHandle);
            //Addressables.ReleaseInstance(downloadOp);

            Log("DOWNLOAD DONE!");
        }

        static void Log(string message) => Debug.Log($"<color=#FF0000>[CDNDownloader]: {message}</color>");
    }
}

public class CdnStatus
{
    public CdnStatus(Status status)
    {
        CurrenStatus = status;
    }
    
    public enum Status
    {
        None, FetchVersion, LoadingCache, ResourceFetch, DownloadStarting, Downloading, Done
    }

    public Status CurrenStatus { get; set; }
    public DownloadStatus DownloadStatus { get; set; }
    public string Description
    {
        get
        {
            return CurrenStatus switch
            {
                Status.FetchVersion => "Fetching current data version",
                Status.LoadingCache => "Loading Cache",
                Status.ResourceFetch => "Resource Fetch",
                Status.DownloadStarting => "Download Starting",
                Status.Downloading => "Downloading",
                Status.Done => "Done",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

}
