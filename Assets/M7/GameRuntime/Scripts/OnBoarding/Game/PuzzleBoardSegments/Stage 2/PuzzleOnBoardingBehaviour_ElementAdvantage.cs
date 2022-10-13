using M7.GameRuntime;
using M7.GameRuntime.Scripts.OnBoarding.Game;
using UnityEngine;

public class PuzzleOnBoardingBehaviour_ElementAdvantage : OnBoardingSegmentBase
{
    [SerializeField] private Sprite elementSprite;

    private OnBoardingDialogBehaviour _onBoardingDialogBehaviour;
    
    public override void Execute()
    {
        base.Execute();
        // Debug.Log($"<color=green>{BattleManager.Instance.EnemyTeam.AliveCharacters[0].UiStatsTarget.gameObject.name}</color>");
        Debug.Log($"<color=green>{BattleManager.Instance.EnemyTeam.ActiveCharacters[0].UIBattle_CharacterStats.VfxElementObj.gameObject}</color>");

        _onBoardingDialogBehaviour = PuzzleBoardOnBoardingManager.Instance.PuzzleBoardOnBoardingUI.OnBoardingDialogBehaviour;
        
        _onBoardingDialogBehaviour.SetImage(elementSprite).ShowDisplayImage();

        _onBoardingDialogBehaviour.ShowOkDialog("Take advantage of the enemies elements for more damage.")
            .SetBoxSize(new Vector2(700, 1100)).SetBoxPosition(new Vector2(0, 340.51f));

        _onBoardingDialogBehaviour.HideTail();
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("");
    }
}
