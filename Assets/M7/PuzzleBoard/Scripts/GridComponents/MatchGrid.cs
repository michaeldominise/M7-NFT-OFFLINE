/*
 * MatchGrid.cs
 * Author: Cristjan Lazar
 * Date: Oct 9, 2018
 */

using System;
using UnityEngine;
using Gamelogic.Grids;
using DG.Tweening;
namespace M7.Match
{

    /// <summary>
    /// Class that contains a grid
    /// </summary>
    public class MatchGrid : MonoBehaviour
    {
        /// <summary>
        /// Used for checking which state the match grid should become active
        /// </summary>
        public GameObject tilePoolRootObject;
        public Transform root;
        public RectGrid<MatchGridCell> Grid { get; private set; }
        public IMap3D<RectPoint> Map { get; private set; }
        public int ColumnCount { get { return Grid.Width; } }
        public int RowCount { get { return Grid.Height; } }
        public bool IsInitialized { get { return Grid != null && Map != null; } }

        /// <summary>
        /// Assign to this match grid a specified grid and map.
        /// </summary>
        public void Initialize(RectGrid<MatchGridCell> grid, IMap3D<RectPoint> map)
        {
            Grid = grid;
            Map = map;
        }
        
#if UNITY_EDITOR
        public void OnDrawGizmosSelected()
        {
            if (!IsInitialized)
                return;

            Vector2 dimensions = Map.To2D().GetCellDimensions();

            foreach (var point in Grid)
                Gizmos.DrawWireCube(Map[point], dimensions);
        }
#endif
        public MatchGridCell GetMatchGridTile(RectPoint rp)
        {
            return Grid[rp];
        }
        
        public MatchGridCell GetLeftSideTile(RectPoint originRectPoint)
        {
            try
            {
                var upperTile = new RectPoint(originRectPoint.X + 1, originRectPoint.Y);
                return Grid[upperTile];
            }
            catch (Exception e)
            {
                return null;
            }
        }
        
        public MatchGridCell GetRightSideTile(RectPoint originRectPoint)
        {
            try
            {
                var upperTile = new RectPoint(originRectPoint.X - 1, originRectPoint.Y);
                return Grid[upperTile];
            }
            catch (Exception e)
            {
                return null;
            }
        }
        
        public MatchGridCell GetUpperTile(RectPoint originRectPoint)
        {
            try
            {
                var upperTile = new RectPoint(originRectPoint.X, originRectPoint.Y + 1);
                return Grid[upperTile];
            }
            catch (Exception e)
            {
                return null;
            }
        }
        
        public MatchGridCell GetLowerTile(RectPoint originRectPoint)
        {
            try
            {
                var upperTile = new RectPoint(originRectPoint.X, originRectPoint.Y - 1);
                return Grid[upperTile];
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}



