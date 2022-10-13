using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace M7
{
    public class SceneTransition : MonoBehaviour
    {

        static SceneTransition _Instance;
        public static SceneTransition Instance
        {
            get
            {
                _Instance = _Instance ?? Instantiate(Resources.Load<SceneTransition>("SceneTransition"));
                return _Instance;
            }
        }

        public static WaitProcess startWaitProcess;
        public static WaitProcess finishWaitProcess;

        public enum ProcessType { Load, Unload }
        public enum Status { Ready, Busy }
        public enum ProcessSceneTaskType { StartWaitTask, MainProcessTask, FinishWaitTask }
        public int ProcessSceneTaskCount => Enum.GetNames(typeof(ProcessSceneTaskType)).Length;
        enum TransitionType { FadeIn, FadeOut }

        [SerializeField] Animator fadeInOutTransition;
        [SerializeField] OnProgressUpdate onProgressUpdate;
        [SerializeField] private GameObject PrefabLoadingPrefab;
        private PreloadBattleScript currentLoadingPrefab;
        public Status currentStatus { get; private set; }

        void Awake()
        {
            Application.backgroundLoadingPriority = ThreadPriority.High;
            _Instance = this;
            DontDestroyOnLoad(this);
        }
        

        public static void LoadScene(string sceneName, Settings settings = default, Callbacks callbacks = default)
        {
            if (SceneManager.GetSceneByName(sceneName) != null || settings.reloadScene)
                Instance.StartCoroutine(Instance.ProcessScene(ProcessType.Load, sceneName, settings, callbacks));
        }

        public static void UnloadScene(string sceneName, Settings settings = default, Callbacks callbacks = default)
        {
            if (SceneManager.GetSceneByName(sceneName) != null)
                Instance.StartCoroutine(Instance.ProcessScene(ProcessType.Unload, sceneName, settings, callbacks));
        }

        public void loadPreloadScreen()
        {
            var PreloadGameObject = Instantiate(PrefabLoadingPrefab);
            currentLoadingPrefab=PreloadGameObject.GetComponent<PreloadBattleScript>();
            DontDestroyOnLoad(currentLoadingPrefab);
        }

        protected virtual IEnumerator ProcessScene(ProcessType processType, string sceneName, Settings settings, Callbacks callbacks)
        {
            Debug.Log($"[SceneTransition] {processType}: {sceneName}");
            var startTime = Time.time;
            Instance.currentStatus = Status.Busy;

            // yield return startWaitProcess.Start(progress => InvokeProgressUpdate(progress, ProcessSceneTaskType.StartWaitTask));
            if (currentLoadingPrefab != null) yield return (currentLoadingPrefab.FadeIn());
            else yield return StartFadeTransition(TransitionType.FadeIn, settings);
            
            TransitionStart(callbacks);
         //   yield return new WaitForSeconds(5);// this is Just a dummy Delay to test LoadingSCreen
            yield return StartProcessScene(processType, sceneName, settings, progress => InvokeProgressUpdate(progress, ProcessSceneTaskType.MainProcessTask));
            yield return UnloadUnusuedAssets(settings);
            // yield return finishWaitProcess.Start(progress => InvokeProgressUpdate(progress, ProcessSceneTaskType.FinishWaitTask));
            TransitionEnd(callbacks);
            if (currentLoadingPrefab == null) 
                yield return StartFadeTransition(TransitionType.FadeOut, settings);
            Debug.Log($"[SceneTransition] {processType}: {sceneName}:LoadTime:{Time.time - startTime}s");
        }
        

        protected virtual IEnumerator StartProcessScene(ProcessType processType, string sceneName, Settings settings, Action<float> progressCallback)
        {
            Debug.Log($"[SceneTransition] StartProcessScene: {sceneName}:{processType}" + (processType == ProcessType.Load ? $":{ settings.loadSceneMode}" : ""));
            var sceneOp = processType == ProcessType.Load ? SceneManager.LoadSceneAsync(sceneName, settings.loadSceneMode) : SceneManager.UnloadSceneAsync(sceneName);

            if (processType == ProcessType.Load)
                sceneOp.completed += asyncOp =>
                {
                    if (settings.loadSceneMode != LoadSceneMode.Single)
                        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
                };

            while (sceneOp != null && !sceneOp.isDone)
            {
                progressCallback?.Invoke(sceneOp.progress);
                yield return new WaitForEndOfFrame();
            }
        }

        void TransitionStart(Callbacks callbacks)
        {
            onProgressUpdate?.Invoke(0);
            callbacks.onStartTransition?.Invoke();
        }

        void TransitionEnd(Callbacks callbacks)
        {
            Instance.currentStatus = Status.Ready;
            callbacks.onFinishTransition?.Invoke();
        }

        IEnumerator StartFadeTransition(TransitionType transitionType, Settings settings)
        {
            Debug.Log($"[SceneTransition] {transitionType}:{settings.fadeDuration}");
            if (transitionType == TransitionType.FadeIn)
                fadeInOutTransition.gameObject.SetActive(settings.fadeDuration != 0);

            if (settings.fadeDuration > 0)
            {
                fadeInOutTransition.speed = 1 / settings.fadeDuration;
                fadeInOutTransition.SetTrigger(transitionType == TransitionType.FadeIn ? "FadeIn" : "FadeOut");

                yield return new WaitForSeconds(settings.fadeDuration);
            }
            
            if (transitionType == TransitionType.FadeOut)
                fadeInOutTransition.gameObject.SetActive(false);
        }

        IEnumerator UnloadUnusuedAssets(Settings settings)
        {
            Debug.Log($"[SceneTransition] UnloadUnusuedAssets");
            if (settings.loadSceneMode != LoadSceneMode.Single)
                yield return Resources.UnloadUnusedAssets();
        }

        public void InvokeProgressUpdate(float taskProgress, ProcessSceneTaskType processSceneTaskType)
        {
            onProgressUpdate?.Invoke((taskProgress + (int)processSceneTaskType) / ProcessSceneTaskCount);
        }    

        public struct Settings
        {
            public LoadSceneMode loadSceneMode;
            public float fadeDuration;
            public bool reloadScene;

            public Settings(LoadSceneMode loadSceneMode = LoadSceneMode.Single, float fadeDuration = 0.2f, bool reloadScene = false)
            {
                this.loadSceneMode = loadSceneMode;
                this.fadeDuration = fadeDuration;
                this.reloadScene = reloadScene;
            }
        }

        public struct Callbacks
        {
            public Action onStartTransition;
            public Action onFinishTransition;

            public Callbacks(Action onStartTransition = null, Action onFinishTransition = null)
            {
                this.onStartTransition = onStartTransition;
                this.onFinishTransition = onFinishTransition;
            }
        }
        [Serializable] public class OnProgressUpdate : UnityEvent<float> { };

        public class WaitProcess : List<IEnumerator>
        {
            public static WaitProcess operator +(WaitProcess a, IEnumerator b)
            {
                a.Add(b);
                return a;
            }

            public static WaitProcess operator -(WaitProcess a, IEnumerator b)
            {
                a.Remove(b);
                return a;
            }

            public IEnumerator Start(Action<float> onTaskFinish)
            {
                WaitProcess list = this;
                for (int i = 0; i < list.Count; i++)
                {
                    IEnumerator process = list[i];
                    if (process == null)
                        continue;
                    yield return process;
                    onTaskFinish?.Invoke((i+1)/this.Count);
                }
                onTaskFinish?.Invoke(1);
            }
        }
    }
}
