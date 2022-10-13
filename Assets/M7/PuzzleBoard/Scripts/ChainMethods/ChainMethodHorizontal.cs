/*
 * ChainMethodHorizontal.cs
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
    /// Chain method for finding horizontal chains in a grid.
    /// </summary>
    [CreateAssetMenu(menuName = "BGGamesCore/Match/ChainMethods/Horizontal")]
    public class ChainMethodHorizontal : ChainMethodBase {

        // TODO Cache results

        // TODO Fix Wild tile bug

        Extensions.RectpointListComparer comparer = new Extensions.RectpointListComparer();

        bool[,] validatedCells;
        Vector2Int[] horizontalSides = new Vector2Int[4];

        /// <summary>
        /// Get all horizontal chains in a grid that is equal to or longer than min chain.
        /// </summary>
        public override List<PointList<RectPoint>> FindChains(MatchGrid matchGrid, PointList<RectPoint> points) {
            return FindChainsFromPoint(matchGrid, matchGrid.Grid.ToPointList());
        }

        /// <summary>
        /// Get horizontal chains in the sorounding tiles that is equal to or longer than min chain.
        /// </summary>
        public List<PointList<RectPoint>> FindChainsFromPoint(MatchGrid matchGrid, PointList<RectPoint> points)
        {
            if (validatedCells == null)
                validatedCells = new bool[matchGrid.ColumnCount, matchGrid.RowCount];

            validatedCells.Reset2DDim();

            List<PointList<RectPoint>> result = new List<PointList<RectPoint>>();
            foreach (var point in points)
                FindHorizontalChainsFromPoint(matchGrid, result, point);

            return result;
        }

        private void FindHorizontalChainsFromPoint(MatchGrid matchGrid, List<PointList<RectPoint>> result, RectPoint point)
        {
            //var chain = Algorithms.GetConnectedSet(matchGrid.Grid,
            //point,
            //(p, q) => CheckMatchHorizontal(point, q, point.Y, matchGrid));

            //if (chain.Count() >= minChain)
            //    result.Add(chain.ToPointList());

            //result = result.Distinct(comparer).ToList();

            var matchGridCell = matchGrid.Grid[point];
            var startingPoint = new Vector2Int(point.X, point.Y);

            if (matchGridCell.CellTypeContainer.CellType.ElementType == Skill.SkillEnums.ElementFilter.All)
            {
                horizontalSides.InitAllSides(startingPoint);
                for (var x = 0; x < 2; x++)
                {
                    if (!horizontalSides[x].IsValidVectPoint(matchGrid.Grid))
                        continue;

                    validatedCells[startingPoint.x, startingPoint.y] = false;

                    var chain = new PointList<RectPoint>();
                    ScanPossibleMoves.GetConnectedSameCell(matchGrid.Grid[point], horizontalSides[x], chain, matchGrid.Grid, ref validatedCells, ScanPossibleMoves.ScanDirection.Right | ScanPossibleMoves.ScanDirection.Left);
                    if (chain.Count >= minChain)
                        result.Add(chain);
                }
            }
            else
            {
                var chain = new PointList<RectPoint>();
                ScanPossibleMoves.GetConnectedSameCell(matchGrid.Grid[point], startingPoint, chain, matchGrid.Grid, ref validatedCells, ScanPossibleMoves.ScanDirection.Right | ScanPossibleMoves.ScanDirection.Left);
                if (chain.Count >= minChain)
                    result.Add(chain);
            }
        }

        private bool CheckMatchHorizontal (RectPoint p, RectPoint q, int row, MatchGrid matchGrid)
        {
            if (p.Y != row)
                return false;

            if (q.Y != row)
                return false;

            RectGrid<MatchGridCell> grid = matchGrid.Grid;
            MatchGridCell x = grid[p];
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

