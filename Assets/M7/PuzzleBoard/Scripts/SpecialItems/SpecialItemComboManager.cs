using M7.GameRuntime;
using M7.Match;
using M7.Skill;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace M7.PuzzleBoard.Scripts.SpecialTiles
{
    public class SpecialItemComboManager : MonoBehaviour
    {
        public static SpecialItemComboManager Instance => BattleManager.Instance.SpecialItemComboManager;
        
        [SerializeField] private List<SpecialTileComboData> specialTileComboList;

        public SkillObject GetSpecialCellCombo(List<CellType> cells) => specialTileComboList.FirstOrDefault(x => x.HasMatch(cells))?.skillToExecute;
    }
}