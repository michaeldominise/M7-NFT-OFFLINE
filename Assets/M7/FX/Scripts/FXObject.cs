using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.FX
{
    public abstract class FXObject : MonoBehaviour
    {
        virtual protected FXManager FXManager { get; private set; }
        [SerializeField] float playDuration = 1;
        [SerializeField] bool playOnstart = true;

        public virtual void Start()
        {
            if (playOnstart)
                Play(true);
        }

        [Button]
        protected virtual void Play(bool despawnAfterPlaying)
        {
            if(despawnAfterPlaying)
                DelayedDespawn(playDuration);
        }

        public void DelayedDespawn(float delay) => StartCoroutine(_DelayedDespawn(playDuration));
        IEnumerator _DelayedDespawn(float delay)
        {
            yield return new WaitForSeconds(delay);
            Despawn();
        }

        public virtual void Despawn() => FXManager._Despawn(this);
    }
}
