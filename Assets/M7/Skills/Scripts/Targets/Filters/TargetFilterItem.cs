using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using M7.FX;

namespace M7.Skill
{
    [Serializable]
    public abstract class TargetFilterItem<TargetType> where TargetType : Component
    {
        [SerializeField, PropertyOrder(1)] protected List<TargetFilterItemCustom<TargetType>> targetFilterItemCustomList;

        public virtual string DebugText => (targetFilterItemCustomList != null && targetFilterItemCustomList.Count > 0) ? targetFilterItemCustomList.Select(x => $", {x?.DebugText ?? "Null"}").Aggregate((a, b) => $"{a}{b}") : "";
        public virtual bool IsValidTarget(Component caster, TargetType target) 
        {
            if (target == null)
                return false;
            return true;
        }

        public virtual List<TargetType> GetTargetFilterCustomItemList(List<TargetType> targetTypeList, Component caster)
        {
            if (targetFilterItemCustomList.Count == 0)
                return targetTypeList;
            
            var localtargetTypeList = new List<TargetType>();
            for (int i = 0; i < targetFilterItemCustomList.Count; i++)
            {
                TargetFilterItemCustom<TargetType> targetFilterItemCustom = targetFilterItemCustomList[i];
                var tempTypeList = targetTypeList.Where(target =>
                    targetFilterItemCustom.IsValidTarget(caster, target)).ToList();
                if (i == 0)
                    localtargetTypeList = tempTypeList;

                else
                    localtargetTypeList = targetFilterItemCustom.JoinType switch
                    {
                        TargetFilterItemCustom<TargetType>.Join.Append => localtargetTypeList.Union(tempTypeList).ToList(),
                        TargetFilterItemCustom<TargetType>.Join.Filter => localtargetTypeList.Intersect(tempTypeList)
                            .ToList(),
                        _ => localtargetTypeList
                    };
            }

            return localtargetTypeList;
        }

        public abstract List<TargetType> GetTargets<CasterType>(CasterType caster, IEnumerable<TargetType> initialTargets) where CasterType : Component;
    }
}