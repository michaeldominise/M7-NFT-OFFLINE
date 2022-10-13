/*
 * WaitWhileAnyTileMoving.cs
 * Author: Cristjan Lazar
 * Date: Oct 31, 2018
 */

using System.Linq;

using Gamelogic.Grids;
using HutongGames.PlayMaker;

namespace M7.Match.PlaymakerActions {
    [ActionCategory("M7/Match")]
    public class WaitWhileAnyTileMoving : FsmStateAction {

        MatchGrid matchGrid;

        public FsmEvent OnTilesFinishedMoving;

        public override void OnEnter()
        {
            matchGrid = PuzzleBoardManager.Instance.ActiveGrid;
        }

        public override void OnUpdate () {
            // TODO Optimize
            // Quite inefficient! Will need to optimize at a later stage.

            var grid = matchGrid.Grid;

            bool isAnyMoving = grid.WhereCell(IsMovable)
                .Any(p => grid[p].CellMotor.IsMoving);

            if (!isAnyMoving)
                Fsm.Event(OnTilesFinishedMoving);
        }

        private bool IsMovable (MatchGridCell tile) {
            if (tile == null)
                return false;

            if (tile.CellTypeContainer.CellType.StaticType != CellType.StaticEnum.NotStatic)
                return false;

            return tile.CellMotor.IsMoving;
        }

    }

}

