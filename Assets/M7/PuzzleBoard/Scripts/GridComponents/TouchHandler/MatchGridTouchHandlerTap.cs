/*
 * MatchGridTouchHandler.cs
 * Author: Cristjan Lazar
 * Date: Oct 22, 2018
 */

using System;
using UnityEngine;

using Gamelogic.Grids;
using System.Collections;
using M7.GameRuntime;
using M7.GameRuntime.Scripts.OnBoarding.Game;
using M7.PuzzleBoard.Scripts.Booster;
using M7.Match.PlaymakerActions;

namespace M7.Match {

    /// <summary>
    /// Handles touch events for tap-based grids.
    /// </summary>
    public class MatchGridTouchHandlerTap : MatchGridTouchHandlerBase {
        public override bool canInteract => base.canInteract && !hasOnGoingTapExecute;
        bool hasOnGoingTapExecute;

        public override IEnumerator TouchHandler()
        {
            TouchList.Clear();

            isInitialized = true;
            while (true)
            {
                if (!canInteract)
                {
                    yield return null;
                    continue;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    // if (!PuzzleBoardManager.Instance.IsMatchInputAllowed ||
                    //     // for puzzle game onboarding
                    //     !PuzzleBoardOnBoardingManager.Instance.canClickCell)
                        if (!PuzzleBoardManager.Instance.IsMatchInputAllowed)
                    {
                        yield return null;
                        continue;
                    }

                    var posFromCam = touchCamera.transform.InverseTransformPoint(transform.position);
                    Vector3 screenPoint = Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;
                    screenPoint.z = posFromCam.z;
                    Vector3 firstPos = touchCamera.ScreenToWorldPoint(screenPoint);

                    RectGrid<MatchGridCell> grid = matchGrid.Grid;
                    IMap3D<RectPoint> map = matchGrid.Map;

                    RectPoint firstPoint = map[firstPos];

                    if (!grid.Contains(firstPoint) || grid[firstPoint] == null || grid[firstPoint].CurrentCellState != MatchGridCell.CellState.Active ||
                        //on boarding
                        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingClickChecker(firstPoint))
                    {
                        yield return null;
                        continue;
                    }
                    
                    // on boarding
                    if (LevelManager.LevelData.DisplayName == "Stage 1" && PuzzleBoardOnBoardingManager.Instance.segmentKey == "HighlightPuzzle")
                    {
                        PuzzleBoardOnBoardingManager.Instance.CurrentSegment.Exit();
                    }
                    
                    firstTouchedCell = grid[firstPoint];
                    PuzzleBoardManager.Instance.CellTouchPoint = firstPoint;

                    if (BoosterManager.Instance.isBoosterActive && BoosterManager.Instance.activeBoosterSkillObject != null)
                    {
                        hasOnGoingTapExecute = true;
                        BoosterManager.Instance.TurnIsInteractable(true);
                        BoosterManager.Instance.TurnIsInteractable(false);
                        PuzzleBoardManager.Instance.CurrentState = PuzzleBoardManager.State.PlayerUseSkill;
                        BoosterManager.Instance.ExecuteBooster(firstTouchedCell, () =>
                            ExecuteSpecialSphereAction.OnExecute(() =>
                            {
                                hasOnGoingTapExecute = false;
                                BoosterManager.Instance.OffToggle();
                            }));
                        yield return null;
                        continue;
                    }

                    if (!firstTouchedCell.IsInteractible)
                    {
                        yield return null;
                        continue;
                    }

                    Debug.Log("firstTouch : " + firstTouchedCell.gameObject.name);

                    AppendToTouchList(firstPoint);
                    TouchActionFinished(TouchList);
                    OnScuccessfulSwipe?.Invoke();

                    yield break;
                }

                yield return null;
            }
        }
        public override void StopTouchListener() { }
    }
}

