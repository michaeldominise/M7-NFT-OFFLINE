using System;
using System.Collections;
using System.Collections.Generic;
using Gamelogic.Grids;
using HutongGames.PlayMaker;
using M7.GameRuntime;
using UnityEngine;

namespace M7.Match.PlaymakerActions
{
    [ActionCategory("M7/Match")]
    public class UpdateTileIcons : FsmStateAction
    {
        public PossibleMoveList possibleMoveList;
        public OmniSphereData.MainTriggerType mainTriggerType;
	    public override void OnEnter()
        {
            UpdateIcon(possibleMoveList.Value, mainTriggerType);
            Finish();
	    }

        public static void UpdateIcon(List<PossibleMove> possibleMoves, OmniSphereData.MainTriggerType triggerType)
        {
            foreach (var matchGridTile in PuzzleBoardManager.Instance.ActiveGrid.Grid)
            {
                var cell = PuzzleBoardManager.Instance.ActiveGrid.Grid[matchGridTile];
                if (cell == null)
                    continue;

                cell.ResetIcon();
                cell.SetSpecialTileComboGlow(false);
            }

            foreach (var possibleMove in possibleMoves)
            {
                if (possibleMove.ElementType == Skill.SkillEnums.ElementFilter.Special && possibleMove.cells.Count > 1)
                    foreach (var cell in possibleMove.cells)
                        cell.SetSpecialTileComboGlow(!BattleManager.Instance.IsGameDone);

                var icon = OmniSpawnerManager.Instance.FindOmniSphereData(triggerType, possibleMove.cells.Count, possibleMove.ElementType)?.GetDisplayIcon(possibleMove.ElementType);
                if (icon)
                    foreach (var cell in possibleMove.cells)
                        cell.UpdateIcon(icon);
            }
        }
    }
}