using M7.GameData;
using M7.Skill;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using Sirenix.OdinInspector;
using M7.FX;
using M7.Match;
using M7.GameRuntime.Scripts.Managers.Avatars;
using M7.GameRuntime.Scripts.Managers.BattleScreen;
using M7.GameRuntime.Scripts.OnBoarding.Game;
using M7.PuzzleBoard.Scripts.Booster;
using M7.PuzzleBoard.Scripts.SpecialTiles;

namespace M7.GameRuntime
{
    public class BattleManager : MonoBehaviour
    {
        public enum GameState { InitializingAssets, StartTurn, Idle, ExecutingSkills, PuzzleBoard, StartAttack, Attacking, EndTurn, NewRound, Win, Defeat, Tie }
        public static BattleManager Instance { get; private set; }
        public static Action onInitFinish;

        #region Instances
        [SerializeField] SkillManager skillManager;
        
        
        [SerializeField] CharacterAnimationEventManager characterAnimationEventManager;
       
        [SerializeField] LevelManager levelManager;
        [SerializeField] TeamManager_Battle playerTeam;
        [SerializeField] TeamManager_Battle enemyTeam;
        [SerializeField] TurnManager turnManager;
        
        [SerializeField] UIActionLogManager uiActionLogManager;

        [SerializeField] CountdownManager countdownManager;
        [SerializeField] GameFinishManager gameFinishManager;

        [SerializeField] WaveTransitionManager waveTransitionManager;
        [SerializeField] ParticleWorldManager particleWorldManager;
        [SerializeField] BattleCameraHandler battleCameraHandler;
        [SerializeField] CastVfxManager castVfxManager;
        [SerializeField] CharacterSelectionTargetManager characterSelectionTargetManager;
        [SerializeField] BattleMessageManager battleMessageManager;
        [SerializeField] AIBehaviour aIBehaviour;
        [SerializeField] PuzzleBoardManager puzzleBoardManager;
        [SerializeField] SkillQueueManager skillQueueManager;
        [SerializeField] TimerBasedMovementManager timerBasedMovemenetManager;
        [SerializeField] DisplayAvatarManager displayAvatarManager;
        [SerializeField] private BoosterManager boosterManager;
        [SerializeField] MoveCounterManager moveCounterManager;
        [SerializeField] private SpecialItemComboManager specialItemComboManager;
        [SerializeField] CurrentSceneChecker currentSceneChecker;
        [SerializeField] AttackInformationManager attackInformationManager;
        [SerializeField] SpecialTileUI specialTileUI;
        [SerializeField] StarConditionManager starConditionManager;
        [SerializeField] UIWaveManager uIWaveManager;
        [SerializeField] PlayerDataSaver playerDataSaver;
        [SerializeField] UIStatusValueManager hurtDamageManager;
        [SerializeField] MoveCounterCaster moveCounterCaster;
        [SerializeField] HintManager hintManager;
        [SerializeField] GaianiteCollectionManager gaianiteCollectionManager;
        [SerializeField] EndGameSet endGameSet;

        public TeamManager_Battle PlayerTeam => playerTeam;
        public TeamManager_Battle EnemyTeam => enemyTeam;
        public SkillManager SkillManager => skillManager;
        public CharacterAnimationEventManager CharacterAnimationEventManager => characterAnimationEventManager;
        public TurnManager TurnManager => turnManager;
        public LevelManager LevelManager => levelManager;
        public UIActionLogManager UIActionLogManager => uiActionLogManager;
        public CountdownManager CountdownManager => countdownManager;
        public WaveTransitionManager WaveTransitionManager => waveTransitionManager;
        public ParticleWorldManager ParticleWorldManager => particleWorldManager;
        public BattleCameraHandler BattleCameraHandler => battleCameraHandler;
        public CastVfxManager CastVfxManager => castVfxManager;
        public CharacterSelectionTargetManager CharacterSelectionTargetManager => characterSelectionTargetManager;
        public BattleMessageManager BattleMessageManager => battleMessageManager;
        public AIBehaviour AIBehaviour => aIBehaviour;
        public PuzzleBoardManager PuzzleBoardManager => puzzleBoardManager;
        public SkillQueueManager SkillQueueManager => skillQueueManager;
        public TimerBasedMovementManager TimerBasedMovemenetManager => timerBasedMovemenetManager;
        public DisplayAvatarManager DisplayAvatarManager => displayAvatarManager;
        public BoosterManager BoosterManager => boosterManager;
        public MoveCounterManager MoveCounterManager => moveCounterManager;
        public SpecialItemComboManager SpecialItemComboManager => specialItemComboManager;
        public CurrentSceneChecker CurrentSceneChecker => currentSceneChecker;
        public AttackInformationManager AttackInformationManager => attackInformationManager;
        public SpecialTileUI SpecialTileUI => specialTileUI;
        public StarConditionManager StarConditionManager => starConditionManager;
        public UIWaveManager UIWaveManager => uIWaveManager;
        public PlayerDataSaver PlayerDataSaver => playerDataSaver;
        public UIStatusValueManager HurtDamageManager => hurtDamageManager;
        public MoveCounterCaster MoveCounterCaster => moveCounterCaster;
        public HintManager HintManager => hintManager;
        public GameFinishManager GameFinishManager => gameFinishManager;
        public GaianiteCollectionManager GaianiteCollectionManager => gaianiteCollectionManager;
        public EndGameSet EndGameSet => endGameSet;
        #endregion Instances


