using M7.GameRuntime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Skill
{
    public partial class SkillManager : MonoBehaviour
    {
        public static SkillManager Instance => BattleManager.Instance?.SkillManager;

        [SerializeField] List<DefaultSkillActionTrigger>  defaultSkillActionTriggers;
        
        public Action<SkillEnums.EventTrigger, Action> onSkillActionTrigger;

        public void Init()
        {
            foreach (var defaultSkillActionTrigger in defaultSkillActionTriggers)
                onSkillActionTrigger += defaultSkillActionTrigger.Execute;
        }

        public static void ExecuteSkill(Component caster, SkillObject skillObject, ConditionalDataValues dataValues, List<Func<List<Component>>> targetManagerData, Action onFinish)
        {   
            skillObject.Execute(caster, dataValues, targetManagerData, onFinish);

            if (skillObject.AnimationType == SkillEnums.SkillAnimationType.Skill)
                Instance.StartCoroutine(BattleManager.Instance.AttackInformationManager.SetInfo(skillObject, 2));
        }

        public static void ExecuteSkillActionTriggers(SkillEnums.EventTrigger eventTrigger, Action onFinish, float delay = 0) => Instance.StartCoroutine(_ExecuteSkillActionTriggers(eventTrigger, onFinish, delay));
        static IEnumerator _ExecuteSkillActionTriggers(SkillEnums.EventTrigger eventTrigger, Action onFinish, float delay = 0)
        {
            var invocationCount = Instance.onSkillActionTrigger?.GetInvocationList().Length ?? 0;
            if (invocationCount == 0)
            {
                onFinish?.Invoke();
                yield break;
            }

            yield return new WaitForSeconds(delay);
            var doneCount = 0;
            Instance.onSkillActionTrigger(eventTrigger, () =>
            {
                doneCount++;
                if (doneCount == invocationCount)
                    onFinish?.Invoke();
            });
        }
    }
}