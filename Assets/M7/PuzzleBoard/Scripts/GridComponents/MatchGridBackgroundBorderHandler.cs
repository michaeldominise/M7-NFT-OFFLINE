using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;

namespace M7.Match
{
    public class MatchGridBackgroundBorderHandler : MonoBehaviour
    {
        public bool isForeGround;
        public GameObject backgroundTile;
        public List<Rule> rules = new List<Rule>();

        public GameObject GetBorder(Neighbours neighbours)
        {
            foreach(var rule in rules)
            {
                if(neighbours.HasFlag(rule.hasTiles) && (neighbours & rule.deadCells) == 0)
                {
                    return rule.borderTile;
                }
            }

            return null;
        }

        public void Instantiate(GameObject obj, Vector2 worldPos)
        {
            var puzzleOverlay = Instantiate(obj, worldPos, Quaternion.identity, transform).GetComponent<PuzzleOverlayHandler>();
            if(isForeGround)
                puzzleOverlay.InitForeGround();
        }

        public void SpawnBackground(Vector2 worldPos)
        {
            Instantiate(backgroundTile, worldPos, Quaternion.identity, transform);
        }
    }

    [System.Serializable]
    public class Rule
    {
        /// <summary>
        /// Cells that may or may not have tiles
        /// This can be empty or not
        /// </summary>
        public Neighbours hasTiles;
        /// <summary>
        /// Cells that are not passable
        /// Also out of bounds cells
        /// </summary>
        public Neighbours deadCells;
        /// <summary>
        /// Object to instantiate for the rule
        /// </summary>
        public GameObject borderTile;
    }

    [System.Flags]
    public enum Neighbours : short
    {
        None = 0,
        Left = 1 << 0,
        Top = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3,
        TopLeft = 1 << 4,
        TopRight = 1 << 5,
        BottomLeft = 1 << 6,
        BottomRight = 1 << 7,
        Center = 1 << 8,
        AllCardinal = ~(-1 << 4),
        All = ~(-1 << 9)
    }
}