using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace M7.Skill
{
    public class StatusEffectValue_FixValue : StatusEffectValue
    {
        enum ValueType { FixValue, FloatMinValue, FloatMaxValue }

        [HorizontalGroup, SerializeField] float inputValue;
        [HorizontalGroup(width: 100), ShowInInspector, HideLabel] ValueType SetValue
        {
            get
            {
                switch(inputValue)
                {
                    case float.MinValue:
                        return ValueType.FloatMinValue;
                    case float.MaxValue:
                        return ValueType.FloatMaxValue;
                    default:
                        return ValueType.FixValue;
                }

            }
            set
            {
                switch (value)
                {
                    case ValueType.FloatMinValue:
                        inputValue = float.MinValue;
                        break;
                    case ValueType.FloatMaxValue:
                        inputValue = float.MaxValue;
                        break;
                    case ValueType.FixValue:
                        inputValue = 0;
                        break;
                }

            }
        }

        [ShowInInspector, DisplayAsString(false)] public override string DebugText => $"Get {inputValue}";

        public override float InputValue
        {
            get => inputValue;
#if UNITY_EDITOR
            set
            {
                base.InputValue = value;
                inputValue = value;
            }
#endif
        }

        public override float GetValue(StatusEffectInstance statusEffectInstance) => inputValue;
    }
}