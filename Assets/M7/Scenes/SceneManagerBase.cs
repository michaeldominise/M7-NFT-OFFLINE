using M7.CDN.Addressable;
using M7.GameData;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using M7.GameRuntime;

namespace M7
{
    public class SceneManagerBase : MonoBehaviour
    {
        protected static Dictionary<int, AssetReference> LoadedAdditiveScenes = new Dictionary<int, AssetReference>();
        static Action<int> onActiveLayerChanged;
        [ShowInInspector, ReadOnly] static List<int> backButtonSceneLayerList = new List<int>();
        [ShowInInspector, ReadOnly] static bool IsReady { get => GameManager.IsInteractable; set => GameManager.IsInteractable = value; }

        [SerializeField] protected int sceneLayer;
        [SerializeField] bool registerBackButton;
        static float lastBackButtonPressed;
        static float backButtonPressInterval = 0.25f;

        protected virtual void Awake() => StartCoroutine(AddToBackbuttonSceneLayerList());

        protected virtual void OnEnable() => onActiveLayerChanged += OnActiveLayerChanged;
        protected virtual void OnDisable() => onActiveLayerChanged -= OnActiveLayerChanged;

        IEnumerator AddToBackbuttonSceneLayerList()
        {
            yield return null;
            if (registerBackButton)
                backButtonSceneLayerList.Add(sceneLayer);
        }

        protected virtual void ExecuteButtonEvent(GameObject gameObject) { }
        public void ExecuteButton(GameObject gameObject) 
        {
            if (!IsReady)
                return;

            ExecuteButtonEvent(gameObject);
        }

        protected virtual void ExecuteToggleEvent(Toggle toggle) { }
        public void ExecuteToggle(Toggle toggle)
        {
            if (!IsReady)
            {
                var onValuesChanged = toggle.onValueChanged;
                toggle.onValueChanged = new Toggle.ToggleEvent();
                toggle.isOn = !toggle.isOn;
                toggle.onValueChanged = onValuesChanged;
                return;
            }

            ExecuteToggleEvent(toggle);
        }

        protected virtual void ExecuteInputEvent(TMP_InputField input) { }
        public void ExecuteInput(TMP_InputField input)
        {
            if (!IsReady)
                return;

            ExecuteInputEvent(input);
        }

        protected virtual void OnActiveLayerChanged(int activeSceneLayer) { }

        public virtual void LoadScene(AssetReference sceneToLoad, LoadSceneMode loadSceneMode = LoadSceneMode.Single, Action<AsyncOperationHandle<SceneInstance>> asyncOperation = null, int? overwriteSceneLayer = null, bool forceLoad = false)
            => StartCoroutine(_LoadScene(sceneToLoad, loadSceneMode, asyncOperation, overwriteSceneLayer, forceLoad));

        IEnumerator _LoadScene(AssetReference sceneToLoad, LoadSceneMode loadSceneMode = LoadSceneMode.Single, Action<AsyncOperationHandle<SceneInstance>> asyncOperation = null, int? overwriteSceneLayer = null, bool forceLoad = false)
        {
            if ((!forceLoad && !IsReady) || string.IsNullOrWhiteSpace(sceneToLoad.AssetGUID))
            {
                asyncOperation?.Invoke(default);
                yield return null;
            }

            IsReady = false;
            var targetSceneLayer = loadSceneMode == LoadSceneMode.Single ? 0 : (overwriteSceneLayer ?? (sceneLayer + 1));
            if (loadSceneMode == LoadSceneMode.Single)
                OnReset();

            yield return Addressables.DownloadDependenciesAsync(sceneToLoad);
            var asyncOp = sceneToLoad.LoadSceneAsync(loadSceneMode);
            asyncOp.Completed += result =>
            {
                IsReady = true;
                if (loadSceneMode == LoadSceneMode.Additive)
                    UnloadAtSceneLayer(targetSceneLayer, false);
                if (result.Status == AsyncOperationStatus.Succeeded)
                {
                    LoadedAdditiveScenes[targetSceneLayer] = sceneToLoad;
                    onActiveLayerChanged?.Invoke(targetSceneLayer);
                }
            };
            asyncOperation?.Invoke(asyncOp);
        }

        public static void OnReset()
        {
            LoadedAdditiveScenes.Clear();
            backButtonSceneLayerList.Clear();
        }

        public static void UnloadAtSceneLayer(int sceneLayer, bool executeActiveLayerChanged = true)
        {
            var LoadedAdditiveScenesCopy = new Dictionary<int, AssetReference>();
            foreach (var LoadedAdditiveScene in LoadedAdditiveScenes)
                LoadedAdditiveScenesCopy[LoadedAdditiveScene.Key] = LoadedAdditiveScene.Value;
            foreach (var LoadedAdditiveScene in LoadedAdditiveScenesCopy)
            {
                if (LoadedAdditiveScene.Key >= sceneLayer && (LoadedAdditiveScene.Value?.IsValid() ?? false))
                {
                    LoadedAdditiveScene.Value?.UnLoadScene();
                    LoadedAdditiveScenes[LoadedAdditiveScene.Key] = null;
                }
            }

            backButtonSceneLayerList.RemoveAll(x => x >= sceneLayer);
  
            if (executeActiveLayerChanged && LoadedAdditiveScenes.Count > 0)
                onActiveLayerChanged?.Invoke(LoadedAdditiveScenes.GetSortedItems().Where(x => x.Value?.IsValid() ?? false).LastOrDefault().Key);
        }

        public static void BackButtonPressed()
        {
            if (lastBackButtonPressed + backButtonPressInterval > Time.time || backButtonSceneLayerList.Count == 0 || MessageBox.HasActiveMessage)
                return;

            lastBackButtonPressed = Time.time + backButtonPressInterval;

            var lastSceneLayer = backButtonSceneLayerList[backButtonSceneLayerList.Count - 1];
            if (lastSceneLayer == 0)
                MessageBox.Create("Are you sure you want to exit?", MessageBox.ButtonType.Ok_No, okLabel: "Confirm", noLabel: "Cancel", onResult: result =>
                {
                    if (result == MessageBox.ButtonType.Ok)
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                }).Show();
            else
                UnloadAtSceneLayer(lastSceneLayer);  
        }

    }
}