#define CHEATS_ON
using M7.GameRuntime;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace M7.Skill
{
    public partial class SkillManager
    {
#if CHEATS_ON
        [SerializeField] SkillObject attackAll;

        [Button]
        public static void WipeTeam(bool isPlayerTeam) => TestSkill(isPlayerTeam ? BattleManager.Instance.PlayerTeam : BattleManager.Instance.EnemyTeam, Instance.attackAll);

        [Button]
        public static void TestSkill(ISkillCaster caster, SkillObject skillObject)
        {
            // var skillCaster = caster as ISkillCaster;
            if (CharacterSelectionTargetManager.Instance.IsActive || SkillQueueManager.Instance.CurrentState == SkillQueueManager.State.Executing)
                return;
            var skillCaster = caster;
            if (skillCaster == null)
            {
                Debug.LogError($"{caster} must be ISkillCaster");
                return;
            }

            SkillQueueManager.Instance.AddSkillToQueue(skillCaster, skillObject);
            SkillQueueManager.Instance.WaitUntilIdle(() =>
            {
                var charBattleList = BattleManager.Instance.PlayerTeam.ActiveCharacters;
                charBattleList.AddRange(BattleManager.Instance.EnemyTeam.ActiveCharacters);
                foreach (var charBattle in charBattleList)
                    charBattle.StatsInstance.UpdateInstanceActions();
                if (!(BattleManager.Instance.EnemyTeam.IsAlive && BattleManager.Instance.PlayerTeam.IsAlive))
                    BattleManager.Instance.SetGameState(BattleManager.GameState.EndTurn);
            });
        }
#endif
    }
}