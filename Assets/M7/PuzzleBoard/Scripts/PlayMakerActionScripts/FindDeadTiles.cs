/*
 * DamageTileChain.cs
 * Author: Cristjan Lazar
 * Date: Oct 31, 2018
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Gamelogic.Grids;
using HutongGames.PlayMaker;

namespace M7.Match.PlaymakerActions {

    [ActionCategory("M7/Match")]
    public class FindDeadTiles : FsmStateAction {

        MatchGrid matchGrid;

        public RectPointList outDeadList;

        public override void OnEnter() {

            matchGrid = PuzzleBoardManager.Instance.ActiveGrid;
            RectGrid<MatchGridCell> grid = matchGrid.Grid;
            IEnumerable<RectPoint> clearables = grid.WhereCell(IsDead);

            outDeadList.Value = clearables.ToPointList();

            Finish();
        }

        public static bool IsDead(MatchGridCell tile) {
            if (tile == null)
                return false;

            if (tile.CellHealth == null)
                return false;

            return tile.CellHealth.IsDead;
        }

    }

}


