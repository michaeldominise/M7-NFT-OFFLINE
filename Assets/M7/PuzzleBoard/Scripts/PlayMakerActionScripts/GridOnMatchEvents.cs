/*
 * FindChains.cs
 * Name: Cristjan Lazar
 * Date: Oct 31, 2018
 */

using System.Collections.Generic;

using Gamelogic.Grids;
using HutongGames.PlayMaker;
using M7.GameRuntime.Scripts.OnBoarding.Game;
using M7.Skill;
using UnityEngine;

namespace M7.Match.PlaymakerActions {

    [ActionCategory("M7/Match")]
    public class GridOnMatchEvents : FsmStateAction {
        public enum Type { SwipeStarted, ExecuteStarted, Executed, Ended }

        public Type type;
        public RectPointListList chainList;
        static ComboData _comboData;
        public static ComboData comboData
        {
            get
            {
                if (_comboData == null)
                    _comboData = new ComboData();
                return _comboData;
            }
        }

        public override void Awake()
        {
            base.Awake();
            
        }

        public override void OnEnter () {
            switch(type)
            {
                case Type.SwipeStarted:
                    PuzzleBoardManager.Instance.onMatchSwipeStarted?.Invoke();
                    comboData.Reset();
                    Finish();
                    break;
                case Type.ExecuteStarted:
                    SkillQueueManager.Instance.WaitUntilIdle(() =>
                    {
                        PuzzleBoardManager.Instance.onMatchExecuteStarted?.Invoke();
                        comboData.Reset(false);
                        chainList.Value = new List<PointList<RectPoint>> { new PointList<RectPoint>() };
                        var rectPoints = PuzzleBoardManager.Instance.ActiveGrid.Grid.WhereCell(x => x != null && x.CurrentCellState == MatchGridCell.CellState.Locked);
                        MatchGridCellSpawner.Instance.DamageCells(rectPoints);
                        Finish();
                    });
                    break;
                case Type.Executed:
                    Finish();
                    break;
                case Type.Ended:
                    PuzzleBoardManager.Instance.onMatchEnded?.Invoke(comboData);
                    chainList.Value.Clear();
                    comboData.Reset();
                    Finish();
                    break;
            }
        }
    }

  

}

