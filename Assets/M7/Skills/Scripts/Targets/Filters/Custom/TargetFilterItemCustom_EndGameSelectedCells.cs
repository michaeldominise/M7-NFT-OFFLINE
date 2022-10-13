using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using M7.Match;

namespace M7.Skill
{
    public class TargetFilterItemCustom_EndGameSelectedCells : TargetFilterItemCustom<MatchGridCell>
    {
        public override string DebugText => $"Target is not in the end game selected cells";
        public override bool IsValidTarget(Component caster, MatchGridCell target) => !TargetManager_SelectDistinctCell.EndGameSelectedCell.Contains(target.CurrentRectPoint) && target.CurrentCellState == MatchGridCell.CellState.Active;
    }
}