        [SerializeField] BattleSceneSettings battleSceneSettings;
        public BattleSceneSettings BattleSettings => battleSceneSettings;
        [ShowInInspector, ReadOnly] public BattleData BattleData { get; private set; } = new BattleData();
        [ShowInInspector, ReadOnly] public GameState CurrentState { get; private set; } = GameState.InitializingAssets;
        [ShowInInspector] public TeamManager_Battle ActiveTeam => TurnManager.CurrentState == TurnManager.State.PlayerTurn ? PlayerTeam : EnemyTeam;

        public bool IsPlayerTeamObject(Component teamObject) => teamObject as MatchGridCell ? TurnManager.Instance.CurrentState == TurnManager.State.PlayerTurn : GetTeamObjects(true).Contains(teamObject);
        public TeamManager_Battle GetAllyTeam(Component teamObject) => IsPlayerTeamObject(teamObject) ? PlayerTeam : EnemyTeam;
        public TeamManager_Battle GetOpponentTeam(Component teamObject) => IsPlayerTeamObject(teamObject) ? EnemyTeam : PlayerTeam;
        public bool CanTeamObjectExecute(Component teamObject) => CurrentState == GameState.ExecutingSkills && TurnManager.CurrentState == (IsPlayerTeamObject(teamObject) ? TurnManager.State.PlayerTurn : TurnManager.State.EnemyTurn);
        public bool IsGameDone { get; set; } = false;

        //ActionHistory
        string actionType;
        string prepositionType;

        public List<Component> GetTeamObjects(bool isPlayer)
        {
            var teamObjects = new List<Component>();
            var team = isPlayer ? PlayerTeam : EnemyTeam;
            teamObjects.Add(team);
            teamObjects.AddRange(team.ActiveCharacters.Select(x => x as Component));
            return teamObjects;
        }

        private void Awake() => Instance = this;

        public void Start()
        {
            CurrentSceneChecker.Init();

            BattleData.roundCount = 0;
            var initCount = 0;
            Action onFinishTrigger = () =>
            {
                initCount++;
                if (initCount == 5)
                {
                    WaveTransitionManager.NewWaveTransition(true, CheckGameState); 
                    onInitFinish?.Invoke();
                }
            };

            WaveTransitionManager.Init();
            PlayerTeam.Init(PlayerDatabase.Teams.CurrentPartySelected, onFinishTrigger);
            EnemyTeam.Init(LevelManager.LevelData.TeamData, onFinishTrigger);
            LevelManager.Init(onFinishTrigger);
            DisplayAvatarManager.Init(PlayerDatabase.Teams.CurrentPartySelected, onFinishTrigger);
            //UIGameBattleManager.Init();
            PuzzleBoardManager.Init(LevelManager.LevelData.MatchGridEditorSavedLevelData);
            SkillQueueManager.Init();
            //BoosterManager.Init();
            if (LevelManager.LevelData.GameMode == LevelData.GameModeType.PVP)
            {
                CountdownManager.Init();
                CountdownManager.onTimerUpdate.gameObject.SetActive(true);
            }

            SkillManager.Init();
            MoveCounterManager.Init();
            SpecialTileUI.Init();
            StarConditionManager.Init();
            UIWaveManager.Init(EnemyTeam.WaveIndex, LevelManager.LevelData.TeamData.Waves.Count);
            GaianiteCollectionManager.Init();
            GameManager.Instance.InitAudio();
            BoosterManager.Instance.Init(onFinishTrigger);
        }

