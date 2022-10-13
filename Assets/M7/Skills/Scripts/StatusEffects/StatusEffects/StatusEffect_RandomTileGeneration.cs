using System;
using System.Collections.Generic;
using M7.Match;
using UnityEngine;
using Random = UnityEngine.Random;

namespace M7.Skill
{
    [Serializable]
    public class StatusEffect_RandomTileGeneration : StatusEffect 
    { 
        [SerializeField] List<CellType> tileTypes;

        public CellType GetTile() => tileTypes[Random.Range(0, tileTypes.Count)];

        public override void Execute(StatusEffectInstance statusEffectInstance, Action onFinish)
        {
            var targetTile = statusEffectInstance.Target as MatchGridCell;

            targetTile.Initialize(GetTile());

            if (ExecuteOnSpawn)
                targetTile.ExecuteSkill(IsForced);

            base.Execute(statusEffectInstance, onFinish);
        }
    }
}