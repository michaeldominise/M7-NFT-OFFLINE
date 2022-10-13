using M7.GameRuntime;
using M7.Match;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace M7.Skill
{
    [Serializable]
    public class StatusEffect_TileGeneration : StatusEffect 
    {
        [SerializeField] CellType tileType;
        public CellType TileType => tileType;
    }	
}