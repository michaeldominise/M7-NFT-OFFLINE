using M7.Match;
using Gamelogic.Grids;
using M7.GameRuntime;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.Match.PlaymakerActions;
using System.Linq;
using M7.GameRuntime.Scripts.Managers.BattleScreen;
using PathologicalGames;
using M7.Skill;
using static M7.GameRuntime.BattleManager;

namespace M7.Match
{
    public class PuzzleBoardManager : MonoBehaviour
    {
        public enum State
        {
            Idle,
            WaitingForPlayerInput,
            TileSwipeStarted,
            PlayerUseSkill,
            ScanningForMatches,
            EndState
        }

        public static PuzzleBoardManager Instance => BattleManager.Instance.PuzzleBoardManager;

        [SerializeField] MatchGrid replacementGrid;
        [SerializeField] MatchGrid mainGrid;
        [SerializeField] MatchGrid secondaryGrid;
        [SerializeField] SpawnPool mainPool;
        [SerializeField] MatchGridCellSpawner activeTileSpawner;
        [SerializeField] MatchGridTouchHandlerBase matchGridTouchHandlerBase;
        [SerializeField] TimerBasedMovementManager timerBasedMovemenetManager;
        [SerializeField] PuzzleBoardSettings puzzleBoardSettings;
        [SerializeField] AutoMatchAI autoMatchAI;
        [SerializeField] OmniSpawnerManager omniSpawnerManager;
        [SerializeField] MatchTimerSlider matchTimerSlider;
        [SerializeField] RectPointListList chainList;
        [SerializeField] CellDeathManager tileDeathManager;
        [SerializeField] CellCombiner tileCombiner;
        [SerializeField] MatchGridCellSpawner matchGridCellSpawner;
        [ShowInInspector, ReadOnly] public State CurrentState { get; set; } = State.Idle;
        public MatchGrid ReplacementGrid => replacementGrid;
        public MatchGrid ActiveGrid => mainGrid;
        public MatchGrid SecondaryGrid => secondaryGrid;
        public SpawnPool ActivePool => mainPool;
        public MatchGridCellSpawner ActiveTileSpawner => activeTileSpawner;
        public MatchGridTouchHandlerBase MatchGridTouchHandlerBase => matchGridTouchHandlerBase;
        public PuzzleBoardSettings PuzzleBoardSettings => puzzleBoardSettings;
        public AutoMatchAI AutoMatchAI => autoMatchAI;
        public OmniSpawnerManager OmniSpawnerManager => omniSpawnerManager;
        public RectPointListList ChainList => chainList;
        public CellDeathManager TileDeathManager => tileDeathManager;
        public CellCombiner TileCombiner => tileCombiner;
        public MatchGridCellSpawner MatchGridCellSpawner => matchGridCellSpawner;

        public bool IsMatchInputAllowed => TurnManager.Instance.CurrentState == TurnManager.State.PlayerTurn || BattleManager.Instance.CurrentState == BattleManager.GameState.Idle || BattleManager.Instance.CurrentState == BattleManager.GameState.PuzzleBoard;
        const float shuffleInterval = 0.15f;
        float shuffleLastRequest;

        [ShowInInspector, ReadOnly] public int LastComboCount { get; set; }
        [ShowInInspector, ReadOnly] public ComboData LastComboData { get; set; }

        public Action onMatchSwipeStarted;
        public Action<ComboData> onMatchSwiped;
        public Action onMatchExecuteStarted;
        public Action<ComboData> onMatchExecuted;
        public Action<ComboData> onMatchEnded;
        public List<SkillEnums.ElementFilter> OmniAffectedElements { get; private set; } = new List<SkillEnums.ElementFilter>();

        // kit
        public RectPoint CellTouchPoint;

        [Button]
        public void Init(MatchGridEditorSavedLevelData levelData)
        {
            onMatchSwipeStarted += () =>
            {
                BattleManager.Instance.SetGameState(BattleManager.GameState.PuzzleBoard);
                matchTimerSlider.Show();
            };
            onMatchExecuted += (ComboData obj) =>
            {
                //BattleManager.Instance.MoveCounterManager.SetCounter(false, BattleManager.Instance.CurrentState != GameState.Win ? 1 : 0, null);
            };
            TargetManager_SelectDistinctCell.Reset();
        }

