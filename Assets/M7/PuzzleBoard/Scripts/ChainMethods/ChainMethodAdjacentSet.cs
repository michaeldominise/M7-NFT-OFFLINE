/* ChainMethodAdjacentSet.cs
 * Author: Cristjan Lazar
 * Date: Oct 25, 2018
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Gamelogic.Grids;

namespace M7.Match {

    /// <summary>
    /// Chain method for finding all adjacent chains in a grid.
    /// </summary>
    [CreateAssetMenu(menuName = "BGGamesCore/Match/ChainMethods/Adjacent")]
    public class ChainMethodAdjacentSet : ChainMethodBase {

        // [SerializeField] private int minChain = 2;

        private List<PointList<RectPoint>> result = new List<PointList<RectPoint>>();

        /// <summary>
        /// Find chain that forms an adjacent set based on an origin point.
        /// </summary>
        public override List<PointList<RectPoint>> FindChains(MatchGrid matchGrid, PointList<RectPoint> tappedPoints) {
            result.Clear();

            if (tappedPoints.Count < 1)
                return result;

            RectGrid<MatchGridCell> grid = matchGrid.Grid;

            var set = Algorithms.GetConnectedSet(grid, 
                                       tappedPoints.First(), 
                                       (x,y) => CheckMatch(grid[x], grid[y]));

            if (set.Count() < minChain)
                return result;

            result.Add(set.ToPointList());
            
            return result;
        }

        private bool CheckMatch(MatchGridCell x, MatchGridCell y) {
            if (x == null)
                return false;

            if (y == null)
                return false;

            if (!x.IsInteractible)
                return false;

            if (!y.IsInteractible)
                return false;

            return x.CellTypeContainer.Matches(y.CellTypeContainer.CellType);
        }

    }

}
