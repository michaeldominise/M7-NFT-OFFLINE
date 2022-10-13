using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M7.Match;
using Gamelogic.Grids;
using UnityEngine;

[Serializable]
public class ComboData
{
    public const int MIN_CHAIN_COUNT = 2;

    /// <summary>
    /// The tile chains list group by tileType.
    /// </summary>
    Dictionary<CellType, CellChainData> cellChainsGroups = new Dictionary<CellType, CellChainData>();
    public Dictionary<CellType,CellChainData> CellChainsGroups { get { return cellChainsGroups; } }
    public int ManaRequiredComboCount;
    public int ComboCount => PuzzleBoardSettings.Instance.comboTriggerType == PuzzleBoardSettings.ComboTriggerType.PerHorizontalAndVertical ? VerticalHorizontalMatchCount : ConnectedCellsCount;
    public int VerticalHorizontalMatchCount { get; private set; }
    public int ConnectedCellsCount { get; private set; }
    public int GridClearCount { get; private set; }
    public CellType LastMatchedCellType { get; private set; }
    public List<List<MatchGridCell>> RecentConnectedCellList { get; private set; } = new List<List<MatchGridCell>>();
    int LastConnectedCellListCount { get; set; }

    CellChainData GetCellType(CellType cellType)
    {
        if (cellChainsGroups.ContainsKey(cellType))
            return cellChainsGroups[cellType];
        return null;
    }

    CellChainData CreateTileChain(CellType cellType)
    {
        var tileChain = new CellChainData(cellType);
        cellChainsGroups.Add(cellType, tileChain);
        return tileChain;
    }

    public void InitConnectedTilesCount()
    {
        LastConnectedCellListCount = Mathf.Max(0, RecentConnectedCellList[0].Count - ConnectedCellsCount);
        ConnectedCellsCount += LastConnectedCellListCount;
        LastConnectedCellListCount = RecentConnectedCellList[0].Count;
        Debug.Log($"[InitConnectedTilesCount] ConnectedTilesCount: {ConnectedCellsCount}");
    }

    public void UpdateConnectedTilesCount()
    {
        ConnectedCellsCount += RecentConnectedCellList.Count();
        Debug.Log($"[UpdateConnectedTilesCount] ConnectedTilesCount: {ConnectedCellsCount}");
    }

    public void GridCleared()
    {
        foreach(var tileChain in cellChainsGroups)
            tileChain.Value.ChainsClear();
        RecentConnectedCellList.Clear();
        GridClearCount++;
        Debug.Log($"[GridCleared] {GridClearCount}");
    }

    public void AddChainList(RectPointListList chainList)
    {
        foreach (var chain in chainList.Value)
            AddChain(chain);
    }

    public void AddChainList(List<PointList<RectPoint>> chainList)
    {
        foreach (var chain in chainList)
            AddChain(chain);
    }

    public void AddChain(PointList<RectPoint> chain)
    {
        if (chain.Count == 0)
            return;
        
        var tileType = PuzzleBoardManager.Instance.ActiveGrid.Grid[chain[0]].CellTypeContainer.CellType;
        var tileChain = GetCellType(tileType) ?? CreateTileChain(tileType);
        tileChain.AddChain(chain);
        LastMatchedCellType = tileType;
        VerticalHorizontalMatchCount++;

        AddConnectedTileList(chain);
    }

    public void AddConnectedTileList(PointList<RectPoint> chain)
    {
        var newConnectedCell = new List<MatchGridCell>();
        for (int i = 0; i < chain.Count; i++)
        {
            RectPoint rectPoint = chain[i];
            var hasMatch = false;
            for (int x = 0; x < RecentConnectedCellList.Count; x++)
            {
                List<MatchGridCell> connectedCell = RecentConnectedCellList[x];
                if (connectedCell.FirstOrDefault(cell => cell.CurrentRectPoint == rectPoint) != null)
                {
                    hasMatch = true;
                    newConnectedCell.AddRange(connectedCell);
                    RecentConnectedCellList.RemoveAt(x);
                    x--;
                    break;
                }
            }
            if(!hasMatch)
                newConnectedCell.Add(PuzzleBoardManager.Instance.ActiveGrid.Grid[rectPoint]);
        }

        RecentConnectedCellList.Add(newConnectedCell);
    }

    public void Reset(bool resetConnectedCellCount = true)
    {
        VerticalHorizontalMatchCount = 0;
        GridClearCount = 0;
        ConnectedCellsCount = resetConnectedCellCount ? 0 : ConnectedCellsCount;
        CellChainsGroups.Clear();
        RecentConnectedCellList.Clear();
        ManaRequiredComboCount = PuzzleBoardSettings.Instance.manaStartGeneration - 1;
    }
}

[Serializable]
public class CellChainData
{
    public CellType CellType { get; private set; }
    public List<PointList<RectPoint>> CurrentChains { get; private set; }
    public int OverallTileCount { get; private set; }

    public CellChainData(CellType TileType)
    {
        this.CellType = TileType;
        CurrentChains = new List<PointList<RectPoint>>();
    }


    public int GetGreaterAndEqualToMinChainCount()
    {
        return OverallTileCount - (CurrentChains.Count * (ComboData.MIN_CHAIN_COUNT - 1));
    }

    public void AddChain(PointList<RectPoint> chain)
    {
        CurrentChains.Add(chain);
        OverallTileCount += chain.Count;
    }

    public void ChainsClear()
    {
        CurrentChains.Clear();
    }
}