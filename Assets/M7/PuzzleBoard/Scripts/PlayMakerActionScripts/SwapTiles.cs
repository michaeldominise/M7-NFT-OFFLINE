/*
 * SwapTiles.cs
 * Author: Cristjan Lazar
 * Date: Oct 31, 2018
 */

using Gamelogic.Grids;
using HutongGames.PlayMaker;
using Sirenix.Serialization;
using UnityEngine;

namespace M7.Match {

    [ActionCategory("M7/Match")]
    public class SwapTiles : FsmStateAction {

        MatchGrid matchGrid;

        public RectPointList touchedPoints;

	    public override void OnEnter() {
		    Debug.Log ("On Enter");
            matchGrid = PuzzleBoardManager.Instance.ActiveGrid;
	        //Swap();
            Finish();
        }

        private void Swap () {
            RectGrid<MatchGridCell> grid = matchGrid.Grid;
            RectPoint p1 = touchedPoints.Value[0];
            RectPoint p2 = touchedPoints.Value[1];
            MatchGridCell temp = grid[p1];

            grid[p1] = grid[p2];
            grid[p2] = temp;

            grid[p1].CurrentRectPoint = p1;
            grid[p2].CurrentRectPoint = p2;
        }

    }

}

