using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using M7.GameData;
using System;
using M7.GameRuntime.Scripts.OnBoarding.Game;
using M7.Match;
using Sirenix.OdinInspector;
using M7.PuzzleBoard.Scripts.Booster;

namespace M7.GameRuntime
{
    public class TurnManager : MonoBehaviour
    {
        public enum State { PlayerTurn, EnemyTurn }
        public static TurnManager Instance => BattleManager.Instance?.TurnManager;

        public State FirstTurn { get; private set; }

        private State _state;

        [ShowInInspector, ReadOnly]
        public State CurrentState
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }
        public bool IsCurrentStateNewRound => CurrentState == FirstTurn;
        TeamManager_Battle PlayerTeam => BattleManager.Instance.PlayerTeam;
        TeamManager_Battle EnemyTeam => BattleManager.Instance.EnemyTeam;
        BattleData BattleData => BattleManager.Instance.BattleData;
        LevelData.GameModeType GameMode => LevelManager.LevelData.GameMode;

        public void Init()
        {
            switch (GameMode)
            {
                case LevelData.GameModeType.Adventure:
                case LevelData.GameModeType.PVP:
                    //var randomTurn = UnityEngine.Random.Range(0, 2);
                    //FirstTurn = randomTurn > 0 ? State.PlayerTurn : State.EnemyTurn;
                    FirstTurn = State.PlayerTurn;
                    SetCurrentState(FirstTurn);
                    // kit
                    // SetCurrentState(State.PlayerTurn);
                    break;
            }
        }

        public void InitNewRound()
        {
            BattleData.roundCount++;

            BattleData.manaRoundCount++;

            if (BattleData.manaRoundCount % 11 == 0)
            {
                BattleData.manaRoundCount = 1;
            }

            BattleManager.Instance.AIBehaviour.SetPattern();
        }

        void SetCurrentState(State state)
        {
            CurrentState = state;
            switch (state)
            {
                case State.PlayerTurn:
                    PlayerTeam.CurrentState = TeamManager_Battle.State.Ready;
                    EnemyTeam.CurrentState = TeamManager_Battle.State.EndTurn;
                    StartCoroutine(BattleManager.Instance.MoveCounterManager.CheckState(BattleManager.Instance.MoveCounterManager.IsOutOfMoves));
                    break;
                case State.EnemyTurn:
                    PlayerTeam.CurrentState = TeamManager_Battle.State.EndTurn;
                    EnemyTeam.CurrentState = TeamManager_Battle.State.Ready;
                    break;
            }
        }

        public void SetEndTurnState(float delay = 0)
        {
            // onboarding
            if (!PuzzleBoardOnBoardingManager.Instance.Stages["Stage 1"].IsDone && 
                !PuzzleBoardOnBoardingManager.Instance.Stages["Stage 1"].SegmentsDictionary["ShowRemainingMoves"].IsDone &&
                CurrentState == State.EnemyTurn)
            {
                PuzzleBoardOnBoardingManager.Instance.PopGameOnBoardingSegment("Stage 1", "ShowRemainingMoves", () =>
                {
                    SetCurrentState(CurrentState != State.PlayerTurn ? State.PlayerTurn : State.EnemyTurn);
                    BattleManager.Instance.SetGameState(IsCurrentStateNewRound ? BattleManager.GameState.NewRound : BattleManager.GameState.StartTurn);        
                });
                return;
            }
            
            SetCurrentState(CurrentState != State.PlayerTurn ? State.PlayerTurn : State.EnemyTurn);
            BattleManager.Instance.SetGameState(IsCurrentStateNewRound ? BattleManager.GameState.NewRound : BattleManager.GameState.StartTurn, delay);
        }
    }
}