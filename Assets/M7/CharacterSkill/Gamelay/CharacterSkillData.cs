using System;
using System.Collections.Generic;
using System.Linq;
using M7.GameRuntime;
using M7.Skill;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.GameData.CharacterSkill
{
    [Serializable]
    public class CharacterSkillData
    {
        [ShowInInspector] List<CharacterSkillCondition_Instance> CharacterSkillCondition_InstanceList { get; set; } = new List<CharacterSkillCondition_Instance>();
        [ShowInInspector] float CurrentProgress => CharacterSkillCondition_InstanceList.Count == 0 ? 0 : (CharacterSkillCondition_InstanceList.Sum(x => x.CurrentProgress) / CharacterSkillCondition_InstanceList.Count);
        [ShowInInspector] public bool IsReady { get; private set; }
        public CharacterInstance_Battle TargetReference { get; set; }
        public Action<float> onSkillPointsUpdate;

        public void Init(CharacterInstance_Battle targetReference)
        {
            TargetReference = targetReference;
            foreach (var characterSkillCondition in targetReference.CharacterObject.CharacterSkillConditionList)
            {
                var instance = new CharacterSkillCondition_Instance(this, characterSkillCondition);
                CharacterSkillCondition_InstanceList.Add(instance);
            }
            onSkillPointsUpdate?.Invoke(CurrentProgress);
            CheckState();
        }

        public void CheckState()
        {
            var isReadyValue = CurrentProgress >= 1;
            if(IsReady != isReadyValue)
            {
                IsReady = isReadyValue;
                TargetReference.AnimateCast(IsReady);
            }
            onSkillPointsUpdate?.Invoke(CurrentProgress);
        }

        [Button]
        public void Execute()
        {
            if (!IsReady || BattleManager.Instance.CurrentState != BattleManager.GameState.Idle || CharacterSelectionTargetManager.Instance.IsActive || SkillQueueManager.Instance.CurrentState == SkillQueueManager.State.Executing)
                return;

            BattleManager.Instance.SetGameState(BattleManager.GameState.ExecutingSkills);
            SkillQueueManager.Instance.AddSkillToQueue(TargetReference, TargetReference.CharacterSkillAsset, onFinished: () => BattleManager.Instance.SetGameState(BattleManager.GameState.Idle));

            //BattleManager.Instance.StartCoroutine(BattleManager.Instance.AttackInformationManager.SetInfo(TargetReference.CharacterSkillAsset, 2));
            Reset();
        }


        [Button]
        public void Reset()
        {
            CharacterSkillCondition_InstanceList.ForEach(x => x?.Reset());
            //ResetTurn();
            CheckState();

            //TargetReference.UIBattle_CharacterStats.AttackChargeText(TargetReference.UIBattle_CharacterStats.attackTurnCount);
        } 

        void ResetTurn()
        {
            TargetReference.StatsInstance.SetValue(SkillEnums.TargetCharacterStats.AttackTurn, 3);
            TargetReference.StatsInstance.UpdateInstanceActions();
        }
    }
}
