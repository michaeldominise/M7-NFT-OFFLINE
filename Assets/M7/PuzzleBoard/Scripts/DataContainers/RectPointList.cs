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

    [CreateAssetMenu(menuName = "BGGamesCore/Match/DataContainers/RectPointList")]
    public class RectPointList : ScriptableObject {

        private PointList<RectPoint> value = new PointList<RectPoint>();
        public PointList<RectPoint> Value { get { return this.value; } set { this.value = value; } }

    }

}

