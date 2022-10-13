using Gamelogic.Grids;
using System.Linq;
using M7.Match.PlaymakerActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.Skill;

namespace M7.Match
{
    public class CellType_DamageCondition_OnNearbyLock : CellType_DamageCondition
    {
        [SerializeField] int minDistance = 1;
        [SerializeField] SkillEnums.ElementFilter nearbyElementType = SkillEnums.ElementFilter.All;
        SkillEnums.BlockerType BlockerType => this.gameObject.transform.parent.GetComponent<CellType>().BlockerType;
        MatchGrid ActiveGrid => PuzzleBoardManager.Instance.ActiveGrid;

        public override bool CanDamage(DamageData damageData)
        {
            if (!base.CanDamage(damageData))
                return false;

            foreach (var item in damageData.chain)
            {
                if (item == null)
                    continue;
                if (CheckTargetDistance(item.CurrentRectPoint, damageData) && CheckElementFilter(item))
                    return true;
            }

            return false;
        }

        private bool CheckTargetDistance(RectPoint rectPointRef, DamageData damageData)
            => Vector2.Distance(rectPointRef.ToVector2Int(), damageData.targetCell.CurrentRectPoint.ToVector2Int()) <= minDistance;

        private bool CheckElementFilter(MatchGridCell cell)
            => cell.CellTypeContainer.Matches(nearbyElementType, true);

        /// <summary>
        /// Not actually Implemented yet. I thought it was drone, turns out its same as the boxes except Drones can be drop but not shuffable
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="rectPointRef"></param>
        private void DestroyNearbySameCellType(SkillEnums.ElementFilter elementType, RectPoint rectPointRef)
        {
            var rectPointUp = rectPointRef + RectPoint.North;
            var rectPointDown = rectPointRef + RectPoint.South;
            var rectPointLeft = rectPointRef + RectPoint.West;
            var rectPointRight = rectPointRef + RectPoint.East;

            var recPointList = new List<RectPoint>() { rectPointUp, rectPointDown, rectPointLeft, rectPointRight};

            foreach (var rectPoint in recPointList)
            {
                if (!ActiveGrid.Grid.Contains(rectPoint))
                    continue;

                var item = ActiveGrid.Grid[rectPoint];

                if (item == null)
                    continue;

                var itemCellType = item.CellTypeContainer.CellType;
                var itemRectPoint = item.CurrentRectPoint;
                if (itemCellType.BlockerType == SkillEnums.BlockerType.Drone && !item.CellHealth.IsDead && /*(*/itemCellType.ElementType == elementType/* || itemCellType.ElementType == SkillEnums.ElementFilter.BasicElements)*/)
                {
                    item.CellHealth.DealDamage(TileChainDamager.DEFAULT_DAMAGE);
                    DestroyNearbySameCellType(elementType, itemRectPoint);
                }

            }
        }
    }
}