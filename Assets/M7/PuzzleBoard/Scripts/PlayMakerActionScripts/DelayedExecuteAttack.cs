using UnityEngine;
using HutongGames.PlayMaker;
using M7.GameRuntime;
using System.Collections;
using System.Linq;
using M7.GameData;
using DarkTonic.MasterAudio;
using M7.GameRuntime.Scripts.OnBoarding.Game;

namespace M7.Match.PlaymakerActions
{

    [ActionCategory("M7/Match")]
    public class DelayedExecuteAttack : FsmStateAction
    {
        public FsmEvent OnExecuteAttackFinish;
        float PlayerDelay => BattleSceneSettings.Instance.HeroToEnemyAttackDelay;
        float EnemyDelay => BattleSceneSettings.Instance.EnemyToHeroAttackDelay;
        MatchGrid ActiveGrid => PuzzleBoardManager.Instance.ActiveGrid;

        public override void OnEnter() => StartCoroutine(DelayExecuteAttack(TurnManager.Instance.CurrentState == TurnManager.State.PlayerTurn ? PlayerDelay : EnemyDelay));

        public IEnumerator DelayExecuteAttack(float delay)
        {
            var charsToAttack = BattleManager.Instance.ActiveTeam.ActiveCharacters.Where(x => x.StatsInstanceBattle.MatchBoardDamage > 0).ToList();
            if (charsToAttack.Count > 0 && TurnManager.Instance.CurrentState == TurnManager.State.PlayerTurn)
            {
                foreach (var charBattle in charsToAttack)
                {
                    charBattle.UIBattle_CharacterStats.AttackChargeText(charBattle.StatsInstance.Attack, 1);
                    MasterAudio.PlaySound("RPG - AttackCharge");
                }
                yield return new WaitForSeconds(0.5f);
                yield return new WaitWhile(() => !CellCombiner.IsDone || ActiveGrid.Grid.Any(x => ActiveGrid.Grid[x] != null && ActiveGrid.Grid[x].CellMotor.IsMoving));
                MatchGridCellSpawner.Instance.ClearDespawn();
                yield return new WaitForSeconds(0.1f);
                yield return new WaitWhile(() => ParticleAttractorManager.Instance.IsAnimating);
                yield return new WaitForSeconds(delay);

                if(PuzzleBoardOnBoardingManager.Instance.Stages.ContainsKey(LevelManager.LevelData.DisplayName))
                {
                    if (!PuzzleBoardOnBoardingManager.Instance.Stages[LevelManager.LevelData.DisplayName].IsDone &&
                    LevelManager.LevelData.DisplayName == "Stage 1" &&
                    !PuzzleBoardOnBoardingManager.Instance.Stages["Stage 1"].SegmentsDictionary["ShowCharge"]
                        .IsDone)
                    {
                        PuzzleBoardOnBoardingManager.Instance.PopGameOnBoardingSegment("Stage 1", "ShowCharge",
                            () => BattleManager.Instance.ExecuteAttack(() => Fsm.Event(OnExecuteAttackFinish)));
                        yield break;
                    }
                }                
            }

            if (BattleManager.Instance.IsGameDone)
            {
                Fsm.Event(OnExecuteAttackFinish);
                yield break;
            }

            BattleManager.Instance.ExecuteAttack(() => Fsm.Event(OnExecuteAttackFinish));
        }
    }
}

