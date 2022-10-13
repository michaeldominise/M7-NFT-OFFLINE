/*
 * ActivateTileMotors.cs
 * Author: Cristjan Lazar
 * Date: Oct 31, 2018
 */

using Gamelogic.Grids;
using HutongGames.PlayMaker;
using UnityEngine;

namespace M7.Match {

    [ActionCategory("M7/Match")]
    public class WaitUntilCameraShake : FsmStateAction
    {
        [SerializeField] private WorldAnimation_Shake cameraShake;
        
        public override void OnEnter() {
            Finish();
        }
    }

}

