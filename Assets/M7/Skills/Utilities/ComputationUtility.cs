using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Skill
{
    public static class ComputationUtility
    {
        public static float Calculate(this SkillEnums.ComputationType computationType, float value, float inputValue)
        {
            var input = inputValue;
            switch (computationType)
            {
                case SkillEnums.ComputationType.Multiply:
                    value *= input;
                    break;
                case SkillEnums.ComputationType.Increase:
                    value += input;
                    break;
                case SkillEnums.ComputationType.Decrease:
                    value -= input;
                    break;
                case SkillEnums.ComputationType.IncreaseMultiply:
                    value *= 1 + input;
                    break;
                case SkillEnums.ComputationType.DecreaseMultiply:
                    value *= 1 - input;
                    break;
            }

            return value;
        }
    }
}