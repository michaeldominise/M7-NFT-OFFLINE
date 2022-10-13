using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.GameRuntime.Scripts.OnBoarding.Game
{
    [Serializable]
    public abstract class OnBoardingSegmentBase: MonoBehaviour, IOnBoardingSegment
    {
        [SerializeField] private bool isDone;

        public bool IsDone => isDone;
        
        public virtual void Execute()
        {
            isDone = true;
        }

        public virtual void Exit()
        {
        }
    }
}