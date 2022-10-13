/*
 * FindChains.cs
 * Name: Cristjan Lazar
 * Date: Oct 31, 2018
 */

using System.Collections.Generic;

using Gamelogic.Grids;
using HutongGames.PlayMaker;
using UnityEngine;
using System.Linq;
using M7.PuzzleBoard.Scripts.SpecialTiles;
using M7.Skill;
using M7.GameRuntime;

namespace M7.Match.PlaymakerActions {

    [ActionCategory("M7/Match")]
    public class FindChains : FsmStateAction {
        public enum FindType { AllGridPoints, LastTouchPoints }

        public ChainMethodBase chainMethod;
        public RectPointList touchedPoints;
        MatchGrid matchGrid;
        public FsmEvent OnFoundChain;
        public FsmEvent OnEmptyChain;
        public FindType findType;
        public FsmBool executeOnMatchSwipe;
        public FsmBool matchIndicatorClearDamage;
        public FsmBool broadcastMatchResult;
        public bool dontAddChain;

        public static event System.Action OnMatch;
    
	    public override void OnEnter () {
		    Debug.Log("OnEnter");
            matchGrid = PuzzleBoardManager.Instance.ActiveGrid;

            List<PointList<RectPoint>> chains = GetChains();
            ResolveChainEvent(chains);
        }

        private List<PointList<RectPoint>> GetChains () {
            if (findType == FindType.AllGridPoints)
            {
                var chains = chainMethod.FindChains(matchGrid, matchGrid.Grid.ToPointList());
                return chains;
            }
            else
            {
                var listRectPoint = new PointList<RectPoint>();
                foreach (var touchedPoint in touchedPoints.Value)
                    listRectPoint.Add(touchedPoint);
                var chains = chainMethod.FindChains(matchGrid, listRectPoint);
                return chains;
            }
        }

        void SetChainsStateToLocked(List<PointList<RectPoint>> chains)
        {
            var cellSkillObject = SpecialItemComboManager.Instance.GetSpecialCellCombo(chains[0].Select(x => matchGrid.Grid[x].CellTypeContainer.CellType).ToList());
            if (cellSkillObject)
                SkillQueueManager.Instance.AddSkillToQueue(matchGrid.Grid[touchedPoints.Value[0]], cellSkillObject, waitToFinishCurrent: false, firstInQueue: true, onFinished: () => MatchGridCellSpawner.Instance.RefreshGrid());
            foreach (var chain in chains)
            {
                var hasOmniTile = false;
                foreach (var rectPoint in chain)
                {
                    if (!hasOmniTile && matchGrid.Grid[rectPoint].CellTypeContainer.MatchesExactly(SkillEnums.ElementFilter.All))
                    {
                        hasOmniTile = true;
                        break;
                    }
                }
                foreach (var rectPoint in chain)
                {
                    var cell = matchGrid.Grid[rectPoint];
                    if (cellSkillObject)
                        cell.IsSkillExecuted = true;
                    cell.CurrentCellState = MatchGridCell.CellState.Locked;
                }
            }
        }

        private void ResolveChainEvent (List<PointList<RectPoint>> chains) {
            var touchCell = matchGrid.Grid[touchedPoints.Value[0]];
            if (chains.Count > 0)
            {
                if (broadcastMatchResult.Value)
                    BroadcastMatchResult(chains);

                if (executeOnMatchSwipe.Value)
                    OnMatchSwipe(chains);

                var omniSphereData = OmniSpawnerManager.Instance.FindOmniSphereData(OmniSphereData.MainTriggerType.ConnectedTileCount, chains[0].Count, touchCell.CellTypeContainer.CellType.ElementType);
                if (matchGrid.Grid[touchedPoints.Value[0]].CellTypeContainer.CellType.ElementType == SkillEnums.ElementFilter.Special)
                    CellCombiner.Instance.TweenCell(chains[0], touchedPoints.Value[0], OnFoundChainExecute);
                else
                {
                    if(omniSphereData != null)
                        CellCombiner.Instance.TweenCell(chains[0], touchedPoints.Value[0], null);
                    OnFoundChainExecute();
                }

                OnMatch?.Invoke();
            }
            else if(touchCell.TouchSkillObject)
            {
                touchCell.ExecuteSkill();
                Fsm.Event(OnFoundChain);
            }
            else
                Fsm.Event(OnEmptyChain);

            void OnFoundChainExecute()
            {
                SetChainsStateToLocked(chains);
                BattleManager.Instance.MoveCounterManager.SetCounter(false, BattleManager.Instance.CurrentState != BattleManager.GameState.Win ? 1 : 0, null);
                Fsm.Event(OnFoundChain);
            }
        }

        void OnMatchSwipe(List<PointList<RectPoint>> chains)
        {
            GridOnMatchEvents.comboData.Reset();
            if(!dontAddChain)
                GridOnMatchEvents.comboData.AddChainList(chains);
            GridOnMatchEvents.comboData.InitConnectedTilesCount();
            MatchInterpreter.Instance.ClearDamage();
            //MatchInterpreter.Instance.Try_InterpretMatch(chains);
            PuzzleBoardManager.Instance.onMatchSwiped?.Invoke(GridOnMatchEvents.comboData);
        }

        void BroadcastMatchResult(List<PointList<RectPoint>> chains)
        {
            if(!dontAddChain)
                PuzzleBoardManager.Instance.ScanOmniAffectedElements(chains);
            //if(matchIndicatorClearDamage.Value)
            //    MatchInterpreter.Instance.ClearDamage();
            //MatchInterpreter.Instance.Try_InterpretMatch(chains);
            //MatchInterpreter.Instance.Try_InterpretMatch();
        }
    }
}

