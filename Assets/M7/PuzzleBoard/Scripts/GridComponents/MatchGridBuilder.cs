/*
 * MatchGridBuilder.cs
 * Author: Cristjan Lazar
 * Date: Oct 10, 2018
 */

using UnityEngine;

using Gamelogic.Grids;
using PathologicalGames;


namespace M7.Match {

    /// <summary>
    /// Builds a grid and a map and supplies it to match grid;
    /// </summary>
    [System.Obsolete]
    public class MatchGridBuilder : MonoBehaviour {

        private enum NEIGHBOR_SETTINGS { MAIN, DIAGONALS, MAIN_AND_DIAGONALS }

        [SerializeField] private MatchGrid matchGrid;

        [SerializeField] private NEIGHBOR_SETTINGS neighborSettings;

        [SerializeField] private MatchGridCellSpawner spawner;

        const float offsetY = 30;

        // TODO Remove this once there is a proper level data injector
        public MatchGridEditorSavedLevelData gridData;

        //private SpawnPool pool;

        // TODO Move this to its own utility class
        public readonly Rect ScreenRect = new Rect(-Screen.width / 2f, -Screen.height / 2f, Screen.width, Screen.height);

        /// <summary>
        /// Build a grid and a map and assign them both to a match grid.
        /// </summary>
        public void Build() {
            RectGrid<MatchGridCell> grid = RectGrid<MatchGridCell>
              .Rectangle(gridData.ColumnCount, gridData.RowCount);

            SetNeighbors(grid);
			Vector2 cellDimensions = PuzzleBoardSettings.Instance.cellDimensions;
                //IMap3D<RectPoint> map = new RectMap(LevelSettings.Instance.cellDimensions)
                IMap3D<RectPoint> map = new RectMap(cellDimensions)
                .WithWindow(ScreenRect)
                .AlignMiddleCenter(grid)
                .To3DXY();

            matchGrid.Initialize(grid, map);

            SpawnTiles(grid, map, gridData);
        }

        private void SetNeighbors (RectGrid<MatchGridCell> grid) {
            switch(neighborSettings) {
                case NEIGHBOR_SETTINGS.MAIN:
                    grid.SetNeighborsMain();
                    break;
                case NEIGHBOR_SETTINGS.DIAGONALS:
                    grid.SetNeighborsDiagonals();
                    break;
                case NEIGHBOR_SETTINGS.MAIN_AND_DIAGONALS:
                    grid.SetNeighborsMainAndDiagonals();
                    break;
                default:
                    grid.SetNeighborsMain();
                    break;
            }
        }

        private void SpawnTiles(RectGrid<MatchGridCell> grid, IMap3D<RectPoint> map, MatchGridEditorSavedLevelData gridData) {
            foreach (RectPoint point in grid) {
                Vector3 worldPoint = map[point];

                CellType tileType = gridData.GetInvertedYTile(point.X, point.Y);

                if (tileType == null)
                    continue;

                print(matchGrid.root);
                grid[point] = spawner.Spawn(worldPoint, tileType);
                //grid[point] = spawner.Spawn(worldPoint, matchGrid.root, tileType);
            }
        }

        #region Unity event methods
        void Start() {
            Build();
        }
        #endregion

    }

}
