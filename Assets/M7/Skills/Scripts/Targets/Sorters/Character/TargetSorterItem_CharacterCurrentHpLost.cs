using M7.GameRuntime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Skill
{
    public class TargetSorterItem_CharacterCurrentHpLost : TargetSorterItem<CharacterInstance_Battle>
    {
        public override object SortValue<CasterType>(CasterType caster, CharacterInstance_Battle target)
            => target.StatsInstance.MaxHp - target.StatsInstance.CurrentHp;
    }
}