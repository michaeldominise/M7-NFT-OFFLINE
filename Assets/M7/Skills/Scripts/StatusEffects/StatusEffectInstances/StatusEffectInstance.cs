using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;

namespace M7.Skill
{
    public class StatusEffectInstance : MonoBehaviour
    {
        static int IncrementID;
        [ShowInInspector, ReadOnly] public int MasterID { get; private set; }
        [ShowInInspector, ReadOnly] public StatusEffect StatusEffect { get; protected set; }
        [ShowInInspector, ReadOnly] public int ExpirationCount { get; private set; }
        [ShowInInspector, ReadOnly] public bool DoesNotExpire => StatusEffect?.Expiration.DoesNotExpire ?? false;
        [ShowInInspector, ReadOnly] public bool IsExpired => ExpirationCount < 0;
        [ShowInInspector, ReadOnly] public Component Caster { get; protected set; }
        [ShowInInspector, ReadOnly] public List<Component> Targets { get; protected set; }
        [ShowInInspector, ReadOnly] public Component Target { get; protected set; }
        [ShowInInspector, ReadOnly] public string StatusEffectId { get; protected set; }
        [ShowInInspector, ReadOnly] public StatusEffectData StatusEffectData { get; protected set; }
        [ShowInInspector, ReadOnly] public IStatusEffectInstanceController TargetStatusEffectInstanceController { get; protected set; }

        public static StatusEffectInstance AttachToTarget<CasterType, TargetType>(CasterType caster, List<Component> targets, TargetType target, string statusEffectId, StatusEffect statusEffect, Action onFinish) where CasterType : Component where TargetType : Component, IStatusEffectInstanceController
        {
            var chance = statusEffect.Chance;
            int toExecute = UnityEngine.Random.Range(0, chance);
            if (toExecute >= chance)
            {
                onFinish?.Invoke();
                return null;
            }

            var statusEffectInstance = Instantiate(statusEffect.InstancePrefab, target.StatusEffectInstanceContainer, false);
            statusEffectInstance.Init(caster, targets, target, statusEffectId, statusEffect);
            statusEffectInstance.TryActivate(onFinish);

            return statusEffectInstance;
        }

        protected virtual void Init<CasterType, TargetType>(CasterType caster, List<Component> targets, TargetType target, string statusEffectId, StatusEffect statusEffect) where CasterType : Component where TargetType : Component, IStatusEffectInstanceController
        {
            Caster = caster;
            Targets = targets;
            Target = target;
            StatusEffectId = statusEffectId;

            TargetStatusEffectInstanceController = target;
            StatusEffect = statusEffect;
            StatusEffectData = StatusEffect.GenerateStatusEffectData(this);
    }

        protected virtual void TryActivate(Action onFinish)
        {
            SkillManager.Instance.onSkillActionTrigger += OnSkillActionTigger;
            OnSkillActionTigger(SkillEnums.EventTrigger.Execute, onFinish);

            MasterID = IncrementID;
            IncrementID++;

            ExpirationCount = DoesNotExpire ? 0 : StatusEffect.Expiration.ExpireCount;
        }

        public void OnSkillActionTigger(SkillEnums.EventTrigger eventTrigger, Action onFinish)
        {
            if ((SkillEnums.EventTrigger.StartTurnCaster | SkillEnums.EventTrigger.StartTurnCaster).HasFlag(eventTrigger))
            {
                if (TurnManager.Instance.CurrentState == TurnManager.State.PlayerTurn ? !BattleManager.Instance.IsPlayerTeamObject(Caster) : BattleManager.Instance.IsPlayerTeamObject(Caster))
                {
                    onFinish?.Invoke();
                    return;
                }
            }
            else if ((SkillEnums.EventTrigger.StartTurnTarget | SkillEnums.EventTrigger.EndTurnTarget).HasFlag(eventTrigger))
            {
                if (TurnManager.Instance.CurrentState == TurnManager.State.PlayerTurn ? !BattleManager.Instance.IsPlayerTeamObject(Target) : BattleManager.Instance.IsPlayerTeamObject(Target))
                {
                    onFinish?.Invoke();
                    return;
                }
            }

            var skillActionTiggers = SkillEnums.SkillActionTigger.None;
            var finishedSkillActionTiggers = SkillEnums.SkillActionTigger.None;
            Action<SkillEnums.SkillActionTigger> onFinishTrigger = actionTigger =>
            {
                finishedSkillActionTiggers |= actionTigger;
                if ((skillActionTiggers ^ finishedSkillActionTiggers) == 0)
                    onFinish?.Invoke();
            };

            if ((StatusEffect.ActivationTrigger | eventTrigger) == StatusEffect.ActivationTrigger)
                skillActionTiggers |= SkillEnums.SkillActionTigger.Execute;
            if ((StatusEffect.Expiration.Trigger | eventTrigger) == StatusEffect.Expiration.Trigger)
                skillActionTiggers |= SkillEnums.SkillActionTigger.Expire;

            var isCasterActiveTeam = BattleManager.Instance.ActiveTeam == BattleManager.Instance.GetAllyTeam(Caster);
            var eventSuccess = eventTrigger switch
            {
                SkillEnums.EventTrigger.StartTurnCaster | SkillEnums.EventTrigger.EndTurnCaster => isCasterActiveTeam,
                SkillEnums.EventTrigger.StartTurnTarget | SkillEnums.EventTrigger.EndTurnTarget => !isCasterActiveTeam,
                _ => true
            };

            if (eventSuccess)
            {
                if ((StatusEffect.ActivationTrigger | eventTrigger) == StatusEffect.ActivationTrigger)
                    Execute(() => onFinishTrigger(SkillEnums.SkillActionTigger.Execute));
                if ((StatusEffect.Expiration.Trigger | eventTrigger) == StatusEffect.Expiration.Trigger)
                    Expire(() => onFinishTrigger(SkillEnums.SkillActionTigger.Expire));
            }
            else
            {
                if ((StatusEffect.ActivationTrigger | eventTrigger) == StatusEffect.ActivationTrigger)
                    onFinishTrigger(SkillEnums.SkillActionTigger.Execute);
                if ((StatusEffect.Expiration.Trigger | eventTrigger) == StatusEffect.Expiration.Trigger)
                    onFinishTrigger(SkillEnums.SkillActionTigger.Expire);
            }

            if (skillActionTiggers == SkillEnums.SkillActionTigger.None)
                onFinishTrigger(SkillEnums.SkillActionTigger.None);
        }

        public virtual void Execute(Action onFinish)
        {
            TargetStatusEffectInstanceController.StatusEffectInstanceLedger.Add(this);
            TargetStatusEffectInstanceController.OnStatusEffectInstanceLedgerUpdate(this, IStatusEffectInstanceController.UpdateType.Add);
            StatusEffect.Execute(this, onFinish);
        }

        public virtual void Expire(Action onFinish)
        {
            if (DoesNotExpire)
            {
                onFinish?.Invoke();
                return;
            }

            ExpirationCount--;
            if (ExpirationCount <= 0)
                DestroyInstance();

            onFinish?.Invoke();
        }

        public virtual void DestroyInstance()
        {
            TargetStatusEffectInstanceController.StatusEffectInstanceLedger.Remove(this);
            TargetStatusEffectInstanceController.OnStatusEffectInstanceLedgerUpdate(this, IStatusEffectInstanceController.UpdateType.Remove);
            SkillManager.Instance.onSkillActionTrigger -= OnSkillActionTigger;
            Destroy(gameObject);
        }

    }
}