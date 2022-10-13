using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameData;

namespace M7.Skill
{
    public abstract class TargetSorterItem<TargetType> : MonoBehaviour, ITargetSorter where TargetType : Component
    {
        [SerializeField] SortUtility.SortData.SortType sortType;
        public SortUtility.SortData.SortType SortType => sortType;

        public abstract object SortValue<CasterType>(CasterType caster, TargetType target) where CasterType : Component;
    }
}