using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using M7.Match;
using M7.Match.PlaymakerActions;
using Gamelogic.Grids;
using PathologicalGames;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using M7.GameRuntime;
using M7.Skill;
using M7.GameData;
using Random = UnityEngine.Random;
using System.Threading.Tasks;

public class OmniSpawnerManager : MonoBehaviour
{
    public static OmniSpawnerManager Instance => PuzzleBoardManager.Instance.OmniSpawnerManager;

    [SerializeField] OmniSphereData[] omniSphereDataList;
    
    [ReadOnly, ShowInInspector] public List<OmniSphereRequestData> requestOmniTileList { get; private set; } = new List<OmniSphereRequestData>();
    [ReadOnly, ShowInInspector] public List<RectPoint> forceSpawnRecPointList { get; private set; } = new List<RectPoint>();

    [ShowInInspector]
    private List<List<MatchGridCell>> matchGridCellFuseCollection = new List<List<MatchGridCell>>();

    public List<List<MatchGridCell>> MatchGridCellFuseCollection => matchGridCellFuseCollection;

    private void Start()
    {
        PuzzleBoardManager.Instance.onMatchEnded += TriggerByCombo; 
    }

    private void TriggerByCombo(ComboData comboData)
    {
        requestOmniTileList.Clear();

        if ((comboData?.ComboCount ?? 0) == 0)
            return;
        var omniSphereData = FindOmniSphereData(OmniSphereData.MainTriggerType.ComboCount, comboData.ComboCount, comboData.LastMatchedCellType.ElementType);

        if (omniSphereData == null)
            return;
        requestOmniTileList.Add(new OmniSphereRequestData
            {
                omniSphereData = omniSphereData,
                rectPointList = new List<RectPoint>()
            });
        //var addOmniTile = comboData.ComboCount - OmniSphereComboGenerationTrigger + 1;
        //if (addOmniTile > 0)
        //    requestOmniTileCount[RPGGameManager.Instance.TurnManager.turnManagerState] += Mathf.Min(addOmniTile, maxOmniSphereGeneration);
    }

    public void TriggerByConnectedTiles(ComboData comboData)
    {
        if ((comboData?.ComboCount ?? 0) == 0)
            return;
        //if (requestOmniTileCount[RPGGameManager.Instance.TurnManager.turnManagerState] >= 4)
        //    return;

        foreach (var connectedTile in comboData.RecentConnectedCellList)
        {

            var elementType = SkillEnums.ElementFilter.None;
            var rectPointList = new List<RectPoint>();
            foreach (var tile in connectedTile)
            {
                var eType = tile.CellTypeContainer.CellType.ElementType;
                if (eType != SkillEnums.ElementFilter.All && elementType == SkillEnums.ElementFilter.None)
                {
                    elementType = eType;
                    if (forceSpawnRecPointList.Count == 0)
                        break;
                }

                if (forceSpawnRecPointList.Contains(tile.CurrentRectPoint))
                {
                    rectPointList.Add(tile.CurrentRectPoint);
                    forceSpawnRecPointList.Remove(tile.CurrentRectPoint);
                }
            }

            var omniSphereData = FindOmniSphereData(OmniSphereData.MainTriggerType.ConnectedTileCount, connectedTile.Count, elementType);
            if (omniSphereData == null)
                continue;

            SpawnCheck(omniSphereData, connectedTile, rectPointList);
        }
    }
    
