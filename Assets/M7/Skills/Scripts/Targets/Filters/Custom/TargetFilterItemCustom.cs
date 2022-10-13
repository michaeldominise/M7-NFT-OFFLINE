using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using UnityEngine.Serialization;

namespace M7.Skill
{
    public class TargetFilterItemCustom<TargetType> : MonoBehaviour, ITargetFilter where TargetType : Component
    {
        public enum Join
        {
            Filter, Append
        }

        [SerializeField] private Join joinType;

        public Join JoinType => joinType;
        
        public virtual string DebugText => $"{name}";
        public virtual bool IsValidTarget(Component caster, TargetType target) => true;
    }
}