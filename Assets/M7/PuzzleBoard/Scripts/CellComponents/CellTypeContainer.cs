/*
 * TileTypeContainer.cs
 * Author: Cristjan Lazar
 * Date: Oct 9, 2018
*/

using M7.Skill;
using UnityEngine;

namespace M7.Match {

    /// <summary>
    /// Component that holds a gameobject's tile type.
    /// </summary>
    public class CellTypeContainer : MonoBehaviour {

        [SerializeField] private CellType cellType;
        [SerializeField] private MatchGridCell matchGridCell;

        MatchGrid SecondaryGrid => PuzzleBoardManager.Instance.SecondaryGrid;
        MatchGridCell SecondaryCell => cellType.TileGridLocation == SkillEnums.CellGridLocation.Main ? SecondaryGrid.Grid[matchGridCell.CurrentRectPoint] : null;
        bool SecondaryCellIsMatchable => SecondaryCell == null || SecondaryCell.CellTypeContainer.CellType.IsMatchable;

        public CellType CellType {
            get { return cellType; }
            set {
                this.cellType = value;
                OnCellTypeChanged?.Invoke();
            }
        }

        public UnityEngine.Events.UnityEvent OnCellTypeChanged;

        bool Matches(int otherMatchValue, bool ignoreSecondaryCell = false)
        {
            var bitValue = cellType.MatchValue | otherMatchValue;
            var isMatch = cellType.IsMatchable && otherMatchValue != 0 && cellType.ElementType != 0 && (bitValue == cellType.MatchValue || bitValue == otherMatchValue);
            return isMatch && (ignoreSecondaryCell || SecondaryCellIsMatchable);
        }

        /// <summary>
        /// Returns true if the tile can form a match against a tile type.
        /// </summary>
        public bool Matches(CellType otherTileType, bool ignoreSecondaryCell = false)
        {
            return otherTileType.IsMatchable && Matches((int)otherTileType.ElementType, ignoreSecondaryCell);
        }

        public bool Matches(SkillEnums.ElementFilter otherElementType, bool ignoreSecondaryCell = false) => Matches((int)otherElementType, ignoreSecondaryCell);
        /// <summary>
        /// Returns true if the tile type matches the other match Id exactly.
        /// </summary>
        public bool MatchesExactly(SkillEnums.ElementFilter otherElementType, bool ignoreSecondaryCell = false)
        {
            return cellType.ElementType == otherElementType && (ignoreSecondaryCell || SecondaryCellIsMatchable);
        }

        /// <summary>
        /// Returns true if the tile type matches the other tile type exactly.
        /// </summary>
        public bool MatchesExactly(CellType otherTileType, bool ignoreSecondaryCell = false)
        {
            return MatchesExactly(otherTileType.ElementType, ignoreSecondaryCell);
        }
    }

}
