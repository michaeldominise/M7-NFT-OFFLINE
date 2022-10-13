using M7.GameRuntime;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace M7.Skill
{
    [Serializable]
    public class StatusEffect_CastSkill : StatusEffect
    {

        public enum CasterReferenceType { Caster, Target};
        [SerializeField] CasterReferenceType casterReference;

        [SerializeField] bool waitUntilSkillDone;
        [SerializeField] bool forceExecuteSkill;

        [SerializeField] SkillObject[] skillObjects;
        [ShowInInspector, DisplayAsString(false)] public override string DebugText
            => base.DebugText 
            + $"Cast {SkillObjects.Length} SkillObjects";

        public SkillObject[] SkillObjects => skillObjects;
        public CasterReferenceType CasterReference => casterReference;
        public bool WaitUntilSkillDone => waitUntilSkillDone;
        public bool ForceExecuteSkill => forceExecuteSkill;

        public override void Execute(StatusEffectInstance statusEffectInstance, Action onFinish)
        {
            var castSkill = statusEffectInstance.StatusEffect as StatusEffect_CastSkill;
            var targetCaster = castSkill.CasterReference == CasterReferenceType.Caster ? statusEffectInstance.Caster : statusEffectInstance.Target;
            var skillObject = castSkill.SkillObjects[UnityEngine.Random.Range(0, castSkill.SkillObjects.Length)];
            skillObject.GetTargetManagerData(targetCaster, targetManagerData => SkillManager.ExecuteSkill(targetCaster, skillObject, null, targetManagerData, castSkill.WaitUntilSkillDone ? onFinish : null));

            if (!castSkill.WaitUntilSkillDone)
                base.Execute(statusEffectInstance, onFinish);
        }
    }
}