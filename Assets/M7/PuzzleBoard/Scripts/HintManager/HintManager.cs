using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace M7.Match
{
    [Serializable]
    public class OnRandomPossibleMove : UnityEvent<PossibleMove> {}

    public class HintManager : MonoBehaviour
    {
        public PossibleMoveList possibleMoveList;
        [Title("This will execute if the user is idle after the countdownDuration", Bold = false, HorizontalLine = false)]
        public OnRandomPossibleMove onRandomPossibleMove;
        Coroutine countdown;
        float intervalDuration = 5;
        public bool ShowHint { get; set; } = true;

        public void StartCountdown(float countdownDuration)
        {
            countdown = StartCoroutine(_StartCountdown(countdownDuration));
        }

        public void StopCountdown()
        {
            StopCoroutine(countdown);
        }

        IEnumerator _StartCountdown(float countdownDuration)
        {
            while (ShowHint)
            {
                yield return new WaitForSeconds(countdownDuration);
                if (possibleMoveList.Value.Count > 0)
                {
                    var rnd = UnityEngine.Random.Range(0, possibleMoveList.Value.Count);
                    onRandomPossibleMove.Invoke(possibleMoveList.Value[rnd]);   
                }
                yield return new WaitForSeconds(intervalDuration);
            }
        }
    }
}