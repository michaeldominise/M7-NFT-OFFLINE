using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


namespace M7.Match
{
    [Serializable]
    public class GoalData
    {
        public CellType tileType;
        public int qty;
    }

    public class LevelGoalsData : SerializedScriptableObject
    {
        [DisableInPlayMode, ShowInInspector, TableList(AlwaysExpanded = true)]
        public GoalData[] goals;
    }

}
