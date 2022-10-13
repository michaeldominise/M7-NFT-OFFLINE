using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GachaEventAnimator : MonoBehaviour
{
    public ParticleSystem[] _particles;
    public GameObject _summonFX;
    public Animator[] _crackAnim;

    // Use this for initialization
    void Awake()
    {
        if (_particles == null)
            return;

        if (_crackAnim == null)
            return;

        if (_summonFX == null)
            return;
        print("");
    }

    void OnEnable()
    {
        Reset();
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Reset()
    {
        foreach (var obj in _particles)
        {
            obj?.Stop();
        }

        gameObject.SetActive(true);
        _summonFX.GetComponent<ParticleSystem>().Stop();
        _summonFX.SetActive(false);

        foreach (var anim in _crackAnim)
        {
            anim.SetBool("explode", false);
        }
    }

    public void DisableEmit()
    {
        foreach (var obj in _particles)
        {
            obj?.Stop();
        }
        
        _summonFX.SetActive(true);
        _summonFX.GetComponent<ParticleSystem>().Play();

        foreach (var anim in _crackAnim)
        {
            anim.SetBool("explode", true);
        }
    }

}