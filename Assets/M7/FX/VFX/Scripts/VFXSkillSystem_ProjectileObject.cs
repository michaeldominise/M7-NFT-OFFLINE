using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace M7.FX
{
    public class VFXSkillSystem_ProjectileObject : MonoBehaviour
    {
        [SerializeField] ParticleSystem projectileParticle;
        [SerializeField] Transform projectileObject;
        [SerializeField] Transform projectileDestination;
        [SerializeField] Ease projectileEase = Ease.Linear;
        [SerializeField] int Loops;

        [SerializeField] bool hideAfterTravelComplete = true;

        public Vector3 ProjectileDestinationPosition => projectileDestination.position;
        
        public void SetDestinationPos(Vector3 vector3)
        {
            if(projectileDestination)
                projectileDestination.position = vector3;
        }

        public void Reset()
        {
            if (projectileParticle != null)
            {
                projectileParticle.transform.localPosition = Vector3.zero;
                projectileParticle.Stop();
            }
            if(projectileDestination)
                projectileDestination.localPosition = Vector3.zero;
            if(projectileObject)
                projectileObject.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        }

        public void Play(float effectDuration)
        {
            gameObject.SetActive(true);
            if(projectileParticle != null)
                projectileParticle.Play();

            var toMove = projectileObject == null ? projectileParticle?.transform : projectileObject;
            //print($"Play effect, duration {effectDuration}, start pos {toMove.position}, destination {projectileDestination.position}");
            toMove.DOMove(projectileDestination.position, effectDuration).OnUpdate(() => {
                 projectileObject.localEulerAngles = Vector3.zero;
            }).SetEase(projectileEase)
            .SetLoops(Loops, LoopType.Restart).onComplete += () =>
            { 
                if (hideAfterTravelComplete)
                    gameObject.SetActive(false);
            };
        }

        private void Update()
        {
            var toMove = projectileObject == null ? projectileParticle?.transform : projectileObject;
            if (!toMove)
                return;

            toMove.right = projectileDestination.position - toMove.position;
            //toMove.localEulerAngles = new (0,0, toMove.rotation.z);
            
            Vector2 direction = projectileDestination.transform.position - toMove.position;
            toMove.rotation = Quaternion.FromToRotation(Vector3.right, direction);
        }
    }
}
