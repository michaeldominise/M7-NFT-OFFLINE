using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameData;
using M7.GameRuntime;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace M7.Skill
{
    public class StatusEffectValue_ResetValueTeamByRound : StatusEffectValue_ResetValue<SkillEnums.TargetTeamStats>
    {
        enum InputType { Custom, InitialTeamSkillPoints, ByRound }

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
                    case InputType.ByRound:

                        return BattleManager.Instance?.BattleData.manaRoundCount ?? 0;
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