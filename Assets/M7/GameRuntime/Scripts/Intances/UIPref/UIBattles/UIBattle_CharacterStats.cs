using M7.GameRuntime.Scripts.UI.OverDrive;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEvents;
using DG.Tweening;
using M7.Skill;
using M7.GameData;

namespace M7.GameRuntime
{
    [RequireComponent(typeof(M7.GameRuntime.UIFollowWorldObject))]
    public class UIBattle_CharacterStats : UIBattle<UIPrefManager_CharacterStats, CharacterInstance_Battle, InstanceActions_CharacterStats>
    {
        public BattleManager castedBattleManager { get { return BattleManager.Instance; } }

        [SerializeField] UIFollowWorldObject objectFollower;

        [SerializeField] UIDelayValue hpText;
        [SerializeField] UIDelayValue hpGage;
        [SerializeField] UIDelayValue hpGageBG;

        //[SerializeField] UIDelayValue odGage;
        //[SerializeField] UIDelayValue odGageBG;

        [SerializeField] UIDelayValue attackTurnText;
        TextMeshProUGUI AttackTurnLabel => attackTurnText.GetComponent<TextMeshProUGUI>();

        [SerializeField] UIDelayValue defenseText;
        [SerializeField] UIDelayValue attackChargeText;
        [SerializeField] TextMeshProUGUI attackChargeLabel;
        [SerializeField] Image rpgElement;
        [SerializeField] Image attackTurnIndicator;

        [SerializeField] Sprite[] threeAttackSpriteIndicators;
        [SerializeField] Sprite[] twoAttackSpriteIndicators;
        [SerializeField] Sprite oneAttackSpriteIndicators;

        [SerializeField] Transform vfxElementParent;
        [SerializeField] GameObject vfxElementObj;
        public GameObject VfxElementObj => vfxElementObj;

        [SerializeField] private CharacterSkillUI heroOverdriveUI;

        protected override InstanceActions_CharacterStats InstanceActions => TargetReference.StatsInstance.InstanceActions;

        public UIFollowWorldObject ObjectFollower => objectFollower;

        CharacterInstance_Battle targetReferenceGlobal;
        public int attackTurnCount;

        public override void Attach(UIPrefManager_CharacterStats uiPrefManager, CharacterInstance_Battle targetReference)
        {
            base.Attach(uiPrefManager, targetReference);

            targetReferenceGlobal = targetReference;
            rpgElement.sprite = targetReference.CharacterObject.Element.DisplaySprite;
            GameObject vfxElem = Instantiate(targetReference.CharacterObject.Element.VfxElement, vfxElementParent);
            vfxElementObj = vfxElem;
            // overdrive UI
            heroOverdriveUI.Init(targetReference);

            objectFollower.SetValues(targetReference.UiStatsTarget, BattleCameraHandler.Instance.UiCamera, BattleCameraHandler.Instance.WorldCamera);
            objectFollower.UpdatePosition();

            Image hpGageImg;
            hpGageImg = hpGage.GetComponent<Image>();
            hpGageImg.color = targetReference.IsPlayerObject ? Color.green : Color.red;

            attackChargeLabel.color = targetReference.Element.ElementColor;
            attackTurnText.transform.parent.gameObject.SetActive(targetReference is CharacterInstance_Battle_Enemy);
            attackTurnIndicator.gameObject.SetActive(targetReference is CharacterInstance_Battle_Enemy);

            attackTurnCount = (int)targetReferenceGlobal.StatsInstance.GetValue(SkillEnums.TargetCharacterStats.AttackTurn);

            StartCoroutine(SetVfxElement(1));
            SetAttackIndicator();

            FadeInItem(BattleSceneSettings.Instance.WaveTransitionDelay);
        }

        public void SetAttackIndicator()
        {
            if (targetReferenceGlobal is CharacterInstance_Battle_Enemy)
            {
                attackTurnText.SetValue(attackTurnCount);

                if (attackTurnCount == 3)
                    attackTurnIndicator.sprite = threeAttackSpriteIndicators[attackTurnCount - 1];
                if (attackTurnCount == 2)
                    attackTurnIndicator.sprite = twoAttackSpriteIndicators[attackTurnCount - 1];
                if (attackTurnCount == 1)
                    attackTurnIndicator.sprite = oneAttackSpriteIndicators;
            }
        }

        protected override void AttachEvents()
        {
            //InstanceActions.onCurrentHpUpdate += hpText.SetValue;
            InstanceActions.onHpPercentageUpdate += HpGauge;
            InstanceActions.onHpPercentageUpdate += HpGaugeBg;

            InstanceActions.onAttackTurnUpdate += AttackTurn;

            // InstanceActions.onOverdrivePercentageUpdate += odGage.SetValue;
            // InstanceActions.onOverdrivePercentageUpdate += odGageBG.SetValue;

            //InstanceActions.onDefenseUpdate += defenseText.SetValue;
            (TargetReference.StatsInstance as StatsInstance_CharacterBattle)?.UpdateInstanceActions();
        }

