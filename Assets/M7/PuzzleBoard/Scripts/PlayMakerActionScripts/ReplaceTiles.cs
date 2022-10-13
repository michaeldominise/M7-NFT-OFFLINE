using System.Collections;
using System.Collections.Generic;
using Gamelogic.Grids;
using HutongGames.PlayMaker;
using UnityEngine;

namespace M7.Match.PlaymakerActions
{
    [ActionCategory("M7/Match")]
    public class ReplaceTiles : FsmStateAction
    {
        public IntList replacementList;

        public override void OnEnter()
        {
            SpawnTiles(replacementList.Value);
            Finish();
        }

        public static void SpawnTiles(List<int> columns)
        {
            var replacementGrid = PuzzleBoardManager.Instance.ReplacementGrid;
            var spawner = PuzzleBoardManager.Instance.ActiveTileSpawner;
            var grid = replacementGrid.Grid;
            var map = replacementGrid.Map;

            var requestedColumns = new List<int>(new int[replacementGrid.ColumnCount]);
            foreach (var col in columns)
                requestedColumns[col]++;
            
            for (int x = 0; x < requestedColumns.Count; x++)
            {
                for (var y = 0; y < requestedColumns[x]; y++)
                {
                    var point = new RectPoint(x, y);
                    grid[point] = spawner.Spawn(map[point]);
                    grid[point].CurrentRectPoint = point;
                }
            }
        }
    }
}
