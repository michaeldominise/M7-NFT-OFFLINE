using Gamelogic.Grids;
using M7.Skill;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Match
{
    public abstract class CellType_DamageCondition : MonoBehaviour
    {
        [SerializeField] SkillEnums.CellDestroyType cellDestroyType = SkillEnums.CellDestroyType.All;

        public class DamageData
        {
            public MatchGridCell targetCell;
            public MatchGridCell mainCell;
            public MatchGridCell secondaryCell;
            public IEnumerable<MatchGridCell> chain;
            public SkillEnums.CellDestroyType cellDestroyType;

            public DamageData(MatchGridCell targetCell, IEnumerable<MatchGridCell> chain, SkillEnums.CellDestroyType cellDestroyType, MatchGridCell mainCell = null, MatchGridCell secondaryCell = null)
            {
                this.targetCell = targetCell;
                this.mainCell = mainCell ?? targetCell;
                this.secondaryCell = secondaryCell ?? targetCell.SecondaryCell;
                this.chain = chain;
                this.cellDestroyType = cellDestroyType;
            }
        }

        public virtual bool CanDamage(DamageData damageData) => cellDestroyType.HasFlag(damageData.cellDestroyType);
    }
}