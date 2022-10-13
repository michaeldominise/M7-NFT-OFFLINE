using System;
using System.Collections;
using System.Collections.Generic;
using Gamelogic.Grids;
using HutongGames.PlayMaker;
using UnityEngine;

namespace M7.Match.PlaymakerActions
{
    [ActionCategory("M7/Match")]
    public class ScanPossibleMoves : FsmStateAction
    {
        public static ScanPossibleMoves Instance { get; private set; }

        public enum ScanType { HorizontalVertical, Adjacent, HorizontalVerticalDiagonal, AdjacentWithDiagonal }
        [Flags]
        public enum ScanDirection
        {
            None = -1,
            Right = 1,
            Left = 2,
            Up = 4,
            Down = 8,
            UpperRight = 16,
            LowerLeft = 32,
            UpperLeft = 64,
            LowerRight = 128,
            AllSides = 15,
            AllSidesWithDiagonals = 255,
        }
        static MatchGrid matchGrid => PuzzleBoardManager.Instance.ActiveGrid;

        //Cached variables
        static bool[,] validatedHCells;
        static bool[,] validatedVCells;
        static bool[,] validatedAdjCells;
        static Vector2Int[] destinationPointList = new Vector2Int[8];
        static Vector2Int[] directionPointList = new Vector2Int[8];
        static Vector2Int nextDirectionPoint;

        public PossibleMoveList possibleMoveList;
        public ScanType validateType;

        public override void Awake()
        {
            Instance = this;
        }

        public static void Init()
        {
            if (validatedHCells == null)
                validatedHCells = new bool[matchGrid.ColumnCount, matchGrid.RowCount];

            if (validatedVCells == null)
                validatedVCells = new bool[matchGrid.ColumnCount, matchGrid.RowCount];

            if (validatedAdjCells == null)
                validatedAdjCells = new bool[matchGrid.ColumnCount, matchGrid.RowCount];
        }

        public override void OnEnter()
        {
            possibleMoveList.Value = Scan(validateType);
            Finish();
        }

        public static int GetMinChain(MatchGridCell cell) => cell.CellTypeContainer.CellType.ElementType == Skill.SkillEnums.ElementFilter.Special ? 1 : ComboData.MIN_CHAIN_COUNT;

