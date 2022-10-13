using M7.GameRuntime;
using M7.Match;
using M7.Skill;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace M7.GameData.CharacterSkill
{
    [CreateAssetMenu(fileName = "CharacterSkillCondition_BlastCellElement", menuName = "Assets/M7/CharacterSkill/CharacterSkillCondition/CharacterSkillCondition_BlastCellElement")]
    public class CharacterSkillCondition_BlastCellElement : CharacterSkillCondition
    {
        [SerializeField] SkillEnums.ElementFilter elementType;

        public override void RegisterAddPoints(CharacterInstance_Battle targetReference, Action<float> onAddPointsAction)
        {
            ParticleAttractorManager.OnParticleCharged += (MatchGridCell matchGridCell) =>
            {
                if (matchGridCell.CellTypeContainer.CellType.ElementType == elementType)
                    onAddPointsAction?.Invoke(1);
            };
        }
    }
}
