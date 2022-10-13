using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using static M7.Skill.SkillEnums;

namespace M7.Match
{
    [CreateAssetMenu(menuName = "BGGamesCore/Match/DataContainers/PossibleMoveList")]
    public class PossibleMoveList : ScriptableObject
    {
        public List<PossibleMove> Value = new List<PossibleMove>();

        public PossibleMove GetMove(float highestCountPriority = 0)
        {
            highestCountPriority = Mathf.Clamp(highestCountPriority, 0, 1);
            if (Value.Count == 0)
                return null;

            var rnd = UnityEngine.Random.Range(0, Mathf.CeilToInt(Value.Count * highestCountPriority));
            return Value[rnd];
        }

        public List<PossibleMove> GetPossibleMove(MatchGridCell targetCell, MatchGridCell destinationCell)
        {
            var possilbeMoves = new List<PossibleMove>();

            foreach (var move in Value)
                if ((move.TargetCell == targetCell && move.DestinationCell == destinationCell) || (move.TargetCell == destinationCell && move.DestinationCell == targetCell))
                    possilbeMoves.Add(move);

            return possilbeMoves;
        }
    }

    [Serializable]
    public class PossibleMove
    {
        /// <summary>
        /// The tiles in this move.
        /// </summary>
        public List<MatchGridCell> cells = new List<MatchGridCell>();
        /// <summary>
        /// The first tile taped by the user
        /// </summary>
        [ShowInInspector] public MatchGridCell TargetCell { get; private set; }
        /// <summary>
        /// The swap destination of the tile. Only applicable in swap gameplay.
        /// </summary>
        [ShowInInspector] public MatchGridCell DestinationCell { get; private set; }

        [ShowInInspector] public ElementFilter ElementType => cells?.FirstOrDefault(x => x && x.CellTypeContainer.CellType.ElementType != ElementFilter.All)?.CellTypeContainer.CellType.ElementType ?? ElementFilter.None;

        public PossibleMove(MatchGridCell targetCell, MatchGridCell destinationCell)
        {
            this.TargetCell = targetCell;
            this.DestinationCell = destinationCell;
        }
    }
}
