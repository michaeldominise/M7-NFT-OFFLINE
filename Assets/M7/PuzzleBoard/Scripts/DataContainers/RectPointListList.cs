/*
 * RectPointList.cs
 * Author: Cristjan Lazar
 * Date: Oct 31, 2018
 */

using System.Collections.Generic;
using UnityEngine;

using Gamelogic.Grids;
using Sirenix.OdinInspector;

namespace M7.Match {

    [CreateAssetMenu(menuName = "BGGamesCore/Match/DataContainers/RectPointListList")]
    public class RectPointListList : ScriptableObject {
        public List<PointList<RectPoint>> Value = new List<PointList<RectPoint>>();
    }
}

