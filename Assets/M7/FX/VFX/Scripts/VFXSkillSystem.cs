using System;
using System.Collections;
using DarkTonic.MasterAudio;
using M7.GameRuntime;
using M7.Match.PlaymakerActions;
using Sirenix.OdinInspector;
// using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

namespace M7.FX.VFX.Scripts
{
    public class VFXSkillSystem : MonoBehaviour
    {
        [SerializeField] VfxTargetDataNonProjectile startVfxTargetData;
        [SerializeField] VfxTargetDataProjectile projectileVfxTargetData;
        [SerializeField] VfxTargetDataNonProjectile endVfxTargetData;
        [FormerlySerializedAs("onEffectActivateDelay")]
        [SerializeField] float onHitExecuteDelay = 2;
        [SerializeField] float onFinishExecuteDelay = 4;
        [SerializeField] int onMoveCounter;
        [SerializeField] private string closingSfxName;
        [SerializeField] bool attachToCaster;

        public float OnHitExecuteDelay => onFinishExecuteDelay;

        [SerializeField] private bool useCameraShake;
        [SerializeField] private Observer.Event cameraShakeEvent;
        
        public float OnFinishDuration => onFinishExecuteDelay;
        public float OnHitExecuteDuration => onHitExecuteDelay;
        public int OnMoveDecrement => onMoveCounter;
        public bool AttachToCaster => attachToCaster;

        private bool SFXEnabled()
        {
            //return Chamoji.Murasaki7.Settings.SettingsAPI.SoundsEnabled;
            return true;
        }

        [Button(ButtonSizes.Medium)]
        public void ExecuteSkillTest(Transform startTarget, Transform endTarget)
        {
            ExecuteSkill(new Transform[] { startTarget }, new Transform[] { endTarget }, ParticleWorldManager.CameraType.SystemUI, ParticleWorldManager.CameraType.World);
        }

        [Button(ButtonSizes.Medium)]
        public virtual void ExecuteSkill(
            Transform[] startTargets, 
            Transform[] endTargets, 
            ParticleWorldManager.CameraType startCameraType = ParticleWorldManager.CameraType.SystemUI, 
            ParticleWorldManager.CameraType endCameraType = ParticleWorldManager.CameraType.World,
            System.Action onHit = null,
            System.Action onFinish = null, bool flip = false)
        {
            Init(flip);
            startVfxTargetData.SetTargetParticlePosition(startTargets, startCameraType, startCameraType);
            projectileVfxTargetData.SetTargetParticlePosition(startTargets, endTargets, startCameraType, endCameraType);
            endVfxTargetData.SetTargetParticlePosition(endTargets, startCameraType, endCameraType);

            // StartCoroutine(VFXExecute(startVfxTargetData, startTargets.Length));
            // StartCoroutine(VFXExecute(projectileVfxTargetData, startTargets.Length * endTargets.Length));
            // StartCoroutine(VFXExecute(endVfxTargetData, endTargets.Length));

            var startTargetData = new VFXData(startVfxTargetData, startTargets.Length);
            var projectileData = new VFXData(projectileVfxTargetData, startTargets.Length * endTargets.Length);
            var endTargetData = new VFXData(endVfxTargetData, endTargets.Length);

            StartCoroutine(ExecuteSkillEffect(startTargetData, projectileData, endTargetData, onHit, onFinish));
            PlaySFX();

        }

