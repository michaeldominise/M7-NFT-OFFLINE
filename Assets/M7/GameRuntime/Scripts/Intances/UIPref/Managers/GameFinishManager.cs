using DarkTonic.MasterAudio;
using Gamelogic.Grids;
using M7;
using M7.FX.VFX.Scripts;
using M7.GameData;
using M7.GameRuntime;
using M7.Match;
using M7.Match.PlaymakerActions;
using M7.ServerTestScripts;
using M7.Skill;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M7.GameRuntime.Scripts.OnBoarding.Game;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFinishManager : SceneManagerBase
{
    [SerializeField] GameObject GameOverPanel;
    [SerializeField] GameObject VictoryBannerPanel;

    [SerializeField] GameObject SkipPanel;
    [SerializeField] Image coverImage;
    [SerializeField] Image[] starImage;
    [SerializeField] Sprite winSprite;
    [SerializeField] Sprite loseSprite;
    [SerializeField] Sprite drawSprite;
    [SerializeField] TextMeshProUGUI[] titleText;
    [SerializeField] public AssetReference battleScene;
    [SerializeField] public AssetReference mainScene;

    [SerializeField] private GameObject defeatPlaceholder;
    [SerializeField] private GameObject victoryPlaceholder;

    [SerializeField] SkillObject skillObjectTest;

    public int currentStageIdx;
    [SerializeField] bool isFinishVictoryRocket;

    float Interval => vfxMovesFlyto.OnHitExecuteDuration;
    [SerializeField] VFXSkillSystem vfxMovesFlyto;

    //[SerializeField] int cubeCount;

    public Animator Animator => transform.GetChild(0).GetComponent<Animator>();

    Coroutine gameFinishCoroutine;
    Coroutine victoryRocketCoroutine;
    public void GameFinish() => gameFinishCoroutine = StartCoroutine(OnGameFinish(BattleManager.Instance.CurrentState));

    public bool IsGameFinishAnimationDone => gameFinishCoroutine == null;
    public bool IsRocketAnimationDone = false;

    public List<MatchGridCell> matchGridCellRocketList = new List<MatchGridCell>(); 

    IEnumerator OnGameFinish(BattleManager.GameState state)
    {
        UIBattleSettingsManager.Instance.isGameDone = true;
        foreach (var titletext in titleText)
            titletext.text = LevelManager.LevelData.DisplayName;

        MasterAudio.PlaylistsMuted = true;
        switch (state)
        {
            case BattleManager.GameState.Win:

                //SkipPanel.SetActive(true);
                SetFinalMoveCounter();
                yield return new WaitForSeconds(BattleSceneSettings.Instance.VictoryShowDelay + 0.01f);
                MasterAudio.PlaySound("End Game - Win Draft 3");
                VictoryBannerPanel.SetActive(true);

                AnimatorClipInfo[] animClipInfo = VictoryBannerPanel.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
                yield return new WaitForSeconds(animClipInfo[0].clip.length);
                yield return new WaitForSeconds(BattleSceneSettings.Instance.VictoryToRocketDelay);

                VictoryBannerPanel.SetActive(false);
                InitVictoryRocket(skillObjectTest, BattleManager.Instance.MoveCounterManager.finalMoveCount);

                //float rocketSpawn = 0.2f * BattleManager.Instance.MoveCounterManager.finalMoveCount;
                //float rocketExplosion = 0.4f * BattleManager.Instance.MoveCounterManager.finalMoveCount;
                //yield return new WaitForSeconds(rocketSpawn + rocketExplosion);
                yield return new WaitUntil(() => victoryRocketCoroutine == null);
                yield return new WaitUntil(() => ParticleAttractorManager.Instance.IsAnimating);
                yield return new WaitUntil(() => !ParticleAttractorManager.Instance.IsAnimating);
                yield return new WaitForSeconds(BattleSceneSettings.Instance.ResultScreenShowDelay);
                GameOverPanel.SetActive(true);
                //SkipPanel.SetActive(false);
                coverImage.sprite = winSprite;
                BattleManager.Instance.GaianiteCollectionManager.SaveCubeCount();
                //SetEndGameData(0);
                
                // onboarding
                if (!PuzzleBoardOnBoardingManager.Instance.Stages["Stage 1"].IsDone && 
                    !PuzzleBoardOnBoardingManager.Instance.Stages["Stage 1"].SegmentsDictionary["ShowGaianite"].IsDone)
                {
                    PuzzleBoardOnBoardingManager.Instance.PopGameOnBoardingSegment("Stage 1", "ShowGaianite");
                }
                
                break;
            case BattleManager.GameState.Defeat:
                coverImage.sprite = loseSprite;
                //yield return new WaitForSeconds(2.3f);
                defeatPlaceholder.SetActive(true);
                break;
            default:
                coverImage.sprite = drawSprite;
                break;
        }
        gameFinishCoroutine = null;
    }

    protected override void ExecuteButtonEvent(GameObject gameObject)
    {
        switch (gameObject.name)
        {
            case "Continue_Button":
                BattleManager.Instance.GaianiteCollectionManager.SetCollected();
                if(LevelManager.LevelData.MasterID == PlayerDatabase.CampaignData.currentStage)
                {
                    var currentStage = int.Parse(PlayerDatabase.CampaignData.currentStage.Replace("Stage_", ""));
                    PlayerDatabase.CampaignData.currentStage = $"Stage_{currentStage + 1}";
                }
                break;
            case "ContinueNoEnergy_Button":
                LoadScene(BattleManager.Instance.GameFinishManager.mainScene, LoadSceneMode.Single, overwriteSceneLayer: 0, forceLoad: true);
                break;
            case "TryAgain_Button":
                DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.Energy_EndGame);
                break;
            case "SkipEndGame_Button":
                SkillQueueManager.Instance.skip = true;
                StopCoroutine(gameFinishCoroutine);
                gameFinishCoroutine = null;
                SkipPanel.SetActive(false);
                GameOverPanel.SetActive(true);
                MasterAudio.PlaySound("End Game - Win Draft 3");
                Animator.SetBool("IsSkip", SkillQueueManager.Instance.skip);
                SetEndGameData(0);
                break;

        }
    }

    public void SetEndGameData(int starAmount)
    {
        for (int x = 0; x < PlayerDatabase.CampaignData.SaveableStageDatas.Count; x++)
        {

            if(PlayerDatabase.CampaignData.SaveableStageDatas[x].StageID == PlayerDatabase.CampaignData.currentStage)
            {
                SaveableStageData saveableStageDataTemp = PlayerDatabase.CampaignData.SaveableStageDatas[x];
                currentStageIdx = PlayerDatabase.CampaignData.SaveableStageDatas.FindIndex(x => x.Equals(saveableStageDataTemp));
                //PlayerDatabase.CampaignData.SaveableStageDatas[currentStageIdx].starsCompleted = starAmount;
            }
        }
    }

    void SetFinalMoveCounter()
    {
        BattleManager.Instance.MoveCounterManager.finalMoveCount = BattleManager.Instance.MoveCounterManager.currentMoveCount;
    }

    [Button]
    public void InitVictoryRocket(SkillObject skillObject, int amount) => victoryRocketCoroutine = StartCoroutine(_InitVictoryRocket(skillObject, amount));
    IEnumerator _InitVictoryRocket(SkillObject skillObject, int amount)
    {
        vfxMovesFlyto.UpdateProjectileTravelDuration(BattleSceneSettings.Instance.MovesToRocketTransformDuration);
        for (int i = 0; i < amount; i++)
        {
            SkillQueueManager.Instance.AddSkillToQueue(BattleManager.Instance.MoveCounterCaster, skillObject != null ? skillObject : skillObjectTest, waitToFinishCurrent: false, isForced: true);
            yield return new WaitForSeconds(BattleSceneSettings.Instance.MovesToRocketTransformInterval);
        }

        var proceed = false;
        SkillQueueManager.Instance.WaitUntilIdle(() => proceed = true);
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => proceed);
        yield return new WaitForSeconds(BattleSceneSettings.Instance.RocketStartExplosionDelay);

        var specialItems = new List<MatchGridCell>();
        var pointList = PuzzleBoardManager.Instance.ActiveGrid.Grid.ToPointList();
        for (int i = pointList.Count - 1; i >= 0; i--)
        {
            MatchGridCell cell = PuzzleBoardManager.Instance.ActiveGrid.Grid[pointList[i]];
            if (cell == null)
                continue;
            if (cell.CellTypeContainer.CellType.ElementType.HasFlag(SkillEnums.ElementFilter.Special))
                specialItems.Add(cell);
        }

        foreach (var specialItem in specialItems)
        {
            if (specialItem.ExecuteSkill(true))
            {
                proceed = false;
                SkillQueueManager.Instance.WaitUntilIdle(() => proceed = true);
                yield return new WaitUntil(() => proceed);
            }
        }

        yield return new WaitForSeconds(0.5f);
        MatchGridCellSpawner.Instance.RefreshGrid();
        yield return new WaitForSeconds(1.5f);
        MatchGridCellSpawner.Instance.ClearDespawn();
        victoryRocketCoroutine = null;
    }
}
