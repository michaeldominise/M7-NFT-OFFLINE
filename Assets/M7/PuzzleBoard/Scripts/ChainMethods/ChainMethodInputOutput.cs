/* ChainMethodInputOutput.cs
 * Author: Cristjan Lazar
 * Date: Oct 26, 2018
 */

using System.Collections.Generic;
using UnityEngine;

using Gamelogic.Grids;

namespace M7.Match {

    /// <summary>
    /// Chain method for that forms a chain from a set of tapped points.
    /// </summary>
    [CreateAssetMenu(menuName = "BGGamesCore/Match/ChainMethods/InputOutput")]
    public class ChainMethodInputOutput : ChainMethodBase {

        private List<PointList<RectPoint>> result = new List<PointList<RectPoint>>();

        /// <summary>
        /// Form a chain based on the tapped points (input is output).
        /// </summary>
        public override List<PointList<RectPoint>> FindChains(MatchGrid matchGrid, PointList<RectPoint> tappedPoints) {
            result.Clear();

            result.Add(tappedPoints);

            return result;
        }

    }

}

