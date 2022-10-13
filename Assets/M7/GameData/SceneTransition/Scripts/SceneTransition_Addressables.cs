using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using M7.CDN.Addressable;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
namespace M7
{
    public class SceneTransition_Addressables : SceneTransition
    {
        public SceneTransitionAddressableSettings addressableSettings;
        private Dictionary<string, AsyncOperationHandle<SceneInstance>> loadedAddressableScenes;

        protected override IEnumerator StartProcessScene(ProcessType processType, string sceneName, Settings settings, Action<float> progressCallback)
        {
            var addressableScene = addressableSettings.sceneList.FindAssetReference(sceneName);
            if (addressableScene != null)
            {
                Debug.Log($"[SceneTransition] StartProcessScene:Addressable: {sceneName}:{processType}" + (processType == ProcessType.Load ? $":{ settings.loadSceneMode}" : ""));
                AsyncOperationHandle<SceneInstance> sceneOp;
                if (processType == ProcessType.Load)
                {
                    sceneOp = Addressables.LoadSceneAsync(sceneName);
                    sceneOp.Completed += handle =>
                    {
                        if (handle.Status != AsyncOperationStatus.Succeeded)
                            return;

                        handle.Result.ActivateAsync();
                        loadedAddressableScenes[sceneName] = handle;
                    };
                }
                else
                    sceneOp = Addressables.UnloadSceneAsync(loadedAddressableScenes[sceneName]);


                if (processType == ProcessType.Load)
                    sceneOp.Completed += asyncOp =>
                    {
                        if (settings.loadSceneMode != LoadSceneMode.Single)
                            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
                    };

                while (!sceneOp.IsDone)
                {
                    progressCallback?.Invoke(sceneOp.PercentComplete);
                    yield return new WaitForEndOfFrame();
                }
            }
            else
                yield return base.StartProcessScene(processType, sceneName, settings, progressCallback);
        }
    }
}
