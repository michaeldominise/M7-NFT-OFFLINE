using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.Skill
{
    public abstract class StatusEffect : MonoBehaviour
    {
        [SerializeField] SkillDisplayStats displayStats;
        [SerializeField] protected SkillEnums.EventTrigger activationTrigger = SkillEnums.EventTrigger.Execute;
        [SerializeField] protected ExpirationData expiration;
        [SerializeField] protected StatusEffectInstance instancePrefab;
        [SerializeField] protected ConditionalData[] conditionals;
        [SerializeField, Range(0, 100)]
        protected int chance = 100;

        [SerializeField] private bool executeOnSpawn;
        [SerializeField] private bool isForced;

        [ShowInInspector, DisplayAsString(false)] public virtual string DebugText 
            => $"Activate effect on {activationTrigger}. "
               + (expiration.DoesNotExpire ? "DoesNotExpire. " : $"Expires on {expiration.Trigger} in {expiration.ExpireCount} counts. ");

        public SkillDisplayStats DisplayStats => displayStats;
        public StatusEffectInstance InstancePrefab => instancePrefab;
        public SkillEnums.EventTrigger ActivationTrigger => activationTrigger;
        public ExpirationData Expiration => expiration;
        public bool ExecuteOnSpawn => executeOnSpawn;
        public bool IsForced => isForced;

        public int Chance => chance;
        [ShowInInspector] public virtual float Value
        {
            get => 0;
#if UNITY_EDITOR
            set { }
#endif
        }
        public bool IsValid(ConditionalDataValues dataValues, Component caster, Component target) => conditionals.Length == 0 || conditionals.FirstOrDefault(x => x.IsValid(dataValues, caster, target)) != null;
        public virtual float ComputeIntervalPerTarget(int index, Component caster, Component target, float value) => index * value;
        public virtual void Execute(StatusEffectInstance statusEffectInstance, Action onFinish) => onFinish?.Invoke();
        public virtual StatusEffectData GenerateStatusEffectData(StatusEffectInstance statusEffectInstance) => new StatusEffectData();
    }
}