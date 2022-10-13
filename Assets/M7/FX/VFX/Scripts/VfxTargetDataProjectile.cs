using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DarkTonic.MasterAudio;
using M7.Skill;
using Sirenix.OdinInspector;
using UnityEngine;

namespace M7.FX
{
    [System.Serializable]
    public class VfxTargetDataProjectile : VfxTargetData
    {
        [SerializeField] protected VFXSkillSystem_ProjectileObject _projectileObject;
        [SerializeField] float travelDuration = 1;
        [ShowInInspector, ReadOnly] List<List<VFXSkillSystem_ProjectileObject>> _projectileObjectList = new List<List<VFXSkillSystem_ProjectileObject>>();

        [SerializeField] private bool playSimultaneously = true;
        [HideIf("playSimultaneously")]
        [SerializeField] private bool waitAllProjectileToFinish;
        [HideIf("playSimultaneously")]
        [SerializeField] private float intervalPerProjectile;

        public bool PlaySimultaneously => playSimultaneously;
        public bool WaitAllProjectileToFinish => waitAllProjectileToFinish;
        public float TravelDuration => travelDuration;
        public float IntervalPerProjectile => intervalPerProjectile;

        public TargetType TargetType => _targetType;
        
        public override void Init(bool flip)
        {
            Reset();
            if (isInitialized)
                return;

            if (_targetType == TargetType.None)
                return;

            _projectileObject.gameObject.SetActive(false);
            _projectileObjectList.Add(new List<VFXSkillSystem_ProjectileObject> { _projectileObject });
            _projectileObject.transform.localScale = new Vector3(Mathf.Abs(_projectileObject.transform.localScale.x) * (flip ? -1 : 1), _projectileObject.transform.localScale.y, _projectileObject.transform.localScale.z);

            base.Init(flip);
        }

        public void UpdateTravelDuration(float newTravelDuration) => travelDuration = newTravelDuration;

        public override void Reset()
        {
            _projectileObjectList.ForEach(dataList => dataList.ForEach(data => data.Reset()));
        }

        public void SetTargetParticlePosition(Transform[] startTargets, Transform[] endTargets, ParticleWorldManager.CameraType startCameraType, ParticleWorldManager.CameraType endCameraType)
        {
            if (_targetType == TargetType.None)
                return;

            var startTargetPos = GetTargetParticlePosition(startTargets, startCameraType, endCameraType, false);
            Vector3[] endTargetPos = endTargets.Select(data => ParticleWorldManager.Instance.GetParticleLocalPositionFromCameraType(data.position, startCameraType, endCameraType)).ToArray();

            for (int startTargetIndex = 0; startTargetIndex < startTargetPos.Length; startTargetIndex++)
            {
                // Debug.Log("TARGET POS: " + startTargetPos[x]);
                if (startTargetIndex >= _projectileObjectList.Count)
                    _projectileObjectList.Add(new List<VFXSkillSystem_ProjectileObject> { _projectileObject });
                for (int endTargetIndex = 0; endTargetIndex < endTargetPos.Length; endTargetIndex++)
                {
                    if (endTargetIndex >= _projectileObjectList[startTargetIndex].Count)
                        _projectileObjectList[startTargetIndex].Add(Object.Instantiate(_projectileObject, _projectileObject.transform.parent));

                    _projectileObjectList[startTargetIndex][endTargetIndex].transform.position = startTargetPos[startTargetIndex];
                    _projectileObjectList[startTargetIndex][endTargetIndex].SetDestinationPos(endTargetPos[endTargetIndex]);
                    ParticleWorldManager.SetLayer(_projectileObjectList[startTargetIndex][endTargetIndex].gameObject, endCameraType);
                }
            }
        }

        public override bool PlayEffect(int targetInstance = 0)
        {
            if (!base.PlayEffect())
                return false;

            const int playedParticles = 0;

            if (playSimultaneously)
                PlayEffectNoDelay(playedParticles, targetInstance);
            else
                SkillManager.Instance.StartCoroutine(PlayEffectWithDelay(playedParticles, targetInstance));
            
            return true;
        }

        private void PlayEffectNoDelay(int playedParticles, int targetInstance)
        {
            var audioSource = _projectileObjectList[0][0].GetComponent<AudioSource>();
            
            for (var x = 0; x < _projectileObjectList.Count; x++)
            {
                for (var y = 0; y < _projectileObjectList[x].Count; y++)
                {
                    if (playedParticles >= targetInstance) continue;
                    
                    playedParticles++;
                    
                    Debug.Log($"Play effect no delay, VFXTargetData, duration {travelDuration}, particle object {_projectileObjectList[x][y].gameObject.name}");
                    
                    // if (Settings.SettingsAPI.SoundsEnabled)
                    //     audioSource.PlayDelayed(_SFXDelay);
                    if (Settings.SettingsAPI.SoundsEnabled)
                        Debug.Log ("SFX Settings Player : " + sfxName);
                        MasterAudio.PlaySound(sfxName, delaySoundTime: _SFXDelay);
                    _projectileObjectList[x][y].Play(travelDuration);
                }
            }
        }
        
        private IEnumerator PlayEffectWithDelay(int playedParticles, int targetInstance)
        {
            var delay = new WaitForSeconds(intervalPerProjectile);
            
            var audioSource = _projectileObjectList[0][0].GetComponent<AudioSource>();
            
            for (var x = 0; x < _projectileObjectList.Count; x++)
            {
                for (var y = 0; y < _projectileObjectList[x].Count; y++)
                {
                    if (playedParticles >= targetInstance) continue;
                    
                    playedParticles++;
                    
                    Debug.Log($"Play effect with delay, VFXTargetData, duration {travelDuration}, particle object {_projectileObjectList[x][y].gameObject.name}");
                    
                    // if (Settings.SettingsAPI.SoundsEnabled)
                    //     audioSource.PlayDelayed(_SFXDelay);

                    if (Settings.SettingsAPI.SoundsEnabled)
                        MasterAudio.PlaySound(sfxName, delaySoundTime: _SFXDelay);
                    
                    _projectileObjectList[x][y].Play(travelDuration);
                    yield return delay;
                }
            }
        }
    }
}