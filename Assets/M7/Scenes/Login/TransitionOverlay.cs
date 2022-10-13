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
using Sirenix.OdinInspector;

namespace M7
{
    public class TransitionOverlay : MonoBehaviour
    {
        public static TransitionOverlay Instance => GameManager.Instance.TransitionOverlay;

        [SerializeField] TextMeshProUGUI description;
        [SerializeField] Slider progressBar;
        [SerializeField] GameObject loadingIcon;
        [SerializeField] CanvasGroup container;

        int isShowCount;
        public bool IsShow => isShowCount > 0;

        [Button]
        public static void Show(float fadeDuration = 1, Action onFinish = null)
        {
            if (!Instance)
                return;
            Instance.container.blocksRaycasts = true;
            Instance.container.interactable = true;
            Instance.container.DOFade(1, fadeDuration).onComplete += () =>
            {
                Instance.description.gameObject.SetActive(true);
                Instance.loadingIcon.SetActive(true);
                Instance.isShowCount++;
                onFinish?.Invoke();
            };
        }

        [Button]
        public static void Hide(float fadeDuration = 0.25f, Action onFinish = null)
        {
            if (!Instance)
                return;
            Instance.description.text = "";
            Instance.description.gameObject.SetActive(false);
            Instance.container.DOFade(0, fadeDuration).onComplete += () =>
            {
                onFinish?.Invoke();
                Instance.container.blocksRaycasts = false;
                Instance.container.interactable = false;
                Instance.isShowCount--;
            };
        }

        private void Awake()
        {
            container.alpha = 0;
            container.blocksRaycasts = false;
            container.interactable = false;
            loadingIcon.SetActive(false);
            description.gameObject.SetActive(false);
        }
    }
}