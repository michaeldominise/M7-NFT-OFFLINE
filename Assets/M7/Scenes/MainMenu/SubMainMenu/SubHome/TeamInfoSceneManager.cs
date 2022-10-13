using M7.CDN.Addressable;
using M7.GameData;
using System.Collections;
using System.Collections.Generic;
using M7.GameRuntime.Scripts.Energy;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using M7.ServerTestScripts;
using M7.GameRuntime;

namespace M7
{
    public class TeamInfoSceneManager : SceneManagerBase
    {
        public static TeamInfoSceneManager Instance { get; private set; }

        [SerializeField] AssetReference battleScene;
        
        #if UNITY_EDITOR
        [SerializeField] private bool isFree;
        #endif
        
        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        protected override void ExecuteButtonEvent(GameObject gameObject)
        {
            switch (gameObject.name)
            {
                case "Battle_Button":
                    if (!PlayerDatabase.CanPlayerBattle())
                        return;

                    DownloadDataRuntime.Instance.isLoadScene = true;
                    LoadNextSceneCallBack(DownloadDataRuntime.Instance.isLoadScene);
                    //DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.Energy_StartGame);
                    break;
                case "Close_Button":
                    UnloadAtSceneLayer(sceneLayer);
                    break;

            }
        }

        public void LoadNextSceneCallBack(bool execute)
        {
            if (execute)
            {
                TransitionOverlay.Show(0);
                BattleManager.onInitFinish += () => TransitionOverlay.Hide();
                LoadScene(battleScene, UnityEngine.SceneManagement.LoadSceneMode.Single, result =>
                {
                    if (result.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                        return;
                });
            }
        }
    }
}