using System;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using M7.FX;
using System.Linq;
using M7.FX.VFX.Scripts;
using M7.Match;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace M7.Skill
{
    [Serializable]
    public class TargetListData
    {
        public enum JoinType { Add, Intersect }

        [ValueDropdown("@M7.Skill.TargetManagerIndex.TreeViewOfTargetManagers")]
        [SerializeField] List<int> targetManagerIndexList;
        [SerializeField] JoinType joinType;

        public List<Component> GetTargetList(List<Func<List<Component>>> targetManagerData)
        {
            IEnumerable<Component> targets = null;
            for (int i = 0; i < targetManagerIndexList.Count; i++)
            {
                int targetManagerIndex = targetManagerIndexList[i];
                var targetList = targetManagerData[targetManagerIndex]?.Invoke();
                if (i == 0)
                    targets = targetList;
                else if (joinType == JoinType.Add)
                    targets = targets.Union(targetList);
                else
                    targets = targets.Intersect(targetList);
            }
            return targets?.ToList() ?? new List<Component>();
        }

        public bool IsValidTarget(SkillObject skillObject, Component caster, Component target)
        {
            var result = false;
            for (int i = 0; i < targetManagerIndexList.Count; i++)
            {
                var targetManager = skillObject.TargetManagerHandler.GetTargetManager(targetManagerIndexList[i]);
                if (!targetManager.ValidateOnSkillDataExecute)
                {
                    result = true;
                    continue;
                }

                if (joinType == JoinType.Add)
                {
                    if (targetManager.IsValidTarget(caster, target))
                        return true;
                }
                else
                {
                    if (targetManager.IsValidTarget(caster, target))
                        result = true;
                    else
                        return false;
                }
            }

            return result;
        }
    }

    public abstract class SkillData
    {
        [SerializeField] TargetListData targetListData;
        [SerializeField] public VFXSkillSystem vfx;
        [SerializeField] float executionDelay;
        [SerializeField] float intervalPerTargetExeution;
        [SerializeField, FormerlySerializedAs("waitVfxBeforeExecuting")] bool waitVfxAndDelayBeforeExecuting = true;

        public virtual List<StatusEffect> StatusEffects { get; }
        public float ExeutionDelay => executionDelay;
        public bool WaitVfxBeforeExecuting => waitVfxAndDelayBeforeExecuting;
        public TargetListData TargetListData => targetListData;

        public abstract float Value
        {
            get;
#if UNITY_EDITOR
            set;
#endif
        }

        public void PlayVFX(Component caster,
            List<Component> targets,
            SkillEnums.SkillAnimationType skillAnimationType,
            ParticleWorldManager.CameraType startCameraType = ParticleWorldManager.CameraType.SystemUI,
            ParticleWorldManager.CameraType endCameraType = ParticleWorldManager.CameraType.World,
            Action onHit = null,
            Action onFinish = null)
        {
            if (vfx == null)
            {
                onHit?.Invoke();
                onFinish?.Invoke();
                return;
            }


            if (caster != null)
            {
                var casterBattle =  caster as CharacterInstance_Battle;
                var startTransform = casterBattle?.GetVfxOffsetTransform(skillAnimationType) ?? caster.transform;

                var skillVfx = ParticleWorldManager.Instance.SpawnVFX<VFXSkillSystem>(vfx.gameObject, startTransform.position, startCameraType, endCameraType);
                if (skillVfx.AttachToCaster)
                    skillVfx.transform.parent = caster.transform;

                skillVfx.ExecuteSkill(new Transform[] { startTransform },
                    targets.Where(x => x != null).Select(x => (x as CharacterInstance_Battle)?.GetVfxOffsetBody() ?? x.transform).ToArray(), startCameraType, endCameraType, onHit, () =>
                        {
                            ParticleWorldManager.Instance.DespawnPartcle(skillVfx.transform);
                            onFinish?.Invoke();
                        }, !BattleManager.Instance.IsPlayerTeamObject(caster) && casterBattle != null);
            }
        }


        public virtual void Execute(SkillObject skillObject, Component caster, List<Component> targets, ConditionalDataValues dataValues, Action onFinish = null)
        {
            if(StatusEffects == null || StatusEffects.Count == 0 || targets == null || targets.Count == 0)
            {
                onFinish?.Invoke();
                return;
            }

            var doneCount = 0;
            Action onDone = () =>
            {
                doneCount++;
                if (doneCount == StatusEffects.Count)
                    onFinish?.Invoke();
            };
            for (int i = 0; i < StatusEffects.Count; i++)
            {
                StatusEffect statusEffect = StatusEffects[i];
                if(statusEffect == null)
                {
                    onDone?.Invoke();
                    continue;
                }
                ExecuteStatusEffect(skillObject, caster, targets, $"{skillObject.MasterID}.{statusEffect.name}.{i}", statusEffect, dataValues, onDone);
            }
        }

        void ExecuteStatusEffect(SkillObject skillObject, Component caster, List<Component> targets, string statusEffectId, StatusEffect statusEffect, ConditionalDataValues dataValues, Action onFinish = null)
        {
            if(targets == null || targets.Count == 0)
            {
                onFinish?.Invoke();
                return;
            }

            var doneCount = 0;
            Action onDone = () =>
            {
                doneCount++;
                if (doneCount == targets.Count)
                    onFinish?.Invoke();
            };

            for (int i = 0; i < targets.Count; i++)
            {
                Component target = targets[i];
                SkillManager.Instance.StartCoroutine(ExecuteStatusEffectPerTarget(skillObject, caster, targets, target, statusEffectId, statusEffect, dataValues, i, onDone));
            }
        }

        IEnumerator ExecuteStatusEffectPerTarget(SkillObject skillObject, Component caster, List<Component> targets, Component target, string statusEffectId, StatusEffect statusEffect, ConditionalDataValues dataValues, int index, Action onDone = null)
        {
            yield return new WaitForSeconds(statusEffect.ComputeIntervalPerTarget(index, caster, target, intervalPerTargetExeution));
            if (!targetListData?.IsValidTarget(skillObject, caster, target) ?? false || !statusEffect.IsValid(dataValues, caster, target))
            {
                onDone?.Invoke();
                yield break;
            }

            var targetCharacter = target as CharacterInstance_Battle;
            var targetTeam = target as TeamManager_Battle;
            var targetTile = target as MatchGridCell;
            if (targetCharacter != null)
                StatusEffectInstance.AttachToTarget(caster, targets, targetCharacter, statusEffectId, statusEffect, onDone);
            else if (targetTeam)
                StatusEffectInstance.AttachToTarget(caster, targets, targetTeam, statusEffectId, statusEffect, onDone);
            else if (targetTile)
                StatusEffectInstance.AttachToTarget(caster, targets, targetTile, statusEffectId, statusEffect, onDone);
            else
                onDone?.Invoke();
        }

        public void UpdateTargetStats(List<Component> targets, SkillObject skillObject)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                Component target = targets[i];
                var targetCharacter = target as CharacterInstance_Battle;
                if (targetCharacter == null)
                    continue;

                for (int x = 0; x < StatusEffects.Count; x++)
                {
                    StatusEffect statusEffect = StatusEffects[x];
                    (targetCharacter.StatsInstance as StatsInstance_CharacterBattle).SetStatusValueDataReady($"{skillObject.MasterID}.{statusEffect.name}.{x}");
                }
            }
        }
    }
}