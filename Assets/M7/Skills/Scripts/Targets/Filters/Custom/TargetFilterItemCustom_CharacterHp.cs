using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;

namespace M7.Skill
{
    public class TargetFilterItemCustom_CharacterHp : TargetFilterItemCustom<CharacterInstance_Battle>
    {
        enum ConditionType { GreaterThan, LessThan, Equal, GreaterThanOrEqual, LessThanOrEqual }

        [SerializeField] float hpPercentage;
        [SerializeField] ConditionType condition;
        public override string DebugText => $"Hp is {condition} by {hpPercentage * 100}%";
        public override bool IsValidTarget(Component caster, CharacterInstance_Battle target)
        {
            var isValid = true;

            switch (condition)
            {
                case ConditionType.GreaterThan:
                    isValid = target.StatsInstance.CurrentHp / target.StatsInstance.MaxHp > hpPercentage;
                    break;
                case ConditionType.LessThan:
                    isValid = target.StatsInstance.CurrentHp / target.StatsInstance.MaxHp < hpPercentage;
                    break;
                case ConditionType.Equal:
                    isValid = target.StatsInstance.CurrentHp / target.StatsInstance.MaxHp == hpPercentage;
                    break;
                case ConditionType.GreaterThanOrEqual:
                    isValid = target.StatsInstance.CurrentHp / target.StatsInstance.MaxHp >= hpPercentage;
                    break;
                case ConditionType.LessThanOrEqual:
                    isValid = target.StatsInstance.CurrentHp / target.StatsInstance.MaxHp <= hpPercentage;
                    break;
            }

            return isValid;
        }
    }
}