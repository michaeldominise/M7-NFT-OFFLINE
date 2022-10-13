using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Grids;
using M7.GameRuntime;
using M7.GameRuntime.Scripts.OnBoarding.Game;
using M7.Match.PlaymakerActions;
using M7.Skill;
// using Gamelogic.Extensions;
using PathologicalGames;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.Match
{
    public class MatchGridCellSpawner : MonoBehaviour
    {
        public static MatchGridCellSpawner Instance => PuzzleBoardManager.Instance.MatchGridCellSpawner;

        [SerializeField] CellBag tileBag;
        [SerializeField] private Transform[] poolRootList;
        [SerializeField] RectPointListList chainList;
        [SerializeField] RectPointList touchedPoints;

        public List<CellType> TileTypeQueue => TileTypeQueue;
        public bool HasDespawn => despawnList.Count > 0;
        MatchGrid ActiveGrid => PuzzleBoardManager.Instance.ActiveGrid;
        SpawnPool ActivePool => PuzzleBoardManager.Instance.ActivePool;
        List<CellType> tileTypesQueue = new List<CellType>();
        List<MatchGridCell> despawnList = new List<MatchGridCell>();
        Coroutine updateTileIconCoroutine;
        Coroutine clearDespawnCoroutine;

        /// <summary>
        /// Initialize the specified tileBag.
        /// </summary>
        /// <param name="tileBag">Tile bag.</param>
        public void Initialize(CellBag tileBag)
        {
            this.tileBag = tileBag;

        }

        /// <summary>
        /// Spawn the specified worldPos and specificTileType if is set else get tile from queue.
        /// </summary>
        /// <returns>The MatchGridTile.</returns>
        /// <param name="worldPos">World position.</param>
        /// <param name="specificTileType">Specific tile type.</param>
        public MatchGridCell Spawn(Vector3 worldPos, CellType specificTileType = null, SkillEnums.CellGridLocation cellGridLocation = SkillEnums.CellGridLocation.Main)
        {
            return Spawn(worldPos, null, specificTileType, cellGridLocation);
        }

        /// <summary>
        /// Spawn the specified worldPos and specificTileType if is set else get tile from queue.
        /// </summary>
        /// <returns>The spawn.</returns>
        /// <param name="worldPos">World position.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="specificTileType">Specific tile type.</param>
        public MatchGridCell Spawn(Vector3 worldPos, Transform parent, CellType specificTileType = null, SkillEnums.CellGridLocation cellGridLocation = SkillEnums.CellGridLocation.Main)
        {
            var tileType = specificTileType;
            if (specificTileType == null)
            {
                if (cellGridLocation != SkillEnums.CellGridLocation.Main)
                    return null;
                //if (tileQueue.Count == 0)
                //    ResetQueue();
                //tileType = tileQueue.Dequeue();
                if (tileTypesQueue.Count == 0)
                    ResetQueue();
                //tileType = tileTypesQueue.
                tileType = Dequeue();
            }

            var obj = ActivePool.Spawn(tileType.Prefab, worldPos, Quaternion.identity);
            obj.transform.localScale = Vector3.one;

            var tileObject = obj.GetComponent<MatchGridCell>();
            tileObject.Initialize(tileType);
            obj.parent = parent ?? poolRootList[(int)tileObject.CellTypeContainer.CellType.TileGridLocation];

            return tileObject;
        }

        /// <summary>
        /// Despawn the tile and make it available to pool again.
        /// </summary>
        /// <param name="tile">Tile.</param>
        public void Despawn(MatchGridCell tile, float delay = 0)
        {
            tile.transform.parent = ActivePool.transform;
            despawnList.Add(tile);
            StartCoroutine(_Despawn(tile, delay));
        }

        IEnumerator _Despawn(MatchGridCell tile, float delay)
        {
            yield return new WaitForSeconds(delay);
            tile.gameObject.SetActive(false);
        }

        public void ClearDespawn(float delay = 0)
        {
            if (clearDespawnCoroutine != null || despawnList.Count == 0)
                return;
            clearDespawnCoroutine = StartCoroutine(_ClearDespawn(delay));
        }
        IEnumerator _ClearDespawn(float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            var despawnListCopy = new List<MatchGridCell>(despawnList);
            despawnList.Clear();

            var cellElementGroup = new Dictionary<SkillEnums.ElementFilter, List<MatchGridCell>>();
            
            foreach (var cell in despawnListCopy)
            {
                if (!cellElementGroup.ContainsKey(cell.CellTypeContainer.CellType.ElementType))
                    cellElementGroup[cell.CellTypeContainer.CellType.ElementType] = new List<MatchGridCell>();
                if (cell.ShowParticleAttactor)
                    cellElementGroup[cell.CellTypeContainer.CellType.ElementType].Add(cell);
            }

            if(TurnManager.Instance.CurrentState == TurnManager.State.PlayerTurn)
                ParticleAttractorManager.Instance.Spawn(cellElementGroup, () =>
                {
                    foreach (var cell in despawnListCopy)
                        ActivePool.Despawn(cell.transform);
                });
            clearDespawnCoroutine = null;
        }

        /// <summary>
        /// Resets the queue.
        /// </summary>
        public void ResetQueue()
        {
            var cellListCopy = new List<CellType>(tileBag.TileTypeList);
            var grouptotalWeight = PuzzleBoardSettings.Instance.groupColorChanceRates.Length > 0 ? PuzzleBoardSettings.Instance.groupColorChanceRates.Sum(x => x.weightRate) : 0;
            while (cellListCopy.Count > 0)
            { 
                var rnd = Random.Range(0, cellListCopy.Count);
                if (grouptotalWeight > 0)
                {
                    var groupRnd = Random.Range(0, grouptotalWeight);
                    var weightValue = 0;
                    foreach (var groupColorChanceRate in PuzzleBoardSettings.Instance.groupColorChanceRates)
                    {
                        weightValue += groupColorChanceRate.weightRate;
                        if(groupRnd < weightValue)
                        {
                            var targetCellType = cellListCopy[rnd];
                            for (var x = 0; x < groupColorChanceRate.cellCount; x++)
                            {
                                var cellType = tileBag.TileTypeList.FirstOrDefault(x => x == targetCellType);
                                if (cellType == null)
                                    break;
                                Enqueue(cellType);
                                cellListCopy.Remove(cellType);
                            }
                            break;
                        }
                    }
                }
                else
                {
                    Enqueue(tileBag.TileTypeList[rnd]);
                    cellListCopy.RemoveAt(rnd);
                }
            }
        }

        public void InsertAtStart(IEnumerable<CellType> newTileTypes)
        {
            var items = newTileTypes.ToArray();

            for (int i = items.Length - 1; i >= 0; i--)
                InsertAtStart(items[i]);
        }

        public void InsertAtStart(CellType newTileType)
        {
            if (tileTypesQueue.Count == 0)
                ResetQueue();

            tileTypesQueue.Insert(0, newTileType);
        }

        public void Enqueue(CellType newTileType)
        {
            tileTypesQueue.Add(newTileType);
        }

        private CellType Dequeue()
        {
            var item = tileTypesQueue[0];

            tileTypesQueue.RemoveAt(0);

            return item;
        }

        [Button]
        public void DamageCells(List<Vector2Int> vector2List) => DamageCells(vector2List.Select(x => x.ToRectPoint()).ToList());

        public void DamageCells(IEnumerable<RectPoint> rectPointList)
        {
            if (chainList.Value == null)
                chainList.Value = new List<PointList<RectPoint>>();
            if (chainList.Value.Count == 0)
                chainList.Value.Add(new PointList<RectPoint>());
            foreach (var rectPoint in rectPointList)
                if (!chainList.Value[0].Contains(rectPoint))
                    chainList.Value[0].Add(rectPoint);

            var hasDeadCell = false;
            if (chainList.Value[0].Count > 0)
            {
                InitComboData();
                hasDeadCell = TileChainDamager.DamageTileChain(rectPointList.Select(x => ActiveGrid.Grid[x]).ToList());
            }

            if (!hasDeadCell)
                return;

            RefreshGrid();
        }

        void InitComboData()
        {
            GridOnMatchEvents.comboData.GridCleared();
            GridOnMatchEvents.comboData.AddChainList(chainList);
            PuzzleBoardManager.Instance.LastComboData = GridOnMatchEvents.comboData;
            PuzzleBoardManager.Instance.LastComboCount = GridOnMatchEvents.comboData.ComboCount;
            if (GridOnMatchEvents.comboData.GridClearCount > 1)
                GridOnMatchEvents.comboData.UpdateConnectedTilesCount();
            MatchInterpreter.Instance.Try_InterpretMatch(chainList.Value);

            if (chainList != null)
            {
                PuzzleBoardManager.Instance.onMatchExecuted?.Invoke(GridOnMatchEvents.comboData);
                if (touchedPoints.Value != null && touchedPoints.Value.Count > 0)
                {
                    var cell = PuzzleBoardManager.Instance.ActiveGrid.Grid[touchedPoints.Value[0]];
                    if (cell != null && cell.CellTypeContainer.CellType.ElementType != SkillEnums.ElementFilter.Special)
                    {
                        OmniSpawnerManager.Instance.requestOmniTileList.Clear();
                        OmniSpawnerManager.Instance.TriggerByConnectedTiles(GridOnMatchEvents.comboData);
                    }
                }
            }
        }

        public void RefreshGrid(float delay = 0) => StartCoroutine(_RefreshGrid(delay));

        IEnumerator _RefreshGrid(float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            yield return new WaitUntil(() => CellCombiner.IsDone);

            DropTiles.DropGrid(DropTiles.DropMode.FROM_ORIGIN);
            ActivateTileMotors.MoveTiles();
            ReplaceTiles.SpawnTiles(TallyReplacementTiles.CountReplacementTiles());
            DropTiles.DropGrid(DropTiles.DropMode.FROM_TOP);
            ActivateTileMotors.MoveTiles();
            RefreshSibling(ActiveGrid);
        }

        public void RefreshSibling(MatchGrid matchGrid)
        {
            foreach (var rectPoint in matchGrid.Grid)
            {
                var cell = matchGrid.Grid[rectPoint];
                if (cell == null)
                    continue;
                var childCount = PuzzleBoardManager.Instance.ActiveGrid.ColumnCount * cell.CurrentRectPoint.Y + cell.CurrentRectPoint.X;
                cell.transform.SetSiblingIndex(childCount);
                cell.container.transform.localPosition += Vector3.back * (0.01f * childCount + cell.container.transform.localPosition.z);
            }

            if(updateTileIconCoroutine == null)
                updateTileIconCoroutine = StartCoroutine(ExecuteUpdateTileIcons());
        }

        IEnumerator ExecuteUpdateTileIcons()
        {
            yield return null;
            yield return new WaitWhile(() => 
            {
                foreach (var rectPoint in ActiveGrid.Grid)
                    if (ActiveGrid.Grid[rectPoint] != null && ActiveGrid.Grid[rectPoint].CellMotor.IsMoving)
                        return true;
                return false;
            });
            var possibleMoves = ScanPossibleMoves.Scan(ScanPossibleMoves.ScanType.Adjacent);
            UpdateTileIcons.UpdateIcon(possibleMoves, OmniSphereData.MainTriggerType.ConnectedTileCount);
            updateTileIconCoroutine = null;
        }
    }
}
