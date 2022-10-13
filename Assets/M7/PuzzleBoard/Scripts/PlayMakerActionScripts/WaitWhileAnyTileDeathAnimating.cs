/*
 * WaitWhileAnyTileMoving.cs
 * Author: Cristjan Lazar
 * Date: Oct 31, 2018
 */

using System.Linq;

using Gamelogic.Grids;
using HutongGames.PlayMaker;
using M7.GameRuntime;

namespace M7.Match.PlaymakerActions {
    [ActionCategory("M7/Match")]
    public class WaitWhileAnyTileDeathAnimating : FsmStateAction {

        MatchGrid matchGrid;

        public RectPointList deathList;

        public FsmEvent OnTileFinishedDeathAnimating;

        private bool isOmniTile;
        
        public override void OnEnter()
        {
            matchGrid = PuzzleBoardManager.Instance.ActiveGrid;

            // isOmniTile = OmniSpawnerManager.Instance.MatchGridCellFuseCollection.Count > 0;
            // if (isOmniTile)
            // {
            //     BattleManager.Instance.TileCombiner.TweenCell();
            // }
        }

        public override void OnUpdate() {
            
            // if(isOmniTile) return;
            
            // TODO Optimize
            // Quite inefficient! Will need to optimize at a later stage.

            var grid = matchGrid.Grid;

            bool isAnyDeathAnimating = deathList.Value.Select(p => grid[p])
                                                .Any(IsDeathAnimating);

            if (!isAnyDeathAnimating)
                Fsm.Event(OnTileFinishedDeathAnimating);
        }

        private bool IsDeathAnimating (MatchGridCell tile) {
            if (tile == null)
                return false;

            if (tile.CellDeath == null)
                return false;

            return tile.CellDeath.IsPlaying;
        }

    }

}

