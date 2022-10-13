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
    public class TargetFilterItemCustom_TileDistance : TargetFilterItemCustom<MatchGridCell>
    {
        enum ConditionType { GreaterThan, LessThan, Equal, GreaterThanOrEqual, LessThanOrEqual }

        [SerializeField] Vector2Int rectPointDistance;
        [SerializeField] ConditionType condition;
        public override string DebugText => $"Caster to target tile distance is {condition} to {rectPointDistance}";
        public override bool IsValidTarget(Component caster, MatchGridCell target)
        {
            var casterTile = caster as MatchGridCell;
            var distance = casterTile.CurrentRectPoint - target.CurrentRectPoint;
            var isValid = true;
            switch (condition)
            {
                case ConditionType.GreaterThan:
                    isValid = Mathf.Abs(distance.X) > rectPointDistance.x && Mathf.Abs(distance.Y) > rectPointDistance.y;
                    break;
                case ConditionType.LessThan:
                    isValid = Mathf.Abs(distance.X) < rectPointDistance.x && Mathf.Abs(distance.Y) < rectPointDistance.y;
                    break;
                case ConditionType.Equal:
                    isValid = Mathf.Abs(distance.X) == rectPointDistance.x && Mathf.Abs(distance.Y) == rectPointDistance.y;
                    break;
                case ConditionType.GreaterThanOrEqual:
                    isValid = Mathf.Abs(distance.X) >= rectPointDistance.x && Mathf.Abs(distance.Y) >= rectPointDistance.y;
                    break;
                case ConditionType.LessThanOrEqual:
                    isValid = Mathf.Abs(distance.X) <= rectPointDistance.x && Mathf.Abs(distance.Y) <= rectPointDistance.y;
                    break;
            }

            return isValid;
        }
    }
}