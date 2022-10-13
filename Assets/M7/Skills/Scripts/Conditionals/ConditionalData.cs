using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;

namespace M7.Skill
{
    public abstract class ConditionalData : MonoBehaviour
    {
        public virtual bool IsValid(ConditionalDataValues dataValues, Component caster, Component target) => true;
    }
}