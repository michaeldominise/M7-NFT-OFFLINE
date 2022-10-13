using HutongGames.PlayMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using M7.Skill;

[ActionCategory("M7/EndGame")]
public class EndGameChecker : FsmStateAction
{
    public FsmEvent OnExecuteAttackFinish;
    public override void OnEnter()
    {
        CheckEndGame();
    }

    public void CheckEndGame()
    {
        SkillQueueManager.Instance.WaitUntilIdle(() =>
        {
            Debug.Log("GAME FINISH EXECUTED ---------- !!!");
            Fsm.Event(OnExecuteAttackFinish);
        });
        
    }
}
