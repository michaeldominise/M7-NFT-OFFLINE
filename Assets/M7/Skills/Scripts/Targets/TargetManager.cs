using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace M7.Skill
{
    public abstract class TargetManager<TargetFilterType, TargetFilterItemType, TargetType> : TargetManager where TargetFilterType : TargetFilter<TargetFilterItemType, TargetType> where TargetFilterItemType : TargetFilterItem<TargetType> where TargetType : Component
    {
        [SerializeField] TargetFilterType filter;
        [SerializeField] TargetSorter<TargetType> sorter;

        public override bool IsValidTarget(Component caster, Component target) => filter.IsValidTarget(caster, target as TargetType);

        public override List<Component> GetTargets<CasterType>(CasterType caster, bool ignoreTargetCount = false)
        {
            var castedInitialTargets = new List<TargetType>();
            var targets = filter.GetTargets(caster, castedInitialTargets);

            if (targets.Count == 0)
                return targets.ToList<Component>();

            var sortedTargets = sorter.SortTargets(caster, targets);
            return (ignoreTargetCount ? sortedTargets : sortedTargets.Take(TargetCount)).ToList<Component>();
        }
    }

    public abstract class TargetManager : MonoBehaviour
    {
        [SerializeField, HideInInspector] protected int targetCount = 1;
        [SerializeField] bool validateOnSkillDataExecute = true;
        [ShowInInspector] public virtual int TargetCount { get => targetCount; private set => targetCount = value; }
        public bool ValidateOnSkillDataExecute => validateOnSkillDataExecute;
        public abstract bool IsValidTarget(Component caster, Component target);
        public abstract List<Component> GetTargets<CasterType>(CasterType caster, bool ignoreTargetCount = false) where CasterType : Component;
    }
}