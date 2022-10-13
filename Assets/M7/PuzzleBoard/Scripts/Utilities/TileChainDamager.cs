using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamelogic.Grids;
using M7.Skill;
using M7.GameRuntime;
using System.Linq;

namespace M7.Match {

    public static class TileChainDamager {

        public static readonly int DEFAULT_DAMAGE = 1;

        public static MatchGrid ActiveGrid => PuzzleBoardManager.Instance.ActiveGrid;
        public static MatchGrid SecondaryGrid => PuzzleBoardManager.Instance.SecondaryGrid;

        /// <summary>
        /// Deal the default damage to every tile in a chain.
        /// </summary>
        public static bool DamageTileChain (IEnumerable<MatchGridCell> chain) {
            var mainGrid = ActiveGrid.Grid;
            var secondaryGrid = SecondaryGrid.Grid;
            var pointList = mainGrid.ToPointList();
            var value = false;

            foreach (var rectPoint in pointList)
            {

                var mainCell = mainGrid[rectPoint];
                if (mainCell == null)
                {
                    var secondaryCell = secondaryGrid[rectPoint];
                    if (secondaryCell != null)
                        secondaryCell.TryDealDamage(new CellType_DamageCondition.DamageData(secondaryCell, chain, SkillEnums.CellDestroyType.Normal), DEFAULT_DAMAGE);
                    continue;
                }
                
                value = true;
                if (OmniSpawnerManager.Instance.IsRectPointHasRequest(rectPoint))
                    mainCell.CellHealth.AddHealth(DEFAULT_DAMAGE);
                mainCell.TryDealDamage(new CellType_DamageCondition.DamageData(mainCell, chain, SkillEnums.CellDestroyType.Normal), DEFAULT_DAMAGE);
            }

            return value;
        }
    }

}

