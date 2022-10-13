using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace M7.Tools
{
    /// <summary>
    /// Custom Visual element for grid view
    /// </summary>
    public class GridView : VisualElement
    {
        public Action<Vector2Int, ToolMode> onCellClick;
        public bool enabled;

        private GridDimension _dimension;

        private Vector2Int _selectedCellCoord = new Vector2Int();

        private VisualElement _selectedCell;

        private Vector2 _tileSize = Vector2.zero;
        private Vector2Int _tileBrushSize = Vector2Int.one;

        public Vector2Int CellBrushSize
        {
            set
            {
                _tileBrushSize = value;
                EditorApplication.delayCall += () =>
                {
                    _selectedCell.style.width = TileSize.x * _tileBrushSize.x;
                    _selectedCell.style.height = TileSize.y * _tileBrushSize.y;
                };
            }
        }

        public void SetDimension(GridDimension dimension, bool clear = false)
        {
            if(clear)
            {
                Clear();
            }

            _dimension = dimension;
            for(int i = 0; i < dimension.height; i++)
            {
                AddRow();
                for(int u = 0; u < dimension.width; u++)
                {
                    AddCell(i, u);
                }
            }

            //_selectedCell = new VisualElement();
            //_selectedCell.AddToClassList("cell-selected");
            //_selectedCell.style.position = Position.Absolute;
            //_selectedCell.pickingMode = PickingMode.Ignore;
            //_selectedCell.style.visibility = Visibility.Hidden;
            //this.Add(_selectedCell);

            EditorApplication.delayCall += () =>
            {
                _tileSize = new Vector2(this.resolvedStyle.width / dimension.width, this.resolvedStyle.height / dimension.height);
            };
        }
        
        public void AddRow()
        {
            VisualElement row = new VisualElement();
            row.AddToClassList("puzzle-row");
            Add(row);
        }

        public void AddCell(int row, int col)
        {
            if(row >= childCount)
            {
                Debug.LogWarning("Row cannot be found.");
            }
            VisualElement cell = new VisualElement();
            cell.AddToClassList("puzzle-cell");

            this[row].Add(cell);
        }

        public void PaintCell(int row, int col, Texture2D image)
        {
            this[row][col].style.backgroundImage = image;
        }

        public void PaintCell(int row, int col, Action<VisualElement, Vector2> paintAction)
        {
            paintAction?.Invoke(this[row][col], TileSize);
        }

        public void EraseCell(int row, int col)
        {
            this[row][col].Clear();
            this[row][col].style.backgroundImage = null;
        }

        public void EraseCell(int row, int col, Action<VisualElement> eraseAction)
        {
            eraseAction?.Invoke(this[row][col]);
        }

        private Vector2 TileSize
        {
            get
            {
                return new Vector2(this.resolvedStyle.width / _dimension.width, this.resolvedStyle.height / _dimension.height);
            }
        }

        public void UpdateSelectedCellBox()
        {
            EditorApplication.delayCall += () =>
            {
                _selectedCell.style.width = TileSize.x * _tileBrushSize.x;
                _selectedCell.style.height = TileSize.y * _tileBrushSize.y;
            };
        }

        /// <summary>
        /// Factory class for this element to be shown in UI Builder
        /// </summary>
        #region UXML
        public new class UxmlFactory : UxmlFactory<GridView, UxmlTraits> { }
        #endregion
    }

    public class GridDimension
    {
        public int width;
        public int height;

        public GridDimension(int square)
        {
            this.width = square;
            this.height = square;
        }

        public GridDimension(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    public enum ToolMode
    {
        Paint,
        Erase
    }
}