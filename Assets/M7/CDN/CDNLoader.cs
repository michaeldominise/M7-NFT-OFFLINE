using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M7.CDN.Addressable;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace M7.CDN
{
    public class CDNLoader : MonoBehaviour
    {
        [SerializeField] private bool m_EnableLogs = false;

        [SerializeField] private TextMeshProUGUI m_DownloadResourcesText = null;
        [SerializeField] private TextMeshProUGUI m_DownloadProgressText = null;
        [SerializeField] private Slider m_DownloadProgressSlider = null;

        [SerializeField] private Button m_DownloadButton = null;
        [SerializeField] private TextMeshProUGUI m_DownloadButtonText = null;
        [SerializeField] private Animator m_DownloadButtonAnimator = null;

        [SerializeField] private GameObject m_TipsObj = null;

        [TitleGroup("Localization Strings")]
        static private string m_ConnectingString => "Connecting...";
        static private string m_DownloadingString => "Downloading...";
        static private string m_DownloadResourceFormat => "Downloading resources... {0}";
        static private string m_RecommendedWifiString => "NOTE: WI-FI connection is recommended.";
        static private string m_DownloadButtonString => "Click To Download";
        static private string m_DailyRewardString => "Loading Daily Rewards";
        static private string m_LoadingCacheString => "Loading Cache";
        static private string m_StartingGame => "Starting Game";
        static private string m_ResourceFetchString => "Fetching Resources";

        public bool IsDownloading { get; private set; } = false;
        public bool IsDisconnected { get; private set; } = false;
        
        public static Action<string> onUpdateResourcesText;
        public static Action<string> onUpdateProgressText;
        public static Action<float, float> onInitializeSlider;
        public static Action<float> onUpdateSlider;
        public static Action<string> onStatusUpdate;

        public event System.Action EventDownloadStarted = () => { };

        public const string CLEAR_CACHE_ONCE = "CLEAR_CACHE_ONCE";

		public static bool isCDNCurrentVersionInit = false;
		public static bool isCDNLoaded = false;
        static string initialAddressableLabel;
        static Action onLoaded;
        static Action onFinish;
        static long m_AssetDownloadSize = 0;
        static AsyncOperationHandle<long> getAddresablesDownloadSize;

        public AsyncOperationHandle downloadOp;
        public Coroutine downloadCoroutine;

        public string CDNTag;

        public static IEnumerator Show(AddressableAssetHelper.CDN_LABELS cdnLabel, Action onLoaded = null, Action onFinish = null, bool showConfimationDownload = false)
        {
            if (isCDNLoaded)
                yield break;

            initialAddressableLabel = cdnLabel.ToAddressableLabel();
            CDNLoader.onFinish = onFinish;
            CDNLoader.onLoaded = onLoaded;

            onStatusUpdate?.Invoke("Fetching current data version...");
            //yield return CDNCurrentVersionInit();

            onStatusUpdate?.Invoke(m_LoadingCacheString);
            yield return AddressableInit();

            onStatusUpdate?.Invoke(m_ResourceFetchString);
            yield return CheckDownloadSize();

            if (m_AssetDownloadSize == 0)
            {
                onLoaded?.Invoke();
                onFinish?.Invoke();
            }
            else
            {
                if (showConfimationDownload)
                {
                    //MessageBoxPopup.CreateOk("Notice", "We'll download new assets to continue playing progress. This may take few mins.")
                    //    .AddCallbacks(null, LoadCDNLoaderScene)
                    //    .Show();
                }
                else
                    LoadCDNLoaderScene();
            }
        }

        static void LoadCDNLoaderScene()
        {
        //    SceneTransition.LoadScene("CDNLoader", new SceneTransition.Settings { loadSceneMode = LoadSceneMode.Additive }, new SceneTransition.Callbacks { onFinishTransition = onLoaded });
        }

        static IEnumerator CDNCurrentVersionInit()
        {
            if (isCDNCurrentVersionInit)
                yield break;
            bool versionValidatorDone = false;
            VersionValidator.ValidateCurrentVersion(Application.version, VersionValidator.VersionValidatorType.CDN,
                (VersionValidator.ValidateCurrentVersionResponse versionResponse) =>
                {
                    if (versionResponse.Flag)
                    {
                        M7AddressableProfile.CDNCurrentVersion = versionResponse.Payload;
                        versionValidatorDone = true;
                    }
                }
            );
            versionValidatorDone = true;

            yield return new WaitUntil(() => versionValidatorDone);
            isCDNCurrentVersionInit = true;
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
                    Debug.Log("Cache Cleared!");
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

        static IEnumerator CheckDownloadSize()
        {
            getAddresablesDownloadSize = Addressables.GetDownloadSizeAsync(initialAddressableLabel);
            yield return new WaitUntil(() => getAddresablesDownloadSize.GetDownloadStatus().IsDone);
            m_AssetDownloadSize = getAddresablesDownloadSize.Result;
            Debug.Log($"AssetDownloadSize: {m_AssetDownloadSize}");
            Addressables.ReleaseInstance(getAddresablesDownloadSize);
        }

        public void Awake()
        {
            // StartCoroutine(Show(AddressableAssetHelper.CDN_LABELS.default_additive, () =>
            // {
            initialAddressableLabel=AddressableAssetHelper.CDN_LABELS.default_replace.ToAddressableLabel();
            StartCoroutine(CheckDownloadSize());
                SetDownloadResourcesText(m_DownloadingString);
                SetDownloadProgressText("0%");
                SetDownloadButtonText(m_DownloadButtonString);
            //    SetTipsObjActive(true);
                SetSliderActive(false);
                SetDownloadButtonActive(false);
                isCDNLoaded = true;
                // }));
        }

        public void OnEnable()
        {
            //NetworkMethods.OnNetworkError += OnNetworkError;
            //NetworkMethods.OnNetworkErrorCallback += OnNetworkError;
            //ReconnectSensor.OnReconnect += OnReconnect;
        }

        public void OnDisable()
        {
            //NetworkMethods.OnNetworkError -= OnNetworkError;
            //NetworkMethods.OnNetworkErrorCallback -= OnNetworkError;
            //ReconnectSensor.OnReconnect -= OnReconnect;
        }

        public IEnumerator Start()
        {
            Log("Initializing started ..");
            yield return UnloadUnusuedAssets();

            IsDownloading = false;

            SetDownloadResourcesText(string.Format(m_DownloadResourceFormat + "\n\n" + m_RecommendedWifiString, m_AssetDownloadSize.ToSizeString()));
           // SetTipsObjActive(true);
            SetSliderActive(true);
            DownloadResources();
            //SetDownloadButtonActive(true);
        }

        public IEnumerator UnloadUnusuedAssets()
        {        
            Log("Cleanup started ..");

            // Unload unusued asset after unloading the scene!
            AsyncOperation unloadAssetOp = Resources.UnloadUnusedAssets();
            var waitOp = new WaitUntil(() => unloadAssetOp.isDone && unloadAssetOp.progress >= 1f);
            yield return waitOp;

            long memory = System.GC.GetTotalMemory(false);
            Log("Memory before GC: " + memory.ToSizeString());

            //Removed GC Collect since UnloadUnusedAssets calls this internally
            //System.GC.Collect();

            //memory = System.GC.GetTotalMemory(false);
            //Log("Memory after GC: " + memory.ToSizeString());
        }

        public void DownloadResources()
        {
            if(!IsDownloading)
            {
                IsDownloading = true;
                
                SetDownloadButtonAnimatorActive(false);
                SetDownloadButtonText(m_DownloadingString);
                SetDonwloadButtonInteractable(false);
                
                if(m_DownloadButtonAnimator != null)
                {
                    m_DownloadButtonAnimator.transform.localScale = Vector3.one;
                }

                // StartCoroutine(DownloadWithLabelRoutine());
                downloadCoroutine = StartCoroutine(DownloadPackedTogetherRoutine("CDN1"));
				EventDownloadStarted();
            }
        }

        public void LoadNextScene()
        {
            isCDNLoaded = false;
            SetDownloadResourcesText(m_StartingGame);
            onStatusUpdate?.Invoke(m_StartingGame);
          //  SceneTransition.UnloadScene("CDNLoader", default, new SceneTransition.Callbacks { onFinishTransition = onFinish });
        }

        public void SetDownloadResourcesText(string downloadSizeText)
        {
            if(m_DownloadResourcesText != null && !IsDisconnected)
            {
                m_DownloadResourcesText.text = downloadSizeText;
                onUpdateResourcesText?.Invoke(downloadSizeText);
            }
        }

        public void SetDownloadProgressText(string progressText)
        {
            if(m_DownloadProgressText != null && !IsDisconnected)
            {
                m_DownloadProgressText.text = progressText;
                onUpdateProgressText?.Invoke(progressText);
            }
        }

        public void SetTipsObjActive(bool active)
        {
            if(m_TipsObj != null)
            {
                m_TipsObj.SetActive(active);
            }
        }

        public void SetSliderActive(bool active)
        {
            if(m_DownloadProgressSlider != null)
            {
                m_DownloadProgressSlider.gameObject.SetActive(active);
            }
        }

        public void SetDownloadButtonAnimatorActive(bool active)
        {
            if(m_DownloadButtonAnimator != null)
            {
                m_DownloadButtonAnimator.enabled = active;
            }
        }

        public void SetDownloadButtonText(string message)
        {
            if(m_DownloadButtonText != null)
            {
                m_DownloadButtonText.text = message;
            }
        }

        public void SetDownloadButtonActive(bool active)
        {
            if(m_DownloadButton != null)
            {
                m_DownloadButton.gameObject.SetActive(active);
            }
        }

        public void SetDonwloadButtonInteractable(bool active)
        {
            if(m_DownloadButton != null)
            {
                m_DownloadButton.interactable = active;
            }
        }

        public void InitDownloadProgress(float min, float max)
        {
            if(m_DownloadProgressSlider != null)
            {
                m_DownloadProgressSlider.minValue = min;
                m_DownloadProgressSlider.maxValue = max;
                m_DownloadProgressSlider.value = 0;

                onInitializeSlider?.Invoke(min, max);
            }
        }

        public void SetDownloadProgress(float progress)
        {
            if(m_DownloadProgressSlider != null && !IsDisconnected)
            {
                m_DownloadProgressSlider.value = progress;

                onUpdateSlider?.Invoke(progress);
            }
        }

        private void OnReconnect()
        {
            Addressables.InitializeAsync();
            IsDisconnected = false;
        }

        private IEnumerator DownloadPackedTogetherRoutine(string cdnTag)
        {
            InitDownloadProgress(0f, 1f);
            initialAddressableLabel = cdnTag;
            downloadOp = Addressables.DownloadDependenciesAsync(initialAddressableLabel);
            while (!downloadOp.GetDownloadStatus().IsDone && downloadOp.GetDownloadStatus().Percent != 1)
            {
                var downloadStatus = downloadOp.GetDownloadStatus();
                var downloadedBytes = downloadOp.GetDownloadStatus().DownloadedBytes;
                int roundedProgress = Mathf.RoundToInt(downloadOp.GetDownloadStatus().Percent * 100f);
                SetDownloadProgressText(string.Format("{0:0}%", roundedProgress));
                SetDownloadProgress(downloadOp.GetDownloadStatus().Percent);
                SetDownloadResourcesText(string.Format(m_DownloadResourceFormat + "\n\n" + m_RecommendedWifiString, $"{Convert.ToInt64(downloadedBytes).ToSizeString()}/{Convert.ToInt64(m_AssetDownloadSize).ToSizeString()}"));
                yield return new WaitForSeconds(0.1f);
            }

            SetDownloadResourcesText(string.Format(m_DownloadResourceFormat + "\n\n" + m_RecommendedWifiString, $"{Convert.ToInt64(m_AssetDownloadSize).ToSizeString()}/{Convert.ToInt64(m_AssetDownloadSize).ToSizeString()}"));
            SetDownloadProgressText(string.Format("{0:0}%", 100f));
            SetDownloadProgress(1);

            Addressables.ReleaseInstance(downloadOp);

            Log("DOWNLOAD DONE!");
            
         //   LoadNextScene();
            //GetAssetDownloadSizeAsync();
        }

        private void Log(string message)
        {
            if(m_EnableLogs)
            {
                Debug.Log(string.Format("<color=#FF0000>[{0}]: {1}</color>", name, message));
            }
        }
    }
}
