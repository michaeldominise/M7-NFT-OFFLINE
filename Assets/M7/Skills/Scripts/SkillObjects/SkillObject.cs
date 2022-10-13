using Sirenix.OdinInspector;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using M7.FX;
using UnityEngine.Serialization;
using M7.Match;

namespace M7.Skill
{
    public class SkillObject : MonoBehaviour
    {
        [ShowInInspector, PropertyOrder(-1)] public string MasterID
        {
            get => name;
            set => name = value;
        }

        [SerializeField] SkillDisplayStats displayStats = new SkillDisplayStats();
        [SerializeField, FoldoutGroup("Animation"), FormerlySerializedAs("animationType"), HideInInspector] SkillEnums.SkillTransitionType transitionType;
        [SerializeField, FoldoutGroup("Animation"), FormerlySerializedAs("skillVariation")] SkillEnums.SkillAnimationType animationType;
        [SerializeField, FoldoutGroup("Animation")] ParticleWorldManager.CameraType startCameraType;
        [SerializeField, FoldoutGroup("Animation")] ParticleWorldManager.CameraType endCameraType;

        [SerializeField, TitleGroup("SkillEffect")] TargetManagerHandler targetManagerHandler;
        [SerializeField, TitleGroup("SkillEffect")] List<SkillData_MultipleEffect> dataList;
        [SerializeField] private float delayOnFinish;

        public List<SkillData_MultipleEffect> DataList => dataList;
        public SkillDisplayStats DisplayStats => displayStats;
        public SkillEnums.SkillTransitionType TransitionType => transitionType;
        public SkillEnums.SkillAnimationType AnimationType => animationType;
        public TargetManagerHandler TargetManagerHandler => targetManagerHandler;

        public float DelayOnFinish => delayOnFinish;
    
        public int DisplayValue => (int)(dataList?.Sum(x => x.Value) ?? 0);
        public void Execute(Component caster, ConditionalDataValues dataValues, List<Func<List<Component>>> targetManagerData, Action onFinish) => SkillManager.Instance.StartCoroutine(_Execute(caster, dataValues, targetManagerData, onFinish));
        public void GetTargetManagerData(Component caster, Action<List<Func<List<Component>>>> onFinish) => targetManagerHandler.GetTargetManagerData(caster, onFinish);
        IEnumerator _Execute(Component caster, ConditionalDataValues dataValues, List<Func<List<Component>>> targetManagerData, Action onFinish)
        {
            Debug.Log($"[SkillObject] Execute {name}");
            var finishCount = 0;
            foreach (var skillData in dataList)
            {
                var proceed = !skillData.WaitVfxBeforeExecuting;
                SkillManager.Instance.StartCoroutine(SkillDataExecute(skillData, caster, dataValues, targetManagerData, () =>
                {
                    proceed = true;
                    SkillManager.Instance.StartCoroutine(OnFinishExecute());
                }));
                yield return new WaitUntil(() => proceed);
            }

            IEnumerator OnFinishExecute()
            {
                finishCount++;
                if (finishCount != dataList.Count)
                    yield break;

                yield return new WaitForSeconds(DelayOnFinish);
                onFinish?.Invoke();
            }
        }

        IEnumerator SkillDataExecute(SkillData skillData, Component caster, ConditionalDataValues dataValues, List<Func<List<Component>>> targetManagerData, Action onFinish)
        {
            yield return new WaitForSeconds(skillData.ExeutionDelay);

            var targets = skillData.TargetListData.GetTargetList(targetManagerData);
            var proceed = false;
            if (!skillData.WaitVfxBeforeExecuting)
                skillData.Execute(this, caster, targets, dataValues);

            //yield return (caster as ISkillCaster)?.OnPreSkillCasted(this, targetManagerData[0]);

            ExecuteCharacterAnimation(caster);
            skillData.PlayVFX(caster, targets, animationType, startCameraType, endCameraType,
                   () =>
                   {
                       skillData.UpdateTargetStats(targets, this);
                       if (skillData.WaitVfxBeforeExecuting)
                           skillData.Execute(this, caster, targets, dataValues, () => proceed = true);
                       else
                           proceed = true;
                   });


            yield return new WaitUntil(() => proceed);
            foreach (var target in targets)
            {
                var charInstance = target as CharacterInstance_Battle;
                var teamInstance = target as TeamManager_Battle;
                var cellInstance = target as MatchGridCell;

                if (charInstance)
                    charInstance.StatsInstance.UpdateInstanceActions();
                else if (teamInstance)
                    teamInstance.StatsInstance.UpdateInstanceActions();
            }

            onFinish?.Invoke();
        }

        void ExecuteCharacterAnimation(Component caster)
        {
            var casterCharacter = caster as CharacterInstance_Battle;
            if (!casterCharacter)
                return;   

            casterCharacter.SetTriggerAnimation(animationType.ToString());
        }
    }
}