        public static List<PossibleMove> Scan(ScanType validateType)
        {
            Init();
            var possibleMoves = new List<PossibleMove>();
            var targetPoint = Vector2Int.zero;

            for (var x = 0; x < matchGrid.ColumnCount; x++)
            {
                for (var y = 0; y < matchGrid.RowCount; y++)
                {
                    targetPoint.Set(x, y);

                    var targetCell = matchGrid.Grid[new RectPoint(targetPoint.x, targetPoint.y)];
                    if (targetCell == null || targetCell.CurrentCellState != MatchGridCell.CellState.Active || !targetCell.IsInteractible)
                        continue;

                    switch (validateType)
                    {
                        case ScanType.HorizontalVertical:
                        case ScanType.HorizontalVerticalDiagonal:
                            if (validateType == ScanType.HorizontalVertical)
                                destinationPointList.InitAllSides(targetPoint);
                            else
                                destinationPointList.InitAllSidesWithDiagonals(targetPoint);

                            for (var i = 0; i < destinationPointList.Length; i++)
                            {
                                if (!destinationPointList[i].IsValidVectPoint(matchGrid.Grid))
                                    continue;

                                var destinationCell = matchGrid.Grid[new RectPoint(destinationPointList[i].x, destinationPointList[i].y)];
                                if (destinationCell.CurrentCellState != MatchGridCell.CellState.Active || !destinationCell.IsInteractible)
                                    continue;

                                validatedHCells.Reset2DDim();
                                validatedVCells.Reset2DDim();

                                directionPointList.InitAllSides(destinationPointList[i]);

                                PossibleMove cellPossibleMovesH = new PossibleMove(targetCell, destinationCell);
                                PossibleMove cellPossibleMovesV = new PossibleMove(targetCell, destinationCell);

                                for (var dirIndex = 0; dirIndex < 4; dirIndex++)
                                {
                                    if (!directionPointList[dirIndex].IsValidVectPoint(matchGrid.Grid))
                                        continue;

                                    if (dirIndex < 2)
                                    {
                                        validatedHCells[directionPointList[dirIndex].x, directionPointList[dirIndex].y] = false;
                                        if (targetCell.CellTypeContainer.CellType.MatchValue == 0)
                                            validatedHCells[destinationPointList[i].x, destinationPointList[i].y] = false;

                                        VirtuallyMoveTile(targetCell, destinationCell, targetPoint, destinationPointList[i], directionPointList[dirIndex], cellPossibleMovesH.cells, ref validatedHCells, ScanDirection.Right | ScanDirection.Left);

                                        if (cellPossibleMovesH.cells.Count >= GetMinChain(targetCell))
                                        {
                                            AddToPossibleMove(possibleMoves, cellPossibleMovesH);
                                            if (targetCell.CellTypeContainer.CellType.MatchValue != 0)
                                                dirIndex++;
                                        }

                                        if (targetCell.CellTypeContainer.CellType.MatchValue == 0)
                                            cellPossibleMovesH = new PossibleMove(targetCell, destinationCell);
                                    }
                                    else
                                    {
                                        validatedVCells[directionPointList[dirIndex].x, directionPointList[dirIndex].y] = false;
                                        if (targetCell.CellTypeContainer.CellType.MatchValue == 0)
                                            validatedVCells[destinationPointList[i].x, destinationPointList[i].y] = false;

                                        VirtuallyMoveTile(targetCell, destinationCell, targetPoint, destinationPointList[i], directionPointList[dirIndex], cellPossibleMovesV.cells, ref validatedVCells, ScanDirection.Up | ScanDirection.Down);

                                        if (cellPossibleMovesV.cells.Count >= GetMinChain(targetCell))
                                        {
                                            AddToPossibleMove(possibleMoves, cellPossibleMovesV);
                                            if (targetCell.CellTypeContainer.CellType.MatchValue != 0)
                                                dirIndex++;
                                        }

                                        if (targetCell.CellTypeContainer.CellType.MatchValue == 0)
                                            cellPossibleMovesV = new PossibleMove(targetCell, destinationCell);
                                    }
                                }
                            }
                            break;
                        case ScanType.Adjacent:
                        case ScanType.AdjacentWithDiagonal:
                            validatedAdjCells.Reset2DDim();
                            var tilePossibleMovesAdj = new PossibleMove(targetCell, targetCell);
                            VirtuallyMoveTile(targetCell, targetCell, targetPoint, targetPoint, targetPoint, tilePossibleMovesAdj.cells, ref validatedAdjCells, validateType == ScanType.Adjacent ? ScanDirection.AllSides : ScanDirection.AllSidesWithDiagonals);
                            if (tilePossibleMovesAdj.cells.Count >= GetMinChain(targetCell))
                                AddToPossibleMove(possibleMoves, tilePossibleMovesAdj);
                            break;
                    }
                }
            }
            return possibleMoves;
        }

        static void AddToPossibleMove(List<PossibleMove> possibleMoveList, PossibleMove possibleMove)
        {
            for (var x = 0; x < possibleMoveList.Count; x++)
            {
                if (possibleMoveList[x].cells.Count < possibleMove.cells.Count)
                {
                    possibleMoveList.Insert(x, possibleMove);
                    return;
                }
            }

            possibleMoveList.Add(possibleMove);
        }

        static void VirtuallyMoveTile(MatchGridCell targetTile, MatchGridCell destinationTile, Vector2Int targetPoint, Vector2Int destinationPoint, Vector2Int directionPoint, List<MatchGridCell> sameConnectedTiles, ref bool[,] validatedTiles, ScanDirection scanDirection)
        {
            var startRectPoint = targetPoint.ToRectPoint();
            var destRectPoint = destinationPoint.ToRectPoint();

            matchGrid.Grid[startRectPoint] = destinationTile;
            matchGrid.Grid[destRectPoint] = targetTile;

            GetConnectedSameTile(targetTile, directionPoint, sameConnectedTiles, matchGrid.Grid, ref validatedTiles, scanDirection);

            matchGrid.Grid[startRectPoint] = targetTile;
            matchGrid.Grid[destRectPoint] = destinationTile;
        }

