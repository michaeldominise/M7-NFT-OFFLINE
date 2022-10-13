using M7.GameData;
using M7.Skill;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace M7.GameRuntime
{
    [Serializable]
    public class StatsInstance_CharacterBattle : StatsInstance_Character
    {
        class StatusValueData
        {
            public CharacterInstance_Battle caster;
            public string statusEffectId;
            public float value;
            public bool isReady;
            public UIStatusValueManager.DamageType damageType;

            public StatusValueData(CharacterInstance_Battle caster, string skillId, float value, UIStatusValueManager.DamageType damageType)
            {
                this.caster = caster;
                this.statusEffectId = skillId;
                this.value = value;
                this.damageType = damageType;
            }

            public void SetIsReady()
            {
                isReady = true;
            }
        }
        [ShowInInspector] public virtual float MatchBoardDamage => GetValue(SkillEnums.TargetCharacterStats.MatchBoardDamage);
        [ShowInInspector] public virtual float DamageReduction => GetValue(SkillEnums.TargetCharacterStats.DamageReduction);
        [ShowInInspector] public virtual bool IsStunned => GetValue(SkillEnums.TargetCharacterStats.Stun) > 0;

        List<StatusValueData> statusValueDataList = new List<StatusValueData>();

        protected override List<StatusEffectInstance> StatusEffectInstanceLedger => TargetInstance_Battle?.StatusEffectInstanceLedger;
        public CharacterInstance_Battle TargetInstance_Battle => TargetInstance as CharacterInstance_Battle;
        public bool CanAttackStatusEffect => !IsStunned;

        public Action<float> onTakenHit;
        float matchBoardDamage, damageReduction, stunStatusEffectCount;


        public virtual bool CanAttack() => CanAttackStatusEffect;
        public override float GetValue(SkillEnums.TargetCharacterStats statType, bool addLedgerValues = true) =>
            statType switch
            {
                SkillEnums.TargetCharacterStats.MatchBoardDamage => !addLedgerValues ? matchBoardDamage : GetValueWithLedger(matchBoardDamage, statType),
                SkillEnums.TargetCharacterStats.DamageReduction => !addLedgerValues ? damageReduction : GetValueWithLedger(damageReduction, statType),
                SkillEnums.TargetCharacterStats.Stun => !addLedgerValues ? stunStatusEffectCount : GetValueWithLedger(stunStatusEffectCount, statType),
                _ => base.GetValue(statType, addLedgerValues)
            };

        public override void AddValue(SkillEnums.TargetCharacterStats statType, float value)
        {
            switch (statType)
            {
                case SkillEnums.TargetCharacterStats.CurrentHp:
                    if (value < 0)
                    {
                        var reducedDamage = value - (value * damageReduction);
                        var damage = Mathf.Min(defense + reducedDamage, 0);
                        SetValue(SkillEnums.TargetCharacterStats.Defense, GetValue(SkillEnums.TargetCharacterStats.Defense, false) + value);
                        value = damage;
                    }
                    goto default;
                default:
                    base.AddValue(statType, value);
                    break;
            }
        }

        public override void SetValue(SkillEnums.TargetCharacterStats statType, float value)
        {
            switch (statType)
            {
                case SkillEnums.TargetCharacterStats.MatchBoardDamage:
                    matchBoardDamage = Mathf.Max(value, 0);
                    break;
                case SkillEnums.TargetCharacterStats.DamageReduction:
                    damageReduction = Mathf.Max(value, 0);
                    break;
                case SkillEnums.TargetCharacterStats.Stun:
                    stunStatusEffectCount = Mathf.Max(value, 0);
                    break;
            }
            base.SetValue(statType, value);
        }

        public override void InitStatValues()
        {
            matchBoardDamage = 0;
            damageReduction = 0;
            base.InitStatValues();
        }

        internal void DamageThis(CharacterInstance_Battle caster, string skillId, float value, UIStatusValueManager.DamageType damageType)
        {
            value = value * (1 + DamageReduction);
            AddValue(SkillEnums.TargetCharacterStats.CurrentHp, value);
            statusValueDataList.Add(new StatusValueData(caster, skillId, value, damageType));

            caster.OnSkillActionTigger(SkillEnums.EventTrigger.OnAttack, null);
            (TargetInstance as CharacterInstance_Battle).OnSkillActionTigger(SkillEnums.EventTrigger.OnDefend, null);
            if (TargetInstance_Battle.IsAlive)
                onTakenHit?.Invoke(value);
        }
        internal void HealThis(CharacterInstance_Battle caster, string skillId, float value, UIStatusValueManager.DamageType damageType)
        {
            statusValueDataList.Add(new StatusValueData(caster, skillId, value, damageType));

            if (TargetInstance_Battle.StatsInstance.CurrentHp < TargetInstance_Battle.StatsInstance.MaxHp)
                AddValue(SkillEnums.TargetCharacterStats.CurrentHp, value);
        }

        public override void UpdateInstanceActions()
        {
            InstanceActions.onMatchBoardDamageUpdate?.Invoke(MatchBoardDamage);
            InstanceActions.onDamageReductionUpdate?.Invoke(DamageReduction);
            base.UpdateInstanceActions();
            SkillManager.Instance.StartCoroutine(UpdateUIStatusValueManager());
        }

        public IEnumerator UpdateUIStatusValueManager ()
        {
            var statusValueDataListCopy = new List<StatusValueData>(statusValueDataList);

            for (int i = 0; i < statusValueDataListCopy.Count; i++)
            {
                StatusValueData statusValueData = statusValueDataListCopy[i];
                if (!statusValueData.isReady)
                    continue;

                statusValueDataList.Remove(statusValueData);
                if (statusValueData.value < 0)
                {
                    if (!TargetInstance_Battle.IsAlive && i == statusValueDataListCopy.Count - 1)
                        TargetInstance_Battle.Kill();
                    else
                        TargetInstance_Battle.SetTriggerAnimation("TakenHit");
                }

                UIStatusValueManager.Instance.Play(TargetInstance_Battle.GetVfxOffsetBody().position, statusValueData.value, statusValueData.damageType, statusValueData.caster?.Element, statusValueData.value > 0);
                yield return new WaitForSeconds (0.6f);
            }
        }

        public void SetStatusValueDataReady(string statusEffectId) => statusValueDataList.FirstOrDefault(x => x.statusEffectId == statusEffectId)?.SetIsReady();
    }
}
