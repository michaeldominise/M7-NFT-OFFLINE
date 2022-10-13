using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace M7.GameRuntime
{
    public class AIBehaviour : MonoBehaviour
    {
        public enum AIDifficulty { None, Predictable, UnPredictable }
        public AIDifficulty CurrentAIDifficulty;

        public enum behavior { do_nothing, do_action }
        public behavior aiBehavior;

        int roundCount;

        public List<string> predictablePattern;
        public List<string> unpredictablePattern;
        //public unpredictablePatternList unpredictablePattern = new unpredictablePatternList();

        public void SetPattern()
        {
            switch (CurrentAIDifficulty)
            {
                case AIDifficulty.None:
                    aiBehavior = behavior.do_nothing;
                    break;
                case AIDifficulty.Predictable:

                    if(roundCount >= predictablePattern.Count)
                    {
                        roundCount = 1;
                    }
                    else
                    {
                        roundCount++;
                    }
                    Debug.Log(roundCount + " :ROUND COUNT");
                    aiBehavior = (behavior)Enum.Parse(typeof(behavior), predictablePattern[roundCount - 1]);
                    

                    break;
                case AIDifficulty.UnPredictable:
                    aiBehavior = (behavior)Enum.Parse(typeof(behavior), unpredictablePattern[UnityEngine.Random.Range(0, unpredictablePattern.Count)]);
                    break;
            }
        }

        //[System.Serializable]
        //public class unpredictableIndexPatternList
        //{
        //    public List<string> behavior;
        //}

        //[System.Serializable]
        //public class unpredictablePatternList
        //{
        //    public List<unpredictableIndexPatternList> pattern;
        //}
    }
}