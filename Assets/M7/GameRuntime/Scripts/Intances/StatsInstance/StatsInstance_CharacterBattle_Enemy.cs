using M7.GameData;
using M7.Skill;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    [Serializable]
    public class StatsInstance_CharacterBattle_Enemy : StatsInstance_CharacterBattle
    {
        float attackTurn;
        [ShowInInspector] public virtual float AttackTurn => GetValue(SkillEnums.TargetCharacterStats.AttackTurn);
        [ShowInInspector] public float AttackTurnPublic => AttackTurn;

        public override bool CanAttack() => (attackTurn == 1 || TargetInstance_Battle.CharacterSkillData.IsReady) && base.CanAttack();

        public override float GetValue(SkillEnums.TargetCharacterStats statType, bool addLedgerValues = true) =>
            statType switch
            {
                SkillEnums.TargetCharacterStats.AttackTurn => addLedgerValues ? attackTurn : GetValueWithLedger(attackTurn, statType),
                _ => base.GetValue(statType, addLedgerValues)
            };

        public override void SetValue(SkillEnums.TargetCharacterStats statType, float value)
        {
            switch (statType)
            {
                case SkillEnums.TargetCharacterStats.AttackTurn:
                    var maxAttackTurn = (SaveableCharacterData as SaveableCharacterData_Enemy)?.AttackTurn ?? 1;
                    attackTurn = maxAttackTurn == 1 ? 1 : (Mathf.Repeat(value - 1, maxAttackTurn) + 1);
                    InstanceActions?.onAttackTurnUpdate?.Invoke(attackTurn);
                    break;
            }
            base.SetValue(statType, value);
        }

        public override void InitStatValues()
        {
            attackTurn = Mathf.Max((SaveableCharacterData as SaveableCharacterData_Enemy)?.AttackTurn ?? 1, 1);
            InstanceActions?.onAttackTurnUpdate?.Invoke(AttackTurn);
            base.InitStatValues();
        }
    }
}
