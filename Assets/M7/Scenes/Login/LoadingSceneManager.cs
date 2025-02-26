    using System.Collections;
using M7;
using M7.CDN;
using M7.GameRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;
using M7.GameData;
    using M7.GameRuntime.Scripts.BackEnd.CosmosDB.Account;
    using M7.GameRuntime.Scripts.Settings;
    using M7.ServerTestScripts;
    using Newtonsoft.Json;

    namespace M7
{
    public class LoadingSceneManager : SceneManagerBase
    {
        public static LoadingSceneManager Instance { get; private set; }

        [SerializeField] string initialCdnLabel = "CDN1";
        [SerializeField] GameObject progressUIContanier;
        [SerializeField] TextMeshProUGUI description;
        [SerializeField] Slider cdnProgressBar;
        [SerializeField] AssetReference nextScene;
        [SerializeField] AssetReference signUpLoginScene;
        [SerializeField] AssetReference activationScene;
        [SerializeField] CanvasGroup container;
        [SerializeField] float fadeInDuration = 3;

        bool LoginDone => PlayerDatabase.AccountProfile.IsInitialized && PlayfabDataManager.Instance.IsInitialized;
        CdnStatus.Status CurrentCdnStatus { get; set; }

        protected override void Awake()
        {
            Instance = this;
            container.alpha = 0;
            StartCoroutine(Init());
            base.Awake();
        }

        public IEnumerator Init()
        {
            yield return new WaitForEndOfFrame();
            //PlayerDatabase.ResetLocalData();
            PlayerDatabase.LoadLocalData();
            container.DOFade(1, fadeInDuration).onComplete += TryLogin;
        }

        public void TryLogin()
        {
            // check if logged in
            var savedPlayFabPlayer = LocalData.GetPlayer();

            if (string.IsNullOrWhiteSpace(savedPlayFabPlayer))
            {
                print($"Is login done {LoginDone}");
                LoadScene(signUpLoginScene, LoadSceneMode.Additive);
                return;
            }

            //PlayerDatabase.AccountProfile.OverwriteValues(savedPlayFabPlayer);

            // logged in but account is not yet activated
            if (PlayerDatabase.AccountProfile.PlayerStatus == PlayerStatus.New)
            {
                LoadScene(activationScene, LoadSceneMode.Additive);
                return;
            }

            PlayfabDataManager.Instance.Init();
            //PlayfabDataManager.Instance.PlayFabLogin(PlayerDatabase.AccountProfile.Email, PlayfabDataManager.Instance.Init, OnLoginErrorCallback);
            
            void OnLoginErrorCallback() => MessageBox.Create("Cannot login", MessageBox.ButtonType.Ok, "Playfab Login");
        }

        public void StartCDNDownload()
        {
            if(CurrentCdnStatus == CdnStatus.Status.None)
                StartCoroutine(CDNDownloader.Execute(initialCdnLabel, OnStatusUpdate));
        }

        public void OnStatusUpdate(CdnStatus status)
        {
            if(status.CurrenStatus != CdnStatus.Status.None)
                CurrentCdnStatus = status.CurrenStatus;
            switch (status.CurrenStatus)
            {
                case CdnStatus.Status.FetchVersion:
                case CdnStatus.Status.LoadingCache:
                case CdnStatus.Status.ResourceFetch:
                    description.text = status.Description;
                    break;
                case CdnStatus.Status.DownloadStarting:
                    description.text = status.Description;
                    progressUIContanier.SetActive(true);
                    break;
                case CdnStatus.Status.Downloading:
                    UpdateProgressBar($"{status.DownloadStatus.DownloadedBytes.ToSizeString()} of {status.DownloadStatus.TotalBytes.ToSizeString()}", status.DownloadStatus.Percent);
                    break;
                case CdnStatus.Status.Done:
                    progressUIContanier.SetActive(false);
                    OnSceneReady();
                    break;
            }
        }

        public void UpdateProgressBar(string description, float progress)
        {
            progressUIContanier.SetActive(true);
            this.description.text = description;
            cdnProgressBar.value = progress;
        }

        public void OnSceneReady()
        {
            if (CurrentCdnStatus != CdnStatus.Status.Done)
                return;
            
            if (!LoginDone)
                return;

            TransitionOverlay.Show(0);
            LoadScene(nextScene, LoadSceneMode.Single, overwriteSceneLayer: 0);
        }
    }
}