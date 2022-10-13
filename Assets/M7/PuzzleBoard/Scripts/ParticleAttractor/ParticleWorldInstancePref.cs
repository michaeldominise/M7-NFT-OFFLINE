using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using Sirenix.OdinInspector;
using System;
using M7.GameRuntime;

public class ParticleWorldInstancePref : MonoBehaviour
{
    [SerializeField] ParticleSystem _particle;

    bool isPVP = LevelManager.ChapterData != null ? LevelManager.GameMode == M7.GameData.LevelData.GameModeType.PVP : false;
    protected ParticleSystem particle { get { _particle = _particle ?? GetComponent<ParticleSystem>(); return _particle; } }

    [Button]
    public virtual void Play(Action onFinish)
    {
        if (!particle)
        {
            if (onFinish != null)
                onFinish();
            return;
        }

        StartCoroutine(_Play(onFinish));
    }

    protected virtual IEnumerator _Play(Action onFinish)
    {
        if (!isPVP)
        {
            particle.Play();
            yield return new WaitWhile(() => particle.isPlaying);
        }

        if (onFinish != null)
            onFinish();
    }
}