        public static void GetConnectedSameCell(MatchGridCell targetTile, Vector2Int directionPoint, PointList<RectPoint> rectPoints, RectGrid<MatchGridCell> matchGrid, ref bool[,] validatedTiles, ScanDirection scanDirection = ScanDirection.None)
        {
            var tileType = targetTile.CellTypeContainer.CellType;
            GetConnectedSameTile(ref tileType, directionPoint, null, matchGrid, ref validatedTiles, scanDirection, scanDirection, rectPoints);
        }

        public static void GetConnectedSameTile(MatchGridCell targetTile, Vector2Int directionPoint, List<MatchGridCell> sameConnectedTiles, RectGrid<MatchGridCell> matchGrid, ref bool[,] validatedTiles, ScanDirection scanDirection = ScanDirection.None)
        {
            var tileTpye = targetTile.CellTypeContainer.CellType;
            GetConnectedSameTile(ref tileTpye, directionPoint, sameConnectedTiles, matchGrid, ref validatedTiles, scanDirection, scanDirection, null);
        }

        static void GetConnectedSameTile(ref CellType tileType, Vector2Int directionPoint, List<MatchGridCell> sameConnectedTiles, RectGrid<MatchGridCell> matchGrid, ref bool[,] validatedTiles, ScanDirection scanDirectionOriginal, ScanDirection scanDirection, PointList<RectPoint> rectPoints)
        {
            if (!directionPoint.IsValidVectPoint(matchGrid))
                return;

            if (validatedTiles[directionPoint.x, directionPoint.y])
                return; // Already validated don't continue

            var dirRectPoint = directionPoint.ToRectPoint();
            var compareTile = matchGrid[dirRectPoint];
            if (compareTile == null)
                return;
            else if (compareTile != null)
            {
                if (!compareTile.Matches(tileType))
                    return; // return if not same tile

                validatedTiles[directionPoint.x, directionPoint.y] = true;

                if (sameConnectedTiles != null)
                    sameConnectedTiles.Add(compareTile);
                if (rectPoints != null)
                    rectPoints.Add(dirRectPoint);

                if (scanDirection == ScanDirection.None)
                    return;

                //Change targetTile if it's wild tile
                if (tileType.ElementType == Skill.SkillEnums.ElementFilter.All)
                    tileType = compareTile.CellTypeContainer.CellType;
            }

            //All horizontal and vertical validator
            //check tile in right
            if ((scanDirection & ScanDirection.Right) > 0)
            {
                nextDirectionPoint.Set(directionPoint.x + 1, directionPoint.y);
                GetConnectedSameTile(ref tileType, nextDirectionPoint, sameConnectedTiles, matchGrid, ref validatedTiles, scanDirectionOriginal, scanDirectionOriginal ^ ScanDirection.Left, rectPoints);
            }
            //check tile in left
            if ((scanDirection & ScanDirection.Left) > 0)
            {
                nextDirectionPoint.Set(directionPoint.x - 1, directionPoint.y);
                GetConnectedSameTile(ref tileType, nextDirectionPoint, sameConnectedTiles, matchGrid, ref validatedTiles, scanDirectionOriginal, scanDirectionOriginal ^ ScanDirection.Right, rectPoints);
            }
            //check tile in up
            if ((scanDirection & ScanDirection.Up) > 0)
            {
                nextDirectionPoint.Set(directionPoint.x, directionPoint.y + 1);
                GetConnectedSameTile(ref tileType, nextDirectionPoint, sameConnectedTiles, matchGrid, ref validatedTiles, scanDirectionOriginal, scanDirectionOriginal ^ ScanDirection.Down, rectPoints);
            }
            //check tile in down
            if ((scanDirection & ScanDirection.Down) > 0)
            {
                nextDirectionPoint.Set(directionPoint.x, directionPoint.y - 1);
                GetConnectedSameTile(ref tileType, nextDirectionPoint, sameConnectedTiles, matchGrid, ref validatedTiles, scanDirectionOriginal, scanDirectionOriginal ^ ScanDirection.Up, rectPoints);
            }

            //All diagonal validator
            //check tile in upper right
            if ((scanDirection & ScanDirection.UpperRight) > 0)
            {
                nextDirectionPoint.Set(directionPoint.x + 1, directionPoint.y + 1);
                GetConnectedSameTile(ref tileType, nextDirectionPoint, sameConnectedTiles, matchGrid, ref validatedTiles, scanDirectionOriginal, scanDirectionOriginal ^ ScanDirection.LowerLeft, rectPoints);
            }
            //check tile in lower left
            if ((scanDirection & ScanDirection.LowerLeft) > 0)
            {
                nextDirectionPoint.Set(directionPoint.x - 1, directionPoint.y - 1);
                GetConnectedSameTile(ref tileType, nextDirectionPoint, sameConnectedTiles, matchGrid, ref validatedTiles, scanDirectionOriginal, scanDirectionOriginal ^ ScanDirection.UpperRight, rectPoints);
            }
            //check tile in upper left
            if ((scanDirection & ScanDirection.UpperLeft) > 0)
            {
                nextDirectionPoint.Set(directionPoint.x - 1, directionPoint.y + 1);
                GetConnectedSameTile(ref tileType, nextDirectionPoint, sameConnectedTiles, matchGrid, ref validatedTiles, scanDirectionOriginal, scanDirectionOriginal ^ ScanDirection.LowerRight, rectPoints);
            }
            //check tile in lower right
            if ((scanDirection & ScanDirection.LowerRight) > 0)
            {
                nextDirectionPoint.Set(directionPoint.x + 1, directionPoint.y - 1);
                GetConnectedSameTile(ref tileType, nextDirectionPoint, sameConnectedTiles, matchGrid, ref validatedTiles, scanDirectionOriginal, scanDirectionOriginal ^ ScanDirection.UpperLeft, rectPoints);
            }
        }
    }

