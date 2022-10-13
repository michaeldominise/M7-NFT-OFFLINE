using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace M7.Match
{
    [Serializable]
    public class OnTimerStateChange : UnityEvent<TimerBasedMovementManager.TimerType> { }

    public class TimerBasedMovementManager : MonoBehaviour
    {

        public static TimerBasedMovementManager Instance;
        public enum TimerType { TimerHold, TimerStart, TimerUpdate, TimerReset, TimerEnd }


        [Header("ComboTimerStates")]
        [SerializeField] ComboTimerState[] _states;

        [SerializeField] ComboTimerEnums.DecrementState _stateIndex;
        ComboTimerState _activeState
        {
            get
            {
                ComboTimerState toReturn = null;

                foreach (ComboTimerState cts in _states)
                    if (cts.timerStateType == _stateIndex)
                        toReturn = cts;

                return toReturn;
            }
        }

        float _activeDeductionRate { get { return _activeState.deductionRate; } }
        float _activeStartDuration { get { return _activeState.GetStartingValOverride + additionalStartDuration; } }
        float _activeMinVal { get { return _activeState.GetMinValOverride; } }
        public float additionalStartDuration { get; set; }

        [Header("Variables")]


        [Tooltip("Current duration starting value is the default duration and will be deducted by deduction treshhold every after move")]
        [ReadOnly]
        public float currentTimerDuration;

        [Tooltip("Reads from TimerBasedMovementAction FSM in Playmaker. Constantly changing.")]
        [ReadOnly]
        public float timerProgress;

        [Tooltip("Reads from TimerBasedMovementAction FSM in Playmaker. Constantly changing.")]
        [ReadOnly]
        public float timerRawValue;

        [Tooltip("Determines if player input has begun on their turn. ")]
        [ReadOnly]
        public bool hasTimerStartedThisTurn;


        public float timerRawRemaining
        {
            get
            {
                return currentTimerDuration - timerRawValue;
            }
        }

        public string timerRawRemaining_String
        {
            get
            {
                return (currentTimerDuration - timerRawValue).ToString("0.00");
            }
        }
        public float timerProgressRemaining
        {
            get
            {
                return 1 - timerProgress;
            }
        }


        public Action OnTimerBasedMovementStartAction; // used by the playmaker
        public OnTimerStateChange onTimerStateChange;
        private Coroutine startTimerCoroutine;
        public Action onTimerMidProgress;
        bool onTimerMidProgressDone;

        private void Awake()
        {
            Instance = this;
        }

        public void UpdateTimerValues(float progress, float raw)
        {
            timerRawValue = raw;
            timerProgress = progress;
            if (progress > 0.5 && !onTimerMidProgressDone)
            {
                onTimerMidProgress?.Invoke();
                onTimerMidProgressDone = true;
            }
        }

        public void StartTimer()
        {
            if (startTimerCoroutine != null)
                return;
            startTimerCoroutine = StartCoroutine(_StartTimer());
        }

        IEnumerator _StartTimer()
        {
            yield return new WaitWhile(() => OnTimerBasedMovementStartAction == null);
            OnTimerBasedMovementStartAction();
            startTimerCoroutine = null;
        }

        public void InitializeToDefault()
        {
            OnTimerBasedMovementStartAction = null;
            currentTimerDuration = _activeStartDuration;
        }

        public void DecreaseAndResetTimer()
        {
            onTimerMidProgressDone = false;
            currentTimerDuration = Mathf.Max(currentTimerDuration * (1 - _activeDeductionRate), _activeMinVal);
        }
        public void ResetTimer()
        {
            currentTimerDuration = _activeStartDuration;
            additionalStartDuration = 0;
            onTimerMidProgressDone = false;
            UpdateTimerValues(0, currentTimerDuration);
        }
    }
}
