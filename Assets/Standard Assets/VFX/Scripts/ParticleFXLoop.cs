using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleFXLoop : MonoBehaviour
{
    ParticleSystem _particleFX;
    [SerializeField] float _duration;

    void Start()
    {
        SetSeed();
    }

    void SetSeed()
    {
        _particleFX = GetComponent<ParticleSystem>();
        var pfx = _particleFX;
        pfx.useAutoRandomSeed = false;
        pfx.randomSeed = 0;
        _duration = pfx.main.duration;

        PlayFX();
    }

    void PlayFX()
    {
        StartCoroutine(LoopFX());
    }

    IEnumerator LoopFX()
    {
        var pfx = _particleFX;
        pfx.Play();
        float time = 0;
        while (time < _duration)
        {
            time += Time.deltaTime;
            var fxLoop = _particleFX.main;
            fxLoop.loop = false;
            yield return null;
        }
        if (pfx.isPlaying)
            pfx.Stop();

        PlayFX();
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Alpha0))
            DimBackground();*/

        if (Input.GetKeyDown(KeyCode.Alpha9))
            StartCoroutine(LoopFX());
    }
}
