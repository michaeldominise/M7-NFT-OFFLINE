using Gamelogic.Grids;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.Skill;

namespace M7.Match
{
    public class CellType_DamageCondition_OnLock_GridLocation : CellType_DamageCondition
    {
        [SerializeField] SkillEnums.ElementFilter elementType;
        [SerializeField] SkillEnums.CellGridLocation gridLocation;

        public override bool CanDamage(DamageData damageData)
        {
            var cell = gridLocation == SkillEnums.CellGridLocation.Main ? damageData.mainCell : damageData.secondaryCell;
            return cell != null && elementType.HasFlag(cell.CellTypeContainer.CellType.ElementType) && damageData.chain.Contains(cell) && base.CanDamage(damageData);
        }
    }
}