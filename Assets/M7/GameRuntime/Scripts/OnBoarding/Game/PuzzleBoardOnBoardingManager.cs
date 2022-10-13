using System;
using System.Collections.Generic;
using Gamelogic.Grids;
using M7.Match;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.GameRuntime.Scripts.OnBoarding.Game
{
    public class PuzzleBoardOnBoardingManager : MonoBehaviour
    {
        public static PuzzleBoardOnBoardingManager Instance;

        [SerializeField] private PuzzleOnBoardingServer puzzleOnBoardingServer;
        
        [SerializeField] private PuzzleBoardOnBoardingUI puzzleBoardOnBoardingUI;

        [SerializeField] private PuzzleBoardStages stages;

        private OnBoardingSegmentBase _currentSegment;
        
        private Action _onExit;
        
        #region PROPS

        public PuzzleOnBoardingServer PuzzleOnBoardingServer => puzzleOnBoardingServer;
        
        public PuzzleBoardStages Stages
        {
            get => stages;
            set => stages = value;
        }

        public bool onBoardingIsActive;

        public string segmentKey;

        public OnBoardingSegmentBase CurrentSegment => _currentSegment;
        
        public PuzzleBoardOnBoardingUI PuzzleBoardOnBoardingUI => puzzleBoardOnBoardingUI;
        
        public List<MatchGridCell> possibleMove;

        #endregion


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                puzzleOnBoardingServer = new PuzzleOnBoardingServer(this);
                puzzleOnBoardingServer.GetPuzzleBoardJsonToServer();
                DontDestroyOnLoad(gameObject);
                return;
            }
            
            Destroy(gameObject);
        }


        // shows the next game on boarding segment
        public void PopGameOnBoardingSegment(string stageKey, string pSegmentKey, Action onExit = null)
        {
            try
            {
                // if (stage1PuzzleGameOnBoardingSegmentsDictionary[key].IsDone) return;
                if(!onBoardingIsActive) return;
                if (stages[stageKey].IsDone) return;

                segmentKey = pSegmentKey;
                
                _onExit = onExit;
                
                _currentSegment = stages[stageKey].SegmentsDictionary[pSegmentKey];
                _currentSegment.Execute();
            }
            catch (Exception e)
            {
                Debug.Log($"No more queues, {e.Message}");                
            }
        }

        public void Exit()
        {
            _currentSegment.Exit();
            _onExit?.Invoke();
        }
        
        public bool PuzzleBoardOnBoardingClickChecker(RectPoint rectPoint)
        {
            try
            {
                var contains =
                    Instance.possibleMove.Contains(PuzzleBoardManager.Instance.ActiveGrid.GetMatchGridTile(rectPoint));
                
                return segmentKey switch
                {
                    "Stage 1_HighlightPuzzle" => Instance.onBoardingIsActive && !Instance.Stages["Stage 1"].IsDone && !contains,
                    // "Stage 1_ShowCube" => Instance.onBoardingIsActive && !Instance._currentSegment.IsDone && !contains,
                    _ => false
                };
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return false;
            }
        }

        [Button]
        private void CheckPuzzleGameOnBoardingState()
        {
            foreach (var stage in stages)
                stage.Value.IsDone = true;
        }
        
        [Button]
        private void UnCheckPuzzleGameOnBoardingState()
        {
            foreach (var stage in stages)
                stage.Value.IsDone = false;
        }
    }
}