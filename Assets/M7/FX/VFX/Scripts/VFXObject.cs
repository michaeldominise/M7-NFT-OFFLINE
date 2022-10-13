using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace M7.FX
{
    public class VFXObject : FXObject
    {
        protected override FXManager FXManager => VFXManager.Instance;

        [SerializeField] UnityEvent sourceExecute;
        [SerializeField, BoxGroup("Projectile")] float projectileTravelDuration = 0;
        [SerializeField, BoxGroup("Projectile")] Transform projectileContainer;
        [SerializeField, BoxGroup("Projectile")] Transform destinationContainer;
        [SerializeField, ShowIf("HasProjectile"), BoxGroup("Projectile")] UnityEvent hitExecute;
        public bool HasProjectile => projectileTravelDuration > 0;

        public void Init(Vector3 sourcePosition, Vector3 destinationPoition)
        {
            transform.position = sourcePosition;
            destinationContainer.position = destinationPoition;
        }

        protected override void Play(bool despawnAfterPlaying)
        {
            base.Play(despawnAfterPlaying);
            
            sourceExecute.Invoke();
            if (HasProjectile)
                projectileContainer.DOLocalMove(destinationContainer.localPosition, projectileTravelDuration).onComplete += hitExecute.Invoke;
        }
    }
}
