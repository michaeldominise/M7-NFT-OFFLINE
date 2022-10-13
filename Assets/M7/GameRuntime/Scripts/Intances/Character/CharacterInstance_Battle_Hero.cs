using System.Collections.Generic;
using UnityEngine;

    #if UNITY_EDITOR
using DG.Tweening;
using M7.Tools.Scripts;
    #endif

using M7.GameData;
using Sirenix.OdinInspector;
using M7.Skill;
using System.Collections;
using System;

namespace M7.GameRuntime
{
    public class CharacterInstance_Battle_Hero : CharacterInstance_Battle
    {
        [SerializeField] StatsInstance_CharacterBattle statsInstance;
        public override StatsInstance_Character StatsInstance => statsInstance;
    }
}
