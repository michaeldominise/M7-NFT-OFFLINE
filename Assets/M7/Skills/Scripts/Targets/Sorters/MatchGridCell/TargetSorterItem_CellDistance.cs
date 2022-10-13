using M7.GameRuntime;
using M7.Match;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Skill
{
    public class TargetSorterItem_CellDistance : TargetSorterItem<MatchGridCell>
    {
        public override object SortValue<CasterType>(CasterType caster, MatchGridCell target)
        {
            var casterTile = caster as MatchGridCell;
            if (caster == null)
                return 0;
            return Vector2.Distance(new Vector2(casterTile.CurrentRectPoint.X, casterTile.CurrentRectPoint.Y), new Vector2(target.CurrentRectPoint.X, target.CurrentRectPoint.Y));
        }
    }
}