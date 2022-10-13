using UnityEngine;
using DG.Tweening;
using M7.GameRuntime;
using TMPro;
using M7.GameData;
using System.Collections;
using M7;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;
using System;
using M7.ServerTestScripts;
using UnityEngine.Events;

public class MenuTeamSelector : SceneManagerBase
{
    public static TeamData_Player SelectedTeamToShow { get; set; }
    public static float curTeamAtk;

    [SerializeField] AssetReference teamEditScene;
    [SerializeField] Camera selectorCamera;
    [SerializeField] TeamManager_Editable playerTeam_Editable;
    [SerializeField] Transform platformParentObject;
    [SerializeField] TextMeshProUGUI currentTeamSelected;
    [SerializeField] TextMeshProUGUI totalAttack;
    [SerializeField] TextMeshProUGUI totalHP;
    [SerializeField] PaginationManager pagination;
    [SerializeField] int poolSize = 3;
    [SerializeField] float teamObjectDistance = 5;
    [SerializeField] bool saveSelectedTeamName = true;

    public UnityEvent<float> horizontalTransition;
    public Transform PlatformParentObject => platformParentObject;
    [ShowInInspector, ReadOnly] int currentIndexRaw = -1;
    [ShowInInspector] int CurrentIndex => (int)Mathf.Repeat(currentIndexRaw, TeamPool.Count);
    [ShowInInspector] public TeamManager_Editable CurrentTeamSelected => TeamPool.Count > 0 ? TeamPool[CurrentIndex] : null;
    [ShowInInspector, ReadOnly] int IsReadyCount { get; set; }
    [ShowInInspector, ReadOnly] List<TeamManager_Editable> TeamPool { get; set; } = new List<TeamManager_Editable>();
    [ShowInInspector] public static bool Interactable { get; set; } = true;

    private IEnumerator Start()
    {
        yield return null;
        
        currentIndexRaw = -1;
        playerTeam_Editable.gameObject.SetActive(false);
        for (var x = 0; x < poolSize; x++)
        {
            var team = Instantiate(playerTeam_Editable, platformParentObject);
            team.gameObject.SetActive(true);
            team.OnPlatformClicked = OnPlatformClicked;
            team.SelectorCamera = selectorCamera;
            TeamPool.Add(team);
        }
        Refresh(true);
        if (saveSelectedTeamName)
            PlayerDatabase.Teams.onValuesChanged += Refresh;
    }
    
    private void OnDestroy() => PlayerDatabase.Teams.onValuesChanged -= Refresh;

    private void Refresh() => Refresh(false);
    private void Refresh(bool force)
    {
        var newIndex = PlayerDatabase.Teams.CurrentPartySelectedIndex;
        if(!force && Mathf.Repeat(currentIndexRaw, TeamPool.Count) == Mathf.Repeat(newIndex, TeamPool.Count))
            return;
            
        SelectTeam(newIndex, true);
    }

    void InitializeTeam(int index, TeamData_Player teamData_Player, Action onFinish)
    {
        var team = TeamPool[(int)Mathf.Repeat(index, TeamPool.Count)];
        team.transform.localPosition = Vector3.right * teamObjectDistance * index;
        team.Init(teamData_Player.Waves[0], onFinish);
    }

    void SelectTeam(int newIndex, bool instantTransition = false)
    {
        if(currentIndexRaw == newIndex)
            return;

        var loadDoneCount = 0;
        Action onLoadFinish = () =>
        {
            loadDoneCount++;
            IsReadyCount--;

            if (loadDoneCount == poolSize)
            {
                InitDisplayValues();
            }
        };

        currentIndexRaw = newIndex;
        for (var x = 0; x < poolSize; x++)
        {
            IsReadyCount++;
            var indexToLoad = x + currentIndexRaw - (int)(poolSize / 2f);
            InitializeTeam(indexToLoad, PlayerDatabase.Teams.GetPartyAtIndex(indexToLoad),  onLoadFinish);
        }

        if (saveSelectedTeamName)
        {
            PlayerDatabase.Teams.selectedTeamName = PlayerDatabase.Teams.GetPartyAtIndex(currentIndexRaw).TeamName;
            PlayerDatabase.Teams.IsDirty = true;
            PlayerDatabase.SaveToLocal();
            //DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.SetCharacterTeam);
        }
        RefreshPlayformPosition(instantTransition);
        pagination.UpdateValues(CurrentIndex, PlayerDatabase.Teams.teamDataList.Count);
    }

    public void InitDisplayValues()
    {
        currentTeamSelected.text = PlayerDatabase.Teams.GetPartyAtIndex(currentIndexRaw).TeamName;
        totalAttack.text = CurrentTeamSelected.TotalTeamAttack.ToString("0");
        totalHP.text = (CurrentTeamSelected.TotalTeamHp * 3).ToString("0");

        curTeamAtk = CurrentTeamSelected.TotalTeamAttack;
    }

    void RefreshPlayformPosition(bool instant)
    {
        var newPos = Vector3.left * teamObjectDistance * currentIndexRaw;
        if (instant)
        {
            horizontalTransition?.Invoke(newPos.x);
            platformParentObject.transform.localPosition = newPos;
        }
        else
        {
            IsReadyCount++;
            var localMove = platformParentObject.transform.DOLocalMove(newPos, 1);
            localMove.onUpdate = () => horizontalTransition?.Invoke(platformParentObject.localPosition.x);
            localMove.onComplete = () => IsReadyCount--;
        }
    }

    protected virtual void OnPlatformClicked(TeamManager_Editable teamManager, int index)
    {
        if(IsReadyCount == 0 && teamEditScene != null && Interactable)
            LoadScene(teamEditScene, UnityEngine.SceneManagement.LoadSceneMode.Additive, overwriteSceneLayer: 15);
    }

    protected override void OnActiveLayerChanged(int activeSceneLayer)
    {
        platformParentObject.gameObject.SetActive(activeSceneLayer == sceneLayer);
        base.OnActiveLayerChanged(activeSceneLayer);
    }

    protected override void ExecuteButtonEvent(GameObject gameObject)
    {
        switch (gameObject.name)
        {
            case "Prev":
                if (IsReadyCount == 0)
                    SelectTeam(currentIndexRaw - 1);
                break;
            case "Next":
                if (IsReadyCount == 0)
                    SelectTeam(currentIndexRaw + 1);
                break;
            default:
                base.ExecuteButtonEvent(gameObject);
                break;
        }
    }
}
