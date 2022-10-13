using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using M7.GameRuntime;
using M7.FX;
using M7.FX.VFX.Scripts;

namespace VFX
{
    public class VFXBombSystem : VFXSkillSystem
    {
        [SerializeField]
        GameObject _onSpawn, _onIdle, _onExplode, _onMatched;
        [SerializeField] TMP_Text _counter;
        [SerializeField] ParticleSystem _bombQueue, _queueDamage;
        [SerializeField] float _duration = 1;
        float emitRate = 1;

        ParticleColorChanger fxColor(GameObject obj) { return obj.GetComponent<ParticleColorChanger>(); }

        public bool Explode { get; set; }
        public TMP_Text counterText { get { return _counter; } }

        // Start is called before the first frame update
        void Start()
        {
            ParticleWorldManager.SetLayerRecursively(gameObject, gameObject.layer);
        }

        private void OnEnable()
        {
            Bomb_OnSpawn();
        }

        public void Bomb_OnSpawn()
        {
            if (_onSpawn != null) _onSpawn.SetActive(true);
            if (_onIdle != null) _onIdle.SetActive(false);
            if (_onExplode != null) _onExplode.SetActive(false);
            if (_onMatched != null) _onMatched.SetActive(false);
            if (_queueDamage != null) _queueDamage.Stop();

            if (_bombQueue != null)
            {
                var fx = _bombQueue.emission;
                fx.rateOverTime = 1;
            }

            Invoke("Bomb_ShowIdle", _duration);
        }

        void Bomb_ShowIdle()
        {
            if (_onSpawn != null) _onSpawn.SetActive(false);
            if (_onIdle != null) _onIdle.SetActive(true);

            Bomb_CountAnimation();
            CancelInvoke();
        }

        void Bomb_CountAnimation()
        {
            if (_onIdle != null)
            {
                Animation countAnim = _onIdle.GetComponent<Animation>();
                if (countAnim != null)
                    countAnim.Play();
            }
            //Debug.LogError("Invoking = " + IsInvoking());
        }

        public void Bomb_OnExplode()
        {
            if (_onIdle != null) _onIdle.SetActive(false);
            if (_onExplode != null) _onExplode.SetActive(true);

            Explode = true;

        }

        public void Bomb_OnMatched()
        {
            if (_onIdle != null) _onIdle.SetActive(false);
            if (_onMatched != null) _onMatched.SetActive(true);
        }

        public void Bomb_UpdateCounter(int value)
        {
            if (_counter != null) _counter.text = value.ToString();

            if (_queueDamage != null) _queueDamage.Play();

            if (_bombQueue != null)
            {
                var fx = _bombQueue.emission;
                emitRate++;
                fx.rateOverTime = emitRate;
            }

            Bomb_CountAnimation();
        }

        //public void AffiliateColor(Color affinity)
        //{
        //    fxColor(_onIdle)?.ApplyColorScheme(affinity);
        //}
    }
}