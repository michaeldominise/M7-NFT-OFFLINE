using M7.GameRuntime;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Skill
{
    public class TargetSorterItem_TeamAliveCharacterCount : TargetSorterItem<TeamManager_Battle>
    {
        [SerializeField] SkillEnums.HpStatusFilter hpStatusFilter = SkillEnums.HpStatusFilter.Alive;
        public override object SortValue<CasterType>(CasterType caster, TeamManager_Battle target)
            => target.ActiveCharacters.Where(x =>
            {
                if ((hpStatusFilter & SkillEnums.HpStatusFilter.Alive) != 0)
                    return x.IsAlive;
                if ((hpStatusFilter & SkillEnums.HpStatusFilter.Dead) != 0)
                    return !x.IsAlive;
                return false;
            })?.Count() ?? 0;
    }
}