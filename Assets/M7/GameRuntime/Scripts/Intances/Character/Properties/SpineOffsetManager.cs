using M7.Skill;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace M7.GameRuntime
{
    [System.Serializable]
    public class SpineOffsetManager : MonoBehaviour
    {
        [SerializeField] SkillVfxOffset[] skillVfxOffsetList;
        [SerializeField] Transform bodyOffset;

        public SkillVfxOffset[] SkillVfxOffsetList => skillVfxOffsetList;
        public Transform BodyOffset => bodyOffset;

        public Transform GetVfxOffsetTransform(SkillEnums.SkillAnimationType skillAnimationType)
        {
            if (SkillVfxOffsetList == null || SkillVfxOffsetList.Length == 0)
                return transform;

            return SkillVfxOffsetList.FirstOrDefault(x => x.skillAnimationType == skillAnimationType)?.offset ?? transform;
        }
    }
}
