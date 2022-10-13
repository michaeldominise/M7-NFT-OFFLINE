using M7.GameRuntime.Scripts.OnBoarding.Game;
using UnityEngine;

public class PuzzleOnBoardingBehaviour_ShowCharge : OnBoardingSegmentBase
{
    private bool _isActive;
    
    public override void Execute()
    {
        if(_isActive) return;

        _isActive = true;
        
        base.Execute();
        // PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.ShowDialog("Tap cubes beside drones to blast them");
        // PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.ShowPanel(ShowDialog);
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.gameObject.SetActive(true);
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetBoxPosition(new Vector2(0, -184));
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetBoxSize(new Vector2(700, 400));
        
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.ShowTail();
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetTailPosition(new Vector2(-285, 222));
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.SetTailRotation(
            new Vector3(0, 0, 180));
        ShowDialog();
        Debug.Log("<color=green>PuzzleOnBoardingBehaviour_ShowCharge</color>");
        // Time.timeScale = 0;
    }

    public override void Exit()
    {
        base.Exit();
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.HideTail();
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.HideDialog();
    }

    private void ShowDialog()
    {
        PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour.ShowOkDialog("Blasted cubes charge heroes for attack.");
    }
}
