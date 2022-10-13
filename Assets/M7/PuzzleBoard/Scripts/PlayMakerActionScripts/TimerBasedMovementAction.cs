using System.Collections.Generic;
using UnityEngine;

using HutongGames.PlayMaker;
using System;
using M7.GameRuntime;
using M7.Skill;
using System.Collections;
using M7.PuzzleBoard.Scripts.Booster;

namespace M7.Match.PlaymakerActions {

    [ActionCategory("M7/Match")]
    public class TimerBasedMovementAction : FsmStateAction { 
        public static TimerBasedMovementAction Instance { get; private set; }

        [RequiredField] public TimerBasedMovementManager timerBasedMovementData;
        public TimerBasedMovementManager.TimerType timerType;
        public bool realTime;
        public RectPointList outTouchedPoints;

        public FsmEvent onTimerEndEvent;
        public FsmEvent onTimerEndEventArena;
        public FsmEvent onActiveGridChanged;
        public FsmEvent onEndGameFinished;

        static float timer;
        static float startTime;

        #region Tutorial
        /// <summary>
        /// Needs to be reset to true everytime the gameplay scene starts.
        /// </summary>
        public static bool IsPaused = false;
        #endregion

        public override void OnEnter() {
            Instance = this;
            switch (timerType)
            {
                case TimerBasedMovementManager.TimerType.TimerHold:
                    PuzzleBoardManager.Instance.CurrentState = PuzzleBoardManager.State.Idle;
                    SkillQueueManager.Instance.WaitUntilIdle(() =>
                    {
                        if (BattleManager.Instance.IsGameDone)
                            return;
                        else if (BoosterManager.Instance != null && BoosterManager.Instance.isBoosterActive)
                            Finish();
                        else
                        {
                            timerBasedMovementData.InitializeToDefault();
                            timerBasedMovementData.OnTimerBasedMovementStartAction = Finish;
                        }
                    });
                    break;
                case TimerBasedMovementManager.TimerType.TimerStart:
                    if(BattleManager.Instance.IsGameDone)
                        goto default;

                    outTouchedPoints.Value.Clear();
                    AutoMatchAI.Instance.moveCount = 0;
                    PuzzleBoardManager.Instance.CurrentState = PuzzleBoardManager.State.WaitingForPlayerInput;
                    goto default;
                case TimerBasedMovementManager.TimerType.TimerUpdate:
                    PuzzleBoardManager.Instance.CurrentState = PuzzleBoardManager.State.TileSwipeStarted;
                    timer = 0;
                    startTime = FsmTime.RealtimeSinceStartup;
                    break;
                case TimerBasedMovementManager.TimerType.TimerReset:
                    timerBasedMovementData.DecreaseAndResetTimer();
                    goto default;
                case TimerBasedMovementManager.TimerType.TimerEnd:
                    PuzzleBoardManager.Instance.CurrentState = PuzzleBoardManager.State.EndState;
                    goto default;
                default:
                    Finish();
                    break;
            }
            timerBasedMovementData.onTimerStateChange?.Invoke(timerType);
        }

        public override void OnExit()
        {
            if (timerType == TimerBasedMovementManager.TimerType.TimerHold)
                timerBasedMovementData.OnTimerBasedMovementStartAction = null;
        }

        public override void OnUpdate()
        {
            if (timerType != TimerBasedMovementManager.TimerType.TimerUpdate)
                return;

            //if (Turn.HasWaitRequest)
            //    return;

            if (realTime)
                timer = FsmTime.RealtimeSinceStartup - startTime;
            else
                timer += IsPaused ? 0 : Time.deltaTime;

            if (timer >= timerBasedMovementData.currentTimerDuration)
            {
                Finish();
                if (onTimerEndEvent != null)
                    Fsm.Event(onTimerEndEvent);
            }

            TimerBasedMovementManager.Instance.UpdateTimerValues(Mathf.Min(timer / timerBasedMovementData.currentTimerDuration, 1f), timer);
        }

        public static void ResetTimer(bool isPaused = false)
        {
            timer = 0;
            startTime = FsmTime.RealtimeSinceStartup;
            IsPaused = isPaused;
        }

        public void ExecuteEndGameState() => Fsm.Event(onEndGameFinished);

#if UNITY_EDITOR

        public override float GetProgress()
        {
            return Mathf.Min(timer / timerBasedMovementData.currentTimerDuration, 1f);
        }

#endif
    }
}

