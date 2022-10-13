using System;
using System.Collections.Generic;
using M7.Match;
using M7.Skill;

namespace M7.PuzzleBoard.Scripts.SpecialTiles
{
    [Serializable]
    public class SpecialTileComboData
    {
        public List<CellType> cellTypeComboList;
        public SkillObject skillToExecute;

        public bool HasMatch(List<CellType> cells)
        {
            var matchCount = 0;
            var cellTypeComboListCopy = new List<CellType>(cellTypeComboList);
            foreach (var cell in cells)
            {
                if (cellTypeComboListCopy.Contains(cell))
                {
                    matchCount++;
                    cellTypeComboListCopy.Remove(cell);
                    if(matchCount > 1)
                        return true;
                }
            }

            return false;
        }
    }
}