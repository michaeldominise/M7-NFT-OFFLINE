using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using M7.GameData;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace M7.GameRuntime
{
    public class CountdownManager : MonoBehaviour
    {
        public static CountdownManager Instance => BattleManager.Instance?.CountdownManager;

        public enum State { Start, Stop }

        [ShowInInspector] public State CurrentState { get; private set; }
        [SerializeField] public UIValue onTimerUpdate;

        [SerializeField] UnityEvent onCountdownFinish;
        [SerializeField] UnityEvent<float> onCountdownUpdate;
        Coroutine timerCoroutine;

        public void Init()
        {
            onCountdownFinish.AddListener(() => BattleManager.Instance.SetGameState(BattleManager.GameState.EndTurn));
            onTimerUpdate.gameObject.SetActive(false);
            SetState(State.Stop);
        }

        [Button]
        public void SetState(State state)
        {
            CurrentState = state;
            switch (state)
            {
                case State.Start:
                    if (timerCoroutine != null)
                        StopCoroutine(timerCoroutine);
                    timerCoroutine = StartCoroutine(StartTimer());
                    break;
                case State.Stop:
                    onTimerUpdate.gameObject.SetActive(false);
                    if (timerCoroutine != null)
                        StopCoroutine(timerCoroutine);
                    break;
            }
        }

        IEnumerator StartTimer()
        {
            var countdownValue = 0;
            onTimerUpdate.gameObject.SetActive(true);
            switch (LevelManager.GameMode)
            {
                case LevelData.GameModeType.Adventure:
                    countdownValue = BattleManager.Instance.BattleSettings.AdventureTurnCoundownDuration;
                    break;
                case LevelData.GameModeType.PVP:
                    countdownValue = BattleManager.Instance.BattleSettings.PvpTurnCoundownDuration;
                    break;
                default:
                    yield break;
            }

            while (countdownValue > 0 && CurrentState != State.Stop)
            {
                onTimerUpdate.SetValue(countdownValue);
                yield return new WaitForSeconds(1);
                countdownValue--;
                onCountdownUpdate.Invoke(countdownValue);
            }

            onTimerUpdate.gameObject.SetActive(false);
            timerCoroutine = null;
            onCountdownFinish?.Invoke(); 
        }
    }
}

