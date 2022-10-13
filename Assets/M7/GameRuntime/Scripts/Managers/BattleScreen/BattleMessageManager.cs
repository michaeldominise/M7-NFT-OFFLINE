using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using M7.GameData;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using M7.Skill;
using M7.FX;
using DG.Tweening;
using TMPro;

namespace M7.GameRuntime
{
    public class BattleMessageManager : MonoBehaviour
    {
        public static BattleMessageManager Instance => BattleManager.Instance?.BattleMessageManager;

        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] TextMeshProUGUI description;
        [SerializeField] float fadeInOutDuration = 0.2f;
        Coroutine showCoroutine;

        private void Awake()
        {
            canvasGroup.gameObject.SetActive(false);
            canvasGroup.alpha = 0;
        }

        public void Show(string text, float duration = 2)
        {
            if (showCoroutine != null)
                StopCoroutine(showCoroutine);
            showCoroutine = StartCoroutine(_Show(text, duration));
        }

        IEnumerator _Show(string text, float duration = 2)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.DOFade(1, fadeInOutDuration);
            description.text = text;
            if (duration < 0)
                yield break;

            yield return new WaitForSeconds(duration);
            Hide();
        }

        public void Hide()
        {
            canvasGroup.DOFade(0, fadeInOutDuration).onComplete += () => canvasGroup.gameObject.SetActive(false);
        }
    }
}

