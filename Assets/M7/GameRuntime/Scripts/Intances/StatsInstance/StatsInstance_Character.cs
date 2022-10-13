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
    public class StatsInstance_Character : StatsInstance<BaseCharacterInstance, InstanceActions_CharacterStats, SkillEnums.TargetCharacterStats>
    {
        protected float currentHp, maxHp, attack, defense, mining, luck, durability;

        [SerializeField] protected bool ignoreMultiplier;
        protected SaveableCharacterData SaveableCharacterData => TargetInstance?.SaveableCharacterData;
        [ShowInInspector] protected CharacterObject CharacterObject => TargetInstance?.CharacterObject;
        [ShowInInspector] public virtual float CurrentHp => GetValue(SkillEnums.TargetCharacterStats.CurrentHp);
        [ShowInInspector] public virtual float MaxHp => GetValue(SkillEnums.TargetCharacterStats.MaxHp);
        [ShowInInspector] public virtual float Attack => GetValue(SkillEnums.TargetCharacterStats.Attack);
        [ShowInInspector] public virtual float Defense => GetValue(SkillEnums.TargetCharacterStats.Defense);
        [ShowInInspector] public virtual float Passion => GetValue(SkillEnums.TargetCharacterStats.Passion);
        [ShowInInspector] public virtual float Luck => GetValue(SkillEnums.TargetCharacterStats.Luck);
        [ShowInInspector] public virtual float Durability => GetValue(SkillEnums.TargetCharacterStats.Durability);

        protected override List<StatusEffectInstance> StatusEffectInstanceLedger => null;

        public override float GetValue(SkillEnums.TargetCharacterStats statType, bool addLedgerValues = true) =>
            statType switch
            {
                SkillEnums.TargetCharacterStats.CurrentHp => !addLedgerValues ? currentHp : GetValueWithLedger(currentHp, statType),
                SkillEnums.TargetCharacterStats.MaxHp => !addLedgerValues ? maxHp : GetValueWithLedger(maxHp, statType),
                SkillEnums.TargetCharacterStats.Attack => !addLedgerValues ? attack : GetValueWithLedger(attack, statType),
                SkillEnums.TargetCharacterStats.Defense => !addLedgerValues ? defense : GetValueWithLedger(defense, statType),
                SkillEnums.TargetCharacterStats.Passion => !addLedgerValues ? mining : GetValueWithLedger(mining, statType),
                SkillEnums.TargetCharacterStats.Luck => !addLedgerValues ? luck : GetValueWithLedger(luck, statType),
                SkillEnums.TargetCharacterStats.Durability => !addLedgerValues ? durability : GetValueWithLedger(durability, statType),
                _ => 0
            };

        public override void SetValue(SkillEnums.TargetCharacterStats statType, float value)
        {
            switch (statType)
            {
                case SkillEnums.TargetCharacterStats.CurrentHp:
                    currentHp = Mathf.Clamp(value, 0, maxHp);
                    break;
                case SkillEnums.TargetCharacterStats.MaxHp:
                    maxHp = Mathf.Max(value, 0);
                    break;
                case SkillEnums.TargetCharacterStats.Attack:
                    attack = Mathf.Max(value, 0);
                    break;
                case SkillEnums.TargetCharacterStats.Defense:
                    defense = Mathf.Max(value, 0);
                    break;
                case SkillEnums.TargetCharacterStats.Passion:
                    mining = Mathf.Max(value, 0);
                    break;
                case SkillEnums.TargetCharacterStats.Luck:
                    luck = Mathf.Max(value, 0);
                    break;
                case SkillEnums.TargetCharacterStats.Durability:
                    durability = Mathf.Max(value, 0);
                    break;
            }
            base.SetValue(statType, value);
        }

        public override void InitStatValues()
        {
            currentHp = CharacterObject?.CombatStats.Hp ?? 0;
            attack = CharacterObject?.CombatStats.Attack ?? 0;
            defense = 0;
            mining = CharacterObject?.CombatStats.Passion ?? 0;
            luck = CharacterObject?.CombatStats.Luck ?? 0;
            durability = CharacterObject?.CombatStats.Durability ?? 0;

            TargetInstance.SaveableCharacterData.SaveableStats.AddValues(ref currentHp, ref attack, ref defense, ref mining, ref luck, ref durability);
            if (TargetInstance != null && TargetInstance.EquipmentItems != null)
                foreach (var equipmentItem in TargetInstance.EquipmentItems)
                    equipmentItem?.AdditionalStats.AddValues(ref currentHp, ref attack, ref defense, ref mining, ref luck, ref durability);

            if (!ignoreMultiplier)
            {
                currentHp *= CharacterObject?.CombatStats.HpModifier ?? 1;
                attack *= CharacterObject?.CombatStats.AttackModifier ?? 1;
                defense *= CharacterObject?.CombatStats.DefenseModifier ?? 1;
                mining *= CharacterObject?.CombatStats.MiningModifier ?? 1;
                luck *= CharacterObject?.CombatStats.LuckModifier ?? 1;
                durability *= CharacterObject?.CombatStats.DurabilityModifier ?? 1;
            }

            maxHp = currentHp;
            base.InitStatValues();
        }

        public override void UpdateInstanceActions()
        {
            InstanceActions.onCurrentHpUpdate?.Invoke(CurrentHp);
            InstanceActions.onHpPercentageUpdate?.Invoke(CurrentHp / MaxHp);
            InstanceActions.onAttackUpdate?.Invoke(Attack);
            InstanceActions.onDefenseUpdate?.Invoke(Defense);
            InstanceActions.onMiningUpdate?.Invoke(Passion);
            InstanceActions.onLuckUpdate?.Invoke(Luck);
            InstanceActions.onDurabilityUpdate?.Invoke(Durability);
        }
    }
}