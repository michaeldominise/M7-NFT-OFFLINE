using UnityEngine;
using System;
using Newtonsoft.Json;

namespace M7.GameData
{
    [Serializable]
    public class GoalData
    {
        public enum GoalType { Cube, Cube_2, Cube_3}

        [JsonProperty] [SerializeField] GoalType goalType = GoalType.Cube;
        [JsonProperty] [SerializeField] Sprite displayIcon;
        [JsonProperty] [SerializeField] int goalCount;

        [JsonIgnore] public GoalType CurGoalType => goalType;
        [JsonIgnore] public Sprite DisplayIcon => displayIcon;
        [JsonIgnore] public int GoalCount => goalCount;
    }
}
