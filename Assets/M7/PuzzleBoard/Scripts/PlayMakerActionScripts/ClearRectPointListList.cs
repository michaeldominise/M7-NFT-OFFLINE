/*
 * WaitForTouchedTiles.cs
 * Author: Cristjan Lazar
 * Date: Oct 30, 2018
 */

using System.Collections.Generic;
using UnityEngine;

using HutongGames.PlayMaker;
using Gamelogic.Grids;

namespace M7.Match.PlaymakerActions {

    [ActionCategory("M7/Match")]
    public class ClearRectPointListList : FsmStateAction {

        public RectPointListList rectPointListList;
    
        public override void OnEnter() {
            rectPointListList.Value.Clear();
            Finish();
        }
    }

}

