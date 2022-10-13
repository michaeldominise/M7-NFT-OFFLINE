using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.Match;
using Sirenix.OdinInspector;

public class CellLockManager : MonoBehaviour
{

    [System.Serializable]
    public class LockTypePair
    {
        public CellType tileType;
        public ParticleSystem lockParticle;
    }

    public List<LockTypePair> lockPairs;
    ParticleColorChanger _colorChanger;

    public bool omniOverrideLockParticle { get; set; }

    [Button]
    public void PlayLockParticle(CellType type)
    { 
        bool isLowQuality = false;
        //bool isLowQuality = QualityManager.QLevel == QualityManager.QualityLevel.LOW;
        if (!isLowQuality)
        {
            StopLockParticle();
            //return;
            foreach (LockTypePair ltp in lockPairs)
            {
                bool validMatch = ltp.tileType.ElementType == type.ElementType;
                if (validMatch)
                {
                    ltp.lockParticle.gameObject.SetActive(true);
                    ltp.lockParticle.Play();

                    _colorChanger = ltp.lockParticle.GetComponent<ParticleColorChanger>();
                    if (_colorChanger != null)
                        _colorChanger.ApplyColorScheme(ltp.tileType.ElementType);
                }
            }
        }
    }

    public void StopLockParticle()
    {
        foreach (LockTypePair ltp in lockPairs)
        {
            ltp.lockParticle.Stop();
            ltp.lockParticle.gameObject.SetActive(false);
        }
    }
}
