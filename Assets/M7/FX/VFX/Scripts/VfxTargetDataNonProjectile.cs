using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using M7.Match;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.FX
{
    [System.Serializable]
    public class VfxTargetDataNonProjectile : VfxTargetData
    {
        [SerializeField] protected ParticleSystem _targetParticle;
        [ShowInInspector, ReadOnly] protected List<ParticleSystem> _targetParticleList = new List<ParticleSystem>();

        public List<ParticleSystem> TargetParticleList => _targetParticleList;

        public override void Init(bool flip = false)
        {
            Reset();
            if (isInitialized)
                return;

            if (_targetType == TargetType.None)
                return;

            _targetParticleList.Add(_targetParticle);
            _targetParticle.transform.localScale = new Vector3(Mathf.Abs(_targetParticle.transform.localScale.x) * (flip ? -1 : 1), _targetParticle.transform.localScale.y, _targetParticle.transform.localScale.z);

            base.Init(flip);
        }

        public override void Reset()
        {
            _targetParticleList.ForEach(data =>
            {
                data.Stop();
                data.gameObject.SetActive(false);
            });
        }

        public void SetTargetParticlePosition(Transform[] targets, ParticleWorldManager.CameraType startCameraType, ParticleWorldManager.CameraType endCameraType)
        {
            if (_targetType == TargetType.None)
                return;

            Vector3[] targetPos = GetTargetParticlePosition(targets, startCameraType, endCameraType);
            for (int x = 0; x < targetPos.Length; x++)
            {
                if (x >= _targetParticleList.Count)
                    _targetParticleList.Add(Object.Instantiate(_targetParticle, _targetParticle.transform.parent));

                _targetParticleList[x].transform.position = targetPos[x];
                ParticleWorldManager.SetLayer(_targetParticleList[x].gameObject, endCameraType);
            }
        }

        public override bool PlayEffect(int targetCount = 0)
        {
            if (!base.PlayEffect())
                return false;
            
            for (int x = 0; x < targetCount; x++)
            {
                _targetParticleList[x].gameObject.SetActive(true);
                _targetParticleList[x].Play();
                if (Settings.SettingsAPI.SoundsEnabled)
                {
                    // _targetParticleList[x].GetComponent<AudioSource>().PlayDelayed(_SFXDelay);
                    if (Settings.SettingsAPI.SoundsEnabled)
                        MasterAudio.PlaySound(sfxName, delaySoundTime: _SFXDelay);
                    //Debug.Log($"SFX {_targetParticleList[x].GetComponent<AudioSource>().clip.name}, Played");
                }
            }

            return true;
        }

        public override bool PlayEffectPerProjectile(int targetCount = 0, float projectileTravelDuration = 0)
        {
            if (!base.PlayEffectPerProjectile())
                return false;

            PuzzleBoardManager.Instance.StartCoroutine(_PlayEffect(targetCount, projectileTravelDuration));
            
            return true;
        }

        IEnumerator _PlayEffect(int targetCount, float waitDuration)
        {
            for (var x = 0; x < targetCount; x++)
            {
                Debug.Log($"_Play Effect {_targetParticleList[x].gameObject.name}");
                yield return new WaitForSeconds(waitDuration);
                
                _targetParticleList[x].gameObject.SetActive(true);
                _targetParticleList[x].Play();
                if (Settings.SettingsAPI.SoundsEnabled)
                {
                    // _targetParticleList[x].GetComponent<AudioSource>().PlayDelayed(_SFXDelay);
                    if (Settings.SettingsAPI.SoundsEnabled)
                        MasterAudio.PlaySound(sfxName, delaySoundTime: _SFXDelay);
                    //Debug.Log($"SFX {_targetParticleList[x].GetComponent<AudioSource>().clip.name}, Played");
                }
            }
        }
    }
}