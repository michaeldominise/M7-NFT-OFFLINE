  /*
 * DamageTileChain.cs
 * Author: Cristjan Lazar
 * Date: Oct 31, 2018
 */

using PathologicalGames;
using Gamelogic.Grids;
using HutongGames.PlayMaker;
using M7.Skill;
using M7.GameRuntime;
using System.Linq;
using System.Collections.Generic;

namespace M7.Match.PlaymakerActions {

    [ActionCategory("M7/Match")]
    public class ClearDeadCubes : FsmStateAction {
    
        MatchGrid matchGrid;

        SpawnPool spawnPool;

        public override void OnEnter() {
            
            matchGrid = PuzzleBoardManager.Instance.ActiveGrid;

            var grid = matchGrid.Grid;
            var clearables = grid.WhereCell(t => IsDead(t));

            BattleManager.Instance.StarConditionManager.DestroyTileTest(clearables.Count());

            foreach (var p in clearables) {
                spawnPool.Despawn(grid[p].transform);
                grid[p] = null;
            }

            SkillQueueManager.Instance.WaitUntilIdle(Finish);
        }

        private bool IsDead (MatchGridCell tile) {
            if (tile == null)
                return false;

            if (tile.CellHealth == null)
                return false;
            return tile.CellHealth.IsDead;
        }

    }

}


