using M7.GameData;
using M7.Skill;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime
{
    [System.Serializable]
    public class StatsInstance_CharacterBasic : StatsInstance_Character
    {
        protected override List<StatusEffectInstance> StatusEffectInstanceLedger => null;
    }
}
