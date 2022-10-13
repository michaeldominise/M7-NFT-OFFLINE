using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Skill
{
    [Serializable]
    public class ExpirationData
    {
        [SerializeField] int expireCount = 0;
        [SerializeField] SkillEnums.EventTrigger trigger = SkillEnums.EventTrigger.Execute;

        public int ExpireCount => expireCount;
        [ShowInInspector] public bool DoesNotExpire { get => expireCount < 0; private set => expireCount = -1; }
        [ShowIf("@!DoesNotExpire")] public SkillEnums.EventTrigger Trigger => trigger;
    }
}