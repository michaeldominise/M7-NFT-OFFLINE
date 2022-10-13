using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using M7.FX.VFX.Scripts;
using UnityEngine;
namespace M7.FX
{
    public class VFXLoopEmitterDestroy : MonoBehaviour
    {
        [SerializeField] ParticleSystem _vFXParticle;
        [SerializeField] float _timeDestroy;

        public void OnEnable ()
        {
            StartCoroutine(EmitDestroy(_timeDestroy));
            Debug.Log ("OnEnable");
        }

        IEnumerator EmitDestroy(float waitTime)
        {
            EnableVFX(true);
            yield return new WaitForSeconds(waitTime);
            EnableVFX(false);
        }

        private void EnableVFX (bool e)
        {
            foreach (Transform child in _vFXParticle.transform) {
                child.GetComponent<ParticleSystem>().loop = e;           
            }
        }

        public void OnDisable ()
        {
            Debug.Log ("OnDisable");
        }
    }
}
