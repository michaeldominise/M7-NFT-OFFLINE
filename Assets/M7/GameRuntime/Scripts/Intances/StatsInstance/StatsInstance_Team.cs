using M7.GameData;
using M7.Skill;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    [System.Serializable]
    public class StatsInstance_Team : StatsInstance<TeamManager_Battle, InstanceActions_TeamStats, SkillEnums.TargetTeamStats>
    {
        float skillPoints;
        protected override List<StatusEffectInstance> StatusEffectInstanceLedger => TargetInstance?.StatusEffectInstanceLedger;
        [ShowInInspector] public virtual float SkillPoints => GetValue(SkillEnums.TargetTeamStats.SkillPoints);

        public override float GetValue(SkillEnums.TargetTeamStats statType, bool addLedgerValues = true) =>
            statType switch
            {
                SkillEnums.TargetTeamStats.SkillPoints => addLedgerValues ? skillPoints : GetValueWithLedger(skillPoints, statType),
                _ => 0
            };

        public override void SetValue(SkillEnums.TargetTeamStats statType, float value)
        {
            switch (statType)
            {
                case SkillEnums.TargetTeamStats.SkillPoints:
                    skillPoints = Mathf.Max(value, 0);
                    break;
            }
            base.SetValue(statType, value);
        }

        public override void UpdateInstanceActions()
        {
            InstanceActions.onSkillPointsUpdate?.Invoke(SkillPoints);
        }

        public override void OnStatusEffectInstanceLedgerUpdate(StatusEffectInstance updatedStatusEffectInstance, IStatusEffectInstanceController.UpdateType updateType)
        {
            var statusEffect = updatedStatusEffectInstance.StatusEffect as StatusEffect_TeamStats;
            if (statusEffect == null)
                return;

            switch (statusEffect.TargetStats)
            {
                case SkillEnums.TargetTeamStats.SkillPoints:
                    break;
            }

            base.OnStatusEffectInstanceLedgerUpdate(updatedStatusEffectInstance, updateType);
        }
    }
}
