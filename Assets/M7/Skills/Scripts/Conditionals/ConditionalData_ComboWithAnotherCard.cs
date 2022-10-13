using M7.GameRuntime;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Skill
{
    public class ConditionalData_ComboWithAnotherCard : ConditionalData
    {
        public override bool IsValid(ConditionalDataValues dataValues, Component caster, Component target)
        {
            return dataValues.isComboedWithAnotherCard;
        }
    }
}