    static class GridPointExtension
    {
        public static void InitAllSides(this Vector2Int[] pointList, Vector2Int startingPoint)
        {
            pointList[0].Set(startingPoint.x + 1, startingPoint.y); //tile in right
            pointList[1].Set(startingPoint.x - 1, startingPoint.y); //tile in left
            pointList[2].Set(startingPoint.x, startingPoint.y + 1); //tile in up
            pointList[3].Set(startingPoint.x, startingPoint.y - 1); //tile in down
        }

        public static void InitAllSidesWithDiagonals(this Vector2Int[] pointList, Vector2Int startingPoint)
        {
            pointList.InitAllSides(startingPoint);
            pointList[4].Set(startingPoint.x + 1, startingPoint.y + 1); //tile in upper right
            pointList[5].Set(startingPoint.x - 1, startingPoint.y - 1); //tile in lower left
            pointList[6].Set(startingPoint.x - 1, startingPoint.y + 1); //tile in upper left
            pointList[7].Set(startingPoint.x + 1, startingPoint.y - 1); //tile in lower right
        }

        public static bool IsValidVectPoint(this Vector2Int point, RectGrid<MatchGridCell> matchGrid)
        {
            return point.x >= 0 && point.x < matchGrid.Width && point.y >= 0 && point.y < matchGrid.Height;
        }

        public static bool IsValidRectPoint(this RectPoint point, RectGrid<MatchGridCell> matchGrid)
        {
            return point.X >= 0 && point.X < matchGrid.Width && point.Y >= 0 && point.Y < matchGrid.Height;
        }

        public static void Reset2DDim(this bool[,] mutiArrayBool, bool defaultValue = false)
        {
            if (mutiArrayBool == null)
                return;

            for (var x = 0; x < mutiArrayBool.GetLength(0); x++)
                for (var y = 0; y < mutiArrayBool.GetLength(1); y++)
                    mutiArrayBool[x, y] = defaultValue;
        }

        public static RectPoint ToRectPoint(this Vector2Int point)
        {
            return new RectPoint(point.x, point.y);
        }

        public static Vector2Int ToVector2Int(this RectPoint point)
        {
            return new Vector2Int(point.X, point.Y);
        }
    }
}