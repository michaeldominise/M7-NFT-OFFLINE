using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAnimation : MonoBehaviour {

    protected IEnumerator activeCoroutine;

    public System.Action onStartAnim;
    public System.Action onEndAnim;

    [SerializeField] protected float _animDuration;


    public virtual void PlayAnim(float delay = 0)
    {
        if (onStartAnim != null)
            onStartAnim();
    }

    public virtual void StopAnim()
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);
    }

    protected virtual void PlayAnimCoroutine(IEnumerator coroutine)
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        activeCoroutine = coroutine;
        StartCoroutine(activeCoroutine);
    }


}