        public void SetGameState(GameState gameState, float delay = 0) => StartCoroutine(_SetGameState(gameState, delay));
        IEnumerator _SetGameState(GameState gameState, float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log($"[BattleManager.SetGameState]: {gameState}, [Turn State]: {TurnManager.Instance.CurrentState}");

            CurrentState = gameState;
            CheckGameState();
            CheckEndGameState();
        }

        public void CheckGameState()
        {
            #region ONBOARDING

            OnBoardingStartLevel();

            #endregion


            if (!PlayerTeam.IsAlive && LoadNextWaveTeam(PlayerTeam))
                CurrentState = GameState.InitializingAssets;
            
            if (!EnemyTeam.IsAlive && LoadNextWaveTeam(EnemyTeam))
                CurrentState = GameState.InitializingAssets;
            
            if (CurrentState != GameState.InitializingAssets)
            {
                if (!PlayerTeam.IsAlive)
                    CurrentState = LevelManager.GameMode != LevelData.GameModeType.Adventure && !EnemyTeam.IsAlive ? GameState.Tie : GameState.Defeat;
                else if (!EnemyTeam.IsAlive)
                {
                    CurrentState = GameState.Win;
                    PuzzleOverlayHandler.SetOverlay(false);
                }
            }

            switch (CurrentState)
            {
                case GameState.InitializingAssets:
                    ResetDamage();
                    CountdownManager.SetState(CountdownManager.State.Stop);

                    BoosterManager.Instance.SetPanels(false);

                    if (PlayerTeam.CurrentState != TeamManager_Battle.State.Initializing && EnemyTeam.CurrentState != TeamManager_Battle.State.Initializing)
                    {
                        TurnManager.Init();
                        SetGameState(GameState.NewRound);
                    }
                    break;
                case GameState.NewRound:
                    SkillManager.ExecuteSkillActionTriggers(SkillEnums.EventTrigger.NewRound, () =>
                    {
                        //TurnManager.InitNewRound();
                        SetGameState(GameState.StartTurn);
                    });
                    break;
                case GameState.StartTurn:
                    if(TurnManager.Instance.CurrentState == TurnManager.State.PlayerTurn)
                        PuzzleOverlayHandler.SetOverlay(false);
                    TimerBasedMovementManager.Instance.StartTimer();
                    TimerBasedMovementManager.Instance.hasTimerStartedThisTurn = false;

                    var finishCount = 0;
                    Action onFinishStartTurn = () =>
                    {
                        finishCount++;
                        if (finishCount != 2)
                            return;
                        if (LevelManager.GameMode == LevelData.GameModeType.PVP)
                        {
                            CountdownManager.SetState(CountdownManager.State.Start);
                            CountdownManager.onTimerUpdate.gameObject.SetActive(true);
                        }

                        if (TurnManager.CurrentState == TurnManager.State.PlayerTurn)
                            SetGameState(GameState.Idle);
                        else
                            SetGameState(GameState.StartAttack);
                    };

                    SkillManager.ExecuteSkillActionTriggers(SkillEnums.EventTrigger.StartTurnCaster, onFinishStartTurn);
                    SkillManager.ExecuteSkillActionTriggers(SkillEnums.EventTrigger.StartTurnTarget, onFinishStartTurn);
                    break;
                case GameState.Idle:
                    BoosterManager.Instance.TurnIsInteractable(true);
                    break;
                case GameState.ExecutingSkills:
                    break;
                case GameState.PuzzleBoard:
                    break;
                case GameState.StartAttack:
                    ExecuteAttack(() => SetGameState(GameState.EndTurn));
                    break;
                case GameState.Attacking:
                    break;
                case GameState.EndTurn:
                    CountdownManager.SetState(CountdownManager.State.Stop);

                    var finishStartCount = 0;
                    Action onFinishEndTurn = () =>
                    {
                        finishStartCount++;
                        if (finishStartCount != 2)
                            return;

                        var delay = 0f;
                        if(GetOpponentTeam(ActiveTeam).IsAlive)
                            delay = TurnManager.Instance.CurrentState == TurnManager.State.PlayerTurn ? BattleSceneSettings.Instance.HeroToEnemyTurnDelay : BattleSceneSettings.Instance.EnemyToHeroTurnDelay;
                        TurnManager.Instance.SetEndTurnState(delay);
                    };

                    SkillManager.ExecuteSkillActionTriggers(SkillEnums.EventTrigger.EndTurnCaster, onFinishEndTurn, TurnManager.Instance.CurrentState == TurnManager.State.EnemyTurn ? BattleSceneSettings.Instance.EnemyTurnCountdownDelay : 0);
                    SkillManager.ExecuteSkillActionTriggers(SkillEnums.EventTrigger.EndTurnTarget, onFinishEndTurn);
                    break;
            }
        }

