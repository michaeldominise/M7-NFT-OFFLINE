using Gamelogic.Grids;
using System.Linq;
using M7.Match.PlaymakerActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.Skill;

namespace M7.Match
{
    public class CellType_DamageCondition_OnNearbyLock_LightTube : CellType_DamageCondition
    {
        [SerializeField] Vector2Int minDistance = Vector2Int.one;

        public override bool CanDamage(DamageData damageData)
        {
            if (!base.CanDamage(damageData))
                return false;

            for(var x = -minDistance.x; x < minDistance.x; x++)
                for (var y = -minDistance.y; y < minDistance.y; y++)
                {
                    var rectPointRef = damageData.targetCell.CurrentRectPoint + new RectPoint(x, y);
                    var tileLightTube = damageData.targetCell as MatchGridCell_LightTube;
                    if (!tileLightTube)
                        return false;
                    if (damageData.chain.Contains(damageData.targetCell) && tileLightTube.Damage(PuzzleBoardManager.Instance.ActiveGrid.Grid[rectPointRef].CellTypeContainer.CellType.ElementType))
                        return true;
                }

            return false;
        }
    }
}