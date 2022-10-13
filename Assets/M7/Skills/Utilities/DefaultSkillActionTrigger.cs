using System;
using M7.GameRuntime;
using System.Collections;
using System.Collections.Generic;
using M7.GameData.CharacterSkill;
using UnityEngine;
using UnityEngine.UIElements;

namespace M7.Skill
{
    [System.Serializable]
    public class DefaultSkillActionTrigger
    {
        public SkillEnums.EventTrigger eventTrigger;
        public List<SkillObject> skillObjects;


        public void Execute(SkillEnums.EventTrigger eventTrigger, System.Action onFinish) => SkillManager.Instance.StartCoroutine(_Execute(eventTrigger, onFinish));

        IEnumerator _Execute(SkillEnums.EventTrigger eventTrigger, System.Action onFinish)
        {
            var eventSuccess = eventTrigger switch
            {
                SkillEnums.EventTrigger.StartTurnTarget | SkillEnums.EventTrigger.EndTurnTarget => false,
                _ => true
            };

            if (eventSuccess)
            {
                if ((this.eventTrigger | eventTrigger) == this.eventTrigger)
                    foreach (var skillObject in skillObjects)
                    {
                        var proceed = false;
                        List<Func<List<Component>>> targetManagerData = null;
                        skillObject.GetTargetManagerData(BattleManager.Instance.ActiveTeam, data =>
                        {
                            proceed = true;
                            targetManagerData = data;
                        });
                        yield return new WaitUntil(() => proceed);
                        proceed = false;
                        skillObject.Execute(BattleManager.Instance.ActiveTeam, null, targetManagerData, () => proceed = true);
                        yield return new WaitUntil(() => proceed);
                    }
            }
            onFinish?.Invoke();
        }
    }
}
