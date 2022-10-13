using UnityEngine;
using M7.GameRuntime;

public class MenuTeamSelector_EditTeam : MenuTeamSelector
{
    protected override void OnPlatformClicked(TeamManager_Editable teamManager, int index) 
    {
        if(teamManager.SelectedIndex < 0)
            teamManager.WaveData.SetSaveableCharacterAtIndex(index, null);
    }

    protected override void ExecuteButtonEvent(GameObject gameObject)
    {
        switch (gameObject.name)
        {
            case "Prev":
            case "Next":
                CurrentTeamSelected.ResetState();
                goto default;
            default:
                base.ExecuteButtonEvent(gameObject);
                break;
        }
    }
}