        private static void OnBoardingStartLevel()
        {
            //onboarding
            if (LevelManager.LevelData.DisplayName == "Stage 1" &&
                !PuzzleBoardOnBoardingManager.Instance.Stages["Stage 1"].IsDone &&
                PuzzleBoardOnBoardingManager.Instance.Stages["Stage 1"].SegmentsDictionary["HighlightPuzzle"])
                PuzzleBoardOnBoardingManager.Instance.PopGameOnBoardingSegment("Stage 1",
                    "HighlightPuzzle");

            if (LevelManager.LevelData.DisplayName == "Stage 2" &&
                !PuzzleBoardOnBoardingManager.Instance.Stages["Stage 2"].IsDone &&
                PuzzleBoardOnBoardingManager.Instance.Stages["Stage 2"].SegmentsDictionary["ElementsAdvantage"])
                PuzzleBoardOnBoardingManager.Instance.PopGameOnBoardingSegment("Stage 2",
                    "ElementsAdvantage");
        }

        public void CheckEndGameState()
        {
            if (!IsGameDone)
            {
                switch (CurrentState)
                {
                    case GameState.Win:
                    case GameState.Defeat:
                    case GameState.Tie:
                        ResetDamage();
                        GameFinishManager.GameFinish();
                        IsGameDone = true;
                        HintManager.ShowHint = false;
                        BoosterManager.Instance.TurnIsInteractable(false);
                        BoosterManager.Instance.SetPanels(false);
                        break;
                }
            }
        }

        public void ExecuteAttack(Action onFinish) => StartCoroutine(_ExecuteAttack(onFinish));
        IEnumerator _ExecuteAttack(Action onFinish)
        {
            CurrentState = GameState.StartAttack;
            var charsToAttack = Instance.ActiveTeam.ActiveCharacters.ToList();
            for (int i = 0; i < charsToAttack.Count; i++)
            {
                var charBattle = charsToAttack[i];
                var skillObjectAttack = charBattle.BasicAttackSkillAsset;
                if (PlayerTeam.IsAlive && EnemyTeam.IsAlive && InitAttack(charBattle, ref skillObjectAttack))
                {
                    CurrentState = GameState.Attacking;
                    if (TurnManager.Instance.CurrentState == TurnManager.State.EnemyTurn && !PuzzleOverlayHandler.IsOverlayActive)
                    {
                        PuzzleOverlayHandler.SetOverlay(true);
                        yield return new WaitForSeconds(BattleSceneSettings.Instance.PuzzleBoardOverlayDuration + 0.25f);
                    }

                    SkillQueueManager.AddSkillToQueue(charBattle, skillObjectAttack, false, () =>
                    {
                        charBattle.StatsInstance.SetValue(SkillEnums.TargetCharacterStats.MatchBoardDamage, 0);
                        charBattle.StatsInstance.UpdateInstanceActions();
                    }, onStarted: () => charBattle.UIBattle_CharacterStats.AttackChargeText(0, 0));

                    yield return new WaitForSeconds(BattleSceneSettings.Instance.AttackIntervalPerCharacter);
                }
            }

            if (SkillQueueManager.CurrentState == SkillQueueManager.State.Idle)
                onFinish?.Invoke();
            else
            {
                var proceed = false;
                SkillQueueManager.WaitUntilIdle(() => proceed = true);
                yield return new WaitUntil(() => proceed);
                onFinish?.Invoke();
            }
        }

        bool InitAttack(CharacterInstance_Battle charBattle, ref SkillObject skillObjectAttack)
        {
            if (charBattle == null || !charBattle.IsAlive)
                return false;
            if (TurnManager.Instance.CurrentState == TurnManager.State.EnemyTurn && !InitEnemyAttack(charBattle as CharacterInstance_Battle_Enemy, ref skillObjectAttack))
                return false;
            else if (charBattle.StatsInstanceBattle.MatchBoardDamage <= 0)
                return false;

            return true;
        }

