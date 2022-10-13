
using Gamelogic.Grids;
using HutongGames.PlayMaker;
using M7.GameRuntime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Match.PlaymakerActions
{

    [ActionCategory("M7/Match")]
    public class SetupGridBackground : FsmStateAction
    {
        
        public MatchGrid matchGrid;
        public MatchGridBackgroundBorderHandler bgBorderHandler;
        public MatchGridBackgroundBorderHandler fgBorderHandler;
        public MatchGridEditorSavedLevelData LevelData => LevelManager.LevelData.MatchGridEditorSavedLevelData;

        private List<(Vector2Int, Neighbours)> _neighbourDirections;
        private const string _deadTileName = "Dead Cell";

        public override void Awake()
        {
            base.Awake();

            _neighbourDirections = new List<(Vector2Int, Neighbours)>
            {
                (new Vector2Int(-1, 0), Neighbours.Left),
                (new Vector2Int(1, 0), Neighbours.Right),
                (new Vector2Int(0, 1), Neighbours.Top),
                (new Vector2Int(0, -1), Neighbours.Bottom),
                (new Vector2Int(-1, 1), Neighbours.TopLeft),
                (new Vector2Int(1, 1), Neighbours.TopRight),
                (new Vector2Int(-1, -1), Neighbours.BottomLeft),
                (new Vector2Int(1, -1), Neighbours.BottomRight),
                (new Vector2Int(0, 0), Neighbours.Center)
            };
        }

        public override void OnEnter()
        {
            RectGrid<MatchGridCell> grid = matchGrid.Grid;
            IMap3D<RectPoint> map = matchGrid.Map;

            for(int x = 0; x <= grid.Width; x++)
            {
                for(int y = 0; y <= grid.Height; y++)
                {

                    var point = new RectPoint(x, y);
                    SetupBorder(point, map, grid);
                }
            }

            Finish();
        }

        private void SetupBorder(RectPoint point, IMap3D<RectPoint> map, RectGrid<MatchGridCell> grid)
        {
            Vector3 worldPos = map[point];

            var neighbours = CheckNeighbours(point, grid);

            GameObject bg = bgBorderHandler.GetBorder(neighbours);
            if (bg != null)
            {
                bgBorderHandler.Instantiate(bg, worldPos);
            }
            GameObject fg = fgBorderHandler.GetBorder(neighbours);
            if (fg != null)
            {
                fgBorderHandler.Instantiate(fg, worldPos);
            }
        }

        private Neighbours CheckNeighbours(RectPoint point, RectGrid<MatchGridCell> grid)
        {
            Neighbours neighbours = Neighbours.None;

            foreach(var direction in _neighbourDirections)
            {
                int x = point.X + direction.Item1.x;
                int y = point.Y + direction.Item1.y;

                if(x < 0 || x >= grid.Width)
                {
                    continue;
                }

                if (y < 0 || y >= grid.Height)
                {
                    continue;
                }

                var tile = LevelData.GetInvertedYTile(x, y);

                // check if tile is unpassable or not
                if (tile == null || tile.TileName != _deadTileName)
                {
                    neighbours |= direction.Item2;
                }
            }

            return neighbours;
        }
    }

}

