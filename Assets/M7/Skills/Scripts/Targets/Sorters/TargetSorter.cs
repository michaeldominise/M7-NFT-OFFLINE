using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameData;
using System.Linq;
using Sirenix.OdinInspector;

namespace M7.Skill
{
    [System.Serializable]
    public class TargetSorter<TargetType> where TargetType : Component
    {
        [SerializeField] List<TargetSorterItem<TargetType>> sorterItems;
        [SerializeField] bool randomizeLastSort = true;
        [ShowInInspector, DisplayAsString(false)] public virtual string DebugText 
            => (sorterItems.Count > 0 ? $"Sort by {sorterItems.Select(x => x != null ? $"{x.name}: {x.SortType}" : "")?.Aggregate((a, b) => $"{a}, then by {b}")} " : "")
            + (randomizeLastSort ? "then randomize last sort" : "");

        public virtual IEnumerable<TargetType> SortTargets<CasterType>(CasterType caster, IEnumerable<TargetType> targets) where CasterType : Component
        {
            IOrderedEnumerable<TargetType> sortedItems = null;
            if (sorterItems.Count > 0)
            {
                sortedItems = sorterItems[0].SortType == SortUtility.SortData.SortType.Accending
                    ? targets.OrderBy(target => sorterItems[0].SortValue(caster, target))
                    : targets.OrderByDescending(target => sorterItems[0].SortValue(caster, target));
                for (var x = 1; x < sorterItems.Count; x++)
                {
                    var sortIndex = x;
                    sortedItems = sorterItems[sortIndex].SortType == SortUtility.SortData.SortType.Accending
                        ? sortedItems.ThenBy(target => sorterItems[sortIndex].SortValue(caster, target))
                        : sortedItems.ThenByDescending(target => sorterItems[sortIndex].SortValue(caster, target));
                }
                if (randomizeLastSort)
                    sortedItems = sortedItems.ThenBy(target => Random.value);
            }
            else if (randomizeLastSort)
                sortedItems = targets.OrderBy(target => Random.value);
            else
                return targets;

            return sortedItems;
        }
    }
}