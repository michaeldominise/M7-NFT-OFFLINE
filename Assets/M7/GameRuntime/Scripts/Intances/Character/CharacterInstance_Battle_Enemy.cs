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
    [RequireComponent(typeof(Collider2D))]
    public class CharacterInstance_Battle_Enemy : CharacterInstance_Battle
    {
        [SerializeField] StatsInstance_CharacterBattle_Enemy statsInstance;
        public override StatsInstance_Character StatsInstance => statsInstance;
    }
}
