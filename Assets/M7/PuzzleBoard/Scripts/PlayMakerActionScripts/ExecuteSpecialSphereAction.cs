using System.Collections.Generic;
using UnityEngine;

using HutongGames.PlayMaker;
using System;
using Gamelogic.Grids;
using M7.PuzzleBoard.Scripts.Booster;

namespace M7.Match.PlaymakerActions
{

    [ActionCategory("M7/Match")]
    public class ExecuteSpecialSphereAction : FsmStateAction
    {
        static ExecuteSpecialSphereAction Instance;
        public FsmEvent onExecuteEvent;
        Action onFinish;

        public override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        public override void OnEnter() => OnFinish();

        public static void OnExecute(Action onFinish)
        {
            Instance.onFinish = onFinish;
            Instance.Fsm.Event(Instance.onExecuteEvent);
        }

        public static void OnFinish()
        {
            Instance.onFinish?.Invoke();
            Instance.onFinish = null;
        }
    }
}

