/*
 * MatchGridTouchHandler.cs
 * Author: Cristjan Lazar
 * Date: Oct 22, 2018
 */

using System;
using UnityEngine;

using Gamelogic.Grids;
using M7.GameRuntime;
using System.Collections.Generic;
using System.Collections;

namespace M7.Match {

    /// <summary>
    /// Base class for grid touch handlers.
    /// </summary>
    public abstract class MatchGridTouchHandlerBase : MonoBehaviour {

        [SerializeField] protected MatchGrid matchGrid;
        [SerializeField] protected Camera touchCamera;
        [SerializeField] protected PossibleMoveList possibleMoveList;

        protected PointList<RectPoint> TouchList = new PointList<RectPoint>();
        public static event Action<PointList<RectPoint>> OnTouchActionFinished;
        public static event Action<PointList<RectPoint>> OnTouchListIncrease;
        public static event Action<PointList<RectPoint>, RectPoint> OnTouchListDecrease;

        protected MatchGridCell firstTouchedCell;
        protected bool isInitialized;
        protected bool isMatchStarted;
        public virtual bool canInteract => !(BattleManager.Instance.CurrentState != BattleManager.GameState.Idle
                                  || AutoMatchAI.Instance.IsActivated
                                  || Mathf.Abs(Time.timeScale) < Mathf.Epsilon);

        public Action OnScuccessfulSwipe;

        protected List<PossibleMove> touchedTilePossibleMoves = new List<PossibleMove>();
        public bool hasOngoingSkillAnimation = false;

        public void Start()
        {
            PuzzleBoardManager.Instance.onMatchSwipeStarted += () => isMatchStarted = true;
            PuzzleBoardManager.Instance.onMatchEnded += (obj) => isMatchStarted = false;
        }

        protected static void TouchActionFinished (PointList<RectPoint> touchList) {
	        Debug.Log ("Touched Finished");
	        if (OnTouchActionFinished != null)
                OnTouchActionFinished(touchList);
        }

        protected static void TouchListIncreased (PointList<RectPoint> touchList) {
            if (OnTouchListIncrease != null)
                OnTouchListIncrease(touchList);
        }

        protected static void TouchListDecreased (PointList<RectPoint> touchList, RectPoint removedTile) {
            if (OnTouchListDecrease != null)
                OnTouchListDecrease(touchList, removedTile);
        }

        protected void AppendToTouchList (RectPoint point) {
            TouchList.Add(point);
            TouchListIncreased(TouchList);
        }

        protected RectPoint RemoveLastFromTouchList () {
            RectPoint top = TouchList[TouchList.Count - 1];
            TouchList.RemoveAt(TouchList.Count - 1);
            TouchListDecreased(TouchList, top);
            return top;
        }

        protected void ShowPossibleMoveHighlights(bool isShow, MatchGridCell selectedTile)
        {
            foreach (var possibleMove in touchedTilePossibleMoves)
                foreach (var tile in possibleMove.cells)
                    if (tile != selectedTile && tile.CurrentCellState == MatchGridCell.CellState.Active);
                        //tile.CellPulsiveGlow(isShow);
        }

        public virtual IEnumerator TouchHandler()
        {
            yield break;
        }

        #region Unity event methods
        public void StartTouchListener()
        {
            StartCoroutine(TouchHandler());
        }

        #endregion

        public virtual void StopTouchListener()
        {
            StopAllCoroutines();
            if (firstTouchedCell)
            {
                firstTouchedCell.CellMotor.Move(matchGrid.Map[firstTouchedCell.CurrentRectPoint]);
                firstTouchedCell.transform.localScale = Vector3.one;
            }
        }
    }
}