        private IEnumerator ExecuteSkillEffect(VFXData startTarget, VFXData projectile, VFXData endTarget, Action onHit,
            Action onFinish)
        {
            yield return new WaitForSeconds(startTarget.VfxTargetData.effectDelay);
            startTarget.VfxTargetData.PlayEffect(startTarget.InstanceCount);

            if (!((VfxTargetDataProjectile)projectile.VfxTargetData).PlaySimultaneously &&
                ((VfxTargetDataProjectile)projectile.VfxTargetData).TargetType != VfxTargetData.TargetType.None)
            {
                yield return new WaitForSeconds(startTarget.VfxTargetData.effectDelay);

                var projectileTargetData = projectile.VfxTargetData as VfxTargetDataProjectile; 
                
                if(projectileTargetData == null)
                    yield break;
                
                projectile.VfxTargetData.PlayEffect(projectile.InstanceCount);
                yield return new WaitForSeconds(projectileTargetData.TravelDuration);
                endTarget.VfxTargetData.PlayEffectPerProjectile(projectile.InstanceCount, projectileTargetData.IntervalPerProjectile);
                
                if(projectileTargetData.WaitAllProjectileToFinish)
                    WaitForVFXToComplete(onHit, onHitExecuteDelay, onFinish, onFinishExecuteDelay);
                else 
                    WaitForVFXToComplete(onHit, projectileTargetData.TravelDuration, onFinish,// * 0.80f  * 2f
                        // projectileTargetData.TravelDuration * 2f);
                        (projectileTargetData.TravelDuration + projectileTargetData.IntervalPerProjectile * endTarget.InstanceCount) * 2f);
                yield break;
            }
            
            StartCoroutine(VFXExecute(projectileVfxTargetData, projectile.InstanceCount));
            StartCoroutine(VFXExecute(endVfxTargetData, endTarget.InstanceCount));

            // yield return new WaitForSeconds(projectile.VfxTargetData.effectDelay);
            // projectile.VfxTargetData.PlayEffect(projectile.InstanceCount);
            
            // yield return new WaitForSeconds(endTarget.VfxTargetData.effectDelay);
            // endTarget.VfxTargetData.PlayEffect(endTarget.InstanceCount);
            
            WaitForVFXToComplete(onHit, onHitExecuteDelay, onFinish, onFinishExecuteDelay);
        }

        public void UpdateProjectileTravelDuration(float travelDuration)
        {
            onHitExecuteDelay = travelDuration;
            projectileVfxTargetData.UpdateTravelDuration(travelDuration);
        }
        
        private void WaitForVFXToComplete(Action onHit, float pOnHitExecuteDelay, Action onFinish, float pOnFinishExecuteDelay)
        {
            StartCoroutine(ExecuteCallback(() =>
            {
                CameraShake();
                onHit?.Invoke();
                
                if (Settings.SettingsAPI.SoundsEnabled)
                    MasterAudio.PlaySound(closingSfxName, pOnFinishExecuteDelay);
                
            }, pOnHitExecuteDelay));
            StartCoroutine(ExecuteCallback(() =>
            {
                Reset();
                onFinish?.Invoke();
            }, pOnFinishExecuteDelay));
        }

        private void Init(bool flip)
        {
            gameObject.SetActive(true);
            startVfxTargetData.Init(flip);
            projectileVfxTargetData.Init(flip);
            endVfxTargetData.Init(flip);

            BattleManager.Instance.MoveCounterManager.SetCounter(false, OnMoveDecrement, null);
        }

        IEnumerator ExecuteCallback(Action callBack, float delay)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        private void Reset()
        {
            startVfxTargetData.Reset();
            projectileVfxTargetData.Reset();
            endVfxTargetData.Reset();
        }

        IEnumerator VFXExecute(VfxTargetData vfxTargetData,int instanceCount = 0)
        {
            yield return new WaitForSeconds(vfxTargetData.effectDelay);
            vfxTargetData.PlayEffect(instanceCount);
        }

        #region SFX FUNCTION

        [SerializeField] PlayableDirector SFXPlayer;
        private void PlaySFX()
        {
            if (!SFXEnabled()) return;
            
            if (SFXPlayer != null)
                SFXPlayer.Play();
        }

        #endregion

        #region Camera Shake

        [Tooltip("nth times of camera shakes")]
        [SerializeField] int shakeTimes = 1;
        [Tooltip("nth interval each camera shakes")]
        [SerializeField] float shakeIntervals = 1;

        public void CameraShake()
        {
            // StopCoroutine(Shake());
            // StartCoroutine(Shake());
            if(useCameraShake && cameraShakeEvent != null)
                cameraShakeEvent?.Occured();
        }

        private IEnumerator Shake()
        {
            print("Camera shake");
            int shakeCount = 0;
            while (shakeCount < shakeTimes)
            {
                BattleWorldCamera.Instance.ShakeCamera_Strong();
                yield return new WaitForSeconds(shakeIntervals);                
                shakeCount++;
                yield return null;
            }

            BattleWorldCamera.Instance.ResetToOrigPos();
            StopCoroutine(Shake());
        }

        private void OnDisable()
        {
            StopCoroutine(Shake());
        }

        #endregion
    }
}