        private void HpGauge(float value)
        {
            if(hpGage != null) hpGage.SetValue(value);    
        }
        
        private void HpGaugeBg(float value)
        {
            if(hpGageBG != null) hpGageBG.SetValue(value);
        }

        private void AttackTurn(float value)
        {
            if (!this)
                return;
            StartCoroutine(delayAttackTurn(value));
        }

        private IEnumerator delayAttackTurn(float value)
        {
            float delay;
            if (value == attackTurnCount)
                delay = 0.5f;
            else
                delay = 0.25f;

            yield return new WaitForSeconds(delay);
            if (attackTurnText != null)
            {
                if (targetReferenceGlobal is CharacterInstance_Battle_Enemy)
                {
                    if (heroOverdriveUI.CharacterSkillData.IsReady)
                        attackTurnText.GetComponent<TextMeshProUGUI>().text = "S";
                    else if (!heroOverdriveUI.CharacterSkillData.IsReady)
                        attackTurnText.SetValue(value);

                    if (attackTurnCount == 3)
                        attackTurnIndicator.sprite = threeAttackSpriteIndicators[(int)value - 1];
                    if (attackTurnCount == 2)
                        attackTurnIndicator.sprite = twoAttackSpriteIndicators[(int)value - 1];
                    if (attackTurnCount == 1)
                        attackTurnIndicator.sprite = oneAttackSpriteIndicators;

                    if(VfxElementObj != null)
                        VfxElementObj.SetActive(value <= 1 || heroOverdriveUI.CharacterSkillData.IsReady ? true : false);

                    //if (SkillQueueManager.Instance.CurrentState == SkillQueueManager.State.Idle && TurnManager.Instance.CurrentState == TurnManager.State.EnemyTurn)
                    //{
                    //    heroOverdriveUI.CharacterSkillData.Execute();
                    //    //(TargetReference.StatsInstance as StatsInstance_CharacterBattle)?.UpdateInstanceActions();
                    //    //attackTurnText.SetValue(attackTurnCount);
                    //    //TargetReference.StatsInstance.UpdateInstanceActions();
                    //}
                }

                //attackTurnText.SetValue(value);
            }
        }

        public void AttackChargeText(float value, float sizeMultiplier = 1)
        {
            if (attackChargeText == null)
                return;
            attackChargeText.SetValue(value);
            attackChargeText.transform.parent.localScale = Vector3.one * (1 + 0.15f * sizeMultiplier);
        }

        protected override void ResetEvents()
        {
            //InstanceActions.onCurrentHpUpdate -= hpText.SetValue;
            InstanceActions.onHpPercentageUpdate -= HpGauge;
            InstanceActions.onHpPercentageUpdate -= HpGaugeBg;

            InstanceActions.onAttackTurnUpdate -= AttackTurn;

            // InstanceActions.onOverdrivePercentageUpdate -= odGage.SetValue;
            // InstanceActions.onOverdrivePercentageUpdate -= odGageBG.SetValue;

            //InstanceActions.onDefenseUpdate -= defenseText.SetValue;
            //InstanceActions.onMatchBoardDamageUpdate -= AttackChargeText;
            //InstanceActions.onMatchBoardDamageUpdate -= AttackChargeText;
        }

        public override void ResetValues()
        {
            //hpText.SetValue(0);
            hpGage.SetValue(0);
            hpGageBG.SetValue(0);
            attackChargeText.SetValue(0);
            attackTurnText.SetValue(0);
        }

#if UNITY_EDITOR
        public void ResetValuesAlive()
        {
            hpText.SetValue(1);
            hpGage.SetValue(1);
            hpGageBG.SetValue(1);
            attackChargeText.SetValue(0);
        }
#endif

        public void DestroyItem(float delay = 0) => UIPrefManager.DestroyItem(this, delay);

        public void FadeOutItem(float fadeOutTime)
        {
            this.GetComponent<CanvasGroup>().DOFade(0, fadeOutTime);
            DestroyItem(fadeOutTime);
        }
        public void FadeInItem(float fadeOutTime)
        {
            this.GetComponent<CanvasGroup>().DOFade(1, fadeOutTime).SetDelay(targetReferenceGlobal is CharacterInstance_Battle_Enemy ? 1f : 0);
        }

        IEnumerator SetVfxElement(float timeAmount)
        {
            yield return new WaitForSeconds(timeAmount);
            vfxElementObj?.SetActive(targetReferenceGlobal is CharacterInstance_Battle_Enemy && attackTurnCount <= 1 ? true : false);
        }
    }
}