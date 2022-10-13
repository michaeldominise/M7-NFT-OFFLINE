/*
 * MatchGridTouchHandlerSwipe.cs
 * Author: Cristjan Lazar
 * Date: Oct 22, 2018
 */
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using Gamelogic.Grids;

using UnityEngine.UI;
using M7.Match;
using M7.GameRuntime;

namespace M7.Match
{

    /// <summary>
    /// Handles touch events for swipe based grids.
    /// </summary>
    public class MatchGridTouchHandlerSwipe : MatchGridTouchHandlerBase
    {
        [SerializeField] string id;
        public string Id { get { return id; }
            set { id = value; } }


        private Vector2 cellDragDistance { get { return PuzzleBoardSettings.Instance.cellDimensions; } }
        private float cellTouchRadius { get { return PuzzleBoardSettings.Instance.cellTouchRadius; } }

        Vector3 currentVelocity;
        MatchGridCell destinationTile;

	    public PointList<RectPoint> GetTouchList // Swipe List.
        {
            get { return TouchList; }
        }

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

                if (Input.GetMouseButtonDown(0))
                {
                    if (!PuzzleBoardManager.Instance.IsMatchInputAllowed) {
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

                    var isBeyondDragDistance = false;

                    if (!grid.Contains(firstPoint) || grid[firstPoint].CurrentCellState != MatchGridCell.CellState.Active)
                    {
                        yield return null;
                        continue;
                    }

                    firstTouchedCell = grid[firstPoint];

                    if (!string.IsNullOrWhiteSpace(Id) && Id != firstTouchedCell.Id)
                    {
                        yield return null;
                        continue;
                    }

	                if (firstTouchedCell.TouchSkillObject && PuzzleBoardManager.Instance.CurrentState == PuzzleBoardManager.State.WaitingForPlayerInput)
                    {
                        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
                        PuzzleBoardManager.Instance.CurrentState = PuzzleBoardManager.State.PlayerUseSkill;
                        //firstTouchedTile.OnTapExecute(() =>
                        //{
                        //    print("TapExecuted");
                        //    PlaymakerActions.ScanPossibleMoves.Instance.Scan();
                        //    BattleManager.Instance.ExecuteAttack(() => PuzzleBoardManager.Instance.CurrentState = PuzzleBoardManager.State.WaitingForPlayerInput);
                        //});
                        yield return null;
	                }

                    if (!firstTouchedCell.IsInteractible)
                    { 
                        yield return null;
                        continue;
                    }

                    if (Vector2.Distance(firstTouchedCell.transform.position, firstPos) > cellTouchRadius)
                    {
                        yield return null;
                        continue;
                    }
					
	                Debug.Log ("firstTouch : " + firstTouchedCell.gameObject.name);

	                firstTouchedCell.SelectTile(true);
	                firstTouchedCell.transform.localScale = Vector3.one * 1.3f;
                    firstTouchedCell.transform.SetAsLastSibling();
                    Vector3 touchOffsetPos = firstTouchedCell.transform.position - firstPos;

                    Vector3 destinationPos;
                    RectPoint destinationPoint = new RectPoint();
                    MatchGridCell tryDestinationTile = null;

                    while (Input.GetMouseButton(0))
                    {
                        screenPoint = Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;
                        screenPoint.z = posFromCam.z;
                        destinationPos = touchCamera.ScreenToWorldPoint(screenPoint);
                        destinationPoint = map[destinationPos];
                        destinationPoint = new RectPoint(Mathf.Clamp(destinationPoint.X, firstPoint.X - 1, firstPoint.X + 1), Mathf.Clamp(destinationPoint.Y, firstPoint.Y - 1, firstPoint.Y + 1));

                        var touchDistance = Vector2.Distance(map[firstPoint], firstTouchedCell.transform.position);
                        //var touchDistance = map[firstPoint] - fistTouchedTile.transform.position;

                        //if (Mathf.Abs(touchDistance.x) > cellDragDistance.x || Mathf.Abs(touchDistance.y) > cellDragDistance.y)
                        if (touchDistance > cellDragDistance.y)
                        {
                            isBeyondDragDistance = true;
                            break;
                        }

                        if (grid.Contains(destinationPoint))
                        {
                            if (grid[destinationPoint] == firstTouchedCell)
                            {
                                if (tryDestinationTile != null)
                                {
                                    tryDestinationTile = null;
                                    ResetDestinationTile();
                                }
                            }
                            else if (grid[destinationPoint] != firstTouchedCell && tryDestinationTile != grid[destinationPoint])
                            {
                                tryDestinationTile = grid[destinationPoint];

                                if (destinationTile != grid[destinationPoint] && grid[destinationPoint].IsInteractible
                                    && grid[destinationPoint].CurrentCellState == MatchGridCell.CellState.Active
                                    && grid.GetNeighbors(firstPoint).Contains(destinationPoint)
                                    //&& OnPicked.PassesInvocationList(destinationPoint)
                                    && (string.IsNullOrWhiteSpace(Id) || Id == tryDestinationTile.Id))
                                {
                                    ShowPossibleMoveHighlights(false, firstTouchedCell);
                                    touchedTilePossibleMoves = possibleMoveList.GetPossibleMove(firstTouchedCell, grid[destinationPoint]);
                                    ShowPossibleMoveHighlights(true, firstTouchedCell);

                                    if (destinationTile != null)
                                    {
                                        destinationTile.CellMotor.Move(map[destinationTile.CurrentRectPoint]);
                                        currentVelocity = Vector3.zero;
                                    }

                                    destinationTile = grid[destinationPoint];
                                    destinationTile.transform.SetAsLastSibling();
                                    firstTouchedCell.transform.SetAsLastSibling();
                                }
                                else
                                    ResetDestinationTile();
                            }
                        }

                        if (destinationTile != null)
                        {
                            if (touchedTilePossibleMoves.Count > 0 || isMatchStarted)
                            {
                                var targetPos = Vector3.Lerp(map[destinationTile.CurrentRectPoint], map[firstPoint], touchDistance / cellDragDistance.y);
                                destinationTile.transform.position = Vector3.SmoothDamp(destinationTile.transform.position, targetPos, ref currentVelocity, 0.1f);
                            }
                            else
                                ResetDestinationTile();
                        }

                        destinationPos += touchOffsetPos;
                        //destinationPos.z = -1;
                        firstTouchedCell.transform.position = destinationPos;
                        yield return null;
                    }

                    ShowPossibleMoveHighlights(false, firstTouchedCell);

	                if ((isBeyondDragDistance || Input.GetMouseButtonUp(0)) && destinationTile != null)
                    {
                        AppendToTouchList(firstPoint);
                        AppendToTouchList(destinationTile.CurrentRectPoint);
                        TouchActionFinished(TouchList);
		                OnScuccessfulSwipe?.Invoke();

                        yield break;
                    }

                    firstTouchedCell.CellMotor.Move(map[firstPoint]);
                    firstTouchedCell.transform.localScale = Vector3.one;
                    firstTouchedCell.SelectTile(false);
	                ResetDestinationTile();
                    //TouchList.Clear();
                }

                yield return null;
            }
        }

        void ResetDestinationTile()
        {
            if (destinationTile)
            {
                ShowPossibleMoveHighlights(false, firstTouchedCell);

                destinationTile.CellMotor.Move(matchGrid.Map[destinationTile.CurrentRectPoint]);
                currentVelocity = Vector3.zero;
                destinationTile = null;
            }
        }

        public override void StopTouchListener()
        {
            base.StopTouchListener();
            ResetDestinationTile();
        }
    }
}

