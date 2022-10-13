using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;

namespace M7.Skill
{
    public class TargetFilterItemCustom_CharacterStatsInstanceCanAttack : TargetFilterItemCustom<CharacterInstance_Battle>
    {
        [SerializeField] bool canAttack = true;
        public override string DebugText => $"Can attack is {canAttack}";
        public override bool IsValidTarget(Component caster, CharacterInstance_Battle target) => target.StatsInstanceBattle.CanAttackStatusEffect == canAttack;
    }
}