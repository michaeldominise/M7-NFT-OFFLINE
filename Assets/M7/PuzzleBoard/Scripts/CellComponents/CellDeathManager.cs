using System;
using System.Collections;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using UnityEngine;
using M7.Match;
using M7.Settings;
using Sirenix.OdinInspector;
using PathologicalGames;

public class CellDeathManager : MonoBehaviour
{
    public static CellDeathManager Instance => PuzzleBoardManager.Instance.TileDeathManager;
    [SerializeField] private string sfxName;
    
    [Serializable]
    public class DeathTypePair
    {
        public CellType tileType;
        public List<ParticleSystem> lockParticle;
    }

    public List<DeathTypePair> lockPairs;
    [SerializeField] SpawnPool pool;

    ParticleColorChanger _colorChanger;


    [Button]
    public void PlayDeathParticle(MatchGridCell tile)
    {
        //bool isLowQuality = QualityManager.QLevel == QualityManager.QualityLevel.LOW;
        bool isLowQuality = false;
        var tileType = tile.CellTypeContainer.CellType;
        if(!isLowQuality)
        {
            if (lockPairs.Count > 0)
            {
                // audioSource.PlayOneShot(destroyCubeClip);
                if (SettingsAPI.SoundsEnabled)
                    MasterAudio.PlaySound(sfxName);
            }

            foreach (DeathTypePair ltp in lockPairs)
            { 
                if (ltp.tileType == tileType)
                {
                    if (ltp.lockParticle.Count <= tile.CellHealth.Health)
                        break;

                    var lockParticle = pool.Spawn(ltp.lockParticle[tile.CellHealth.Health].transform, transform).GetComponent<ParticleSystem>();
                    lockParticle.transform.position = tile.transform.position + Vector3.back;
                    lockParticle.gameObject.SetActive(true);
                    lockParticle.Play();
                    // audio
                    
                    if (SettingsAPI.SoundsEnabled)
                        MasterAudio.PlaySound(sfxName);
                    
                    StartCoroutine(DespawnParticle(lockParticle.transform, lockParticle.main.startLifetime.constant));
                    break;
                }
            }
        }
    }

    IEnumerator DespawnParticle(Transform target, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        pool.Despawn(target);
    }
}
