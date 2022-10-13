using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using System.Linq;
using Sirenix.OdinInspector;
using M7.Match;

namespace M7.Skill
{
    [Serializable]
    public class TargetFilterItem_MatchGridTile : TargetFilterItem<MatchGridCell>
    {
        [SerializeField] protected SkillEnums.ElementFilter elementFilter = SkillEnums.ElementFilter.All;
        [SerializeField] protected SkillEnums.MatchGridTilePatternFilter patternFilter = SkillEnums.MatchGridTilePatternFilter.All;
        [SerializeField] protected SkillEnums.MatchGridTileSelectionFilter selectionFilter = SkillEnums.MatchGridTileSelectionFilter.All;
        public override string DebugText => $"Get MatchGridTile that ElementFilter: {elementFilter}, PatternFilter: {patternFilter}, SelectionFilter: {selectionFilter}{base.DebugText}.";

        public override bool IsValidTarget(Component caster, MatchGridCell target)
        {
            var cell = target;
            if (cell == null)
                return false;
            else if (cell.CellHealth.IsDead)
                return false;

            return base.IsValidTarget(caster, target) && FilterElement(caster, target) && FilterPattern(caster, target) && FilterSelection(caster, target);
        }
        public override List<MatchGridCell> GetTargets<CasterType>(CasterType caster, IEnumerable<MatchGridCell> initialTargets)
        {
            var list = initialTargets.Count() > 0 ? initialTargets : GetMatchGridTileList(caster);
            return list.Where(x => IsValidTarget(caster, x)).ToList();
        }

        #region MatchGridTile Filters
        public static List<MatchGridCell> GetMatchGridTileList(Component caster)
        {
            var list = new List<MatchGridCell>();
            list.AddRange(PuzzleBoardManager.Instance.ActiveGrid.Grid.Select(x => PuzzleBoardManager.Instance.ActiveGrid.Grid[x]));
            return list;
        }

        protected bool FilterElement(Component caster, MatchGridCell target) => (elementFilter | target.CellTypeContainer.CellType.ElementType) == elementFilter;
        protected bool FilterPattern(Component caster, MatchGridCell target)
        {
            var casterTile = caster as MatchGridCell;
            if ((patternFilter | SkillEnums.MatchGridTilePatternFilter.All) == patternFilter)
                return true;
            if ((patternFilter | SkillEnums.MatchGridTilePatternFilter.Horizontal) == patternFilter && casterTile.CurrentRectPoint.Y == target.CurrentRectPoint.Y)
                return true;
            if ((patternFilter | SkillEnums.MatchGridTilePatternFilter.Vertical) == patternFilter && casterTile.CurrentRectPoint.X == target.CurrentRectPoint.X)
                return true;

            var distance = casterTile.CurrentRectPoint - target.CurrentRectPoint;
            if ((patternFilter | SkillEnums.MatchGridTilePatternFilter.DiagonalLeftUpToRightDown) == patternFilter && (distance.X + distance.Y) == 0)
                return true;
            if ((patternFilter | SkillEnums.MatchGridTilePatternFilter.DiagonalLeftUpToRightDown) == patternFilter && distance.X == distance.Y)
                return true;

            return false;
        }
        protected bool FilterSelection(Component caster, MatchGridCell target) 
        {
            var casterTile = caster as MatchGridCell;
            if ((selectionFilter | SkillEnums.MatchGridTileSelectionFilter.All) == selectionFilter)
                return true;
            if ((selectionFilter | SkillEnums.MatchGridTileSelectionFilter.Caster) == selectionFilter && casterTile == target)
                return true;
            if ((selectionFilter | SkillEnums.MatchGridTileSelectionFilter.Others) == selectionFilter && casterTile != target)
                return true;
             return false;
        }
        #endregion
    }
}