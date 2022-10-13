using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using M7.Match;
using System.IO;
using System;
using UnityEditor.UIElements;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using M7.GameRuntime;
using M7.GameData;
using Fasterflect;
using M7.Tools.Utility;

namespace M7.Tools
{
    /// <summary>
    /// Main Entry class of level editor
    /// </summary>
    public class LevelEditor : EditorWindow
    {
        #region UXML
        /// <summary>
        /// UI asset of the editor
        /// </summary>
        public VisualTreeAsset _uxml;
        /// <summary>
        /// Stylesheet asset of the editor
        /// </summary>
        public StyleSheet _stylesheet;
        #endregion

        // Values assigned in the inspector
        #region Data
        /// <summary>
        /// Normal cells
        /// </summary>
        public CellBag baseCells;
        /// <summary>
        /// Level data being used for preview mode
        /// </summary>
        public LevelData previewLevelData;
        #endregion


        #region Constants
        private const string defaultCreateLocation = "Assets/M7/PuzzleBoard/ScriptableObjects/Levels";
        #endregion

        public CellType testCell;
        public string cellsLocation = "Assets/M7/PuzzleBoard/Prefabs/Cell";

        private VisualElement _puzzleArea;
        private GridViewCollections _gridViewCollection;
        private VisualElement _cellsContainer;
        
        private VisualElement _selectedBrushPreview;

        /// <summary>
        /// UI elements inside the toolbar
        /// </summary>
        #region Toolbar UI
        private ToolbarButton _loadButton;
        private ToolbarButton _saveButton;
        private ToolbarButton _saveAsButton;
        private ToolbarMenu _createNewMenu;
        private ToolbarButton _previewButton;
        private ObjectField _selectedDataField;

        private Toolbar _gridToolbar;
        private TextField _gridNameTextField;

        private ObjectField _cellBagField;
        private ObjectField _goalsDataField;
        #endregion

        private GridDimension _dimension = new GridDimension(0);
        private CellType _selectedCellBrush;
        private CellItem _selectedCellItem;

        private Vector2Int _origBoardSize;
        private bool _autoSave;
        private bool _isDirty;

        private SerializedObject _selectedSerializedObject;

        /// <summary>
        /// Entry point to show the editor
        /// </summary>
        [MenuItem("M7/Level Editor")]
        public static void ShowEditor()
        {
            LevelEditor wnd = GetWindow<LevelEditor>();
            wnd.titleContent = new GUIContent("Level Editor");
        }

        /// <summary>
        /// Initialize the UIs and their callbacks
        /// </summary>
        public void CreateGUI()
        {
            _uxml.CloneTree(rootVisualElement);

            _puzzleArea = rootVisualElement.Q<VisualElement>("puzzle-area");
            _gridViewCollection = rootVisualElement.Q<VisualElement>("grid-collection") as GridViewCollections;
            _cellsContainer = rootVisualElement.Q<ScrollView>("tiles-scrollview").contentContainer;
            _selectedBrushPreview = rootVisualElement.Q<VisualElement>("selected-brush-preview");

            InitializeToolbar();
            InitializeGridToolbar();

            EditorUtility.DisplayProgressBar("Level Editor", "Initializing Cell Sets...", 0f);
            InitializeCellSets();
            EditorUtility.DisplayProgressBar("Level Editor", "Initializing Grid...", 0.5f);
            InitializeGrid();
            EditorUtility.DisplayProgressBar("Level Editor", "Done", 1f);
            EditorUtility.ClearProgressBar();

            InitializePuzzleArea();

            
        }

        #region Puzzle Area
        private bool mousedown;
        /// <summary>
        /// Initializes the mouse callbacks in the puzzle area
        /// Pan, Zoom and center fit
        /// </summary>
        private void InitializePuzzleArea()
        {
            _puzzleArea.RegisterCallback<MouseDownEvent>((e) =>
            {
                mousedown = true;
            });

            _puzzleArea.RegisterCallback<MouseMoveEvent>((e) =>
            {
                if(mousedown && e.pressedButtons == 4)
                {
                    _gridViewCollection.style.left = _gridViewCollection.style.left.value.value + e.mouseDelta.x;
                    _gridViewCollection.style.top = _gridViewCollection.style.top.value.value + e.mouseDelta.y;
                }
            });
            _puzzleArea.RegisterCallback<MouseLeaveEvent>(e => mousedown = false);
            _puzzleArea.RegisterCallback<MouseUpEvent>(
                e => mousedown = false);
            _puzzleArea.RegisterCallback<WheelEvent>((e) =>
            {
                _gridViewCollection.style.width = _gridViewCollection.style.width.value.value + e.delta.y;
                _gridViewCollection.style.height = _gridViewCollection.style.height.value.value + e.delta.y;
                _gridViewCollection.UpdateSelectedCellBox();
            });

            rootVisualElement.Q<Button>("center-fit-button").clicked += CenterFit;
            rootVisualElement.Q<Button>("randomize-button").clicked += Randomize;
            rootVisualElement.Q<Button>("clear-button").clicked += ClearGrid;
        }

