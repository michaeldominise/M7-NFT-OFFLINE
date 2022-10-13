/* ChainMethodBase.cs
 * Author: Cristjan Lazar
 * Date: Oct 26, 2018
 */

using System.Collections.Generic;
using UnityEngine;

using Gamelogic.Grids;
using Sirenix.OdinInspector;

namespace M7.Match {

    public abstract class ChainMethodBase : ScriptableObject {
        [SerializeField] protected int minChain = 3;
        public abstract List<PointList<RectPoint>> FindChains(MatchGrid matchGrid, PointList<RectPoint> points);
    }

}

