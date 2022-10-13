using System.Collections;
using System.Collections.Generic;
using System.Linq;
using M7.GameData;
using M7.GameRuntime;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
// using AnimatorController = UnityEditor.Animations.AnimatorController;
//using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;
using Image = UnityEngine.UI.Image;

public class PreloadBattleScript : MonoBehaviour
{
    [SerializeField] PreloadbattleTeamManager playerTeam;
    [SerializeField] PreloadbattleTeamManager enemyTeam;
    [SerializeField] LevelData levelData;
    [SerializeField] private TextMeshProUGUI PlayerName;
    [SerializeField] private Image blackLayer;
    [SerializeField] float closeDuration = 1;
    [SerializeField] Animator PreloadingAnimator;
    [SerializeField] GameObject loadingIcon;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        loadingIcon.SetActive(false);
        yield return new WaitForSeconds(closeDuration);
        loadingIcon.SetActive(true);
        playerTeam.Init(PlayerDatabase.Teams.CurrentPartySelected.Waves[0], null);
        enemyTeam.Init(levelData.TeamData.Waves[levelData.TeamData.Waves.Count-1], null);
     //   PlayerName.text= Player.CurrentPlayer.profileName;
        DontDestroyOnLoad(gameObject);
        BattleManager.onInitFinish += ExecuteFadeOut;
    }

    public IEnumerator FadeIn()
    {
        PreloadingAnimator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(PreloadingAnimator.GetFloat("MotionTime"));
        blackLayer.color = Color.clear;
    }

    void ExecuteFadeOut() => StartCoroutine(FadeOut());

    public IEnumerator FadeOut()
    {
        loadingIcon.SetActive(false);
        yield return new WaitForEndOfFrame();
        PreloadingAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(PreloadingAnimator.GetFloat("MotionTime"));
        BattleManager.onInitFinish -= ExecuteFadeOut;
        DestroyImmediate(gameObject);
    }
    
    public float GetAnimationTime(string FadeAnimation)
    {

        // AnimatorControllerLayer animatorControllerLayer = (PreloadingAnimator.runtimeAnimatorController as AnimatorController)?.layers[0];
        // foreach (var stateName in animatorControllerLayer.stateMachine.anyStateTransitions)
        // {
        //     print(stateName.conditions.Length);
        //     if(FadeAnimation == stateName.name){
        //         return stateName.duration;
        //     }
        // }
        return 0;
    }
}