        bool InitEnemyAttack(CharacterInstance_Battle_Enemy charBattle, ref SkillObject skillObjectAttack)
        {
            if (charBattle == null)
                return false;

            
            var enemyStats = charBattle.StatsInstance as StatsInstance_CharacterBattle_Enemy;
            if (enemyStats.CanAttack())
            {
                if (charBattle.CharacterSkillData.IsReady)
                {
                    skillObjectAttack = charBattle.CharacterSkillAsset;
                    charBattle.StatsInstance.SetValue(SkillEnums.TargetCharacterStats.AttackTurn, 1);
                    charBattle.CharacterSkillData.Reset();
                }
                else
                    enemyStats.SetValue(SkillEnums.TargetCharacterStats.MatchBoardDamage, enemyStats.Attack);
            }
            else
                return false;
            return true;
        }

        bool LoadNextWaveTeam(TeamManager_Battle team)
        {
            if (team.CurrentState != TeamManager_Battle.State.Initializing && team.HasNextWave)
            {
                team.LoadNextWaveTeam(BattleSettings.WaveTransitionDelay, () => WaveTransitionManager.Instance.NewWaveTransition(this != PlayerTeam, CheckGameState));
                return true;
            }

            return false;
        }

        public void SetActionLog(CharacterInstance_Battle Subject, string SkillType, CharacterInstance_Battle Subject_2, string SkillName)
        {
            string logTime = Time.time.ToString("00:00");
            string actionLog;

            switch (SkillType)
            {
                case "Attack":
                    actionType = "attacked";
                    prepositionType = "with";

                    if (Subject.IsPlayerObject)
                    {
                        actionLog = "<color=cyan><b>" + Subject.SaveableData.DisplayName + "</b></color>" + " " + actionType + " " + "<color=red><b>" + Subject_2.SaveableData.DisplayName + "</b></color>" + " " + prepositionType + " " + "<b>" + SkillName + "</b>";
                        uiActionLogManager.SetInstance(actionLog, logTime);
                    }
                    else
                    {
                        actionLog = "<color=red><b>" + Subject.SaveableData.DisplayName + "</b></color>" + " " + actionType + " " + "<color=cyan><b>" + Subject_2.SaveableData.DisplayName + "</b></color>" + " " + prepositionType + " " + "<b>" + SkillName + "</b>";
                        uiActionLogManager.SetInstance(actionLog, logTime);
                    }

                    //BattleData.actionHistoryLog.Add(actionLog);
                    //BattleData.logTime.Add(logTime);
                    break;
                case "Skill":
                    actionType = "used";
                    prepositionType = "the";

                    if (Subject.IsPlayerObject)
                    {
                        actionLog = "<color=cyan><b>" + Subject.SaveableData.DisplayName + " " + "</b></color>" + actionType + " " + prepositionType + " " + "<b>" + SkillName + "</b>";
                        uiActionLogManager.SetInstance(actionLog, logTime);
                    }
                    else
                    {
                        actionLog = "<color=cyan><b>" + Subject.SaveableData.DisplayName + " " + "</b></color>" + actionType + " " + " " + prepositionType + " " + "<b>" + SkillName + "</b>";
                        uiActionLogManager.SetInstance(actionLog, logTime);
                    }

                    break;
                case "Defense":
                    actionType = "used";
                    prepositionType = "the";

                    if (Subject.IsPlayerObject)
                    {
                        actionLog = "<color=cyan><b>" + Subject.SaveableData.DisplayName + " " + "</b></color>" + actionType + " " + prepositionType + " " + "<b>" + SkillName + "</b>";
                        uiActionLogManager.SetInstance(actionLog, logTime);
                    }
                    else
                    {
                        actionLog = "<color=cyan><b>" + Subject.SaveableData.DisplayName + " " + "</b></color>" + actionType + " " + " " + prepositionType + " " + "<b>" + SkillName + "</b>";
                        uiActionLogManager.SetInstance(actionLog, logTime);
                    }
                    break;
            }
        }

        public void ResetDamage()
        {
            foreach (var charBattle in Instance.PlayerTeam.ActiveCharacters.Where(charBattle => charBattle.IsAlive))
            {
                charBattle.StatsInstance.SetValue(SkillEnums.TargetCharacterStats.MatchBoardDamage, 0);
                charBattle.UIBattle_CharacterStats.AttackChargeText(0);
            }
            foreach (var charBattle in Instance.EnemyTeam.ActiveCharacters.Where(charBattle => charBattle.IsAlive))
            {
                charBattle.StatsInstance.SetValue(SkillEnums.TargetCharacterStats.MatchBoardDamage, 0);
                charBattle.UIBattle_CharacterStats.AttackChargeText(0);
            }
        }
    }
}