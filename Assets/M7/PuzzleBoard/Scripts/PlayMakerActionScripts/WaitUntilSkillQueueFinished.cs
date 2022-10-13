/*
 * WaitWhileAnyTileMoving.cs
 * Author: Cristjan Lazar
 * Date: Oct 31, 2018
 */

using System.Linq;

using Gamelogic.Grids;
using HutongGames.PlayMaker;
using M7.Skill;

namespace M7.Match.PlaymakerActions {
    [ActionCategory("M7/Match")]
    public class WaitUntilSkillQueueFinished : FsmStateAction {
        public override void OnEnter() => SkillQueueManager.Instance.WaitUntilIdle(Finish);
    }

}

