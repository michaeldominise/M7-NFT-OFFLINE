/*
 * InitializeGrid.cs
 * Author: Cristjan Lazar
 * Date: Oct 30, 2018
 */

using UnityEngine;

using HutongGames.PlayMaker;
using Gamelogic.Grids;
using M7.GameRuntime;

namespace M7.Match.PlaymakerActions
{

    [ActionCategory("M7/Match")]
    public class InitializeGrid : FsmStateAction
    {
        public enum NEIGHBOR { MAIN, DIAGONALS, MAIN_AND_DIAGONALS }

        public MatchGrid matchGrid;
        public Transform root;
        public NEIGHBOR neighborMode;

        MatchGridEditorSavedLevelData LevelData => LevelManager.LevelData.MatchGridEditorSavedLevelData;

        readonly Rect ScreenRect = new Rect(-Screen.width / 2f, -Screen.height / 2f, Screen.width, Screen.height);
        const float offsetY = 30;

        public override void OnEnter()
        {
            Build();
            SetNeighbors(matchGrid.Grid);
            Finish();
        }

        private void Build()
        {
            RectGrid<MatchGridCell> grid = RectGrid<MatchGridCell>
                .Rectangle(LevelData.ColumnCount, LevelData.RowCount);
            Vector2 cellDimensions = PuzzleBoardSettings.Instance.cellDimensions;

            Vector2 rectDimensions = new Vector2(grid.Width, grid.Height);

            Rect gridRect = new Rect(-rectDimensions.x / 2f,
                -rectDimensions.y / 2f,
                rectDimensions.x,
                rectDimensions.y);

            IMap3D<RectPoint> map = new RectMap(cellDimensions)
                .WithWindow(gridRect)
                .AlignMiddleCenter(grid)
                .Translate(root.transform.position)
                .To3DXY(root.transform.position.z);

            matchGrid.Initialize(grid, map);
        }

        private void SetNeighbors(RectGrid<MatchGridCell> grid)
        {
            switch (neighborMode)
            {
                case NEIGHBOR.MAIN:
                    grid.SetNeighborsMain();
                    break;
                case NEIGHBOR.DIAGONALS:
                    grid.SetNeighborsDiagonals();
                    break;
                case NEIGHBOR.MAIN_AND_DIAGONALS:
                    grid.SetNeighborsMainAndDiagonals();
                    break;
                default:
                    grid.SetNeighborsMain();
                    break;
            }
        }

    }

}

