using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using M7.Match;

namespace M7.Skill
{
    [Serializable]
    public class TargetFilter<TargetFilterItem, TargetType> where TargetFilterItem : TargetFilterItem<TargetType> where TargetType : Component
    {
        [SerializeField] TargetFilterItem filter;
        [SerializeField] TargetFilterItem[] fallbackfilters;

        [ShowInInspector, DisplayAsString(false)]
        public virtual string DebugText
            => $"{filter.DebugText}"
            + (fallbackfilters.Length > 0 ? $", or if none {fallbackfilters.Select(x => x.DebugText).Aggregate((a, b) => $"{a}, or if none {b}")}" : "");

        public bool IsValidTarget(Component caster, TargetType target)
        {
            if (filter.IsValidTarget(caster, target))
                return true;
            foreach (var fallbackfilter in fallbackfilters)
                if (!fallbackfilter.IsValidTarget(caster, target))
                    return true;

            return false;
        }

        public virtual List<TargetType> GetTargets<CasterType>(CasterType caster, IEnumerable<TargetType> initialTargets) where CasterType : Component
        {
            var targets = filter.GetTargets(caster, initialTargets);

            for (var x = 0; x < fallbackfilters.Length && targets.Count == 0; x++)
                if(targets == null || targets.Count == 0)
                    targets = fallbackfilters[x].GetTargets(caster, initialTargets);
            
            return filter.GetTargetFilterCustomItemList(targets, caster);
        }
    }

    [Serializable]
    public class TargetFilter_Character : TargetFilter<TargetFilterItem_CharacterInstanceBattle, CharacterInstance_Battle> { }
    [Serializable]
    public class TargetFilter_Team : TargetFilter<TargetFilterItem_Team, TeamManager_Battle> { }
    [Serializable]
    public class TargetFilter_MatchGridTile : TargetFilter<TargetFilterItem_MatchGridTile, MatchGridCell> { }
}