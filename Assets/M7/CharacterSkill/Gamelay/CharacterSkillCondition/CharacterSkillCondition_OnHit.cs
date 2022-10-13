using M7.GameRuntime;
using M7.Match;
using M7.Skill;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace M7.GameData.CharacterSkill
{
    [CreateAssetMenu(fileName = "CharacterSkillCondition_OnHit", menuName = "Assets/M7/CharacterSkill/CharacterSkillCondition/CharacterSkillCondition_OnHit")]
    public class CharacterSkillCondition_OnHit : CharacterSkillCondition
    {
        public enum AddType { PerHit, DamageVaule }
        [SerializeField] AddType addCondition;

        public override void RegisterAddPoints(CharacterInstance_Battle targetReference, Action<float> onAddPointsAction)
        {
            (targetReference.StatsInstance as StatsInstance_CharacterBattle).onTakenHit += value => onAddPointsAction?.Invoke(addCondition == AddType.DamageVaule ? value : 1);
        }
    }
}
