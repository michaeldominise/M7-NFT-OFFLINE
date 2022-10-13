using M7.GameRuntime;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace M7.GameData.CharacterSkill
{
    public abstract class CharacterSkillCondition : ScriptableObject
    {
        [SerializeField] protected float requiredPoints = 5;
        public float RequiredPoints => requiredPoints;

        public abstract void RegisterAddPoints(CharacterInstance_Battle targetReference, Action<float> onAddPointsAction);
    }
}
