using System;
using System.Collections.Generic;
using M7.Match;
using UnityEngine;
using Random = UnityEngine.Random;

namespace M7.Skill
{
    [Serializable]
    public class StatusEffect_RandomSkill : StatusEffect 
    { 
        [SerializeField] List<SkillObject> skillObjects;

        public SkillObject GetSkill()
        {
            return skillObjects[Random.Range(0, skillObjects.Count)];
        }
    }
}