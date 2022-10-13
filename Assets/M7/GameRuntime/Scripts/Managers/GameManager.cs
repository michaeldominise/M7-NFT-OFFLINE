using M7.GameData;
using M7.Match;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using M7.ServerTestScripts;
using M7.PlayfabCloudscript.ServerTime;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
using System;

namespace M7.GameRuntime
{
    public class GameManager : SceneManagerBase
    {
        static GameManager _Instance;
        public static GameManager Instance
        {
            get
            {
                _Instance = _Instance ?? Instantiate(Resources.Load<GameManager>("GameManager"));
                return _Instance;
            }
        } 
            
        static int IsInteractableCount { get; set; }
        [ShowInInspector, ReadOnly] public static bool IsInteractable { get => IsInteractableCount == 0; set { IsInteractableCount = value ? (IsInteractableCount - 1) : (IsInteractableCount + 1); } }

        [SerializeField] TextAsset bundleVersionText;
        [SerializeField] AddressableAssetDisposeManager addressableAssetDisposeManager;
        [SerializeField] WindowEventManager windowEventLedger;
        [SerializeField] PlayerDatabase playerDatabase;
        [SerializeField] GlobalDatabase globalDatabase;
        [SerializeField] DownloadDataRuntime downloadDataRuntime;
        [SerializeField] TransitionOverlay transitionOverlay;
        [SerializeField] AssetReference loadingScene;
        [SerializeField] float saveableDataRefreshInterval = 0.1f;
        [SerializeField] AddHeroNftManager addHeroNftManager;

        public TextAsset BundleVersionText => bundleVersionText;
        public AddressableAssetDisposeManager AddressableAssetDisposeManager => addressableAssetDisposeManager;
        public WindowEventManager WindowEventManager => windowEventLedger;
        public PlayerDatabase PlayerDatabase => playerDatabase;
        public GlobalDatabase GlobalDatabase => globalDatabase;
        public DownloadDataRuntime DownloadDataRuntime => downloadDataRuntime;
        public TransitionOverlay TransitionOverlay => transitionOverlay;
        public AddHeroNftManager AddHeroNftManager => addHeroNftManager;

        protected override void Awake()
        {
            if (_Instance != null && _Instance != this)
                Destroy(gameObject);
            else
                Init();
            base.Awake();
        }

        void Init()
        {
            DontDestroyOnLoad(gameObject);
            StartCoroutine(StartSaveableDataClean());
            Application.targetFrameRate = 60;
        }

        public void InitAudio()
        {
            if (PlayerPrefs.GetInt(Settings.SettingsAPI.MUSIC_PREF_KEY, 1) == 1)
            {
                Settings.SettingsAPI.MusicEnabled = true;
            }
            else
            {
                Settings.SettingsAPI.MusicEnabled = false;
            }

            if (PlayerPrefs.GetInt(Settings.SettingsAPI.SOUND_PREF_KEY, 1) == 1)
            {
                Settings.SettingsAPI.SoundsEnabled = true;
            }
            else
            {
                Settings.SettingsAPI.SoundsEnabled = false;
            }
        }

        IEnumerator StartSaveableDataClean()
        {
            while (true)
            {
                try { DirtyData.CleanDirtyList(); }
                catch (Exception) { }
                yield return new WaitForSeconds(saveableDataRefreshInterval);
            }
        }

        public static void RestartGameDialog(bool showNo = true)
        {
            MessageBox.Create("Game will restart.", showNo ? MessageBox.ButtonType.Ok_No : MessageBox.ButtonType.Ok, onResult: result =>
            {
                if (result == MessageBox.ButtonType.Ok)
                {
                    PlayerDatabase.SaveToLocal();
                    Instance.LoadScene(Instance.loadingScene, overwriteSceneLayer: 0, forceLoad: true);
                }
            }).Show();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                BackButtonPressed();
        }

        void OnApplicationPause() => PlayerDatabase.SaveToLocal();
        void OnApplicationQuit() => PlayerDatabase.SaveToLocal();
    }
}