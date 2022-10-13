using System.Collections.Generic;
using UnityEngine;

using HutongGames.PlayMaker;
using System;
using Gamelogic.Grids;
using M7.Skill;
using M7.GameRuntime;
using M7.PuzzleBoard.Scripts.Booster;

namespace M7.Match.PlaymakerActions
{

    [ActionCategory("M7/Match")]
    public class SetEndTurnState : FsmStateAction
    {
        public override void OnEnter()
        {
            if (BattleManager.Instance.IsGameDone)
                return;

            if (!BoosterManager.Instance.isBoosterActive || !BattleManager.Instance.EnemyTeam.IsAlive)
                BattleManager.Instance.SetGameState(BattleManager.GameState.EndTurn); 
            else
                BattleManager.Instance.SetGameState(BattleManager.GameState.Idle);

            Finish();
        }
    }
}

