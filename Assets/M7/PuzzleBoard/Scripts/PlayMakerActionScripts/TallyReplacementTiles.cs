/*
 * TallyReplacementTiles.cs
 * Author: Cristjan Lazar
 * Date: Oct 30, 2018
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Gamelogic.Grids;
using HutongGames.PlayMaker;

namespace M7.Match {
    [ActionCategory("M7/Match")]
    public class TallyReplacementTiles : FsmStateAction
    {
        public IntList outReplacementList;

        public override void OnEnter() {
            outReplacementList.Value = CountReplacementTiles();
            Finish();
        }

        public static List<int> CountReplacementTiles () {
            var listInt = new List<int>();
            RectGrid<MatchGridCell> grid = PuzzleBoardManager.Instance.ActiveGrid.Grid;

            RectPoint down = new RectPoint(0, -1);

            for (int i = 0; i < grid.Width; i++) {
                var p = new RectPoint(i, grid.Height - 1);

                while (grid.Contains(p)) 
                {
                    if (grid[p] != null)
                    {
                        if (grid[p].CellTypeContainer.CellType.StaticType == CellType.StaticEnum.StaticPassable)
                        {
                            p += down;
                            continue;
                        }
                        else
                            break;
                    }

                    listInt.Add(i);
                    p += down;
                }
            }

            return listInt;
        }

    }

}

