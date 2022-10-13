using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using M7.FX;
using M7.GameRuntime;
using M7.Match;

namespace VFX
{
    public class VFXOmniBombSystem : MonoBehaviour
    {
        enum Target { ManaCore, TimerIncrease }

        [Header("Omni Sphere")]
        public GameObject _onTapFX;
        //[SerializeField] SpriteData _sphere;
        [SerializeField] float dimEffectDuration = 1;

        [Header("FX Attractor")]
        [SerializeField] Target FinalDestination = Target.ManaCore;
        [SerializeField] particleAttractorCustom projectileFX;
        [SerializeField] Transform endTarget;

        ParticleSystem OnTapFX;
        private SpriteRenderer OmniSphere { get { return GetComponentInParent<MatchGridCell>().SpriteRenderer; } }
        private Transform FXEndTarget(Target location)
        {
            Transform loc = null;
            switch (location)
            {
                case Target.ManaCore:
                    //loc = ManaPoolUI.Instance.CoreStateIndicator;
                    break;
                case Target.TimerIncrease:
                    //loc = VFX_ComboTimerIncreaseDisplay.Instance.transform;
                    break;
            }
            return loc;
        }

        private void OnEnable()
        {
            //Bomb_OnSpawn();
            //if(_onTapFX)
            //    _onTapFX.SetActive(false);
            ParticleWorldManager.SetLayerRecursively(gameObject, gameObject.layer);
            if (projectileFX != null)
            {
                var mainParticle = projectileFX.GetComponent<ParticleSystem>().main;
                mainParticle.playOnAwake = false;
                projectileFX.target = FXEndTarget(FinalDestination);
                ParticleWorldManager.SetLayerRecursively(projectileFX.gameObject, BattleCameraHandler.Instance.particleLayer);
            }
        }

        [Button("Excecute Omni", ButtonSizes.Medium)]
        public void Omni_OnTap()
        {
            //Bomb_OnExplode();

            //if (_onTapFX)
            //{
            //    _onTapFX.SetActive(true);

            //    //play child particle
            //    OnTapFX = _onTapFX.GetComponentInChildren<ParticleSystem>();
            //    OnTapFX.Play();
            //}

            //sprite effect
            OmniSphere.DOFade(0, 1);
            OmniSphere.transform.DOScale(1.5F, 1);

            //dim fx
            //BackgroundDimmerVFX.Instance.DimBackground(dimEffectDuration);

            //play attractor fx if any
            if (projectileFX != null)
            {
                projectileFX.transform.position = ParticleWorldManager.Instance.GetParticleLocalPositionFromCameraType(projectileFX.transform.position, ParticleWorldManager.CameraType.Particle);
                projectileFX?.GetComponent<ParticleSystem>().Play();
            }
            
            //display time increase hud, for time increase function only
            //if (FinalDestination == Target.TimerIncrease)
            //    VFX_ComboTimerIncreaseDisplay.Instance.DisplayTimeIncrease();
        }
    }

}
