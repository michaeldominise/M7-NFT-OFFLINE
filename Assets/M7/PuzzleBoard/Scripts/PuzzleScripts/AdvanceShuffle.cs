using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M7.Match;
using Sirenix.OdinInspector;
using UnityEngine;
using M7.Match.PlaymakerActions;
using Gamelogic.Grids;

public static class AdvanceShuffle
{
    public static int minChain => ComboData.MIN_CHAIN_COUNT;
    public static void RequestShuffle(MatchGrid matchGrid, int minChainListCount, float duration)
    {
        matchGrid.Grid.Shuffle();
        var originalMatchGrid = matchGrid.Grid;
        var virtualMatchGrid = new RectGrid<MatchGridCell>(originalMatchGrid.Width, originalMatchGrid.Height);

        var chainList = new List<List<MatchGridCell>>();

        for (var x = 0; x < matchGrid.ColumnCount; x++)
        { 
            for (var y = 0; y < matchGrid.RowCount; y++)
            {
                var originalColCount = x;
                var originalRowCount = y;
                ScanPossibleChain(ref originalColCount, ref originalRowCount, x, y, virtualMatchGrid, originalMatchGrid, chainList, minChainListCount);
            }
        }

        for (var x2 = 0; x2 < matchGrid.ColumnCount; x2++)
        {
            for (var y2 = 0; y2 < matchGrid.RowCount; y2++)
            {
                var rectPoint = new RectPoint(x2, y2);
                matchGrid.Grid[rectPoint] = virtualMatchGrid[rectPoint];
                matchGrid.Grid[rectPoint].CellMotor.MoveLerp(matchGrid.Map[rectPoint], duration);
                matchGrid.Grid[rectPoint].CurrentRectPoint = rectPoint;
            }
        }

        MatchGridCellSpawner.Instance.RefreshSibling(PuzzleBoardManager.Instance.ActiveGrid);
    }

    static void ScanPossibleChain(ref int originalColCount, ref int originalRowCount, int colCount, int rowCount, RectGrid<MatchGridCell> virtualMatchGrid, RectGrid<MatchGridCell> originalMatchGrid, List<List<MatchGridCell>> chainList, int minChainListCount)
    {
        var targetRectPoint = new RectPoint(colCount, rowCount);
        var virtualTargetTile = virtualMatchGrid[targetRectPoint];
        if (virtualTargetTile != null)
            return;

        var targetTile = originalMatchGrid[targetRectPoint];
        var connectedTiles = new List<MatchGridCell>();
        var validatedTiles = new bool[originalMatchGrid.Width, originalMatchGrid.Height];
        ScanPossibleMoves.GetConnectedSameTile(targetTile, new Vector2Int(colCount, rowCount), connectedTiles, virtualMatchGrid, ref validatedTiles, ScanPossibleMoves.ScanDirection.Left | ScanPossibleMoves.ScanDirection.Right);
        ScanPossibleMoves.GetConnectedSameTile(targetTile, new Vector2Int(colCount, rowCount), connectedTiles, virtualMatchGrid, ref validatedTiles, ScanPossibleMoves.ScanDirection.Up | ScanPossibleMoves.ScanDirection.Down);

        if (connectedTiles.Count < 2)
            virtualMatchGrid[targetRectPoint] = originalMatchGrid[targetRectPoint];
        else
        {
            var chainIndex = SearchExistingConnectedChain(chainList, connectedTiles);
            if (chainIndex < 0)
            {
                if (chainList.Count < minChainListCount)
                {
                    chainList.Add(connectedTiles);
                    virtualMatchGrid[targetRectPoint] = originalMatchGrid[targetRectPoint];
                }
                else
                {
                    if (rowCount + 1 > originalMatchGrid.Height)
                        ScanPossibleChain(ref originalColCount, ref originalRowCount, colCount, rowCount + 1, virtualMatchGrid, originalMatchGrid, chainList, minChainListCount);
                    else if (colCount + 1 > originalMatchGrid.Width)
                        ScanPossibleChain(ref originalColCount, ref originalRowCount, colCount + 1, 0, virtualMatchGrid, originalMatchGrid, chainList, minChainListCount);
                    else
                    {
                        chainList.Add(connectedTiles);
                        virtualMatchGrid[targetRectPoint] = originalMatchGrid[targetRectPoint];
                    }
                }
            }
            else
            {
                chainList[chainIndex] = connectedTiles;
                virtualMatchGrid[targetRectPoint] = originalMatchGrid[targetRectPoint];
            }
        }
    }

    static int SearchExistingConnectedChain(List<List<MatchGridCell>> chainList, List<MatchGridCell> connectedTiles)
    {
        for (var a = 0; a < chainList.Count; a++)
        {
            for (var b = 0; b < chainList[a].Count; b++)
            {
                for (var c = 0; c < connectedTiles.Count; c++)
                {
                    if (connectedTiles[c] == chainList[a][b])
                        return a;
                }
            }
        }

        return -1;
    }
}