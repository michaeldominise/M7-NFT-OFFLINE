using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using M7.Match;
using Gamelogic.Grids;
using M7.Match.PlaymakerActions;
using System.Linq;
using M7.GameRuntime;

public class AutoMatchAI : MonoBehaviour
{
    public static AutoMatchAI Instance => PuzzleBoardManager.Instance.AutoMatchAI;
    [SerializeField] AutoMatchSetting[] _automatchSettings;
    [SerializeField] GameObject _automatchButton;

    AutoMatchSetting _activeAutomatchSetting
    {
        get
        {

            foreach (AutoMatchSetting ams in _automatchSettings)
                if (ams.settingType == _activeSettingIndex)
                    return ams;
            
            return null;
        }
    }

    [SerializeField] AutoMatchSetting.AutoMatchSettingType _activeSettingIndex;
    [SerializeField] AutoMatchSetting.AutoMatchSettingType _activeEnemySettingIndex = AutoMatchSetting.AutoMatchSettingType.Level3;
    public AutoMatchSetting.AutoMatchSettingType ActivePlayerSettingIndex => _activeSettingIndex;
    public AutoMatchSetting.AutoMatchSettingType ActiveEnemySettingIndex => _activeEnemySettingIndex;


    public void SetAutomatchSetting(AutoMatchSetting.AutoMatchSettingType ams)
    {
        _activeSettingIndex = ams;
    }
    public float matchSpeedMin
    {
        get
        {
            return _activeAutomatchSetting.matchSpeedMin;
        }
    }

   
    public float matchSpeedMax
    {
        get
        {
            return _activeAutomatchSetting.matchSpeedMax;
        }
    }


    public float correctPct { get { return _activeAutomatchSetting.chanceForCorrectMatch; } }

    public float highestCountPriority
    {
        get
        {
            return _activeAutomatchSetting.highestCountPriority;
        }
    }

    public float minMatchesPerTurn
    {
        get
        {
            return _activeAutomatchSetting.minMatchesPerTurn;
        }
    }

    bool isActivated;
    [ShowInInspector]
    public bool IsActivated
    {
        get { return isActivated; }
        set
        {
            if (isActivated == value)
                return;  
            isActivated = value;
            if (isActivated)
                MakeReady();
        }
    }

    public PossibleMoveList possibleMoveList;

    public Action<PointList<RectPoint>> OnMoveFound;

    Coroutine makeReadyCoroutine;

    public int moveCount;

    private void Start()
    {
        return;
        //_automatchButton.SetActive(RPGGameData.GetLevelTotalStarCount(RPGGameData.Instance.LevelData) == 3 ||
        //    RPGGameData.Instance.LevelData.LevelGameMode == GameMode.DailyMission_Affinty ||
        //    RPGGameData.Instance.LevelData.LevelGameMode == GameMode.DailyMission_Exp ||
        //    RPGGameData.Instance.LevelData.LevelGameMode == GameMode.DailyMission_Gaianite ||
        //    RPGGameData.Instance.LevelData.LevelGameMode == GameMode.DailyMission_Enhancement ||
        //    RPGGameData.Instance.LevelData.LevelGameMode == GameMode.FriendQuest);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.A))
                IsActivated = !IsActivated;
            else if (Input.GetKeyDown(KeyCode.Alpha1))
                _activeSettingIndex = AutoMatchSetting.AutoMatchSettingType.Level1;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                _activeSettingIndex = AutoMatchSetting.AutoMatchSettingType.Level2;
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                _activeSettingIndex = AutoMatchSetting.AutoMatchSettingType.Level3;
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                _activeSettingIndex = AutoMatchSetting.AutoMatchSettingType.Level4;
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                _activeSettingIndex = AutoMatchSetting.AutoMatchSettingType.Level5;
        }
    }
