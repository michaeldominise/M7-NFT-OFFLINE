using System;
using Newtonsoft.Json;
using UnityEngine;

namespace M7.GameRuntime.Scripts.OnBoarding.Game
{
    [Serializable]
    public class OnBoardingSegmentsDictionary: UnitySerializedDictionary<string, OnBoardingSegmentBase> { }

    [Serializable]
    public class PuzzleBoardStages: UnitySerializedDictionary<string, PuzzleGameOnBoarding> { }

    [Serializable]
    public class PuzzleGameOnBoarding
    {
        [JsonIgnore][SerializeField] private OnBoardingSegmentsDictionary _segmentsDictionary;
        [JsonProperty] [SerializeField] private bool isDone;
        
        [JsonIgnore] public bool IsDone
        {
            get => isDone;
            set => isDone = value;
        }

        [JsonIgnore]
        public OnBoardingSegmentsDictionary SegmentsDictionary => _segmentsDictionary;

        public OnBoardingSegmentBase GetSegment(string key)
        {
            return _segmentsDictionary.ContainsKey(key) ? _segmentsDictionary[key] : null;
        }
    }
}