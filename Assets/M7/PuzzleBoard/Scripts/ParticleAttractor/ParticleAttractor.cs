using M7.FX;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAttractor : MonoBehaviour
{
    [SerializeField] ParticleSystem[] subParticles;
    [SerializeField] particleAttractorCustom particleAttractor;
    [SerializeField] ParticleWorldInstancePref_DurationBased particleWorldInstancePref_DurationBased;

    public void Init (Color color, Vector3 endPos, float lifeTimeDuration)
    {
        foreach (var subParticle in subParticles)
        {
            var main = subParticle.main;
            main.startColor = color;
        }

        particleAttractor.target.transform.position = endPos;
        particleAttractor.UpdateLifeTimeDuration(lifeTimeDuration);
        particleWorldInstancePref_DurationBased.lifeDuration = lifeTimeDuration;
        particleWorldInstancePref_DurationBased.Play(() => ParticleWorldManager.Instance.DespawnPartcle(transform));
    }
}