        [Button]
        public void RequestAdvanceShuffleAndScan(int minChains, float duration = 0.5f)
        {
            RequestAdvanceShuffle(minChains, duration);
            matchGridCellSpawner.RefreshGrid(duration + 0.1f);
        }                
        
        public void RequestAdvanceShuffle(int minChains, float duration = 0.5f)
        {
            if (shuffleLastRequest + shuffleInterval > Time.time)
                return;

            shuffleLastRequest = Time.time;
            AdvanceShuffle.RequestShuffle(ActiveGrid, minChains, duration);
        }

        public void RequestShuffle(float duration = 0.5f)
        {
            if (shuffleLastRequest + shuffleInterval > Time.time)
                return;

            shuffleLastRequest = Time.time;
            var shuffleDestinationList = ActiveGrid.Grid.Where(x => ActiveGrid.Grid[x] != null && ActiveGrid.Grid[x].CellTypeContainer.CellType.Shufflable).ToList();

            foreach (var rectPoint in ActiveGrid.Grid)
            {
                if (ActiveGrid.Grid[rectPoint] == null || !ActiveGrid.Grid[rectPoint].CellTypeContainer.CellType.Shufflable)
                    continue;

                var newRectPoint = shuffleDestinationList[UnityEngine.Random.Range(0, shuffleDestinationList.Count)];
                var fromCell = ActiveGrid.Grid[rectPoint];
                var toCell = ActiveGrid.Grid[newRectPoint];

                fromCell.CurrentRectPoint = newRectPoint;
                if (toCell != null)
                    toCell.CurrentRectPoint = rectPoint;

                ActiveGrid.Grid[rectPoint] = toCell;
                ActiveGrid.Grid[newRectPoint] = fromCell;
            }

            foreach (var rectPoint in ActiveGrid.Grid)
                ActiveGrid.Grid[rectPoint]?.CellMotor.MoveLerp(ActiveGrid.Map[rectPoint], duration);
            matchGridCellSpawner.RefreshSibling(ActiveGrid);
        }
        
        public List<MatchGridCell> RequestRandomTiles(int count, CellType tileType, Func<MatchGridCell, bool> tileCondition = null)
        {
            var grid = ActiveGrid.Grid;

            var validCells = grid.Where(p => grid[p].Matches(tileType) && (tileCondition == null || tileCondition(grid[p])))
                                 .ToPointList()
                                 .BetterShuffle();

            // Better shuffle js used here because just using Shuffle made it seem like spawn locations of Omni tiles
            // were not random enough. Could be false observation, in which case BetterShuffle() ought to be replaced
            // with just Shuffle().

            var result = new PointList<RectPoint>();

            int i = 0;
            int v = 1;

            while (i < count)
            {
                //if (overrideRandomPoints.Count > 0)
                //    result.Add(overrideRandomPoints.Dequeue());
                //else
                    result = result.Union(validCells.Except(result).Take(1)).ToPointList();

                i++;
            }

            return result.Select(p => grid[p])
                         .ToList();
        }
        public void ScanOmniAffectedElements(List<PointList<RectPoint>> chains)
        {
            foreach (var chain in chains)
            {
                var hasOmniTile = false;
                foreach (var rectPoint in chain)
                {
                    if (ActiveGrid.Grid[rectPoint].CellTypeContainer.MatchesExactly(SkillEnums.ElementFilter.All))
                    {
                        hasOmniTile = true;
                        break;
                    }
                }

                if (!hasOmniTile)
                    continue;

                foreach (var rectPoint in chain)
                {
                    var element = ActiveGrid.Grid[rectPoint].CellTypeContainer.CellType.ElementType;
                    if (!OmniAffectedElements.Contains(element))
                        OmniAffectedElements.Add(element);
                }
            }
        }

        public MatchGridCell RequestTileOnGrid(int column, int row, Func<MatchGridCell, bool> tileCondition = null)
        {
            var grid = ActiveGrid.Grid;
            var rectPoint = new RectPoint(column, row);
            return rectPoint.IsValidRectPoint(grid) && (tileCondition == null || tileCondition(grid[rectPoint])) ? grid[rectPoint] : null;
        }
    }
}