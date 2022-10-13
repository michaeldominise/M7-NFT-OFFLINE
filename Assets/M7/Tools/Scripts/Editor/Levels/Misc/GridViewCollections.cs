using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace M7.Tools
{
    /// <summary>
    /// Collection of grid views visual elements
    /// This just does the normal grid view functions but does it
    /// to all the enabled grid views
    /// </summary>
    public class GridViewCollections : VisualElement
    {
        private List<GridView> _gridViews = new List<GridView>();

        public Action<Vector2Int, ToolMode> onCellClick;

        private GridDimension _dimension;

        private Vector2Int _selectedCellCoord = new Vector2Int();

        private VisualElement _selectedCell;

        private Vector2 _tileSize = Vector2.zero;
        private Vector2Int _tileBrushSize = Vector2Int.one;
        private int _activeGridIndex = 0;

        public void Add(GridView gridView)
        {
            gridView.style.position = Position.Absolute;
            //gridView.style.top = 0;
            //gridView.style.left = 0;
            //gridView.style.bottom = 0;
            //gridView.style.right = 0;
            _gridViews.Add(gridView);
            var grids = this.Q<VisualElement>("grids");
            if (this.Q<VisualElement>("grids") == null)
            {
                grids = new VisualElement();
                grids.style.position = Position.Absolute;
                grids.style.left = 0;
                grids.style.right = 0;
                grids.style.bottom = 0;
                grids.style.top = 0;
                grids.name = "grids";
                this.Add(grids);
                grids.SendToBack();
            }

            gridView.style.height = Length.Percent(100f);
            gridView.style.width = Length.Percent(100f);

            grids.Add(gridView);
        }

        public void EnableGrid(int gridIndex)
        {
            _activeGridIndex = gridIndex;
        }

        public int GridCount => _gridViews.Count;

        public GridView EnabledGrid => _gridViews[_activeGridIndex];

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
            if (clear)
            {
                Clear();
            }

            _dimension = dimension;

            for (int i = 0; i < dimension.height; i++)
            {
                AddRow();
                for (int u = 0; u < dimension.width; u++)
                {
                    AddCell(i, u);
                }
            }

            _selectedCell = new VisualElement();
            _selectedCell.AddToClassList("cell-selected");
            _selectedCell.style.position = Position.Absolute;
            _selectedCell.pickingMode = PickingMode.Ignore;
            _selectedCell.style.visibility = Visibility.Hidden;
            this.Add(_selectedCell);

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
            if (row >= childCount)
            {
                Debug.LogWarning("Row cannot be found.");
            }
            VisualElement cell = new VisualElement();
            cell.AddToClassList("puzzle-cell");

            cell.RegisterCallback<MouseEnterEvent>(
                e =>
                {
                    _selectedCell.style.left = col * TileSize.x;
                    _selectedCell.style.top = row * TileSize.y;
                    _selectedCell.style.visibility = Visibility.Visible;
                    _selectedCellCoord.x = col;
                    _selectedCellCoord.y = row;

                    if (_tileBrushSize.x + col > _dimension.width)
                    {
                        return;
                    }
                    if (_tileBrushSize.y + row > _dimension.height)
                    {
                        return;
                    }

                    if (e.pressedButtons == 1)
                    {
                        onCellClick?.Invoke(new Vector2Int(col, row), ToolMode.Paint);
                    }
                    else if (e.pressedButtons == 2)
                    {
                        onCellClick?.Invoke(new Vector2Int(col, row), ToolMode.Erase);
                    }
                });
            cell.RegisterCallback<MouseOutEvent>(
                e => _selectedCell.style.visibility = Visibility.Hidden);
            cell.RegisterCallback<MouseDownEvent>(
                e =>
                {
                    if (_tileBrushSize.x + col > _dimension.width)
                    {
                        return;
                    }
                    if (_tileBrushSize.y + row > _dimension.height)
                    {
                        return;
                    }
                    if (e.pressedButtons == 1)
                    {
                        onCellClick?.Invoke(new Vector2Int(col, row), ToolMode.Paint);
                    }
                    else if (e.pressedButtons == 2)
                    {
                        onCellClick?.Invoke(new Vector2Int(col, row), ToolMode.Erase);
                    }
                });

            this[row].Add(cell);
        }

        public void PaintCell(int row, int col, Texture2D image)
        {
            EnabledGrid.PaintCell(row, col, image);
        }

        public void PaintCell(int row, int col, Action<VisualElement, Vector2> paintAction)
        {
            paintAction?.Invoke(EnabledGrid[row][col], TileSize);
        }

        public void EraseCell(int row, int col)
        {
            EnabledGrid[row][col].Clear();
            EnabledGrid[row][col].style.backgroundImage = null;
        }

        public void EraseCell(int row, int col, Action<VisualElement> eraseAction)
        {
            eraseAction?.Invoke(EnabledGrid[row][col]);
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

        public void ClearGrids()
        {
            _gridViews.Clear();
            this.Q<VisualElement>("grids")?.Clear();
        }

        #region UXML
        public new class UxmlFactory : UxmlFactory<GridViewCollections, UxmlTraits> { }
        #endregion
    }
}