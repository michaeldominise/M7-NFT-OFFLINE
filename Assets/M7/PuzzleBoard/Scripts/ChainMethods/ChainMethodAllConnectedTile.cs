/*
 * ChainMethodHorizontalAndVertical.cs
 * Author: Cristjan Lazar
 * Date: Oct 26, 2018
 */

using System.Collections.Generic;
using UnityEngine;

using Gamelogic.Grids;
using M7.Match.PlaymakerActions;

namespace M7.Match {

    /// <summary>
    /// Chain method for finding all horizontal and vertical chains in a grid.
    /// </summary>
    [CreateAssetMenu(menuName = "BGGamesCore/Match/ChainMethods/ChainMethodAllConnectedTile")]
    public class ChainMethodAllConnectedTile : ChainMethodBase
    {
        bool[,] validatedCells;
        Vector2Int[] cellSides = new Vector2Int[4];

        /// <summary>
        /// Get all vertical chains in a grid that is equal to or longer than min chain.
        /// </summary>
        public override List<PointList<RectPoint>> FindChains(MatchGrid matchGrid, PointList<RectPoint> points)
        {
            return FindChainsFromPoint(matchGrid, points);
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
                FindChainsFromPoint(matchGrid, result, point);
            return result;
        }

        private void FindChainsFromPoint(MatchGrid matchGrid, List<PointList<RectPoint>> result, RectPoint point)
        {
            var matchGridCell = matchGrid.Grid[point];
            var startingPoint = new Vector2Int(point.X, point.Y);

            cellSides.InitAllSides(startingPoint);
            for (var x = 0; x < 4; x++)
            {
                if (!cellSides[x].IsValidVectPoint(matchGrid.Grid))
                    continue;

                validatedCells[startingPoint.x, startingPoint.y] = false;

                var chain = new PointList<RectPoint>();
                ScanPossibleMoves.GetConnectedSameCell(matchGridCell, cellSides[x], chain, matchGrid.Grid, ref validatedCells, ScanPossibleMoves.ScanDirection.AllSides);
                if (chain.Count >= GetMinChain(matchGridCell))
                    result.Add(chain);
            }
        }

        public int GetMinChain(MatchGridCell cell) => cell.CellTypeContainer.CellType.ElementType == Skill.SkillEnums.ElementFilter.Special ? 1 : minChain;
    }

}