#endif

    [Button]
    public void MakeReady()
    {
        if (makeReadyCoroutine != null)
            return;
        makeReadyCoroutine = StartCoroutine(_MakeReady());
    }

    IEnumerator _MakeReady()
    {
        bool canProceed = true;

        yield return new WaitWhile(() =>/* Turn.HasWaitRequest ||*/ !PuzzleBoardManager.Instance.IsMatchInputAllowed);

        if (BattleManager.Instance.IsGameDone)
            canProceed = false;

        else if (!BattleManager.Instance.EnemyTeam.IsAlive || !BattleManager.Instance.PlayerTeam.IsAlive)
            canProceed = false;

        //else if (!RPGGameManager.Instance.TurnManager.isCurrentState_Matchable)
        //    canProceed = false;

        else if (PuzzleBoardManager.Instance.CurrentState == PuzzleBoardManager.State.EndState)
            canProceed = false;

        if (canProceed) 
        {
            var grid = PuzzleBoardManager.Instance.ActiveGrid.Grid;
            foreach (var rectPoint in grid)
            {
                var tile = PuzzleBoardManager.Instance.ActiveGrid.GetMatchGridTile(rectPoint);
                if (tile != null && tile.CellTypeContainer.CellType.MatchValue < 0)
                {
                    canProceed = false;
                    //tile.OnTapExecute(() =>
                    //{
                    //    makeReadyCoroutine = null;
                    //    MakeReady();
                    //});
                    break;
                }
            }
        }

        if (canProceed)
        {
            //Debug.Log("AutoMatch TryAction");
            PointList<RectPoint> pointList = null;

            var rndVal = UnityEngine.Random.Range(matchSpeedMin, matchSpeedMax);
            yield return new WaitForSeconds(rndVal);

            var rndInt = UnityEngine.Random.value;
            if (moveCount < minMatchesPerTurn || rndInt < correctPct)
            {
                var randomMove = possibleMoveList.GetMove(highestCountPriority);
                if (randomMove != null)
                {
                    pointList = new PointList<RectPoint>
                {
                    randomMove.TargetCell.CurrentRectPoint,
                    randomMove.DestinationCell.CurrentRectPoint
                };
                }
                else
                    pointList = MoveRandom();
            }
            else
                pointList = MoveRandom();

            if (OnMoveFound != null && pointList != null)
                OnMoveFound(pointList);
            makeReadyCoroutine = null;

            moveCount++;
            //StartCoroutine(TestBreak());
        }
        else
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        makeReadyCoroutine = null;
    }

    IEnumerator TestBreak()
    {
        yield return new WaitForSeconds(0.05f);
        Debug.Break();
    }

    public PointList<RectPoint> MoveRandom()
    {
        var x = UnityEngine.Random.Range(0, PuzzleBoardManager.Instance.ActiveGrid.ColumnCount);
        var y = UnityEngine.Random.Range(0, PuzzleBoardManager.Instance.ActiveGrid.RowCount);
        
        var targetPoint = new RectPoint(x, y);

        while (PuzzleBoardManager.Instance.ActiveGrid.Grid[targetPoint].CurrentCellState != MatchGridCell.CellState.Active && PuzzleBoardManager.Instance.ActiveGrid.Grid[targetPoint].IsInteractible)
        {
            x = UnityEngine.Random.Range(0, PuzzleBoardManager.Instance.ActiveGrid.ColumnCount);
            y = UnityEngine.Random.Range(0, PuzzleBoardManager.Instance.ActiveGrid.RowCount);
            targetPoint = new RectPoint(x, y);
        }

        var destinationPoints = new Vector2Int[8];
        destinationPoints.InitAllSidesWithDiagonals(new Vector2Int(x, y));

        var destinationRectPoints = new List<RectPoint>();
        for (int i = 0; i < destinationPoints.Length; i++)
        {
            var rectPoint = destinationPoints[i].ToRectPoint();
            if (destinationPoints[i].IsValidVectPoint(PuzzleBoardManager.Instance.ActiveGrid.Grid) && PuzzleBoardManager.Instance.ActiveGrid.Grid[rectPoint].CurrentCellState == MatchGridCell.CellState.Active && PuzzleBoardManager.Instance.ActiveGrid.Grid[rectPoint].IsInteractible)
                destinationRectPoints.Add(rectPoint);
        }

        if (destinationRectPoints.Count != 0)
        {
            var destinationPoint = destinationRectPoints[UnityEngine.Random.Range(0, destinationRectPoints.Count)];
            return new PointList<RectPoint> { targetPoint, destinationPoint };
        }

        return null;
    }
}
