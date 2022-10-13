using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using Sirenix.OdinInspector;
using System;

public class ParticleWorldInstancePref_DurationBased : ParticleWorldInstancePref {
    public float lifeDuration = 1;

    protected override IEnumerator _Play(Action onFinish)
    {
        particle.Play();
        yield return new WaitForSeconds(lifeDuration);
        if (onFinish != null)
            onFinish();
    }
}
