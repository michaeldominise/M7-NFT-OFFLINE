using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameData;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace M7.Skill
{
    public class StatusEffectValue_ResetValueTeam : StatusEffectValue_ResetValue<SkillEnums.TargetTeamStats>
    {
        enum InputType { Custom, InitialTeamSkillPoints }

        [SerializeField] InputType inputType;

        [ShowInInspector] public override float InputValue
        {
            get
            {
                switch (inputType)
                {
                    case InputType.Custom:
                        return inputValue;
                    case InputType.InitialTeamSkillPoints:
                        return BattleSceneSettings.Instance?.InitialTeamSkillPoints ?? 0;
                }

                return 0;
            }
#if UNITY_EDITOR
            set
            {
                switch (inputType)
                {
                    case InputType.Custom:
                         inputValue = value;
                        break;
                    case InputType.InitialTeamSkillPoints:
                        break;
                }
            }
#endif
        }
    }
}