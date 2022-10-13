/*
 * ActivateTileMotors.cs
 * Author: Cristjan Lazar
 * Date: Oct 31, 2018
 */

using System.Collections;
using System.Linq;
using Gamelogic.Grids;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEvents;

namespace M7.Match {

    [ActionCategory("M7/Match")]
    public class ActivateTileMotors : FsmStateAction {

        public override void OnEnter() {
            MoveTiles();
            Finish();
        }

        public static void MoveTiles()
        {
            var matchGrid = PuzzleBoardManager.Instance.ActiveGrid;
            RectGrid<MatchGridCell> grid = matchGrid.Grid;
            IMap3D<RectPoint> map = matchGrid.Map;

            const float interval = 0.025f;
            for (var x = 0; x < grid.Width; x++)
            {
                var currentInterval = 0f;
                for (var y = 0; y < grid.Height; y++)
                {
                    var rectPoint = new RectPoint(x, y);
                    if (!IsMovable(grid[rectPoint], map[rectPoint]))
                        continue;

                    var isMoving = grid[rectPoint].CellMotor.IsMoving;
                    grid[rectPoint].CellMotor.Move(map[rectPoint], delay: currentInterval);
                    if (!isMoving)
                        currentInterval += interval;
                }
            }
        }

        public static bool IsMovable (MatchGridCell tile, Vector3 pos) {
            if (tile == null)
                return false;

            if (tile.CellMotor == null)
                return false;

            return tile.CellTypeContainer.CellType.StaticType == CellType.StaticEnum.NotStatic && tile.transform.position != pos;
        }

    }

}

