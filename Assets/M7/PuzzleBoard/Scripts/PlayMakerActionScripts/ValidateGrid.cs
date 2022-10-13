using System.Collections;
using System.Collections.Generic;
using Gamelogic.Grids;
using HutongGames.PlayMaker;
using UnityEngine;

namespace M7.Match.PlaymakerActions
{
    [ActionCategory("M7/Match")]
    public class ValidateGrid : FsmStateAction
    {
        public PossibleMoveList possibleMoveList;

        public FsmEvent OnValidGrid;
        public FsmEvent OnInvalidGrid;

        public override void OnEnter()
        {
            var isGridValid = false;
            if (possibleMoveList.Value.Count > 0)
                isGridValid = true;
            else
            {
                var grid = PuzzleBoardManager.Instance.ActiveGrid.Grid;
                foreach (var rectPoint in grid)
                {
                    var cell = PuzzleBoardManager.Instance.ActiveGrid.GetMatchGridTile(rectPoint);
                    if (cell != null && cell.CellTypeContainer.CellType.ElementType == Skill.SkillEnums.ElementFilter.Special)
                    {
                        isGridValid = true;
                        break;
                    }
                }
            }

            if (isGridValid)
                Fsm.Event(OnValidGrid);
            else
                Fsm.Event(OnInvalidGrid);
        }
    }
}