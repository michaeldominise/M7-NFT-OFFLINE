using System.Collections;
using System.Collections.Generic;
using Gamelogic.Grids;
using HutongGames.PlayMaker;
using UnityEngine;

namespace M7.Match.PlaymakerActions
{
    [ActionCategory("M7/Match")]
    public class ShuffleGrid : FsmStateAction
    {
        [RequiredField]
        public FsmFloat time;
        public bool realTime;

        private float startTime;
        private float timer;

        public override void OnEnter()
        {
            PuzzleBoardManager.Instance.RequestShuffle(0.5f);
            startTime = FsmTime.RealtimeSinceStartup;
            timer = 0f;
        }

        public override void OnUpdate()
        {
            // update time

            if (realTime)
            {
                timer = FsmTime.RealtimeSinceStartup - startTime;
            }
            else
            {
                timer += Time.deltaTime;
            }

            if (timer >= time.Value)
            {
                Finish();
            }
        }


#if UNITY_EDITOR

        public override float GetProgress()
        {
            return Mathf.Min(timer / time.Value, 1f);
        }

#endif
    }
}
