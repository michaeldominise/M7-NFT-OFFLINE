/*
 * ChainMethodVertical.cs
 * Author: Cristjan Lazar
 * Date: Oct 26, 2018
 */

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Gamelogic.Grids;
using M7.Match.PlaymakerActions;

namespace M7.Match {

    /// <summary>
    /// Chain method for finding vertical chains in a grid.
    /// </summary>
    [CreateAssetMenu(menuName = "BGGamesCore/Match/ChainMethods/Vertical")]
    public class ChainMethodVertical : ChainMethodBase {

        // TODO Cache results

        // TODO Fix wild tile bug

        Extensions.RectpointListComparer comparer = new Extensions.RectpointListComparer();

        bool[,] validatedCells;
        Vector2Int[] verticalSides = new Vector2Int[4];

        /// <summary>
        /// Get all vertical chains in a grid that is equal to or longer than min chain.
        /// </summary>
        public override List<PointList<RectPoint>> FindChains(MatchGrid matchGrid, PointList<RectPoint> points) {
            return FindChainsFromPoint(matchGrid, matchGrid.Grid.ToPointList());
        }

        /// <summary>
        /// Get vertical chains in the sorounding tiles that is equal to or longer than min chain.
        /// </summary>
        public List<PointList<RectPoint>> FindChainsFromPoint(MatchGrid matchGrid, PointList<RectPoint> points)
        {
            if (validatedCells == null)
                validatedCells = new bool[matchGrid.ColumnCount, matchGrid.RowCount];

            validatedCells.Reset2DDim();

            List<PointList<RectPoint>> result = new List<PointList<RectPoint>>();
            foreach (var point in points)
                FindVerticalChainsFromPoint(matchGrid, result, point);
            return result;
        }

        private void FindVerticalChainsFromPoint(MatchGrid matchGrid, List<PointList<RectPoint>> result, RectPoint point)
        {
            //var chain = Algorithms.GetConnectedSet(matchGrid.Grid,
            //    point,
            //    (p, q) => CheckMatchVertical(point, p, q, point.X, matchGrid));

            //if (chain.Count() >= minChain)
            //result.Add(chain.ToPointList());

            var matchGridTile = matchGrid.Grid[point];
            var startingPoint = new Vector2Int(point.X, point.Y);

            if (matchGridTile.CellTypeContainer.CellType.ElementType == Skill.SkillEnums.ElementFilter.All)
            {
                verticalSides.InitAllSides(startingPoint);
                for (var x = 2; x < 4; x++)
                {
                    if (!verticalSides[x].IsValidVectPoint(matchGrid.Grid))
                        continue;

                    validatedCells[startingPoint.x, startingPoint.y] = false;

                    var chain = new PointList<RectPoint>();
                    ScanPossibleMoves.GetConnectedSameCell(matchGridTile, verticalSides[x], chain, matchGrid.Grid, ref validatedCells, ScanPossibleMoves.ScanDirection.Up | ScanPossibleMoves.ScanDirection.Down);
                    if (chain.Count >= minChain)
                        result.Add(chain);
                }
            }
            else
            {
                var chain = new PointList<RectPoint>();
                ScanPossibleMoves.GetConnectedSameCell(matchGridTile, startingPoint, chain, matchGrid.Grid, ref validatedCells, ScanPossibleMoves.ScanDirection.Up | ScanPossibleMoves.ScanDirection.Down);
                if (chain.Count >= minChain)
                    result.Add(chain);
            }
        }

        private bool CheckMatchVertical(RectPoint target, RectPoint p, RectPoint q, int column, MatchGrid matchGrid) {

            if (p.X != column)
                return false;

            if (q.X != column)
                return false;

            RectGrid<MatchGridCell> grid = matchGrid.Grid;
            MatchGridCell x = grid[target];
            MatchGridCell y = grid[q];

            if (x == null)
                return false;

            if (y == null)
                return false;

            if (!x.IsInteractible)
                return false;

            if (!y.IsInteractible)
                return false;

            return x.CellTypeContainer.CellType.MatchValue == 0 || x.CellTypeContainer.Matches(y.CellTypeContainer.CellType);
        }

    }

}

