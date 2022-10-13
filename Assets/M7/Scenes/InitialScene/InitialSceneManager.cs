using M7.CDN;
using M7.GameRuntime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace M7
{
    public class InitialSceneManager : SceneManagerBase
    {
        [SerializeField] AssetReference nextScene;
        [SerializeField] TextMeshProUGUI progress;
        [SerializeField] GameObject loadingIcon;

        private string labelName = "InitialScene";

        private void Start() => DownloadNextScene();

        void DownloadNextScene()
        {
            StartCoroutine(CDNDownloader.Execute(labelName, OnStatusUpdate));
        }

        private void OnStatusUpdate(CdnStatus status)
        {
            switch (status.CurrenStatus)
            {
                case CdnStatus.Status.Downloading:
                    progress.text = $"{status.DownloadStatus.Percent * 100 : 0}%";
                    break;
                case CdnStatus.Status.Done:
                    LoadScene(nextScene, asyncOperation: result => StartCoroutine(ReadLoadSceneProgress(result)), overwriteSceneLayer: 0);
                    break;
            }
        }

        IEnumerator ReadLoadSceneProgress(AsyncOperationHandle asyncOperation)
        {
            while (!asyncOperation.IsDone)
            {
                progress.text = $"{asyncOperation.PercentComplete * 100 : 0}%";
                yield return null;
            }
        }
    }
}