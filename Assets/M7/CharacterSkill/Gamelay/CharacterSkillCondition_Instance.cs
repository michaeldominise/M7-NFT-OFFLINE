using M7.GameRuntime;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace M7.GameData.CharacterSkill
{
    [Serializable]
    public class CharacterSkillCondition_Instance
    {
        [ShowInInspector, ReadOnly] CharacterSkillCondition CharacterSkillCondition { get; set; }
        CharacterSkillData CharacterSkillData { get; set; }

        [ShowInInspector] float CurrentPoints { get; set; }
        [ShowInInspector] float RequiredPoints => CharacterSkillCondition?.RequiredPoints ?? 0;
        public float CurrentProgress =>  Mathf.Min(CurrentPoints / RequiredPoints, 1);
        public bool IsReady => CurrentProgress == 1;

        public CharacterSkillCondition_Instance(CharacterSkillData characterSkillData, CharacterSkillCondition characterSkillCondition)
        {
            CharacterSkillData = characterSkillData;
            CharacterSkillCondition = characterSkillCondition;
            CharacterSkillCondition.RegisterAddPoints(characterSkillData.TargetReference, AddPoints);
        }

        public void AddPoints(float value)
        {
            if (!CharacterSkillData.TargetReference.IsAlive)
                return;
            CurrentPoints = Mathf.Min(CurrentPoints + value, RequiredPoints);
            CharacterSkillData.CheckState();
        }

        public void Reset() => CurrentPoints = 0;
    }
}
