using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using M7.GameRuntime;
using TMPro;
using Sirenix.OdinInspector;
using M7.Match;
using M7.Skill;
using M7.PuzzleBoard.Scripts.Booster;
using DG.Tweening;
using M7.ServerTestScripts;

public class MoveCounterManager : MonoBehaviour
{
    [SerializeField] GameObject parentPanel;
    [SerializeField] GameObject topPanel;
    [SerializeField] GameObject[] outOfMovesPanel;
    public GameObject[] OutOfMovesPanel => outOfMovesPanel;

    [SerializeField] public int currentMoveCount;
    [SerializeField] public int finalMoveCount;

    [SerializeField] int retryCounter;
    [SerializeField] int closeCounter;

    [SerializeField]public bool IsOutOfMoves => currentMoveCount <= 0;

    [SerializeField] TextMeshProUGUI moveCountText;
    [SerializeField] TextMeshProUGUI playAgainText;
    [SerializeField] TextMeshProUGUI gaiCountText;

    [SerializeField] SkillObject specialItemToGive;

    public void Init()
    {
        moveCountText.text = LevelManager.LevelData.TotalMoveCount.ToString("0");
        currentMoveCount = LevelManager.LevelData.TotalMoveCount;
        //gaiCountText.text = InitialMenuManager.Instance.GameInventoryManager.totalGaiCount.ToString("0");

        InitialMenuManager.Instance.GameInventoryManager.requireGai = InitialMenuManager.Instance.GameInventoryManager.gaiCost * InitialMenuManager.Instance.GameInventoryManager.multiplier;
    }

    [Button]
    public void SetCounter(bool isIncrement, int value, SkillObject specialItem)
    {
        if (isIncrement)
        {
            currentMoveCount += value;

            if (specialItem != null)
            {
                List<SkillObject> specialTilesToGive = new List<SkillObject>
                {
                    specialItemToGive
                };

                SpecialTileInstantiator.Execute(specialTilesToGive);
            }
        }
        else
        {
            currentMoveCount -= value;
        }

        moveCountText.text = currentMoveCount.ToString("0");
    }

    public IEnumerator CheckState(bool value)
    {
        parentPanel.SetActive(value);
        topPanel.SetActive(value);
        playAgainText.text = InitialMenuManager.Instance.GameInventoryManager.requireGai * InitialMenuManager.Instance.GameInventoryManager.multiplier + "";

        switch (value)
        {
            case true:
                PuzzleBoardManager.Instance.MatchGridTouchHandlerBase.StopTouchListener();
                DisableMovesPanel();

                if (retryCounter == 0)
                    OutOfMovesPanel[0].SetActive(true);
                else if (retryCounter >= 1)
                    OutOfMovesPanel[1].SetActive(true);
                //else if (retryCounter > 1)
                //    OutOfMovesPanel[3].SetActive(true);
                break;

            case false:
                yield return new WaitForSeconds(1);
                PuzzleBoardManager.Instance.MatchGridTouchHandlerBase.StartTouchListener();
                break;
        }

        //M7.GameData.PlayerDatabase.Inventories.Currencies.FindItem("Energy").Amount
    }
    public void ButtonTest(string btnValue)
    {
        switch (btnValue)
        {
            case "close":
                DisableMovesPanel();

                if (closeCounter == 0)
                    OutOfMovesPanel[2].SetActive(true);
                else if (closeCounter >= 1)
                {
                    parentPanel.SetActive(false);
                    OutOfMovesPanel[3].SetActive(true);
                }
                closeCounter++;
                break;
            case "retry":
                DownloadDataRuntime.Instance.Init(DownloadDataRuntime.ServerStatus.RetryMove, InitialMenuManager.Instance.GameInventoryManager.multiplier.ToString());
                break;
        }
    }

    public void GiveAdditionalMoves()
    {
        SetCounter(true, 5, retryCounter >= 1 ? specialItemToGive : null);
        StartCoroutine(CheckState(false));
        closeCounter = 0;
        retryCounter++;
        InitialMenuManager.Instance.GameInventoryManager.multiplier++;
        //gaiCountText.text = InitialMenuManager.Instance.GameInventoryManager.totalGaiCount.ToString("0");
    }

    public void DisableMovesPanel()
    {
        for (int i = 0; i < outOfMovesPanel.Length; i++)
        {
            OutOfMovesPanel[i].SetActive(false);
        }

    }
}
