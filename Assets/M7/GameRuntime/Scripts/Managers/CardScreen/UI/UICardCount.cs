using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace M7.GameRuntime
{
    public class UICardCount : MonoBehaviour
    {
        [SerializeField] UIDelayValue teamCardCount;
        [SerializeField] UIDelayValue teamDiscardCount;
        [SerializeField] UIDelayValue opponentCardCount;
        [SerializeField] UIDelayValue opponentDiscardCount;

        [SerializeField] CanvasGroup uIDiscardPile;

        public void Init(float curTeamCardCount, float curTeamDiscardCount, float curOpponentCardCount, float curOpponentDiscardCount)
        {
            teamCardCount.SetValue(curTeamCardCount);
            teamDiscardCount.SetValue(curTeamDiscardCount);
            //opponentCardCount.SetValue(curOpponentCardCount);
            //opponentDiscardCount.SetValue(curOpponentDiscardCount);
        }

        public void DiscardPile(bool _isPlayerTurn)
        {
            //LeanTween.alphaCanvas(uIDiscardPile, _isPlayerTurn ? 1 : 0, 0.5f);
            uIDiscardPile.DOFade(_isPlayerTurn ? 1 : 0, 0.5f);
        }
    }
}