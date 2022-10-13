/*
 * MatchGridEditorSavelLevelData.cs
 * Author: Cristjan Lazar
 * Data: Oct 13, 2018
 */

using UnityEngine;

using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using Newtonsoft.Json;
using M7.Skill;

namespace M7.Match
{
    /// <summary>
    /// Tile position data for a match grid. Used when saving/loading a grid configuration.
    /// </summary>
    [CreateAssetMenu(fileName = "MatchGridEditorSavedLevelData", menuName = "BGGamesCore/Match/MatchGridEditorSavedLevelData")]
    public class MatchGridEditorSavedLevelData : SerializedScriptableObject
    {
        [JsonIgnore] public CellBag tileBag;
        [JsonIgnore] public LevelGoalsData levelGoalListData;

        [JsonIgnore] [OdinSerialize] private MatchLayerGrid mainGrid = new MatchLayerGrid(GridDimensions.DEFAULT.Columns, GridDimensions.DEFAULT.Rows, "Main");
        [JsonIgnore] public MatchLayerGrid MainGrid { get { return mainGrid; } set { mainGrid = value; } }

        [JsonIgnore] [OdinSerialize] private List<MatchLayerGrid> secondaryGrids = new List<MatchLayerGrid>();
        [JsonIgnore] public List<MatchLayerGrid> SecondaryGrids { get { return secondaryGrids; } set { secondaryGrids = value; } }


        [JsonIgnore]
        public int ColumnCount
        {
            get { return mainGrid.Tiles.Length > 0 ? mainGrid.Width : 0; }
            set
            {
                for (var x = 0; x < secondaryGrids.Count; x++)
                    secondaryGrids[x].ResizeGrid(ColumnCount, value);
                mainGrid.ResizeGrid(value, RowCount);
            }
        }
        [JsonIgnore]
        public int RowCount
        {
            get { return mainGrid.Tiles.Length > 1 ? mainGrid.Height : 0; }
            set
            {
                for (var x = 0; x < secondaryGrids.Count; x++)
                    secondaryGrids[x].ResizeGrid(ColumnCount, value);
                mainGrid.ResizeGrid(ColumnCount, value);
            }
        }

        /// <summary>
        /// Gets the tile position with inverted-y coordinate. Used as an adapter for Grids Pro.
        /// </summary>
        public CellType GetInvertedYTile(int x, int y, SkillEnums.CellGridLocation cellGridLocation = SkillEnums.CellGridLocation.Main)
        {
            var matchLayerGrid = cellGridLocation switch
            {
                SkillEnums.CellGridLocation.Secondary => secondaryGrids.Count > 0 ? secondaryGrids[(int)cellGridLocation - 1] : null,
                _ => mainGrid,
            };

            if (matchLayerGrid == null)
                return null;

            int _y = matchLayerGrid.Height - y - 1;
            if(x < 0 || x >= matchLayerGrid.Tiles.GetLength(0))
                return null;

            if (y < 0 || y >= matchLayerGrid.Tiles.GetLength(1))
                return null;

            return matchLayerGrid.Tiles[x, _y];
        }

        /// <summary>
        /// Copy level data into this scriptable.
        /// </summary>
        public MatchGridEditorSavedLevelData Load(MatchGridEditorSavedLevelData data)
        {
            levelGoalListData = data.levelGoalListData;
            tileBag = data.tileBag;

            int columns = data.ColumnCount;
            int rows = data.RowCount;

            mainGrid = LoadMatchLayerGrid(data.mainGrid, columns, rows);
            secondaryGrids = new List<MatchLayerGrid>();
            for (var x = 0; x < data.secondaryGrids.Count; x++)
                secondaryGrids.Add(LoadMatchLayerGrid(data.SecondaryGrids[x], columns, rows));

            return this;
        }

        public MatchLayerGrid LoadMatchLayerGrid(MatchLayerGrid data, int columns, int rows)
        {
            var layerGrid = new MatchLayerGrid(columns, rows, data.name);
            for (var x = 0; x < columns; x++)
                for (var y = 0; y < rows; y++)
                    layerGrid.Tiles[x, y] = data.Tiles[x, y];
            return layerGrid;
        }
    }

    [Serializable]
    public class MatchLayerGrid
    {
        [JsonProperty] public string name = "New LayerGrid";
        [JsonProperty] [SerializeField] private CellType[,] tiles;
        [JsonIgnore] public CellType[,] Tiles { get { return tiles = tiles ?? new CellType[GridDimensions.DEFAULT.Columns, GridDimensions.DEFAULT.Rows]; } }
        [JsonIgnore] public int Width { get { return Tiles.GetLength(0); } }
        [JsonIgnore] public int Height { get { return Tiles.GetLength(1); } }

        public MatchLayerGrid(int colSize, int rowSize, string name = "New LayerGrid")
        {
            tiles = new CellType[colSize, rowSize];
            this.name = name;
        }

        public void ResizeGrid(int colSize, int rowSize)
        {
            tiles = Tiles.ResizeArray(colSize, rowSize);
        }
    }
}
