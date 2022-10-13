using System.Collections.Generic;
using UnityEngine;

    #if UNITY_EDITOR
using DG.Tweening;
using M7.Tools.Scripts;
    #endif

using M7.GameData;
using Sirenix.OdinInspector;
using M7.Skill;
using System.Collections;
using System;
using M7.GameData.CharacterSkill;
using M7.GameRuntime.Scripts.Spine;
using System.Linq;

namespace M7.GameRuntime
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class CharacterInstance_Battle : CharacterInstance_Spine, IStatusEffectInstanceController, ISkillCaster
    {
        public BattleManager castedBattleManager { get { return BattleManager.Instance; } }

        [SerializeField] Transform uiStatsTarget;
        [SerializeField] Transform statusEffectInstanceContainer;
        [SerializeField] GameObject runVfx;
        [SerializeField] private GameObject targetSpriteObject;

        public CharacterSkillData CharacterSkillData = new CharacterSkillData();

        [ShowInInspector] public bool isGodMode;
        [ShowInInspector] public virtual bool IsAlive => isGodMode || StatsInstance?.CurrentHp > 0;
        [ShowInInspector] public RPGElement Element => CharacterObject?.Element;
        [ShowInInspector] public SkillEnums.FormationGroupFilter FormationGroup => transform.GetSiblingIndex() > 2 ? SkillEnums.FormationGroupFilter.FrontLine : SkillEnums.FormationGroupFilter.Backline;
        [ShowInInspector] public List<StatusEffectInstance> ActiveStatusEffectInstances { get; private set; } = new List<StatusEffectInstance>();
        [ShowInInspector] public List<StatusEffectInstance> StatusEffectInstanceLedger { get; private set; } = new List<StatusEffectInstance>();
        [ShowInInspector, ReadOnly] public UIBattle_CharacterStats UIBattle_CharacterStats { get; internal set; }
        [ShowInInspector] public bool IsPlayerObject => BattleManager.Instance?.IsPlayerTeamObject(this) ?? false;
        public StatsInstance_CharacterBattle StatsInstanceBattle => StatsInstance as StatsInstance_CharacterBattle;
        public Transform StatusEffectInstanceContainer => statusEffectInstanceContainer;
        public Transform UiStatsTarget => uiStatsTarget;
        public Transform TransformTarget { get; set; }
        [ShowInInspector, ReadOnly] public SkillEnums.SkillTransitionType LastSkillAnimationState { get; private set; } = SkillEnums.SkillTransitionType.None;

        public override void OnPostLoadAssetReferenceLoaded()
        {
            base.OnPostLoadAssetReferenceLoaded();
            MainSpineInstance?.SetShadowSettings(MainSpineContainer);
            StartCoroutine(EnableSpine());
        }

        public void OnStatusEffectInstanceLedgerUpdate(StatusEffectInstance updatedStatusEffectInstance, IStatusEffectInstanceController.UpdateType updateType) => StatsInstance.OnStatusEffectInstanceLedgerUpdate(updatedStatusEffectInstance, updateType);

        public override void CleanInstance()
        {
            base.CleanInstance();
            Destroy(UIBattle_CharacterStats);
        }
        public IEnumerator OnPreSkillCasted(SkillObject skillObject, Func<List<Component>> getTargets)
        {
            var targets = getTargets?.Invoke();
	        CharacterInstance_Battle currentSelectedTarget = targets.Count > 0 ? targets[0] as CharacterInstance_Battle : this;
	        
            yield return CharacterAnimationEventManager.Instance.MoveToDestination(this, currentSelectedTarget, LastSkillAnimationState, skillObject.TransitionType);
            LastSkillAnimationState = skillObject.TransitionType;
            yield break;
        }
        public IEnumerator OnNewCasterSkillCasted(SkillObject skillObject)
        {
            if (LastSkillAnimationState != SkillEnums.SkillTransitionType.InPlace) // Apply delay "Move Back State" for the specific hero has moving to enemy target.
            {
                AttackAnimationCallback attck = MainSpineInstance.GetComponentInChildren(typeof(AttackAnimationCallback)) as AttackAnimationCallback;
                yield return new WaitUntil (() => attck.IsStateDone == true);
                //Debug.Log (attck.IsStateDone);
            }
           
            yield return CharacterAnimationEventManager.Instance.MoveToDestination(this, null, LastSkillAnimationState, SkillEnums.SkillTransitionType.None);
            LastSkillAnimationState = SkillEnums.SkillTransitionType.None;
            yield break;
        }

        public ISkillCaster.SkillState CurrenSkillState { get; set; }

        public bool ExecuteOnSpawn { get; set; }
        public GameObject GetGameObject()
        {
            return gameObject;
        }

        [Button]
        public void Kill()
        {
            UIBattle_CharacterStats.FadeOutItem(BattleSceneSettings.Instance.WaveTransitionDelay);

            SetTriggerAnimation("Death");
            AnimateCast(false);
            transform.SetParent(BattleManager.Instance.GetAllyTeam(this).DeadCharactersContainer);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 2);
            Destroy(MainSpineInstance.ShadowObject);
            Destroy(UIBattle_CharacterStats.VfxElementObj);
        }

        [Button]
        public void Revive()
        {
            var activeTeam = BattleManager.Instance.ActiveTeam;
            var index = Array.FindIndex(activeTeam.RawCharacters, 0, activeTeam.RawCharacters.Length, x => x == this);
            var saveableData = SaveableData;

            DisposeAssetReference();
            ObjectData = null;
            Destroy(gameObject);
            activeTeam.SetCharacterPosition(index, saveableData, () => SetTriggerAnimation("Revive"));
        }

        public void SetTriggerAnimation(string animParameter)
        {
            MainSpineInstance?.SetTriggerAnimation(animParameter);
            SubSpineInstance?.SetTriggerAnimation(animParameter);
        }

        public void SetBoolAnimation(string animParameter, bool value)
        {
            Debug.Log(animParameter);
            MainSpineInstance?.SetBoolAnimation(animParameter, value);
            SubSpineInstance?.SetBoolAnimation(animParameter, value);
        }

        public void MoveAnimation(bool state, float speed)
        {
            MainSpineInstance?.MoveAnimation(state, speed);
            SubSpineInstance?.MoveAnimation(state, speed);
            runVfx.SetActive(state);
        }

        public void MoveAttackAnimation(bool state, float speed) 
        {
            if (SubSpineInstance == null)
            {
                MainSpineInstance?.MoveAnimation(state, speed);
            }
            else
            {
                SubSpineInstance?.MoveAnimation(state, speed);
            }
            
            runVfx.SetActive(state);
        }

        public void AnimateCast(bool isShow)
        {
            if (isShow)
                CastVfxManager.Instance.AnimateVfx(this);
            else
                CastVfxManager.Instance.RemoveVfx(this);
        }

        public IEnumerator EnableSpine()
        {
            if (!IsPlayerObject)
            {
                MainSpineContainer?.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.5f);

                MainSpineContainer?.gameObject.SetActive(true);
            }
        }

        public void ShowTarget() => targetSpriteObject.gameObject.SetActive(true);
        public void HideTarget() => targetSpriteObject.gameObject.SetActive(false);
        public Transform GetVfxOffsetTransform(SkillEnums.SkillAnimationType skillAnimationType) => (MainSpineInstance.SpineOffsetManager ?? SubSpineInstance.SpineOffsetManager).GetVfxOffsetTransform(skillAnimationType) ?? SubSpineInstance.SpineOffsetManager.GetVfxOffsetTransform(skillAnimationType);
        public Transform GetVfxOffsetBody() => (MainSpineInstance.SpineOffsetManager ?? SubSpineInstance.SpineOffsetManager).BodyOffset ?? SubSpineInstance.SpineOffsetManager.BodyOffset;
        void OnMouseEnter() => CharacterSelectionTargetManager.Instance.SetHoveredCharacter(this);
        private void OnMouseExit() => CharacterSelectionTargetManager.Instance.UnsetHoveredCharacter(this);
        void OnMouseUpAsButton() => CharacterSelectionTargetManager.Instance.SetPreSelectedCharacter(this);
        public void OnSkillActionTigger(SkillEnums.EventTrigger eventTrigger, Action onFinish) => StatusEffectInstanceLedger.ForEach(x => x.OnSkillActionTigger(eventTrigger, onFinish));
    }
}
