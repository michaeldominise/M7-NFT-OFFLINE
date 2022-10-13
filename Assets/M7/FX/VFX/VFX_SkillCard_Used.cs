using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using M7.GameRuntime;

public class VFX_SkillCard_Used : MonoBehaviour
{
    [SerializeField] Transform collectEmber;
    [SerializeField] Transform discardPile;
    [SerializeField] CanvasGroup card;
    [SerializeField] float collectEmberDuration = 0.5f;
    [SerializeField] float collectEmberDelay = 0.2f;

    public float CollectEmberDuration => collectEmberDuration;

    public void PlayVfx(Vector3 targetPos, float duration, System.Action onFinish)
    {
        gameObject.SetActive(true);
        card.alpha = 0;
        card.DOFade(1, 0.2f);
        collectEmber.gameObject.SetActive(TurnManager.Instance.CurrentState == TurnManager.State.PlayerTurn);
        collectEmber.position = targetPos;
        collectEmber.DOMove(discardPile.transform.position + Vector3.back * 0.1f, collectEmberDuration).SetDelay(duration + collectEmberDelay).onComplete += 
            () =>
            {
                onFinish?.Invoke();
                gameObject.SetActive(false);
            };
    }
}
