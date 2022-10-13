using System;
using System.Collections;
using System.Collections.Generic;
using M7.Skill;
using UnityEngine;

public class MoveCounterCaster : MonoBehaviour, ISkillCaster
{
    public ISkillCaster.SkillState CurrenSkillState { get; set; }

    public bool ExecuteOnSpawn { get; set; }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public IEnumerator OnPreSkillCasted(SkillObject skillObject, Func<List<Component>> getTargets) { yield break; }
    public IEnumerator OnNewCasterSkillCasted(SkillObject skillObject) { yield break; }
}
