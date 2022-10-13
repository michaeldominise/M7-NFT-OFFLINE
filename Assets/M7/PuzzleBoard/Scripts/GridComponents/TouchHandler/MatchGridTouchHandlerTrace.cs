/*
 * MatchGridTouchHandlerTrace.cs
 * Author: Cristjan Lazar
 * Date: Oct 22, 2018
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Gamelogic.Grids;

namespace M7.Match {

    /// <summary>
    /// Handles touch events for trace-based grids.
    /// </summary>
    public class MatchGridTouchHandlerTrace : MatchGridTouchHandlerBase {

        [SerializeField] private float cellSensivity = 0.4f;
        [SerializeField] private CellType wildTileType;

        private CellType traceTileType;
        private Stack<RectPoint> pointStack;
   
        // Coroutines not ideal.
        // Possible improvement here via UniRx implementation.
        private IEnumerator TouchHandler () {
            TouchList.Clear();

            while (true) {
                while (Input.GetMouseButton(0)) {
                    RectGrid<MatchGridCell> grid = matchGrid.Grid;
                    IMap3D<RectPoint> map = matchGrid.Map;

                    Vector3 pos = touchCamera.ScreenToWorldPoint(Input.mousePosition);
                    RectPoint point = map[pos];

                    // Make diagonal tracing easier via adjusting cell sensitivity
                    if (Vector2.Distance(pos, map[point]) > cellSensivity) {
                        yield return null;
                        continue;
                    }

                    if (TouchList.Count > 0 && !grid.GetNeighbors(point).Contains(pointStack.Peek())) {
                        yield return null;
                        continue;
                    }

                    if (!grid.Contains(point) || !grid[point].IsInteractible) {
                        yield return null;
                        continue;
                    }

                    MatchGridCell touchedTile = grid[point];
                    CellType touchedType = touchedTile.CellTypeContainer.CellType;

                    if (!touchedTile.Matches(traceTileType)) {
                        yield return null;
                        continue;
                    }

                    traceTileType = touchedType;

                    if (TouchList.Count == 0 || !TouchList.Contains(point)) {
                        AppendToTouchList(point);
                        pointStack.Push(point);

                    }
                    else if (TouchList.Count > 1 && point == TouchList[TouchList.Count - 2]) {
                        RemoveLastFromTouchList();
                        pointStack.Pop();
                    }

                    yield return null;
                }

                if(TouchList.Count > 0) {
                    TouchActionFinished(TouchList);
                    traceTileType = wildTileType;
                    TouchList.Clear();
                    pointStack.Clear();
                }

                yield return null;
            }
        }


        #region Unity event methods
        void Awake() {
            TouchList = new PointList<RectPoint>();
            pointStack = new Stack<RectPoint>();
        }

        void Start() {
            traceTileType = wildTileType;
            StartCoroutine(TouchHandler());
        }
        #endregion
    }

}