    private void SpawnCheck(OmniSphereData omniSphereData, List<MatchGridCell> connectedTile, List<RectPoint> rectPointList)
    {
        switch (omniSphereData.spawnType)
        {
            case OmniSphereData.SpawnType.OnTouchedTile:
                SpawnOnTouchTile(connectedTile, rectPointList, omniSphereData);
                break;
            case OmniSphereData.SpawnType.OnRandomConnectedTiles:
                SpawnRandomConnectedTile(connectedTile, rectPointList, omniSphereData);
                break;
            case OmniSphereData.SpawnType.OnRandomPuzzleBoard:
                SpawnRandomPuzzleBoard(rectPointList, omniSphereData);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SpawnOnTouchTile(List<MatchGridCell> connectedTile, List<RectPoint> rectPointList, OmniSphereData omniSphereData)
    {
        if(!rectPointList.Contains(PuzzleBoardManager.Instance.CellTouchPoint))
            rectPointList.Add(PuzzleBoardManager.Instance.CellTouchPoint);

        requestOmniTileList.Add(new OmniSphereRequestData
        {
            omniSphereData = omniSphereData,
            rectPointList = rectPointList
        });
        
        // foreach (var matchGridCell in connectedTile)
        // {
        //     matchGridCell.CellFusePlay(PuzzleBoardManager.Instance.ActiveGrid.Grid[PuzzleBoardManager.Instance.TileTouchPoint].transform.position);
        // }
    }
    
    private void SpawnRandomConnectedTile(List<MatchGridCell> connectedTile, List<RectPoint> rectPointList, OmniSphereData omniSphereData)
    {
        var connectedTileCopy = new List<MatchGridCell>(connectedTile);
        for (int rnd = Random.Range(0, connectedTileCopy.Count); 0 < connectedTileCopy.Count && rectPointList.Count < omniSphereData.spawnCount; rnd = Random.Range(0, connectedTileCopy.Count))
        {
            var tile = connectedTileCopy[rnd];
            if (!rectPointList.Contains(tile.CurrentRectPoint))
                rectPointList.Add(tile.CurrentRectPoint);
            connectedTileCopy.RemoveAt(rnd);
        }

        requestOmniTileList.Add(new OmniSphereRequestData
        {
            omniSphereData = omniSphereData,
            rectPointList = rectPointList
        });
    }
    
    private void SpawnRandomPuzzleBoard(List<RectPoint> rectPointList, OmniSphereData omniSphereData)
    {
        var puzzleBoardTileCopy = new List<MatchGridCell>(PuzzleBoardManager.Instance.ActiveGrid.Grid.Values);
        for (int rnd = Random.Range(0, puzzleBoardTileCopy.Count); 0 < puzzleBoardTileCopy.Count && rectPointList.Count < omniSphereData.spawnCount; rnd = Random.Range(0, puzzleBoardTileCopy.Count))
        {
            var tile = puzzleBoardTileCopy[rnd];
            if (!rectPointList.Contains(tile.CurrentRectPoint))
                rectPointList.Add(tile.CurrentRectPoint);
            puzzleBoardTileCopy.RemoveAt(rnd);
        }

        requestOmniTileList.Add(new OmniSphereRequestData
        {
            omniSphereData = omniSphereData,
            rectPointList = rectPointList
        });
    }
    
    public OmniSphereData FindOmniSphereData(OmniSphereData.MainTriggerType triggerType, int value, SkillEnums.ElementFilter elementType = SkillEnums.ElementFilter.All)
    {
        foreach (var omniSphereData in omniSphereDataList)
        {
            if (omniSphereData.mainTriggerType == triggerType && omniSphereData.IsWithinValueRange(value) && (omniSphereData.elementType | elementType) == omniSphereData.elementType)
            {
                return omniSphereData;
            }
        }
        return null;
    }

    public bool IsRectPointHasRequest(RectPoint point, OmniSphereData.SubTriggerType subTriggerType = OmniSphereData.SubTriggerType.TileDeath)
    {
        var grid = PuzzleBoardManager.Instance.ActiveGrid.Grid;
        for (int i = 0; i < requestOmniTileList.Count; i++)
        {
            OmniSphereRequestData requestOmniTile = requestOmniTileList[i];
            if (requestOmniTile.omniSphereData.subTriggerType != subTriggerType)
                continue;

            for (int j = 0; j < requestOmniTile.rectPointList.Count; j++)
            {
                RectPoint rectPoint = (RectPoint)requestOmniTile.rectPointList[j];
                if (point == rectPoint)
                {
                	int soaCount = requestOmniTile.omniSphereData.skillObjectToActivate.Count;
                    StartCoroutine(DelayedSkillExecuteAsync(grid[point], requestOmniTile.omniSphereData.skillObjectToActivate[Random.Range(0, soaCount)]));
                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator DelayedSkillExecuteAsync(MatchGridCell matchGridCell, SkillObject skillObject)
    {
        yield return new WaitForSeconds(0.35f);
        SkillQueueManager.Instance.AddSkillToQueue(matchGridCell, skillObject);
    }

    public void ForceSpawnRecPoint(MatchGridCell matchGridTile)
    {
        ForceSpawnRecPoint(matchGridTile.CurrentRectPoint);
    }

    public void ForceSpawnRecPoint(RectPoint rectPoint)
    {
        forceSpawnRecPointList.Add(rectPoint);
    }
}

[System.Serializable]
public class OmniSphereData
{
    [Serializable]
    public class ElementSpriteData
    {
        [HorizontalGroup] public Sprite displayIcon;
        [HorizontalGroup (width:100), HideLabel] public SkillEnums.ElementFilter elementType;
    }

    public enum MainTriggerType { ComboCount, ConnectedTileCount }
    public enum SubTriggerType { TurnStart, TileDeath }
    public enum SpawnType
    {
        OnTouchedTile,
        OnRandomConnectedTiles,
        OnRandomPuzzleBoard
    }
    
    [FormerlySerializedAs("omniTile")]
	public List	<SkillObject> skillObjectToActivate = new List<SkillObject> ();
    public Vector2Int valueRange;
    public SkillEnums.ElementFilter elementType = SkillEnums.ElementFilter.None;
    public int spawnCount;
    public MainTriggerType mainTriggerType = MainTriggerType.ConnectedTileCount;
    public SubTriggerType subTriggerType = SubTriggerType.TileDeath;
    public SpawnType spawnType = SpawnType.OnTouchedTile;
    public ElementSpriteData[] elementSpriteList;

    public bool IsWithinValueRange(int value)
    {
        return (value > valueRange.x && value < valueRange.y) || value == valueRange.x || value == valueRange.y;
    }

    public Sprite GetDisplayIcon(SkillEnums.ElementFilter elementType) => elementSpriteList.FirstOrDefault(x => x.elementType == elementType)?.displayIcon;
}

[System.Serializable]
public class OmniSphereRequestData
{
    public OmniSphereData omniSphereData;
    public List<RectPoint> rectPointList;
}
