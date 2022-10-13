using Gamelogic.Grids;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Match
{
    public class CellType_DamageCondition_OnLock : CellType_DamageCondition
    {
        public override bool CanDamage(DamageData damageData) => damageData.chain.Contains(damageData.targetCell) && base.CanDamage(damageData);

    }
}