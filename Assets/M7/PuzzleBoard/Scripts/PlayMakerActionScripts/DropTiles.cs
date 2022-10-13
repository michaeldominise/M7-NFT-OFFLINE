/*
 * DropTiles.cs
 * Author: Cristjan Lazar
 * Date: Oct 30, 2018
 */

using Gamelogic.Grids;
using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace M7.Match.PlaymakerActions {

    [ActionCategory("M7/Match")]
    public class DropTiles : FsmStateAction {

        public enum DropMode { FROM_ORIGIN, FROM_TOP }

        public DropMode dropMode;

        public override void OnEnter() {
            DropGrid(dropMode);
            Finish();
        }
        public static void DropGrid (DropMode dropMode)
        {
            RectGrid<MatchGridCell> sourceGrid = null;
            RectGrid<MatchGridCell> destGrid = null;

            switch (dropMode)
            {
                case DropMode.FROM_ORIGIN:
                    sourceGrid = destGrid = PuzzleBoardManager.Instance.ActiveGrid.Grid;
                    break;
                case DropMode.FROM_TOP:
                    sourceGrid = PuzzleBoardManager.Instance.ReplacementGrid.Grid;
                    destGrid = PuzzleBoardManager.Instance.ActiveGrid.Grid;
                    break;
            }


            var droppables = sourceGrid.WhereCell(t => IsDroppable(t));

            foreach (var p in droppables) {
                MatchGridCell tile = sourceGrid[p];
                sourceGrid[p] = null;
            
                RectPoint q = FindFloor(destGrid, CheckPoint(p, dropMode, destGrid));
                destGrid[q] = tile;
                destGrid[q].CurrentRectPoint = q;
            }
        }

        public static RectPoint CheckPoint (RectPoint point, DropMode dropMode, RectGrid<MatchGridCell> destGrid) {
            if (dropMode == DropMode.FROM_ORIGIN)
                return point;

            return new RectPoint(point.X, destGrid.Height - 1);
        }

        public static bool IsDroppable (MatchGridCell tile) {
            if (tile == null)
                return false;

            return tile.CellTypeContainer.CellType.StaticType == CellType.StaticEnum.NotStatic;
        }

        public static RectPoint FindFloor(RectGrid<MatchGridCell> grid, RectPoint point)
        {
            var possibleFloor = new List<RectPoint>();
            possibleFloor.Add(point);
            for (var y = point.Y - 1; y >= 0; y--)
            {
                var floor = new RectPoint(point.X, y);
                var cell = grid[floor];
                if (cell == null)    
                    possibleFloor.Add(floor);
                else if (cell.CellTypeContainer.CellType.StaticType == CellType.StaticEnum.StaticNotPssable)
                    break;
            }

            return possibleFloor.LastOrDefault();
        }
    }

}

