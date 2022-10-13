/*
 * ChainMethodHorizontalAndVertical.cs
 * Author: Cristjan Lazar
 * Date: Oct 26, 2018
 */

using System.Collections.Generic;
using UnityEngine;

using Gamelogic.Grids;

namespace M7.Match {

    /// <summary>
    /// Chain method for finding all horizontal and vertical chains in a grid.
    /// </summary>
    [CreateAssetMenu(menuName = "BGGamesCore/Match/ChainMethods/HorizontalAndVerticalMovedTilesOnly")]
    public class ChainMethodHorizontalAndVerticalFromPoint : ChainMethodBase {

        // TODO Cache results

        // TODO Fix wild tile bug

        [SerializeField] private ChainMethodHorizontal chainMethodHorizontal;
        [SerializeField] private ChainMethodVertical chainMethodVertical;

        /// <summary>
        /// Get all vertical and horizontal chains in a grid.
        /// </summary>
        public override List<PointList<RectPoint>> FindChains(MatchGrid matchGrid, PointList<RectPoint> points) {
            List<PointList<RectPoint>> horizontalChains = chainMethodHorizontal.FindChainsFromPoint(matchGrid, points);
            List<PointList<RectPoint>> verticalChains = chainMethodVertical.FindChainsFromPoint(matchGrid, points);

            var result = horizontalChains;
            result.AddRange(verticalChains);
            return result;
        }

    }

}