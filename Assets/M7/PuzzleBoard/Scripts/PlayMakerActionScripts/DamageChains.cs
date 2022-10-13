/*
 * DamageTileChain.cs
 * Author: Cristjan Lazar
 * Date: Oct 31, 2018
 */

using System.Collections.Generic;
using System.Linq;
using Gamelogic.Grids;
using HutongGames.PlayMaker;
using UnityEngine;

namespace M7.Match.PlaymakerActions {
    [ActionCategory("M7/Match")]
    public class DamageChains : FsmStateAction {
    
        public RectPointListList chainList;

        MatchGrid matchGrid;

        public override void OnEnter() {
            List<PointList<RectPoint>> chains = chainList.Value;

            foreach (var chain in chains)
                TileChainDamager.DamageTileChain(chain.Select(x => matchGrid.Grid[x]).ToList());

            Finish();
        }
    }
}


