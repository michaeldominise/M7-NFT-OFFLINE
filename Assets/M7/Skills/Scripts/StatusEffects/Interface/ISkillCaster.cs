using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Skill
{
    public interface ISkillCaster
    {
        [Flags]
        public enum SkillState
        {
            Idle = 1 << 0, 
            Executing = 1 << 1,
            Executed = 1 << 2
        }
        
        public IEnumerator OnPreSkillCasted(SkillObject skillObject, Func<List<Component>> getTargets);
        public IEnumerator OnNewCasterSkillCasted(SkillObject skillObject);
        public SkillState CurrenSkillState { get; set; } 
        public GameObject GetGameObject();
    }
}