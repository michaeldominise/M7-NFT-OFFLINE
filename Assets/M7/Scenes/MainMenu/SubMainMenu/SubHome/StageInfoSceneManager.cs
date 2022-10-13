using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using M7.GameData;
using System.Linq;
using M7.GameRuntime;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using M7;
using M7.ServerTestScripts;

public class StageInfoSceneManager : SceneManagerBase
{
    [SerializeField] GameObject goalUI;
    [SerializeField] Transform goalParentContent;

    [SerializeField] ScrollRect enemyScrollRect;
    [SerializeField] Transform enemyParentContent;
    [SerializeField] Transform starsParentContent;
    public LevelData LevelData => LevelManager.LevelData;
    public string MapName => LevelManager.ChapterData.DisplayName;
    public string StageName => LevelData.DisplayName;

    [SerializeField] TextMeshProUGUI mapNameText;
    [SerializeField] TextMeshProUGUI stageNameText;

    [SerializeField] TextMeshProUGUI wavesText;
    [SerializeField] TextMeshProUGUI stageCapText;
    [SerializeField] TextMeshProUGUI movesText;

    [SerializeField] CharacterInstance_Avatar charactersInstancePref;

    //THIS WILL HAVE ITS OWN SCRIPT
    [SerializeField] AssetReference teamInfoScene;

    protected override void Awake()
    {
        SetStageInfo();
        base.Awake();
    }

    private void Initer()
    {
        mapNameText.text = MapName;
        stageNameText.text = StageName;

        for (int i = 0; i < LevelData.GoalConditions.Length; i++)
            CreateGoalObject(LevelData.GoalConditions[i].DisplayIcon, LevelData.GoalConditions[i].GoalCount);

        Init(LevelData.TeamData.AllSaveableCharacters.Distinct(new Comparer()).ToList(), () => enemyScrollRect.horizontalNormalizedPosition = 0);
    }
    
    void CreateGoalObject(Sprite icon, int value)
    {
        GameObject GoalUI = Instantiate(goalUI, goalParentContent);
        GoalUI.transform.GetChild(0).GetComponent<Image>().sprite = icon;
        GoalUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "X" + value.ToString("0");
    }


    public void Init(List<SaveableCharacterData> saveableCharacters, System.Action onFinish)
    {
        var finishCount = 0;
        for (var x = 0; x < saveableCharacters.Count; x++)
            SetIcon(x, saveableCharacters[x], () =>
            {
                finishCount++;
                if (finishCount == saveableCharacters.Count)
                {
                    //PostInit();
                    onFinish?.Invoke();
                }
            });
    }

    public void SetIcon(int index, SaveableCharacterData saveableCharacter, System.Action onFinish)
    {
        if (saveableCharacter == null || string.IsNullOrWhiteSpace(saveableCharacter.MasterID) || index < 0)
        {
            onFinish?.Invoke();
            return;
        }

        var charInstance = Instantiate(charactersInstancePref, enemyParentContent, false);
        charInstance.name = $"{charactersInstancePref.name} {saveableCharacter.MasterID}";
        charInstance.Init(saveableCharacter, onFinish);
    }

    void InitStars()
    {
       for(int i = 0; i < PlayerDatabase.CampaignData.SaveableStageDatas.Count; i++)
        {
            if(PlayerDatabase.CampaignData.SaveableStageDatas[i].StageID == LevelData.MasterID)
            {
                for(int x = 0; x < PlayerDatabase.CampaignData.SaveableStageDatas[i].starsCompleted; x++)
                {
                    starsParentContent.GetChild(x).GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }

    void SetStageInfo()
    {
        wavesText.text = LevelManager.LevelData.TeamData.Waves.Count.ToString("0");
        stageCapText.text = LevelManager.LevelData.GaianiteCap.ToString("0.00");
        movesText.text = LevelManager.LevelData.TotalMoveCount.ToString("0");

        Initer();
        //InitStars();
    }

    class Comparer : IEqualityComparer<SaveableCharacterData>
    {
        public bool Equals(SaveableCharacterData x, SaveableCharacterData y) => x.MasterID == y.MasterID;

        public int GetHashCode(SaveableCharacterData obj) => obj.MasterID.GetHashCode();
    }

    protected override void ExecuteButtonEvent(GameObject gameObject)
    {
        switch (gameObject.name)
        {
            case "Team_Button":
                LoadScene(teamInfoScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
                break;
            case "Close_Button":
                UnloadAtSceneLayer(sceneLayer);
                break;
        }
    }
}

