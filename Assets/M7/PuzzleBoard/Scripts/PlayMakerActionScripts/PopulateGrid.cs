/*
 * PopulateGrid.cs
 * Author: Cristjan Lazar
 * Date; Oct 30, 2018
 */

using UnityEngine;

using Gamelogic.Grids;
using HutongGames.PlayMaker;
using System.Linq;
using M7.GameRuntime;
using System.Collections.Generic;
using M7.Skill;

namespace M7.Match.PlaymakerActions
{

    [ActionCategory("M7/Match")]
    public class PopulateGrid : FsmStateAction
    {

        public MatchGrid matchGrid;
        public MatchGridCellSpawner spawner;
        public CellBag baseTiles;
        public MatchGridEditorSavedLevelData LevelData => LevelManager.LevelData.MatchGridEditorSavedLevelData;
        public SkillEnums.CellGridLocation cellGridLocation;

        public override void OnEnter()
        {
            spawner.Initialize(baseTiles);
            RectGrid<MatchGridCell> grid = matchGrid.Grid;
            IMap3D<RectPoint> map = matchGrid.Map;

            List<CellType> shuffledBaseTiles = baseTiles.TileTypeList.ToList();
            shuffledBaseTiles.ShuffleSystemRNG();

            foreach (var point in grid)
            {
                Vector3 worldPos = map[point];
                CellType tileType = LevelData.GetInvertedYTile(point.X, point.Y, cellGridLocation);

                if(tileType is RandomCellType)
                {
                    grid[point] = spawner.Spawn(worldPos, cellGridLocation: cellGridLocation);
                } else if (tileType is RandomGroupCellType)
                {
                    int tileTypeIndex = (tileType as RandomGroupCellType).colorGroup - 1;
                    grid[point] = spawner.Spawn(worldPos, shuffledBaseTiles[tileTypeIndex], cellGridLocation);
                } else
                {
                    grid[point] = spawner.Spawn(worldPos, tileType, cellGridLocation);
                }

                if(grid[point] != null)
                    grid[point].CurrentRectPoint = point;
            }

            spawner.RefreshSibling(matchGrid);
            Finish();
        }
    }

}