        /// <summary>
        /// Fit the puzzle to the available space in the puzzle area and centers it
        /// This does not use the lower portion space to not block the instruction text at the bottom
        /// </summary>
        private void CenterFit()
        {
            if(_selectedDataField.value == null)
            {
                return;
            }

            var puzzleAreaSize = new Vector2Int((int)_puzzleArea.resolvedStyle.width - 100, (int)_puzzleArea.resolvedStyle.height - 75);
            var scale = Mathf.Min(puzzleAreaSize.x / (float)_origBoardSize.x, puzzleAreaSize.y / (float)_origBoardSize.y);
            var newBoardSize = new Vector2Int((int)(_origBoardSize.x * scale), (int)(_origBoardSize.y * scale));
            _gridViewCollection.style.width = newBoardSize.x;
            _gridViewCollection.style.height = newBoardSize.y;
            _gridViewCollection.style.left = (puzzleAreaSize.x - newBoardSize.x) / 2;
            _gridViewCollection.style.top = (puzzleAreaSize.y - newBoardSize.y) / 2;
            _gridViewCollection.UpdateSelectedCellBox();
        }

        /// <summary>
        /// Fills all empty cells in the puzzle with a random cell
        /// </summary>
        private void Randomize()
        {
            if (_selectedDataField.value == null)
            {
                return;
            }

            if (EditorUtility.DisplayDialog("Level Editor", "This will fill all empty cells with a random tile. Proceed?", "Yes", "No"))
            {
                MatchGridEditorSavedLevelData levelData = _selectedDataField.value as MatchGridEditorSavedLevelData;
                if (levelData == null)
                {
                    return;
                }


                for (int i = 0; i < levelData.RowCount; i++)
                {
                    for (int u = 0; u < levelData.ColumnCount; u++)
                    {
                        var tile = levelData.MainGrid.Tiles[u, i];

                        if (tile == null)
                        {
                            CellType randomTile = baseCells.cellBagItems[UnityEngine.Random.Range(0, baseCells.cellBagItems.Length )].cellType;
                            levelData.MainGrid.Tiles[u, i] = randomTile;

                            var sprite = randomTile.Sprite;
                            if (sprite != null)
                            {
                                _gridViewCollection.PaintCell(i, u, (ve, tileSize) => DrawCell(ve, randomTile, tileSize));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes all cells in the puzzle
        /// </summary>
        private void ClearGrid()
        {
            if(_selectedDataField.value == null)
            {
                return;
            }

            if (EditorUtility.DisplayDialog("Warning", "This will clear all cells in the puzzle. Proceed?", "Yes", "No"))
            {
                MatchGridEditorSavedLevelData levelData = _selectedDataField.value as MatchGridEditorSavedLevelData;
                if (levelData == null)
                {
                    return;
                }


                for (int i = 0; i < levelData.RowCount; i++)
                {
                    for (int u = 0; u < levelData.ColumnCount; u++)
                    {
                        levelData.MainGrid.Tiles[u, i] = null;
                        _gridViewCollection.EraseCell(i, u);
                    }
                }
            }
        }
        #endregion

        #region Toolbar
        /// <summary>
        /// Initializes the UI's inside the toolbar
        /// Registers callbacks for all the buttons
        /// </summary>
        private void InitializeToolbar()
        {
            _selectedDataField = rootVisualElement.Q<ObjectField>("selected-data");
            _loadButton = rootVisualElement.Q<ToolbarButton>("load-button");
            _saveButton = rootVisualElement.Q<ToolbarButton>("save-button");
            _saveAsButton = rootVisualElement.Q<ToolbarButton>("save-as-button");
            _createNewMenu = rootVisualElement.Q<ToolbarMenu>("create-new-menu");
            _previewButton = rootVisualElement.Q<ToolbarButton>("preview-button");

            _selectedDataField.objectType = typeof(MatchGridEditorSavedLevelData);
            var objectPickerField = _selectedDataField[0][1];
            objectPickerField.style.backgroundImage = null;
            objectPickerField.style.width = 45;
            var label = new Label("Load");
            label.focusable = false;
            label.pickingMode = PickingMode.Ignore;
            label.style.flexGrow = 1;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            objectPickerField.Add(label);

            _selectedDataField.RegisterValueChangedCallback(LoadData);

            _createNewMenu.RegisterCallback<ClickEvent>(CreateNew);
            _previewButton.clicked += PreviewLevel;
            _saveButton.clicked += SaveLevel;

            var autosave = rootVisualElement.Q<ToolbarToggle>("auto-save");
            _autoSave = autosave.value;
            autosave.RegisterValueChangedCallback(OnAutoSaveToggleValueChange);

            _cellBagField = rootVisualElement.Q<ObjectField>("cell-bag");
            _cellBagField.objectType = typeof(CellBag);
            _cellBagField.RegisterValueChangedCallback(OnCellBagChange);
            _goalsDataField = rootVisualElement.Q<ObjectField>("goals");
            _goalsDataField.objectType = typeof(LevelGoalsData);
            _goalsDataField.RegisterValueChangedCallback(OnGoalsDataChange);
        }

        /// <summary>
        /// Change the level data cell bag
        /// </summary>
        /// <param name="evt"></param>
        private void OnCellBagChange(ChangeEvent<UnityEngine.Object> evt)
        {
            LevelDataCellBag = evt.newValue as CellBag;
        }

        /// <summary>
        /// Change the goals data
        /// </summary>
        /// <param name="evt"></param>
        private void OnGoalsDataChange(ChangeEvent<UnityEngine.Object> evt)
        {
            GoalsData = evt.newValue as LevelGoalsData;
        }

        /// <summary>
        /// Toggle the auto save value
        /// </summary>
        /// <param name="evt"></param>
        private void OnAutoSaveToggleValueChange(ChangeEvent<bool> evt)
        {
            _autoSave = evt.newValue;
        }

        /// <summary>
        /// Save the level
        /// This refreshes the assetdatabase so in some cases,
        /// it might freeze the editor for a while
        /// </summary>
        private void SaveLevel()
        {
            if(_selectedDataField.value == null)
            {
                return;
            }

            EditorUtility.SetDirty(_selectedDataField.value);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ToggleWindowTitleDirty(false);
        }

        /// <summary>
        /// Enter preview mode
        /// Saves the current level to the <see cref="previewLevelData"/>
        /// This opens the puzzle battle scene and set the <see cref="LevelPreview"/> and enter play mode
        /// On play mode, the level manager will always use the <see cref="previewLevelData"/> when isInPreview is true
        /// This will fail if there's no <see cref="LevelPreview"/> gameobject in the scene
        /// </summary>
        private void PreviewLevel()
        {
            if(_selectedDataField.value == null)
            {
                return;
            }

            if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/M7/GameRuntime/Scenes/M7Puzzle_BattleScene.unity");
                var preview = FindObjectOfType<LevelPreview>();
                preview.isInPreview = true;
                preview.previewLevelData.SetFieldValue("matchGridEditorSavedLevelData", _selectedDataField.value);
                EditorApplication.EnterPlaymode();
            }
        }

        /// <summary>
        /// Load the level data selected
        /// This will initialize the the UI and populate the grid
        /// replacing all cells and values
        /// </summary>
        /// <param name="evt"></param>
        private void LoadData(ChangeEvent<UnityEngine.Object> evt)
        {
            MatchGridEditorSavedLevelData levelData = evt.newValue as MatchGridEditorSavedLevelData;

            

            if (levelData == null)
            {
                _dimension = new GridDimension(0);
            }
            else
            {
                _selectedSerializedObject = new SerializedObject(levelData);
                
                _dimension = new GridDimension(levelData.ColumnCount, levelData.RowCount);
            }
            InitializeGrid();
            InitializeGridToolbar();
            Rebind();

            ToggleCellSets();

            if (levelData != null)
            {
                PopulateGrid();
                CenterFit();
            }

            _gridViewCollection.CellBrushSize = _selectedCellBrush == null ? Vector2Int.one : _selectedCellBrush.TileDimension;
        }

        /// <summary>
        /// Reinitializes new values
        /// </summary>
        private void Rebind()
        {
            _gridNameTextField.SetEnabled(_currentGrid != 0);
            _gridNameTextField.SetValueWithoutNotify(SelectedGridName);
            _cellBagField.value = LevelDataCellBag;
            _goalsDataField.value = GoalsData;
        }

        private void SaveAs()
        {

        }

        /// <summary>
        /// Creates new level data
        /// Opens a save file panel to save the actual file
        /// Initializes the whole grid replacing all cells
        /// </summary>
        /// <param name="evt"></param>
        private void CreateNew(ClickEvent evt)
        {
            int height = 8;
            int width = 8;
            string savePath = EditorUtility.SaveFilePanel("Save Level Data", defaultCreateLocation, "NewMatchLevelData", "asset");
            if (savePath.Length != 0)
            {
                string saveAssetPath = EditorUtils.GetAssetPathFromAbsolutePath(savePath);
                string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(saveAssetPath);
                MatchGridEditorSavedLevelData levelData = ScriptableObject.CreateInstance<MatchGridEditorSavedLevelData>();
                levelData.RowCount = height;
                levelData.ColumnCount = width;
                AssetDatabase.CreateAsset(levelData, uniqueAssetPath);
                AssetDatabase.SaveAssets();

                _selectedDataField.value = levelData;
                _dimension = new GridDimension(width, height);
                InitializeGrid();
            }
        }
        #endregion

        #region Grid Toolbar
        private int _currentGrid = 0;
        /// <summary>
        /// Initializes the grid toolbar UI
        /// Registers callback for all buttons and grid toggles
        /// </summary>
        private void InitializeGridToolbar()
        {
            _gridNameTextField = rootVisualElement.Q<TextField>("grid-name");
            _gridNameTextField.RegisterValueChangedCallback(OnGridNameChange);
            _gridToolbar = rootVisualElement.Q<Toolbar>("grid-toolbar");
            _gridToolbar.Clear();
            var mainToggle = new ToolbarToggle();
            mainToggle.text = "Main";
            mainToggle.RegisterCallback<ClickEvent>((evt) => { evt.PreventDefault(); ToggleGrid(mainToggle); });
            _gridToolbar.Add(mainToggle);

            var levelData = _selectedDataField.value as MatchGridEditorSavedLevelData;
            if(levelData == null)
            {
                _gridToolbar.Clear();
            } else
            {
                for(int i = 0; i < levelData.SecondaryGrids.Count; i++)
                {
                    var gridToggle = new ToolbarToggle();
                    gridToggle.text = levelData.SecondaryGrids[i].name;
                    gridToggle[0].focusable = false;
                    gridToggle.RegisterCallback<ClickEvent>((evt) => { evt.PreventDefault(); ToggleGrid(gridToggle); });
                    var deleteButton = new Button();
                    deleteButton.text = "x";
                    deleteButton.clicked += () => { RemoveSecondaryGrid(gridToggle); };
                    gridToggle.Add(deleteButton);
                    _gridToolbar.Add(gridToggle);
                }

                var addGridButton = new ToolbarButton();
                addGridButton.text = "+";
                addGridButton.name = "add-grid-button";
                addGridButton.style.marginLeft = 3;
                addGridButton.clicked += AddSecondaryGrid;

                _gridToolbar.Add(addGridButton);
            }

            var settingsToggle = rootVisualElement.Q<ToolbarToggle>("settings-toggle");
            settingsToggle.RegisterValueChangedCallback(OnSettingsToggleValueChange);
            ToggleSettings(false);
        }

        /// <summary>
        /// Show settings callback
        /// </summary>
        /// <param name="evt"></param>
        private void OnSettingsToggleValueChange(ChangeEvent<bool> evt)
        {
            bool show = evt.newValue;
            ToggleSettings(show);
        }

        /// <summary>
        /// Toggle display of settings
        /// </summary>
        /// <param name="show"></param>
        private void ToggleSettings(bool show)
        {
            var settingsPopup = rootVisualElement.Q<VisualElement>("settings-popup");
            settingsPopup.style.visibility = show ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// Adds secondary grid to level data
        /// This also adds another grid tab
        /// </summary>
        private void AddSecondaryGrid()
        {
            var levelData = _selectedDataField.value as MatchGridEditorSavedLevelData;
            var newGrid = new MatchLayerGrid(levelData.ColumnCount, levelData.RowCount);
            levelData.SecondaryGrids.Add(newGrid);
            AddAndEnableGrid($"secondary-grid-{levelData.SecondaryGrids.Count - 1}");

            var gridToggle = new ToolbarToggle();
            gridToggle.text = newGrid.name;
            gridToggle[0].focusable = false;
            gridToggle.RegisterCallback<ClickEvent>((evt) => { evt.PreventDefault(); ToggleGrid(gridToggle); });
            var deleteButton = new Button();
            deleteButton.text = "x";
            deleteButton.clicked += () => {
                if(EditorUtility.DisplayDialog("Warning", "This will delete the grid and all of its tiles permanently. Are you sure you want to continue?", "Yes", "No"))
                {
                    RemoveSecondaryGrid(gridToggle);
                }
            };
            gridToggle.Add(deleteButton);

            _gridToolbar.Add(gridToggle);

            _gridToolbar.Q<ToolbarButton>("add-grid-button").PlaceInFront(gridToggle);
        }

        /// <summary>
        /// Removes a secondary grid
        /// Note: Main grid cannot be removed
        /// </summary>
        /// <param name="toggle"></param>
        private void RemoveSecondaryGrid(ToolbarToggle toggle)
        {
            int index = _gridToolbar.IndexOf(toggle);
            _gridToolbar.RemoveAt(index);

            var levelData = _selectedDataField.value as MatchGridEditorSavedLevelData;
            levelData.SecondaryGrids.RemoveAt(index - 1);

            ToggleGrid(_gridToolbar[0] as ToolbarToggle);
        }

        /// <summary>
        /// Toggle display of secondary grids
        /// Main grid is always shown
        /// </summary>
        /// <param name="selectedToggle"></param>
        private void ToggleGrid(ToolbarToggle selectedToggle)
        {
            UQueryBuilder<ToolbarToggle> toggles = GetGridToggles();
            EditorApplication.delayCall += () =>
            {
                toggles.ForEach((toggle) =>
                {
                    if (toggle == selectedToggle)
                    {
                        toggle.value = true;
                    }

                    toggle.value = false;
                });
            };

            _currentGrid = _gridToolbar.IndexOf(selectedToggle);
            _gridViewCollection.EnableGrid(_currentGrid);
            Rebind();
            PopulateGrid();
            ToggleCellSets();
            SetSelectedCellType(null);
        }

        /// <summary>
        /// Query for all grid toggles in the grid toolbar for enumeration
        /// </summary>
        /// <returns></returns>
        private UQueryBuilder<ToolbarToggle> GetGridToggles()
        {
            return _gridToolbar.Query<ToolbarToggle>();
        }

        /// <summary>
        /// Change grid name
        /// </summary>
        /// <param name="evt"></param>
        private void OnGridNameChange(ChangeEvent<string> evt)
        {
            string newGridName = evt.newValue;
            SelectedGridName = newGridName;
            (_gridToolbar[_currentGrid] as ToolbarToggle).text = newGridName;
        }

        //private VisualElement GridToggle(int index)
        //{

        //}
        #endregion

        #region Tile Sets
        /// <summary>
        /// Gets all cells in the <see cref="cellsLocation"/>
        /// and categorize them by directory
        /// This also registers callbacks for each cell button
        /// </summary>
        private void InitializeCellSets()
        {
            _cellsContainer.Clear();

            string[] directoryPaths = AssetDatabase.GetSubFolders(cellsLocation);

            foreach (string directory in directoryPaths)
            {
                CellGroupFoldout cellFoldout = new CellGroupFoldout();
                cellFoldout.Header = Path.GetFileName(directory);

                var cellTypes = AssetUtility.GetAssets<CellType>(directory, "*.prefab");

                foreach (CellType cellType in cellTypes)
                {
                    CellItem cellItem = new CellItem();
                    if (cellType == null)
                    {
                        continue;
                    }

                    cellItem.CellType = cellType;

                    cellItem.RegisterCallback<ClickEvent>((evt) =>
                    {
                        if (_selectedCellItem != null)
                        {
                            _selectedCellItem.Q<VisualElement>(className: "tile").RemoveFromClassList("tile-selected");
                        }

                        _selectedCellItem = cellItem;
                        _selectedCellItem.Q<VisualElement>(className: "tile").AddToClassList("tile-selected");
                        SetSelectedCellType(cellType);
                    });

                    cellFoldout.Add(cellItem);
                }

                _cellsContainer.Add(cellFoldout);
            }
        }

        /// <summary>
        /// Sets the <paramref name="cellType"/> as selected brush
        /// Changes the image in the <see cref="_selectedBrushPreview"/>
        /// Changes the <see cref="_gridViewCollection.CellBrushSize"/> depending on <paramref name="cellType"/> dimension
        /// </summary>
        /// <param name="cellType"></param>
        private void SetSelectedCellType(CellType cellType)
        {
            _selectedCellBrush = cellType;

            if (_selectedCellBrush == null)
            {
                _selectedBrushPreview.style.backgroundImage = null;
                _selectedBrushPreview.style.unityBackgroundImageTintColor = Color.white;
                _selectedBrushPreview[0].style.backgroundImage = null;

                _gridViewCollection.CellBrushSize = Vector2Int.one;
            }
            else
            {
                _selectedBrushPreview.style.backgroundImage = cellType.Sprite == null ? cellType.SubSprite.texture : cellType.Sprite.texture;
                _selectedBrushPreview.style.unityBackgroundImageTintColor = cellType.BaseColor;
                _selectedBrushPreview[0].style.backgroundImage = cellType.IconSprite ? cellType.IconSprite.texture : null;

                _gridViewCollection.CellBrushSize = cellType.TileDimension;
            }

            
        }

        /// <summary>
        /// Toggle display of cell sets
        /// Commonly hidden when a puzzle is not yet loaded
        /// </summary>
        private void ToggleCellSets()
        {
            UQueryBuilder<CellGroupFoldout> cellFoldouts = _cellsContainer.Query<CellGroupFoldout>();
            cellFoldouts.ForEach(cellFoldout =>
            {
                int cellItemsCount = cellFoldout.contentContainer.childCount;
                int cellsShown = 0;
                for(int i = 0; i < cellItemsCount; i++)
                {
                    CellItem cellItem = (cellFoldout[i] as CellItem);
                    bool show = (int)cellItem.CellType.TileGridLocation == _currentGrid;
                    cellItem.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
                    cellsShown += show ? 1 : 0;
                }

                cellFoldout.style.display = cellsShown > 0 ? DisplayStyle.Flex : DisplayStyle.None;
            });
        }
        #endregion

        #region Grid
        /// <summary>
        /// Initializes grid UI and its callbacks
        /// </summary>
        private void InitializeGrid()
        {
            _origBoardSize = new Vector2Int(_dimension.width * 100, _dimension.height * 117);
            _gridViewCollection.style.width = _origBoardSize.x;
            _gridViewCollection.style.height = _origBoardSize.y;
            _gridViewCollection.SetDimension(_dimension, true);
            _gridViewCollection.onCellClick += OnGridViewClick;

            _gridViewCollection.ClearGrids();

            MatchGridEditorSavedLevelData levelData = _selectedDataField.value as MatchGridEditorSavedLevelData;
            if(levelData == null)
            {
                return;
            }

            AddAndEnableGrid("main-grid");
            
            foreach(MatchLayerGrid sgrid in levelData.SecondaryGrids)
            {
                var secondaryGrid = new GridView();
                secondaryGrid.name = sgrid.name;
                secondaryGrid.SetDimension(_dimension, true);
                _gridViewCollection.Add(secondaryGrid);
            }

            _currentGrid = 0;
        }

        /// <summary>
        /// Populate and add cells to the grid
        /// Cells in the <see cref="_gridViewCollection"/> are the only ones shown
        /// </summary>
        private void PopulateGrid()
        {
            MatchGridEditorSavedLevelData levelData = _selectedDataField.value as MatchGridEditorSavedLevelData;
            if(levelData == null)
            {
                return;
            }

            CellType[,] grid;

            //if(_currentGrid < 1)
            //{
            //    grid = levelData.MainGrid.Tiles;
            //} else
            //{
            //    grid = levelData.SecondaryGrids[_currentGrid - 1].Tiles;
            //}

            grid = levelData.MainGrid.Tiles;
            _gridViewCollection.EnableGrid(0);

            for (int i = 0; i < levelData.RowCount; i++)
            {
                for (int u = 0; u < levelData.ColumnCount; u++)
                {
                    var cell = grid[u, i];

                    if (cell == null)
                    {
                        _gridViewCollection.EraseCell(i, u);
                    }
                    else
                    {
                        _gridViewCollection.PaintCell(i, u, (ve, cellSize) => DrawCell(ve, cell, cellSize));
                    }
                }
            }

            for(int sgrid = 0; sgrid < levelData.SecondaryGrids.Count; sgrid++)
            {
                grid = levelData.SecondaryGrids[sgrid].Tiles;
                _gridViewCollection.EnableGrid(sgrid + 1);

                for (int i = 0; i < levelData.RowCount; i++)
                {
                    for (int u = 0; u < levelData.ColumnCount; u++)
                    {
                        var cell = grid[u, i];

                        if (cell == null)
                        {
                            _gridViewCollection.EraseCell(i, u);
                        }
                        else
                        {
                            _gridViewCollection.PaintCell(i, u, (ve, cellSize) => DrawCell(ve, cell, cellSize));
                        }
                    }
                }
            }
            _gridViewCollection.EnableGrid(_currentGrid);
        }

        /// <summary>
        /// Add and show a new grid by <paramref name="name"/>
        /// </summary>
        /// <param name="name"></param>
        private void AddAndEnableGrid(string name)
        {
            var grid = new GridView();
            grid.name = name;
            grid.SetDimension(_dimension, true);
            _gridViewCollection.Add(grid);
            _gridViewCollection.EnableGrid(_gridViewCollection.GridCount - 1);
        }

        /// <summary>
        /// Grid functions on mouse click, regardless if it's left or right, depending on the <paramref name="mode"/>
        /// <see cref="PaintCell(Vector2Int)"/> for <see cref="ToolMode.Paint"/>
        /// <see cref="EraseCell(Vector2Int)"/> for <see cref="ToolMode.Erase"/>
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="mode"></param>
        private void OnGridViewClick(Vector2Int coord, ToolMode mode)
        {
            switch(mode)
            {
                case ToolMode.Paint:
                    PaintCell(coord);
                    break;
                case ToolMode.Erase:
                    EraseCell(coord);
                    break;
            }
        }

        /// <summary>
        /// Draw the cell layout in the grid
        /// This changes the image in the grid depending on <paramref name="cell"/>
        /// </summary>
        /// <param name="root"></param>
        /// <param name="cell"></param>
        /// <param name="tileSize"></param>
        private void DrawCell(VisualElement root, CellType cell, Vector2 tileSize)
        {
            root.Clear();
            var sprite = cell.Sprite;

            VisualElement ve = new VisualElement();
            ve.style.position = Position.Absolute;
            ve.style.width = Length.Percent(100f * cell.TileDimension.x);
            ve.style.height = Length.Percent(100f * cell.TileDimension.y);
            ve.pickingMode = PickingMode.Ignore;
            root.Add(ve);
            if (sprite != null)
            {
                ve.style.backgroundImage = cell.Sprite.texture;
                ve.style.unityBackgroundImageTintColor = cell.BaseColor;
            } else
            {
                ve.style.backgroundImage = cell.SubSprite.texture;
                ve.style.unityBackgroundImageTintColor = cell.BaseColor;
            }

            ve.style.alignContent = Align.Center;
            ve.style.alignItems = Align.Center;

            if (cell.IconSprite != null)
            {
                VisualElement subVe = new VisualElement();
                subVe.pickingMode = PickingMode.Ignore;
                subVe.style.backgroundImage = cell.IconSprite.texture;
                subVe.style.width = Length.Percent(60f);
                subVe.style.height = Length.Percent(100f);
                subVe.style.position = Position.Absolute;
                subVe.style.bottom = Length.Percent(-10);
                subVe.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
                ve.Add(subVe);
            }
        }

        /// <summary>
        /// Sets the <see cref="_selectedCellBrush"/> to the grid in position <paramref name="coord"/>
        /// Draws the cell in the grid, replacing a previous one if there's one, and
        /// saves it in the loaded level data if <see cref="_autoSave"/> is set
        /// </summary>
        /// <param name="coord"></param>
        private void PaintCell(Vector2Int coord)
        {
            if (_selectedCellBrush == null)
            {
                return;
            }

            GetSelectedGrid()[coord.x, coord.y] = _selectedCellBrush;
            _gridViewCollection.PaintCell(coord.y, coord.x, (ve, cellSize) => DrawCell(ve, _selectedCellBrush, cellSize));

            for(int x = 0; x < _selectedCellBrush.TileDimension.x; x++)
            {
                for(int y = 0; y < _selectedCellBrush.TileDimension.y; y++)
                {
                    if(x == 0 && y == 0)
                    {
                        continue;
                    }
                    EraseCell(new Vector2Int(coord.x + x, coord.y + y));
                }
            }

            ToggleWindowTitleDirty(true);
            if (_autoSave)
            {
                SaveLevel();
            }
        }

        /// <summary>
        /// Removes the cell in the grid and
        /// saves it the loaded level data if <see cref="_autoSave"/> is set
        /// </summary>
        /// <param name="coord"></param>
        private void EraseCell(Vector2Int coord)
        {
            GetSelectedGrid()[coord.x, coord.y] = null;
            _gridViewCollection.EraseCell(coord.y, coord.x);
            ToggleWindowTitleDirty(true);
            if (_autoSave)
            {
                SaveLevel();
            }
        }

        /// <summary>
        /// Since autosave is by default false, the level data is not saved directly
        /// This puts a visual cue for the user that the level data has some unsaved changes
        /// </summary>
        /// <param name="dirty"></param>
        private void ToggleWindowTitleDirty(bool dirty)
        {
            LevelEditor wnd = GetWindow<LevelEditor>();
            wnd.titleContent = new GUIContent($"Level Editor {(dirty ? "*" : "")}");
        }

        /// <summary>
        /// Gets selected grid
        /// Main grid is selected by default on level load
        /// </summary>
        /// <returns></returns>
        private CellType[,] GetSelectedGrid()
        {
            if (_currentGrid < 1)
            {
                return (_selectedDataField.value as MatchGridEditorSavedLevelData).MainGrid.Tiles;
            } else
            {
                return (_selectedDataField.value as MatchGridEditorSavedLevelData).SecondaryGrids[_currentGrid - 1].Tiles;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Auto save when the window is closed
        /// TODO: Show a popup that asks the user to save if the window is dirty else just close
        /// Dev Note: I tried intercepting it with a popup but can't seem to figure out the `Cancel` function
        /// where user may cancel closing the window. This always closes the window
        /// </summary>
        private void OnDestroy()
        {
            if(_selectedDataField.value != null)
            {
                SaveLevel();
            }
        }
        #endregion

        #region Reflection
        /// <summary>
        /// Getter-Setter for Selected grid name
        /// </summary>
        private string SelectedGridName
        {
            get
            {
                if(_selectedDataField.value == null)
                {
                    return "";
                }

                var levelData = _selectedDataField.value as MatchGridEditorSavedLevelData;
                if(_currentGrid < 1)
                {
                    return levelData.GetFieldValue("mainGrid").GetFieldValue("name") as string;
                } else
                {
                    return (string)(levelData.GetFieldValue("secondaryGrids") as List<MatchLayerGrid>)[_currentGrid - 1].GetFieldValue("name");
                }
            }
            set
            {
                if (_selectedDataField.value == null)
                {
                    return;
                }

                var levelData = _selectedDataField.value as MatchGridEditorSavedLevelData;
                if (_currentGrid < 1)
                {
                    levelData.GetFieldValue("mainGrid").SetFieldValue("name", value);
                }
                else
                {
                    (levelData.GetFieldValue("secondaryGrids") as List<MatchLayerGrid>)[_currentGrid - 1].SetFieldValue("name", value);
                }
            }
        }

        private CellBag LevelDataCellBag
        {
            get
            {
                if(_selectedDataField.value == null)
                {
                    return null;
                }

                var levelData = _selectedDataField.value as MatchGridEditorSavedLevelData;
                return levelData.GetFieldValue("tileBag") as CellBag;
            }
            set
            {
                if(_selectedDataField.value == null)
                {
                    return;
                }

                var levelData = _selectedDataField.value as MatchGridEditorSavedLevelData;
                levelData.SetFieldValue("tileBag", value);
            }
        }

        private LevelGoalsData GoalsData
        {
            get
            {
                if (_selectedDataField.value == null)
                {
                    return null;
                }

                var levelData = _selectedDataField.value as MatchGridEditorSavedLevelData;
                return levelData.GetFieldValue("levelGoalListData") as LevelGoalsData;
            }
            set
            {
                if (_selectedDataField.value == null)
                {
                    return;
                }

                var levelData = _selectedDataField.value as MatchGridEditorSavedLevelData;
                levelData.SetFieldValue("levelGoalListData", value);
            }
        }
        #endregion
    }

    public class CreateNewLevelPopupContent : PopupWindowContent
    {
        private int width;
        private int height;
        private Action<int, int> onCreateData;

        public static void Show(Rect activatorRect, int width, int height, Action<int, int> onCreateData)
        {
            CreateNewLevelPopupContent win =
                new CreateNewLevelPopupContent(width, height, onCreateData);
            UnityEditor.PopupWindow.Show(activatorRect, win);
        }

        private CreateNewLevelPopupContent(int width, int height, Action<int, int> onCreateData)
        {
            this.width = width;
            this.height = height;
            this.onCreateData = onCreateData;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200, EditorGUIUtility.singleLineHeight * 3f + 8f);
        }

        public override void OnGUI(Rect rect)
        {
            Vector2 windowSize = GetWindowSize();
            float horizontalMargin = 10f;
            float elementWidth = windowSize.x - horizontalMargin;
            float topMargin = 2f;
            float y = topMargin;
            width = EditorGUI.IntField(new Rect(horizontalMargin / 2f, y, elementWidth, EditorGUIUtility.singleLineHeight), "Width", width);
            height = EditorGUI.IntField(new Rect(horizontalMargin / 2f, y + EditorGUIUtility.singleLineHeight + topMargin, elementWidth, EditorGUIUtility.singleLineHeight), "Height", height);
            if(GUI.Button(new Rect(horizontalMargin / 2f, y + EditorGUIUtility.singleLineHeight * 2f + topMargin, elementWidth, EditorGUIUtility.singleLineHeight), "Create"))
            {
                onCreateData?.Invoke(width, height);
                editorWindow.Close();
            }
        }
    }
}