/*
 * WaitForTouchedTiles.cs
 * Author: Cristjan Lazar
 * Date: Oct 30, 2018
 */

using System.Collections.Generic;
using UnityEngine;

using HutongGames.PlayMaker;
using Gamelogic.Grids;
using System;
using M7.GameRuntime;
using M7.PuzzleBoard.Scripts.Booster;

namespace M7.Match.PlaymakerActions
{

    [ActionCategory("M7/Match")]
    public class WaitForTouchedTiles : FsmStateAction
    {

        MatchGrid matchGrid;
        public RectPointList outTouchedPoints;

        public FsmEvent OnTouchedEvent;

        public override void OnEnter()
        {
            matchGrid = PuzzleBoardManager.Instance.ActiveGrid;
            PuzzleBoardManager.Instance.MatchGridTouchHandlerBase.StartTouchListener();
            TryInit_AutoMatchAI();
        }

        void TryInit_AutoMatchAI()
        {
            if (AutoMatchAI.Instance != null)
            {
                if (AutoMatchAI.Instance.IsActivated)
                {
                    AutoMatchAI.Instance.MakeReady();
                }
                else
                {
                    MatchGridTouchHandlerBase.OnTouchActionFinished += OnTouchAction;
                }

                AutoMatchAI.Instance.OnMoveFound += OnTouchAction;

            }
        }
        public override void OnExit()
        {
            AutoMatchAI.Instance.OnMoveFound -= OnTouchAction;
            MatchGridTouchHandlerBase.OnTouchActionFinished -= OnTouchAction;
            if (BoosterManager.Instance.IsBoosterInteractive)
                BoosterManager.Instance.TurnIsInteractable(false);
            PuzzleBoardManager.Instance.MatchGridTouchHandlerBase.StopTouchListener();
        }

        private void OnTouchAction(PointList<RectPoint> touchedPoints)
	    {
            Debug.Log("OnTouchAction");
            //if(BoosterManager.Instance.activeBoosterSkillObject != null)
            //    BoosterManager.Instance.TurnIsInteractable(false);
            PuzzleBoardManager.Instance.CurrentState = PuzzleBoardManager.State.ScanningForMatches;
            foreach(var rectpoint in touchedPoints)
                if (matchGrid.Grid[rectpoint].CurrentCellState != MatchGridCell.CellState.Active)
                    return;

            for (int i = 0; i < touchedPoints.Count; i++)
                outTouchedPoints.Value.Insert(i, new RectPoint(touchedPoints[i].X, touchedPoints[i].Y));

            Fsm.Event(OnTouchedEvent);
        }
    }
}

