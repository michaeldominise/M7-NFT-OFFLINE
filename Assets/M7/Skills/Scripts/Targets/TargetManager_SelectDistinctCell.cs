using M7.GameRuntime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using M7.Match;
using Sirenix.OdinInspector;
using Gamelogic.Grids;

namespace M7.Skill
{
    public class TargetManager_SelectDistinctCell : TargetManager_MatchGridCell
    {
        public static List<RectPoint> EndGameSelectedCell = new List<RectPoint>();
        public override List<Component> GetTargets<CasterType>(CasterType caster, bool ignoreTargetCount = false)
        {
            var targets = base.GetTargets(caster, ignoreTargetCount).Where(x => (x as MatchGridCell).CurrentCellState == MatchGridCell.CellState.Active).ToList();
            EndGameSelectedCell.AddRange(targets.Select(x => (x as MatchGridCell).CurrentRectPoint));
            return targets;
        }
        public static void Reset() => EndGameSelectedCell.Clear();
    }
}