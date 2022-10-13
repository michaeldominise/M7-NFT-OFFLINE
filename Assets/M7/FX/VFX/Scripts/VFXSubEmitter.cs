using System.Collections;
using System.Collections.Generic;
using M7.FX.VFX.Scripts;
using UnityEngine;
namespace M7.FX
{
    public class VFXSubEmitter : MonoBehaviour
    {
        [SerializeField] float _emitDelay;
        [SerializeField] ParticleSystem _subFX;
        [SerializeField] VFXSkillSystem _skillSystem;

        // Start is called before the first frame update
        void Awake()
        {
            if (_subFX != null)
                _subFX.gameObject.SetActive(false);
        }

        void OnEnable()
        {
            if (_subFX != null)
                _subFX.gameObject.SetActive(false);

            if (_skillSystem != null)
                _emitDelay = _skillSystem.OnHitExecuteDelay;

            Invoke("PlaySubFX", _emitDelay);
        }

        void PlaySubFX()
        {
            if (_subFX != null)
            {
                _subFX.gameObject.SetActive(true);
                _subFX.Play();
            }
        }

        void OnDisable()
        {
            if (_subFX != null)
                _subFX.gameObject.SetActive(false);

            CancelInvoke();
        }
    }
}