using System.Collections;
using System.Collections.Generic;
using M7.GameRuntime.Scripts.OnBoarding.Game;
using UnityEngine;

public class PuzzleOnBoardingBehaviour_ShowRemainingMoves : OnBoardingSegmentBase
{
    public override void Execute()
    {
        base.Execute();
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.gameObject.SetActive(true);
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.ShowPanel(ShowDialog);
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("<color=green>Show Remaining Moves Exit</color>");
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.DestroyMoveCounter();
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.HidePanel();
    }

    private void ShowDialog()
    {
        Debug.Log("<color=green>Show Remaining Moves</color>");
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetBoxSize(
            new Vector2(700, 325));
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetBoxPosition(
            new Vector2(2, 460));
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetTailPosition(
            new Vector2(0, 185));
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetTailRotation(
            new Vector3(0, 0, 180));
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.ShowOkDialog("Defeat all enemies before you run out of moves.");
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.InstantiateMoveCounter();
    }
}
