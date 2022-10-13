using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace M7.Skill
{
    public class StatusEffectValue_CharacterMatchBoardDamage : StatusEffectValue
    {
        [ShowInInspector, DisplayAsString(false)] public override string DebugText => $"Get Character's MatchBoardDamage.";

        public override float GetValue(StatusEffectInstance statusEffectInstance)
        {
            var characterInstance = statusEffectInstance.Caster as CharacterInstance_Battle;
            return characterInstance ? characterInstance.StatsInstanceBattle.Attack + characterInstance.StatsInstanceBattle.MatchBoardDamage : 0;
        }
    }
}